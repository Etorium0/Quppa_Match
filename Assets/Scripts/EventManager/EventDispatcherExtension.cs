using System;
using UnityEngine;

namespace EventManager
{
	public static class EventDispatcherExtension
	{
		public static void RegisterListener(this MonoBehaviour sender, EventID eventID, Action<Component, object> callback)
		{
			EventDispatcher.RegisterListener(eventID, callback);
		}

		public static void RemoveListener(this MonoBehaviour sender, EventID eventID, Action<Component, object> callback)
		{
			EventDispatcher.RemoveListener(eventID, callback);
		}

		public static void PostEvent(this MonoBehaviour sender, EventID eventID, object param)
		{
			EventDispatcher.PostEvent(eventID, sender, param);
		}

		public static void PostEvent(this MonoBehaviour sender, EventID eventID)
		{
			EventDispatcher.PostEvent(eventID, sender);
			Debug.Log(eventID);
		}
	}
}
