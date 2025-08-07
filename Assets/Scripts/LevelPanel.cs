using System.Collections.Generic;
using UnityEngine;

public class LevelPanel : MonoBehaviour
{
	public GameObject levelPageInit;

	public GameObject pagePointInit;

	public Transform pagePointParent;

	public Transform pageParent;

	private int totalLevelInPage = 12;

	[SerializeField]
	private ScrollSnapRect scroll;

	private void OnEnable()
	{
		List<Level> levelFromCurrentMission = SingletonMonoBehaviour<MissionManager>.Instance.GetLevelFromCurrentMission();
		InstanceLevelPage(levelFromCurrentMission);
		int startingPage = levelFromCurrentMission.FindIndex((Level x) => SingletonMonoBehaviour<MissionManager>.Instance.GetLevelState(x.ID, x.MissionID) == LevelState.IsOpen) / totalLevelInPage;
		scroll.startingPage = startingPage;
		//***
        //Advertisements.Instance.ShowBanner(BannerPosition.BOTTOM);
    }

	public void InstanceLevelPage(List<Level> levels)
	{
		int num = Mathf.CeilToInt((float)levels.Count / (float)totalLevelInPage);
		UnityEngine.Debug.Log(num);
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(levelPageInit);
			gameObject.transform.parent = pageParent;
			gameObject.transform.localPosition = Vector3.zero;
			LevelItemMgr component = gameObject.GetComponent<LevelItemMgr>();
			if (i == num - 1)
			{
				component.InstanceLevel(levels.GetRange(i * totalLevelInPage, levels.Count - i * totalLevelInPage));
			}
			else
			{
				component.InstanceLevel(levels.GetRange(i * totalLevelInPage, totalLevelInPage));
			}
			gameObject.transform.localScale = Vector3.one;
			GameObject gameObject2 = UnityEngine.Object.Instantiate(pagePointInit);
			gameObject2.transform.SetParent(pagePointParent);
			gameObject2.transform.localScale = Vector3.one;
		}
	}
}
