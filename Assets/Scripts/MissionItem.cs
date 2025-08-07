using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MissionItem : MonoBehaviour
{
	[SerializeField]
	private int MissionID;

	[SerializeField]
	private Text starText;

	[SerializeField]
	private GameObject lockObject;

	[SerializeField]
	private GameObject animalObject;

	private MissionState missionState;

	private void OnEnable()
	{
		StartCoroutine(UpdateStateMission());
	}

	private IEnumerator UpdateStateMission()
	{
		yield return new WaitForEndOfFrame();
		missionState = SingletonMonoBehaviour<MissionManager>.Instance.GetMissionState(MissionID);
		if (lockObject != null)
		{
			lockObject.SetActive(missionState == MissionState.IsLock);
		}
		if (animalObject != null)
		{
			animalObject.SetActive(missionState == MissionState.IsOpen);
		}
		starText.text = SingletonMonoBehaviour<MissionManager>.Instance.GetStarMission(MissionID);
	}

	public void OnPressMissionItem()
	{
		if (missionState != 0)
		{
			SingletonMonoBehaviour<MissionManager>.Instance.MissionCurrentID = MissionID;
			StartCoroutine(SingletonMonoBehaviour<SceneFader>.Instance.FadeAndLoadScene(SceneFader.FadeDirection.In, "Level"));
		}
	}

	public void OnPressMissionItemClassic()
	{
		SingletonMonoBehaviour<MissionManager>.Instance.MissionCurrentID = 0;
		SingletonMonoBehaviour<MissionManager>.Instance.LevelCurrentID = 0;
		GameDataMgr.scoreTotalClassic = 0;
		StartCoroutine(SingletonMonoBehaviour<SceneFader>.Instance.FadeAndLoadScene(SceneFader.FadeDirection.In, "Game"));
	}
}
