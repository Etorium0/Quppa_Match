using System;
using System.Collections.Generic;
using UnityEngine;

namespace EventManager
{
	public static class EventDispatcher
	{
		private static Dictionary<EventID, List<Action<Component, object>>> _listenersDict = new Dictionary<EventID, List<Action<Component, object>>>();

		public static void RegisterListener(EventID eventID, Action<Component, object> callback)
		{
			if (_listenersDict.ContainsKey(eventID))
			{
				_listenersDict[eventID].Add(callback);
			}
			else if (!_listenersDict.ContainsKey(eventID))
			{
				List<Action<Component, object>> list = new List<Action<Component, object>>();
				list.Add(callback);
				_listenersDict.Add(eventID, list);
			}
		}

		public static void PostEvent(EventID eventID, Component sender, object param = null)
		{
			if (_listenersDict.TryGetValue(eventID, out List<Action<Component, object>> value))
			{
				int i = 0;
				for (int num = value.Count; i < num; i++)
				{
					try
					{
						value[i](sender, param);
					}
					catch (Exception ex)
					{
						UnityEngine.Debug.LogWarningFormat("Error when PostEvent : {0}, message : {1}", eventID.ToString(), ex.Message);
						value.RemoveAt(i);
						if (value.Count == 0)
						{
							_listenersDict.Remove(eventID);
						}
						num--;
						i--;
					}
				}
			}
			else
			{
				UnityEngine.Debug.LogWarningFormat(null, "PostEvent, event : {0}, no listener for this event", eventID.ToString());
			}
		}

		public static void RemoveListener(EventID eventID, Action<Component, object> callback)
		{
			if (_listenersDict.TryGetValue(eventID, out List<Action<Component, object>> value))
			{
				if (value.Contains(callback))
				{
					value.Remove(callback);
					if (value.Count == 0)
					{
						_listenersDict.Remove(eventID);
					}
				}
			}
			else
			{
				UnityEngine.Debug.LogWarningFormat(null, "RemoveListener, event : {0}, no listener found", eventID.ToString());
			}
		}

		public static void RemoveRedundancies()
		{
			foreach (KeyValuePair<EventID, List<Action<Component, object>>> item in _listenersDict)
			{
				List<Action<Component, object>> value = item.Value;
				int count = value.Count;
				for (int num = count - 1; num >= 0; num--)
				{
					Action<Component, object> action = value[num];
					if (action == null || action.Target.Equals(null))
					{
						value.RemoveAt(num);
						if (value.Count == 0)
						{
							_listenersDict.Remove(item.Key);
						}
						num--;
					}
				}
			}
		}

		public static void ClearAllListener()
		{
			_listenersDict.Clear();
		}
	}
}
