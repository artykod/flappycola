using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
	[SerializeField] private PlayerVisual visualPrefab;
	[SerializeField] private BubblePlayerDie bubblePrefab;

	private new Transform transform = null;

	private Session _session;
	private FlappyGame _game;
	private FlappyConfig _config;

	private float _startIdleDelay = 2f;
	private bool _startedPlay = false;
	private float _velocity = 0f;
	private PlayerInfo _selfPlayerInfo;
	private PlayerVisual _playerVisual;
	private Collider2D _lastCollider;
	private readonly List<BubblePlayerDie> _bubbles = new List<BubblePlayerDie>(100);

	private float _immortalTime;

	public PlayerInfo SelfPlayerInfo => _selfPlayerInfo;

	public event System.Action<Player> OnFinish;

	public void Initialize(Session session, FlappyGame game, FlappyConfig config, PlayerInfo playerInfo)
	{
		this.transform = base.transform;

		_session = session;
		_game = game;
		_config = config;

		_selfPlayerInfo = playerInfo;

		_selfPlayerInfo.OnJump += OnPlayerInfoJump;
		_selfPlayerInfo.OnLifeChange += OnPlayerInfoLifeChange;

		visualPrefab.gameObject.SetActive(false);

		_playerVisual = Instantiate(visualPrefab);
		_playerVisual.gameObject.SetActive(true);
		_playerVisual.transform.SetParent(transform, false);
		_playerVisual.OnCollision += OnTriggerEnter2DInternal;
		_playerVisual.Initialize(this, _selfPlayerInfo.CharacterId);

		_startIdleDelay = _game.StartTimer - 1f;

		bubblePrefab.gameObject.SetActive(false);

		for (int i = 0; i < 50; i++)
		{
			var angle = Random.value * Mathf.PI * 2f;
			var size = Random.value * 0.25f + 0.25f;
			var dir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));

			dir *= Random.value;

			var bubble = Instantiate(bubblePrefab);

			bubble.transform.SetParent(transform, false);
			bubble.transform.localPosition = dir;
			bubble.transform.localScale = new Vector3(size, size, 1f);
			bubble.gameObject.SetActive(false);

			_bubbles.Add(bubble);
		}

		_immortalTime = 0f;
	}

	private void Jump()
	{
		_startIdleDelay = 0f;
		_startedPlay = true;
		_velocity = _config.Jump;

		_playerVisual.PlayJump();
	}

	private void OnPlayerInfoJump()
	{
		if (_startIdleDelay > 0f)
		{
			return;
		}

		Jump();
	}

	void OnPlayerInfoLifeChange(int changeValue)
	{
		_playerVisual.PlayLife(changeValue);
	}

	public bool DoneObstacle(Obstacle obstacle)
	{
		var reward = obstacle.TakeRewardForPlayer(_selfPlayerInfo.Guid);

		if (reward != 0)
		{
			_selfPlayerInfo.AddScores(reward, _config.MaxPlayerScore);

			return true;
		}

		return false;
	}

	private void Update()
	{
		if (_startIdleDelay > 0f)
		{
			_startIdleDelay -= Time.deltaTime;

			return;
		}

		if (!_startedPlay)
		{
			_startedPlay = true;
			_playerVisual.PlayFall();
		}

		var pos = transform.position;
		_velocity += _config.Gravity * Time.deltaTime;
		pos.y += _velocity * Time.deltaTime;
		transform.position = pos;

		_playerVisual.RefreshScore(_selfPlayerInfo.Scores, _config.MaxPlayerScore);

		if (_immortalTime > 0f)
		{
			_immortalTime -= Time.deltaTime;
		}
	}

	private bool Finish()
	{
		if (!_selfPlayerInfo.Finish())
		{
			return false;
		}

		OnFinish?.Invoke(this);

		_playerVisual.PlayDie();

		_selfPlayerInfo.OnJump -= OnPlayerInfoJump;
		_selfPlayerInfo.OnLifeChange -= OnPlayerInfoLifeChange;

		enabled = false;

		foreach (var i in _bubbles)
		{
			if (i != null)
			{
				var velocity = i.transform.localPosition.normalized * Random.value * 2f;
				velocity.y -= _config.Gravity * 0.25f;
				i.transform.SetParent(_game.transform, true);
				i.gameObject.SetActive(true);

				i.Initialize(_config.Gravity);
				i.Play(velocity);
			}
		}

		return true;
	}

	private void OnTriggerEnter2DInternal(Collider2D collider, BoxCollider2D selfCollider)
	{
		if (!enabled || _lastCollider == collider)
		{
			return;
		}

		_lastCollider = collider;

		var isGround = collider.gameObject.layer == LayerMask.NameToLayer("Ground");

		if (_immortalTime <= 0f || isGround) {

			Finish();
		}

		if (!enabled)
		{
			return;
		}

		if (SelfPlayerInfo.Lives > 0 && isGround)
		{
			var pos = transform.position;
			pos.y = 0f;
			transform.position = pos;

			_velocity = 0f;
			_lastCollider = null;

			_immortalTime = _config.ImmortalTimeAfterGroundDeath;
			_startedPlay = false;
			_playerVisual.PlayIdle();
			_playerVisual.PlayImmortal(_immortalTime);
		}
	}
}
