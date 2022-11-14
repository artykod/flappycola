using UnityEngine;
using System.Collections;

public class PlayerVisual : MonoBehaviour
{
	private const string TRIGGER_IDLE = "idle";
	private const string TRIGGER_JUMP = "jump";
	private const string TRIGGER_FALL = "fall";
	private const string TRIGGER_DIE  = "die";

	[System.Serializable]
	public class SkinInfo
	{
		public string id = "";
		public Sprite body = null;
	}

	[SerializeField] private SkinInfo[] skins;
	[SerializeField] private SpriteRenderer bodyRenderer;
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

	public delegate void OnCollisionDelegate(Collider2D collider, BoxCollider2D selfCollider);
	public event OnCollisionDelegate OnCollision;

	private Animator _animator;
	private IEnumerator _immortalAnimation;
	private float _scoreIconWaveOffset;

	public void Initialize(Player player, string skinId)
	{
		_animator = GetComponent<Animator>();

		scoreIcon.sharedMaterial = Instantiate(scoreIcon.sharedMaterial);

		scoreText.text = "";

		RefreshScore(0, 0);

		var cloneMaterial = Instantiate(playerMaterial);
		var sr = gameObject.GetComponentsInChildren<SpriteRenderer>();

		foreach (var i in sr)
		{
			if (i.sharedMaterial == playerMaterial)
			{
				i.sharedMaterial = cloneMaterial;
			}
		}

		playerMaterial = cloneMaterial;

		bodyTrigger.OnTriggerEnter += OnPhysicsTriggerEnter;

		foreach (var i in skins)
		{
			if (skinId == i.id)
			{
				bodyRenderer.sprite = i.body;

				break;
			}
		}
	}

	void OnPhysicsTriggerEnter(Collider2D collider, Collider2D selfCollider)
	{
		OnCollision?.Invoke(collider, selfCollider as BoxCollider2D);
	}

	private void Update()
	{
		_scoreIconWaveOffset -= Time.deltaTime * 1.5f;
		_scoreIconWaveOffset %= 1f;

		scoreIcon.sharedMaterial.SetFloat("_WrapX", _scoreIconWaveOffset);
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

			var k = (float)score / (float)maxPlayerScore;
			var scale = 1f;//Mathf.Max(k, 0.5f);

			scoreRoot.localScale = new Vector3(scale, scale, 1f);
			scoreIcon.sharedMaterial.SetFloat("_WrapY", k);
		}
	}

	public void PlayIdle()
	{
		if (_animator == null)
		{
			return;
		}

		_animator.SetTrigger(TRIGGER_IDLE);
	}

	public void PlayJump()
	{
		if (_animator == null)
		{
			return;
		}

		_animator.SetTrigger(TRIGGER_JUMP);
	}

	public void PlayFall()
	{
		if (_animator == null)
		{
			return;
		}

		_animator.SetTrigger(TRIGGER_FALL);
	}

	public void PlayDie()
	{
		if (_animator == null)
		{
			return;
		}

		_animator.SetTrigger(TRIGGER_DIE);

		scoreRoot.gameObject.SetActive(false);
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
		playerMaterial.SetColor("_Color", new Color(1f, 1f, 1f, alpha));
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
