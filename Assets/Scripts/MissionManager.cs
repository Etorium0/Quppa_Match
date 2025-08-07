using System;
using System.Collections.Generic;
using UnityEngine;
using OPS.Obfuscator.Attribute;

[DoNotObfuscateClassAttribute]
public class MissionManager : SingletonMonoBehaviour<MissionManager>
{
	private const string settingPath = "easysave2.txt";

	public List<Mission> Missions = new List<Mission>();

	public List<Mission> MissionsOrigin = new List<Mission>();

	private const string missionKey = "mission";

	private const string levelKey = "level";

	public int MissionCurrentID
	{
		get
		{
			return PlayerPrefs.GetInt("MissionCurrentID", 0);
		}
		set
		{
			PlayerPrefs.SetInt("MissionCurrentID", value);
		}
	}

	public int LevelCurrentID
	{
		get
		{
			return PlayerPrefs.GetInt("LevelCurrentID", 0);
		}
		set
		{
			PlayerPrefs.SetInt("LevelCurrentID", value);
		}
	}

	private new void Awake()
	{
	}

	private List<Mission> CreateMission(List<Mission> origin)
	{
		List<Mission> list = new List<Mission>();
		for (int i = 0; i < origin.Count; i++)
		{
			Mission mission = origin[i];
			list.Add(new Mission());
			int num = 0;
			for (int j = 0; j < mission.levels.Count; j++)
			{
				Level level = mission.levels[j];
				for (int k = 0; k < 7; k++)
				{
					Level level2 = new Level();
					level2.moveMode = (MoveMode)k;
					level2.ID = num;
					level2.MissionID = i;
					level2.width = level.width;
					level2.height = level.height;
					num++;
					list[i].levels.Add(level2);
				}
			}
			list[i].ID = i;
		}
		return list;
	}

	public List<Level> GetLevelFromCurrentMission()
	{
		return Missions[MissionCurrentID].levels;
	}

	public List<Level> GetLevelFromMissionByMissionID(int missionId)
	{
		if (missionId < Missions.Count)
		{
			return Missions[missionId].levels;
		}
		return null;
	}

	public Level GetCurrentLevelData()
	{
		List<Level> levels = Missions.Find((Mission x) => x.ID == MissionCurrentID).levels;
		return levels.Find((Level x) => x.ID == LevelCurrentID);
	}

	public void FinishLevel()
	{
		List<Level> levelFromCurrentMission = GetLevelFromCurrentMission();
		if (LevelCurrentID < levelFromCurrentMission.Count - 1)
		{
			SaveLevelState(LevelCurrentID + 1, MissionCurrentID, LevelState.IsOpen);
		}
		else
		{
			SaveMissionState(MissionCurrentID + 1, MissionState.IsOpen);
		}
	}

	public int GetTotalStar()
	{
		int num = 0;
		for (int i = 0; i < 7; i++)
		{
			List<Level> levelFromMissionByMissionID = GetLevelFromMissionByMissionID(i);
			if (levelFromMissionByMissionID != null)
			{
				foreach (Level item in levelFromMissionByMissionID)
				{
					LevelState levelState = GetLevelState(item.ID, i);
					switch (levelState)
					{
					case LevelState.IsOneStar:
					case LevelState.IsTwoStar:
					case LevelState.IsThreeStar:
						num = (int)(num + (levelState - 1));
						break;
					}
				}
			}
		}
		return num;
	}

	public int GetTotalLevel3Star()
	{
		int num = 0;
		for (int i = 0; i < 7; i++)
		{
			List<Level> levelFromMissionByMissionID = GetLevelFromMissionByMissionID(i);
			if (levelFromMissionByMissionID != null)
			{
				foreach (Level item in levelFromMissionByMissionID)
				{
					LevelState levelState = GetLevelState(item.ID, i);
					if (levelState == LevelState.IsThreeStar)
					{
						num++;
					}
				}
			}
		}
		return num;
	}

	public string GetStarMission(int missionID)
	{
		List<Level> levelFromMissionByMissionID = GetLevelFromMissionByMissionID(missionID);
		int num = 0;
		if (levelFromMissionByMissionID == null)
		{
			return string.Empty;
		}
		foreach (Level item in levelFromMissionByMissionID)
		{
			LevelState levelState = GetLevelState(item.ID, missionID);
			switch (levelState)
			{
			case LevelState.IsOneStar:
			case LevelState.IsTwoStar:
			case LevelState.IsThreeStar:
				num = (int)(num + (levelState - 1));
				break;
			}
		}
		return num + "/" + levelFromMissionByMissionID.Count * 3;
	}

	public void NextLevel()
	{
		List<Level> levelFromCurrentMission = GetLevelFromCurrentMission();
		if (LevelCurrentID < levelFromCurrentMission.Count - 1)
		{
			LevelCurrentID++;
			return;
		}
		MissionCurrentID++;
		LevelCurrentID = 0;
	}

	public bool isWin()
	{
		if (MissionCurrentID != 0 && Missions.Count - MissionCurrentID >= 2)
		{
			return false;
		}
		if (Missions[MissionCurrentID].levels.Count - LevelCurrentID >= 2)
		{
			return false;
		}
		return true;
	}

	public void SaveLevelState(int levelID, int missionID, LevelState state)
	{
		LevelState levelState = GetLevelState(levelID, missionID);
		if (state >= levelState)
		{
			string key = missionID + string.Empty + levelID;
			PlayerPrefs.SetInt(key, (int)state);
		}
	}

	public LevelState GetLevelState(int levelID, int missionID)
	{
		string key = missionID + string.Empty + levelID;
		int defaultValue = (levelID == 0) ? 1 : 0;
		return (LevelState)PlayerPrefs.GetInt(key, defaultValue);
	}

	public MissionState GetMissionState(int MissionID)
	{
		// if (MissionID == 0 || MissionID == 1)
		// {
		// 	return MissionState.IsOpen;
		// }
		// string key = "mission" + MissionID;
		// int defaultValue = (MissionID == 0 || MissionID == 1) ? 1 : 0;
		// try
		// {
		// 	return (MissionState)PlayerPrefs.GetInt(key, defaultValue);
		// }
		// catch (Exception)
		// {
		// 	return MissionState.IsLock;
		// }

		return MissionState.IsOpen;
	}

	public void SaveMissionState(int MissionID, MissionState state)
	{
		string key = "mission" + MissionID;
		PlayerPrefs.SetInt(key, (int)state);
	}
}
