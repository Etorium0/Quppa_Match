using System.Collections.Generic;
using UnityEngine;

public class LevelItemMgr : MonoBehaviour
{
	public GameObject levelItemInit;

	public void InstanceLevel(List<Level> levels)
	{
		for (int i = 0; i < levels.Count; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(levelItemInit);
			gameObject.transform.parent = base.transform;
			gameObject.GetComponent<LevelItem>().SetData(levels[i]);
		}
	}
}
