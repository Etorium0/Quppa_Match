using UnityEngine;

namespace Lean
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class LeanPooledRigidbody2D : MonoBehaviour
	{
		protected virtual void OnSpawn()
		{
		}

		protected virtual void OnDespawn()
		{
			Rigidbody2D component = GetComponent<Rigidbody2D>();
			component.linearVelocity = Vector2.zero;
			component.angularVelocity = 0f;
		}
	}
}
