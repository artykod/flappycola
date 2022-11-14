using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class Obstacle : MonoBehaviour
{
	[SerializeField] private GameObject bubblePrefab;

	private FlappyGame _game;
	private readonly List<GameObject> _bubbles = new List<GameObject>(100);
	private readonly Dictionary<string, int> _rewardsForPlayers = new Dictionary<string, int>();

	public event Action<Obstacle> OnMove;
	public event Action<Obstacle> OnReturnToPool;

	public new Transform transform
	{
		get;
		private set;
	}

	protected virtual void Awake()
	{
		this.transform = base.transform;
	}

	public void Initialize(FlappyGame game)
	{
		_game = game;

		var size = GetComponent<BoxCollider2D>().size * 0.5f;

		bubblePrefab.gameObject.SetActive(false);

		for (var x = -size.x; x < size.x; x += 0.4f)
		{
			for (var y = -size.y; y < size.y; y += 0.4f)
			{
				var bubble = Instantiate(bubblePrefab);

				bubble.gameObject.SetActive(true);
				bubble.transform.SetParent(transform, false);

				var scale = Random.value * 0.5f + 0.5f;
				var xRandom = (Random.value - 0.5f) * 0.3f;
				var yRandom = (Random.value - 0.5f) * 0.3f;

				bubble.transform.localPosition = new Vector3(x + xRandom, y + yRandom);
				bubble.transform.localScale = new Vector3(scale, scale, 1f);

				_bubbles.Add(bubble);

				bubble.gameObject.SetActive(false);
			}
		}
	}

	public void SetupForVisualize()
	{
		_rewardsForPlayers.Clear();

		foreach (var i in _game.CurrentPlayers)
		{
			_rewardsForPlayers.Add(i.Guid, 1);
		}

		foreach (var i in _bubbles)
		{
			i.SetActive(false);
			StartCoroutine(EnableBubbleWithDelay(i, Random.value));
		}
	}

	private IEnumerator EnableBubbleWithDelay(GameObject bubble, float delay)
	{
		while (delay > 0f)
		{
			delay -= Time.deltaTime;

			yield return null;
		}

		bubble.SetActive(true);
	}

	public int TakeRewardForPlayer(string playerGuid)
	{
		var reward = _rewardsForPlayers[playerGuid];

		_rewardsForPlayers[playerGuid] = 0;

		return reward;
	}

	protected virtual void Die()
	{
		ReturnToPool();
	}

	private void Update()
	{
		transform.position += new Vector3(-_game.LevelSpeed * Time.deltaTime, 0f);

		if (transform.position.x < -_game.ScreenSizeWorld.x - 3f)
		{
			Die();

			return;
		}

		OnMove?.Invoke(this);
	}

	private void ReturnToPool()
	{
		OnReturnToPool?.Invoke(this);
	}
}
