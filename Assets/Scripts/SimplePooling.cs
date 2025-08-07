using Lean;
using System.Collections.Generic;
using UnityEngine;

public class SimplePooling : MonoBehaviour
{
	public GameObject Prefab;

	private List<GameObject> clones = new List<GameObject>();

	public void SpawnPrefab()
	{
		Vector3 position = (Vector3)UnityEngine.Random.insideUnitCircle * 6f;
		GameObject item = LeanPool.Spawn(Prefab, position, Quaternion.identity, null);
		clones.Add(item);
	}

	public void DespawnPrefab()
	{
		if (clones.Count > 0)
		{
			int index = clones.Count - 1;
			GameObject clone = clones[index];
			clones.RemoveAt(index);
			LeanPool.Despawn(clone);
		}
	}
}
