using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{
	private const int OBSTACLES_POOL_SIZE = 25;
	private const int LEVEL_GENERATION_DEPTH = 10;

	[SerializeField] private Obstacle obstaclePrefab;

	private Session _session;
	private FlappyGame _game;
	private FlappyConfig _config;
	private Obstacle _lastObstacle;
	private readonly LinkedList<Obstacle> _obstaclesPool = new LinkedList<Obstacle>();
	private int _index = 0;
	private float[] _pseudoRandomSideSeed = new[] { -1f, 1f, };

	public event Action<Obstacle> OnMoveObstacle;

	public void Initialize(Session session, FlappyGame game, FlappyConfig config)
	{
		_session = session;
		_game = game;
		_config = config;

		obstaclePrefab.gameObject.SetActive(false);

		for (int i = 0; i < OBSTACLES_POOL_SIZE; i++)
		{
			AddObstacleToPool();
		}
	}

	private void AddObstacleToPool()
	{
		var obstacle = Instantiate(obstaclePrefab, Vector3.zero, Quaternion.identity, transform);

		obstacle.gameObject.SetActive(true);
		obstacle.Initialize(_game);
		obstacle.gameObject.SetActive(false);

		_obstaclesPool.AddLast(obstacle);
	}

	private Obstacle GetObstacleFromPool()
	{
		var result = default(Obstacle);

		while (result == null)
		{
			if (_obstaclesPool.Count == 0)
			{
				AddObstacleToPool();
			}

			result = _obstaclesPool.Last.Value;

			_obstaclesPool.RemoveLast();

			if (result == null)
			{
				continue;
			}

			result.OnReturnToPool += ReturnToPool;
			result.OnMove += MoveObstacle;
			result.gameObject.SetActive(true);

			result.SetupForVisualize();
		}

		return result;
	}

	private void MoveObstacle(Obstacle obstacle)
	{
		OnMoveObstacle?.Invoke(obstacle);
	}

	private void ReturnToPool(Obstacle obstacle)
	{
		if (obstacle == null)
		{
			return;
		}

		_obstaclesPool.AddFirst(obstacle);

		obstacle.gameObject.SetActive(false);
		obstacle.OnReturnToPool -= ReturnToPool;
		obstacle.OnMove -= MoveObstacle;
	}

	private void GenerateLevel()
	{
		for (int i = 0; i < LEVEL_GENERATION_DEPTH; i++)
		{
			var pos = new Vector3((_lastObstacle == null ? _game.ScreenSizeWorld.x : _lastObstacle.transform.position.x) + _config.ObstaclesDistance, 0f);
			var offset = Random.Range(_config.ObstaclesHeightMin, _config.ObstaclesHeightMax);
			var side = _pseudoRandomSideSeed[_index++];

			if (_index >= _pseudoRandomSideSeed.Length)
			{
				_index = 0;
			}

			pos.y = (_game.ScreenSizeWorld.y + offset) * side;

			var obstacle = GetObstacleFromPool();

			obstacle.transform.position = pos;

			_lastObstacle = obstacle;
		}
	}

	private void Update()
	{
		if (_game.StartTimer > 2f)
		{
			return;
		}

		if (_lastObstacle == null || _lastObstacle.transform.position.x <= _game.ScreenSizeWorld.x)
		{
			GenerateLevel();
		}
	}
}
