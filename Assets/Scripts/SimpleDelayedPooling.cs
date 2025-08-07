using Lean;
using UnityEngine;

public class SimpleDelayedPooling : MonoBehaviour
{
	public GameObject Prefab;

	public float DespawnDelay = 1f;

	public void SpawnPrefab()
	{
		Vector3 position = (Vector3)UnityEngine.Random.insideUnitCircle * 6f;
		GameObject clone = LeanPool.Spawn(Prefab, position, Quaternion.identity, null);
		LeanPool.Despawn(clone, DespawnDelay);
	}
}
