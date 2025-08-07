using Lean;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class SimplePoolBenchmark : MonoBehaviour
{
	public int Step = 100;

	public GameObject Prefab;

	public LeanPool Pool;

	public Text BenchmarkText;

	private List<GameObject> spawnedClones = new List<GameObject>();

	private List<GameObject> instantiatedClones = new List<GameObject>();

	private Stopwatch benchmark = new Stopwatch();

	public void SpawnClones()
	{
		BeginBenchmark();
		for (int i = 0; i < Step; i++)
		{
			Vector3 position = (Vector3)UnityEngine.Random.insideUnitCircle * 6f;
			GameObject item = LeanPool.Spawn(Prefab, position, Quaternion.identity, null);
			spawnedClones.Add(item);
		}
		EndBenchmark("SpawnClones");
	}

	public void DespawnClones()
	{
		BeginBenchmark();
		for (int i = 0; i < Step; i++)
		{
			int num = spawnedClones.Count - 1;
			if (num >= 0)
			{
				GameObject clone = spawnedClones[num];
				spawnedClones.RemoveAt(num);
				LeanPool.Despawn(clone);
			}
		}
		EndBenchmark("DespawnClones");
	}

	public void FastSpawnClones()
	{
		BeginBenchmark();
		for (int i = 0; i < Step; i++)
		{
			Vector3 position = (Vector3)UnityEngine.Random.insideUnitCircle * 6f;
			GameObject item = Pool.FastSpawn(position, Quaternion.identity);
			spawnedClones.Add(item);
		}
		EndBenchmark("FastSpawnClones");
	}

	public void FastDespawnClones()
	{
		BeginBenchmark();
		for (int i = 0; i < Step; i++)
		{
			int num = spawnedClones.Count - 1;
			if (num >= 0)
			{
				GameObject clone = spawnedClones[num];
				spawnedClones.RemoveAt(num);
				Pool.FastDespawn(clone);
			}
		}
		EndBenchmark("FastDespawnClones");
	}

	public void InstantiateClones()
	{
		BeginBenchmark();
		for (int i = 0; i < Step; i++)
		{
			Vector3 position = (Vector3)UnityEngine.Random.insideUnitCircle * 6f;
			GameObject item = UnityEngine.Object.Instantiate(Prefab, position, Quaternion.identity);
			instantiatedClones.Add(item);
		}
		EndBenchmark("SpawnClones");
	}

	public void DestroyClones()
	{
		BeginBenchmark();
		for (int i = 0; i < Step; i++)
		{
			int num = instantiatedClones.Count - 1;
			if (num >= 0)
			{
				GameObject obj = instantiatedClones[num];
				instantiatedClones.RemoveAt(num);
				UnityEngine.Object.DestroyImmediate(obj);
			}
		}
		EndBenchmark("DestroyClones");
	}

	private void BeginBenchmark()
	{
		benchmark.Reset();
		benchmark.Start();
	}

	private void EndBenchmark(string title)
	{
		benchmark.Stop();
		if (BenchmarkText != null)
		{
			BenchmarkText.text = title + " took " + benchmark.ElapsedMilliseconds + "ms";
		}
	}
}
