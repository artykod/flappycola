using UnityEngine;

public class PhysicsTriggerListener : MonoBehaviour
{
	public delegate void OnTriggerEnterDelegate(Collider2D collider, Collider2D selfCollider);
	public event OnTriggerEnterDelegate OnTriggerEnter;

	private Collider2D _selfCollider;

	private void Awake()
	{
		_selfCollider = GetComponent<Collider2D>();
	}

	private void OnTriggerEnter2D(Collider2D collider)
	{
		OnTriggerEnter?.Invoke(collider, _selfCollider);
	}
}
