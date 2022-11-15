using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;

public class PlayerVisual : MonoBehaviour
{
	private const string TRIGGER_IDLE = "idle";
	private const string TRIGGER_JUMP = "jump";
	private const string TRIGGER_FALL = "fall";
	private const string TRIGGER_DIE  = "die";

	[SerializeField] private Animator lifeAnimator;
	[SerializeField] private SpriteRenderer lifeSprite;
	[SerializeField] private TextMesh lifeText;
	[SerializeField] private Animator lifeAnimatorMinus;
	[SerializeField] private SpriteRenderer lifeSpriteMinus;
	[SerializeField] private TextMesh lifeTextMinus;
	[SerializeField] private TextMesh scoreText;
	[SerializeField] private Transform scoreRoot;
	[SerializeField] private Material playerMaterial;
	[SerializeField] private MeshRenderer scoreIcon;
	[SerializeField] private PhysicsTriggerListener bodyTrigger;
	[SerializeField] private Transform _bodyRoot;

	public delegate void OnCollisionDelegate(Collider2D collider, BoxCollider2D selfCollider);
	public event OnCollisionDelegate OnCollision;

	private Animator _animator;
	private Material _playerMaterialLocal;
	private Material _scoreIconMaterialLocal;
	private IEnumerator _immortalAnimation;
	private float _scoreIconWaveOffset;

	public void Initialize(Player player, string skinId)
	{
		_playerMaterialLocal = Instantiate(playerMaterial);
		_scoreIconMaterialLocal = Instantiate(scoreIcon.sharedMaterial);

		ApplyLocalMaterial(gameObject, playerMaterial, _playerMaterialLocal);
		ApplyLocalMaterial(scoreIcon.gameObject, scoreIcon.sharedMaterial, _scoreIconMaterialLocal);

		scoreText.text = "";

		RefreshScore(0, int.MaxValue);

		bodyTrigger.OnTriggerEnter += OnPhysicsTriggerEnter;

		Addressables.LoadAssetAsync<GameObject>(skinId).Completed += InitializePlayerSkin;
	}

	private void OnDestroy()
	{
		Destroy(_playerMaterialLocal);
		Destroy(_scoreIconMaterialLocal);
	}

	private void InitializePlayerSkin(AsyncOperationHandle<GameObject> asset)
	{
		asset.Completed -= InitializePlayerSkin;

		var skinInstance = Instantiate(asset.Result, _bodyRoot, false);

		_animator = skinInstance.GetComponent<Animator>();

		ApplyLocalMaterial(skinInstance, playerMaterial, _playerMaterialLocal);
	}

	private void ApplyLocalMaterial(GameObject target, Material targetMaterial, Material localMaterial)
	{
		var sr = target.GetComponentsInChildren<SpriteRenderer>();

		foreach (var i in sr)
		{
			if (i.sharedMaterial == targetMaterial)
			{
				i.sharedMaterial = localMaterial;
			}
		}

		var mr = target.GetComponentsInChildren<MeshRenderer>();

		foreach (var i in mr)
		{
			if (i.sharedMaterial == targetMaterial)
			{
				i.sharedMaterial = localMaterial;
			}
		}
	}

	private void OnPhysicsTriggerEnter(Collider2D collider, Collider2D selfCollider)
	{
		OnCollision?.Invoke(collider, selfCollider as BoxCollider2D);
	}

	private void Update()
	{
		_scoreIconWaveOffset -= Time.deltaTime * 1.5f;
		_scoreIconWaveOffset %= 1f;

		_scoreIconMaterialLocal.SetFloat("_WrapX", _scoreIconWaveOffset);
	}

	public void PlayLife(int lifes)
	{
		if (lifes == 0 || this == null)
		{
			return;
		}

		if (lifes < 0)
		{
			lifeSpriteMinus.color = Color.black;
			lifeTextMinus.text = "-" + Mathf.Abs(lifes);
			lifeAnimatorMinus.SetTrigger("show");
		}
		else
		{
			lifeSprite.color = Color.white;
			lifeText.text = "+" + Mathf.Abs(lifes);
			lifeAnimator.SetTrigger("show");
		}
	}

	public void RefreshScore(int score, int maxPlayerScore)
	{
		var scoreStr = score.ToString();

		if (scoreStr != scoreText.text)
		{
			scoreText.text = scoreStr;

			var k = score / (float)maxPlayerScore;
			var scale = 1f;//Mathf.Max(k, 0.5f);

			scoreRoot.localScale = new Vector3(scale, scale, 1f);

			_scoreIconMaterialLocal.SetFloat("_WrapY", k);
		}
	}

	public void PlayIdle()
	{
		if (_animator != null)
		{
			_animator.SetTrigger(TRIGGER_IDLE);
		}
	}

	public void PlayJump()
	{
		if (_animator != null)
		{
			_animator.SetTrigger(TRIGGER_JUMP);
		}
	}

	public void PlayFall()
	{
		if (_animator != null)
		{
			_animator.SetTrigger(TRIGGER_FALL);
		}
	}

	public void PlayDie()
	{
		if (_animator != null)
		{
			_animator.SetTrigger(TRIGGER_DIE);

			scoreRoot.gameObject.SetActive(false);
		}
	}

	public void PlayImmortal(float time)
	{
		if (_immortalAnimation != null)
		{
			StopCoroutine(_immortalAnimation);

			_immortalAnimation = null;
		}

		SetPlayerAlpha(1f);
		StartCoroutine(_immortalAnimation = PlayImmortalAnimation(time));
	}

	private void SetPlayerAlpha(float alpha)
	{
		_playerMaterialLocal.SetColor("_Color", new Color(1f, 1f, 1f, alpha));
	}

	private IEnumerator PlayImmortalAnimation(float time)
	{
		var alpha = 1f;
		var delta = 0f;

		while (time > 0f)
		{
			time -= Time.deltaTime;
			delta += Time.deltaTime * 4f;

			var intDelta = (int)delta;

			alpha = delta - intDelta;

			if (intDelta % 2 != 0)
			{
				alpha = 1f - alpha;
			}

			SetPlayerAlpha(alpha);

			yield return null;
		}

		SetPlayerAlpha(1f);

		yield return null;

		_immortalAnimation = null;
	}
}
