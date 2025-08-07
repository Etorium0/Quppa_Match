using UnityEngine;

namespace Lean
{
	[RequireComponent(typeof(Rigidbody))]
	public class LeanPooledRigidbody : MonoBehaviour
	{
		protected virtual void OnSpawn()
		{
		}

		protected virtual void OnDespawn()
		{
			Rigidbody component = GetComponent<Rigidbody>();
			component.linearVelocity = Vector3.zero;
			component.angularVelocity = Vector3.zero;
		}
	}
}
