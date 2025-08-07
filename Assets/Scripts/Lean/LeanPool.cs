using System.Collections.Generic;
using UnityEngine;

namespace Lean
{
	[AddComponentMenu("Lean/Pool")]
	public class LeanPool : MonoBehaviour
	{
		public class DelayedDestruction
		{
			public GameObject Clone;

			public float Life;
		}

		public enum NotificationType
		{
			None,
			SendMessage,
			BroadcastMessage
		}

		public static List<LeanPool> AllPools = new List<LeanPool>();

		public static Dictionary<GameObject, LeanPool> AllLinks = new Dictionary<GameObject, LeanPool>();

		[Tooltip("The prefab the clones will be based on")]
		public GameObject Prefab;

		[Tooltip("Should this pool preload some clones?")]
		public int Preload;

		[Tooltip("Should this pool have a maximum amount of spawnable clones?")]
		public int Capacity;

		[Tooltip("Should this pool send messages to the clones when they're spawned/despawned?")]
		public NotificationType Notification = NotificationType.SendMessage;

		private List<GameObject> cache = new List<GameObject>();

		private List<DelayedDestruction> delayedDestructions = new List<DelayedDestruction>();

		private int total;

		public int Total => total;

		public int Cached => cache.Count;

		public static T Spawn<T>(T prefab) where T : Component
		{
			return Spawn(prefab, Vector3.zero, Quaternion.identity, null);
		}

		public static T Spawn<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component
		{
			return Spawn(prefab, position, rotation, null);
		}

		public static T Spawn<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent) where T : Component
		{
			GameObject prefab2 = (!((Object)prefab != (Object)null)) ? null : prefab.gameObject;
			GameObject gameObject = Spawn(prefab2, position, rotation, parent);
			return (!(gameObject != null)) ? ((T)null) : gameObject.GetComponent<T>();
		}

		public static GameObject Spawn(GameObject prefab)
		{
			return Spawn(prefab, Vector3.zero, Quaternion.identity, null);
		}

		public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
		{
			return Spawn(prefab, position, rotation, null);
		}

		public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
		{
			if (prefab != null)
			{
				LeanPool leanPool = AllPools.Find((LeanPool p) => p.Prefab == prefab);
				if (leanPool == null)
				{
					leanPool = new GameObject(prefab.name + " Pool").AddComponent<LeanPool>();
					leanPool.Prefab = prefab;
				}
				GameObject gameObject = leanPool.FastSpawn(position, rotation, parent);
				if (gameObject != null)
				{
					AllLinks.Add(gameObject, leanPool);
					return gameObject.gameObject;
				}
			}
			else
			{
				UnityEngine.Debug.LogError("Attempting to spawn a null prefab");
			}
			return null;
		}

		public static void Despawn(Component clone, float delay = 0f)
		{
			if (clone != null)
			{
				Despawn(clone.gameObject);
			}
		}

		public static void Despawn(GameObject clone, float delay = 0f)
		{
			if (clone != null)
			{
				LeanPool value = null;
				if (AllLinks.TryGetValue(clone, out value))
				{
					AllLinks.Remove(clone);
					value.FastDespawn(clone, delay);
				}
				else
				{
					UnityEngine.Object.Destroy(clone);
				}
			}
		}

		public GameObject FastSpawn(Vector3 position, Quaternion rotation, Transform parent = null)
		{
			if (Prefab != null)
			{
				while (cache.Count > 0)
				{
					int index = cache.Count - 1;
					GameObject gameObject = cache[index];
					cache.RemoveAt(index);
					if (gameObject != null)
					{
						Transform transform = gameObject.transform;
						transform.localPosition = position;
						transform.localRotation = rotation;
						transform.SetParent(parent, worldPositionStays: false);
						gameObject.SetActive(value: true);
						SendNotification(gameObject, "OnSpawn");
						return gameObject;
					}
					UnityEngine.Debug.LogError("The " + base.name + " pool contained a null cache entry");
				}
				if (Capacity <= 0 || total < Capacity)
				{
					GameObject gameObject2 = FastClone(position, rotation, parent);
					SendNotification(gameObject2, "OnSpawn");
					return gameObject2;
				}
			}
			else
			{
				UnityEngine.Debug.LogError("Attempting to spawn null");
			}
			return null;
		}

		public void FastDespawn(GameObject clone, float delay = 0f)
		{
			if (!(clone != null))
			{
				return;
			}
			if (delay > 0f)
			{
				if (!delayedDestructions.Exists((DelayedDestruction m) => m.Clone == clone))
				{
					DelayedDestruction delayedDestruction = LeanClassPool<DelayedDestruction>.Spawn() ?? new DelayedDestruction();
					delayedDestruction.Clone = clone;
					delayedDestruction.Life = delay;
					delayedDestructions.Add(delayedDestruction);
				}
			}
			else
			{
				cache.Add(clone);
				SendNotification(clone, "OnDespawn");
				clone.SetActive(value: false);
				clone.transform.SetParent(base.transform, worldPositionStays: false);
			}
		}

		public void FastPreload()
		{
			if (Prefab != null)
			{
				GameObject gameObject = FastClone(Vector3.zero, Quaternion.identity, null);
				cache.Add(gameObject);
				gameObject.SetActive(value: false);
				gameObject.transform.SetParent(base.transform, worldPositionStays: false);
			}
		}

		protected virtual void Awake()
		{
			UpdatePreload();
		}

		protected virtual void OnEnable()
		{
			AllPools.Add(this);
		}

		protected virtual void OnDisable()
		{
			AllPools.Remove(this);
		}

		protected virtual void Update()
		{
			for (int num = delayedDestructions.Count - 1; num >= 0; num--)
			{
				DelayedDestruction delayedDestruction = delayedDestructions[num];
				if (delayedDestruction.Clone != null)
				{
					delayedDestruction.Life -= Time.deltaTime;
					if (delayedDestruction.Life <= 0f)
					{
						RemoveDelayedDestruction(num);
						FastDespawn(delayedDestruction.Clone);
					}
				}
				else
				{
					RemoveDelayedDestruction(num);
				}
			}
		}

		private void RemoveDelayedDestruction(int index)
		{
			DelayedDestruction instance = delayedDestructions[index];
			delayedDestructions.RemoveAt(index);
			LeanClassPool<DelayedDestruction>.Despawn(instance);
		}

		private void UpdatePreload()
		{
			if (Prefab != null)
			{
				for (int i = total; i < Preload; i++)
				{
					FastPreload();
				}
			}
		}

		private GameObject FastClone(Vector3 position, Quaternion rotation, Transform parent)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(Prefab, position, rotation);
			total++;
			gameObject.name = Prefab.name + " " + total;
			gameObject.transform.SetParent(parent, worldPositionStays: false);
			return gameObject;
		}

		private void SendNotification(GameObject clone, string messageName)
		{
			switch (Notification)
			{
			case NotificationType.SendMessage:
				clone.SendMessage(messageName, SendMessageOptions.DontRequireReceiver);
				break;
			case NotificationType.BroadcastMessage:
				clone.BroadcastMessage(messageName, SendMessageOptions.DontRequireReceiver);
				break;
			}
		}
	}
}
