using Lean;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ContentMgr : SingletonMonoBehaviour<ContentMgr>
{
	[Serializable]
	public class ContentType
	{
		public string itemType;

		public GameObject go;
	}

	private GameObject zObj;

	public List<ContentType> contents = new List<ContentType>();

	public T GetItem<T>(string key) where T : Component
	{
		GameObject item = GetItem(key);
		if ((bool)item)
		{
			return item.GetComponent<T>();
		}
		return (T)null;
	}

	public T GetItem<T>(string key, Vector3 position) where T : Component
	{
		zObj = GetItem(key);
		zObj.transform.position = position;
		return zObj.GetComponent<T>();
	}

	public T GetItem<T>(string key, Transform parent, Vector3 position) where T : Component
	{
		zObj = GetItem(key);
		zObj.transform.SetParent(parent);
		zObj.transform.position = position;
		return zObj.GetComponent<T>();
	}

	public GameObject GetItem(string key, Vector3 position)
	{
		zObj = GetItem(key);
		zObj.transform.position = position;
		return zObj;
	}

	public GameObject GetItem(string key, Vector3 position, Quaternion rotation)
	{
		zObj = GetItem(key, position);
		zObj.transform.rotation = rotation;
		return zObj;
	}

	public GameObject GetItem(string itemType)
	{
		if (contents.Find((ContentType x) => x.itemType == itemType) != null)
		{
			return LeanPool.Spawn(contents.Find((ContentType x) => x.itemType == itemType).go);
		}
		return null;
	}

	public void Despaw(GameObject go, float delay = 0f)
	{
		LeanPool.Despawn(go, delay);
	}
}
