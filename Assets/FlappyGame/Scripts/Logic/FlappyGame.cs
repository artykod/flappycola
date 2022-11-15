using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlappyGame : MonoBehaviour
{
	[SerializeField] private Player playerPrefab = null;
	[SerializeField] private LevelGenerator levelGeneratorPrefab = null;

	public bool IsPaused
	{
		get => _isPaused;
		set
		{
			_isPaused = value;

			Time.timeScale = _isPaused ? 0f : 1f;

			OnPause?.Invoke(_isPaused);
		}
	}

	public event Action<bool> OnPause;
	public event Action<FlappyGame> OnGameFinish;

	public float StartTimer { get; private set; }
	public float LevelSpeed { get; private set; }
	public Vector3 ScreenSizeWorld => _screenSizeWorld;
	public PlayersCollection CurrentPlayers => new PlayersCollection(_currentPlayers);

	private Session _session;
	private FlappyConfig _config;

	private bool _isPaused;
	private bool _isMatchStarted;
	private Vector3 _screenSizeWorld;
	private LevelGenerator _levelGenerator;
	private float _levelSpeedAccelerationTime;
	private readonly Dictionary<string, Player> _players = new Dictionary<string, Player>();	
	private readonly Dictionary<string, PlayerInfo> _currentPlayers = new Dictionary<string, PlayerInfo>();

	public void Initialize(Session session, FlappyConfig config, PlayerInfo[] players)
	{
		_session = session;
		_config = config;
		_screenSizeWorld = Camera.main.ViewportToWorldPoint(Vector3.one);

		IsPaused = false;
		StartTimer = _config.StartGameTimer + 1f;
		LevelSpeed = _config.LevelSpeed;

		InitializeLevel();
		InitializePlayers(players);

		_isMatchStarted = true;
	}

	private void InitializeLevel()
	{
		levelGeneratorPrefab.gameObject.SetActive(false);

		_levelGenerator = Instantiate(levelGeneratorPrefab, transform);
		_levelGenerator.gameObject.SetActive(true);
		_levelGenerator.Initialize(_session, this, _config);
		_levelGenerator.OnMoveObstacle += OnMoveObstacle;
	}

	private void InitializePlayers(PlayerInfo[] players)
	{
		_currentPlayers.Clear();

		foreach (var p in players)
		{
			_currentPlayers[p.Guid] = p;

			p.OnFinish += OnPlayerInfoFinished;
		}

		var xOffset = -_screenSizeWorld.x + 2f;

		playerPrefab.gameObject.SetActive(false);

		foreach (var i in CurrentPlayers)
		{
			var player = Instantiate(playerPrefab, new Vector3(xOffset, 0f), Quaternion.identity, transform);

			player.gameObject.SetActive(true);
			player.OnFinish += OnPlayerFinish;
			player.Initialize(_session, this, _config, i);

			_players.Add(i.Guid, player);

			xOffset += 3f;
		}
	}

	private void Update()
	{
		StartTimer -= Time.deltaTime;

		_levelSpeedAccelerationTime += Time.deltaTime;

		if (_levelSpeedAccelerationTime >= _config.LevelSpeedAccelerationTimeInterval)
		{
			_levelSpeedAccelerationTime = 0f;
			LevelSpeed += _config.LevelSpeedAcceleration;
		}

		foreach (var kvp in _currentPlayers)
		{
			var player = kvp.Value;

			if (Input.GetMouseButtonDown(player.JumpKey) && !IsPaused)
			{
				if (_currentPlayers.TryGetValue(player.Guid, out var playerInfo))
				{
					playerInfo.Jump();
				}
			}
		}

		if (Input.GetKeyDown(KeyCode.Space))
		{
			IsPaused = !IsPaused;
		}
	}

	private void OnPlayerFinish(Player player)
	{
		_players.Remove(player.SelfPlayerInfo.Guid);
	}

	private void OnMoveObstacle(Obstacle obstacle)
	{
		foreach (var i in _players)
		{
			var player = i.Value;

			if (player == null)
			{
				continue;
			}

			if (player.transform.position.x > obstacle.transform.position.x)
			{
				player.DoneObstacle(obstacle);
			}
		}
	}

	public void OnPlayerInfoFinished(PlayerInfo playerInfo)
	{
		Debug.Log($"Player info finished: {playerInfo.Guid}");

		var activeCount = 0;

		foreach (var i in _currentPlayers)
		{
			if (!i.Value.IsFinished)
			{
				activeCount++;
			}
		}

		if (activeCount < 1)
		{
			FinishMatch(true);
		}
	}

	public void FinishMatch(bool sendMessageToServer)
	{
		if (!_isMatchStarted)
		{
			return;
		}

		Debug.Log("Finish match");

		_isMatchStarted = false;

		StartCoroutine(InvokeAfterDelay(1f, () => OnGameFinish?.Invoke(this)));
	}

	private IEnumerator InvokeAfterDelay(float delay, System.Action action)
	{
		while (delay > 0f)
		{
			delay -= Time.deltaTime;

			yield return null;
		}

		action?.Invoke();
	}
}
