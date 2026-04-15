using EventManager;
using UnityEngine;

public class GameMgr : SingletonMonoBehaviour<GameMgr>
{
	[SerializeField]
	private GameController gameController;

	private int levelIndex;

	private float timePlay;

	public float timeCurrent;

	private int scoreTotal;

	public int resetCount;

	private int levelMax;

	private Level currentLevel;

	private int lifeCount = 5;

	private MoveMode moveMode;

	private LevelState levelState;

	private int revieveCount;

	public bool isPlay;

	private void OnEnable()
	{
		this.RegisterListener(EventID.UpdateScore, delegate(Component sender, object param)
		{
			UpdateScore((int)param);
		});
		PlayGameFromLevel();
	}

	private void OnDisable()
	{
		this.RemoveListener(EventID.UpdateScore, delegate(Component sender, object param)
		{
			UpdateScore((int)param);
		});
	}

	private bool IsClassic()
	{
		return SingletonMonoBehaviour<MissionManager>.Instance.MissionCurrentID == 0;
	}

	public void PlayGameFromLevel()
	{
		scoreTotal = (IsClassic() ? GameDataMgr.scoreTotalClassic : 0);
		resetCount = lifeCount;
		revieveCount = 0;
		SingletonMonoBehaviour<UIManager>.Instance.ShowPage("GamePage");
		currentLevel = SingletonMonoBehaviour<MissionManager>.Instance.GetCurrentLevelData();
		moveMode = currentLevel.moveMode;
		levelIndex = currentLevel.ID;
		gameController.InitMap(currentLevel, moveMode);
		int num = (moveMode != 0) ? 4 : 3;
		timeCurrent = (timePlay = currentLevel.width * currentLevel.height * num);
		isPlay = true;
		this.PostEvent(EventID.UpdateLevel, levelIndex + 1);
		this.PostEvent(EventID.UpdateHint);
		this.PostEvent(EventID.UpdateSwap);
		this.PostEvent(EventID.UpdateScore, 0);
		resetCount = lifeCount;
		this.PostEvent(EventID.UpdateResetCount, resetCount);
	}

	public void ReplayGame()
	{
		//***
       // Advertisements.Instance.ShowInterstitial();
        if (IsClassic())
		{
			GameDataMgr.scoreTotalClassic = 0;
			SingletonMonoBehaviour<MissionManager>.Instance.LevelCurrentID = 0;
		}
		StartCoroutine(SingletonMonoBehaviour<SceneFader>.Instance.FadeAndLoadScene(SceneFader.FadeDirection.In, "Game"));
	}

	public void NextLevel()
	{
		//***
        //Advertisements.Instance.ShowInterstitial();
        SingletonMonoBehaviour<MissionManager>.Instance.NextLevel();
		StartCoroutine(SingletonMonoBehaviour<SceneFader>.Instance.FadeAndLoadScene(SceneFader.FadeDirection.In, "Game"));
	}

	public void UpdateResetCount()
	{
		resetCount--;
		if (resetCount < 0)
		{
			LevelFailed(isTimesup: false);
		}
		else
		{
			this.PostEvent(EventID.UpdateResetCount, resetCount);
		}
	}

	private void Update()
	{
		if (isPlay)
		{
			timeCurrent -= Time.deltaTime;
			this.PostEvent(EventID.UpdateTime, timeCurrent / timePlay);
			if (timeCurrent <= 0f)
			{
				LevelFailed(isTimesup: true);
			}
		}
	}

	public void UpdateScore(int score)
	{
		scoreTotal += score;
		if (GameDataMgr.HighScore < scoreTotal)
		{
			GameDataMgr.HighScore = scoreTotal;
		}
		this.PostEvent(EventID.UpdateScoreTotal, scoreTotal);
	}

	public void PauseGame()
	{
		SingletonMonoBehaviour<UIManager>.Instance.ShowPage("PausePage");
		isPlay = false;
	}

	public void ResumeGame()
	{
		SingletonMonoBehaviour<UIManager>.Instance.ShowPage("GamePage");
		isPlay = true;
	}

	public void OnPressHomeButton()
	{
		//***
       // Advertisements.Instance.ShowInterstitial(); 
        if (SingletonMonoBehaviour<MissionManager>.Instance.MissionCurrentID == 0)
		{
			SingletonMonoBehaviour<SceneFader>.Instance.LoadScene("Mission");
		}
		else
		{
			SingletonMonoBehaviour<SceneFader>.Instance.LoadScene("Level");
		}
	}

	public void LeveFinish()
	{
		isPlay = false;
		float num = (moveMode != 0) ? 1.5f : 1f;
		scoreTotal += (int)(timeCurrent * 10f * num);
		GameDataMgr.scoreTotalClassic = scoreTotal;
		this.PostEvent(EventID.UpdateScoreTotal, scoreTotal);
		levelState = GetLevelStateFromTime();
		SingletonMonoBehaviour<MissionManager>.Instance.FinishLevel();
		SingletonMonoBehaviour<MissionManager>.Instance.SaveLevelState(currentLevel.ID, currentLevel.MissionID, levelState);
		if (SingletonMonoBehaviour<MissionManager>.Instance.isWin())
		{
			isPlay = false;
			SingletonMonoBehaviour<UIManager>.Instance.ShowPage("WinPage");
			SingletonMonoBehaviour<AudioManager>.Instance.Shot("completed");
			this.PostEvent(EventID.UpdateScoreTotal, scoreTotal);
	
		}
		else
		{
			float value = SingletonMonoBehaviour<AudioManager>.Instance.musicVolume;
			SingletonMonoBehaviour<AudioManager>.Instance.musicVolume = 0f;
			//SingletonMonoBehaviour<API>.Instance.ShowFull(delegate
			//{
				SingletonMonoBehaviour<AudioManager>.Instance.musicVolume = value;
				SingletonMonoBehaviour<AudioManager>.Instance.Shot("completed");
				SingletonMonoBehaviour<UIManager>.Instance.ShowPage("LevelCompleted");
				SingletonMonoBehaviour<LevelCompletePanel>.Instance.SetData((currentLevel.ID + 1).ToString(), scoreTotal, levelState);
            //if (!SingletonMonoBehaviour<UIManager>.Instance.isRate && SingletonMonoBehaviour<MissionManager>.Instance.LevelCurrentID > 3 && Random.Range(0, 99) > 70)
            //{
            //	SingletonMonoBehaviour<UIManager>.Instance.ShowPage("RateUsPage");
            //}
            //});
            //AdsManager.Instance.ShowInterstitialAd();
        }
		PostScoreToLeaderBoard();
	}

	private void PostScoreToLeaderBoard()
	{
		int totalStar = SingletonMonoBehaviour<MissionManager>.Instance.GetTotalStar();
		//SingletonMonoBehaviour<LeaderBoardMgr>.Instance.PostHighScoreToLeaderBoard(0, totalStar);
		string achivementID = string.Empty;
		if (totalStar > 10)
		{
			achivementID = "CgkI08PEjZEEEAIQBA";
		}
		if (totalStar > 20)
		{
			achivementID = "CgkI08PEjZEEEAIQBQ";
		}
		if (totalStar > 50)
		{
			achivementID = "CgkI08PEjZEEEAIQBg";
		}
		if (totalStar > 100)
		{
			achivementID = "CgkI08PEjZEEEAIQBw";
		}
		//SingletonMonoBehaviour<LeaderBoardMgr>.Instance.ReportProgresstAchievement(achivementID, 100);
		AchievementStar();
	}

	private void AchievementStar()
	{
		int totalLevel3Star = SingletonMonoBehaviour<MissionManager>.Instance.GetTotalLevel3Star();
		string achivementID = string.Empty;
		if (totalLevel3Star > 10)
		{
			achivementID = "CgkI08PEjZEEEAIQCA";
		}
		if (totalLevel3Star > 20)
		{
			achivementID = "CgkI08PEjZEEEAIQCQ";
		}
		if (totalLevel3Star > 50)
		{
			achivementID = "CgkI08PEjZEEEAIQCg";
		}
		//SingletonMonoBehaviour<LeaderBoardMgr>.Instance.ReportProgresstAchievement(achivementID, 100);
	}

	public void AchievementPikachu()
	{
		int pikachuCount = GameDataMgr.PikachuCount;
		string achivementID = string.Empty;
		if (pikachuCount > 10)
		{
			achivementID = "CgkI08PEjZEEEAIQCw";
		}
		if (pikachuCount > 20)
		{
			achivementID = "CgkI08PEjZEEEAIQDA";
		}
		if (pikachuCount > 30)
		{
			achivementID = "CgkI08PEjZEEEAIQDQ";
		}
		//SingletonMonoBehaviour<LeaderBoardMgr>.Instance.ReportProgresstAchievement(achivementID, 100);
	}

	private LevelState GetLevelStateFromTime()
	{
		LevelState result = LevelState.IsOpen;
		if (timeCurrent / timePlay >= 0.3f)
		{
			result = LevelState.IsThreeStar;
		}
		else if (timeCurrent / timePlay >= 0.2f)
		{
			result = LevelState.IsTwoStar;
		}
		else if (timeCurrent / timePlay >= 0f)
		{
			result = LevelState.IsOneStar;
		}
		return result;
	}

	public void Revive(bool isTimesup)
	{
		//string message = (!isTimesup) ? "Get 2 lives" : "Get 60s Plus";
		//SingletonMonoBehaviour<DialogMessage>.Instance.OpenDialog("You're revived!", message, delegate
		//{
			//SingletonMonoBehaviour<UIManager>.Instance.ShowPage("GamePage");
			isPlay = true;
			if (isTimesup)
			{
				timeCurrent += 60f;
			}
			else
			{
				resetCount += 2;
				this.PostEvent(EventID.UpdateResetCount, resetCount);
			}
		//});
	}
    private bool aaaa;
	public void LevelFailed(bool isTimesup)
	{
		isPlay = false;
		if (revieveCount < 1)
		{
			string title = (!isTimesup) ? "Out of swap" : "Time's up!";
			string message = (!isTimesup) ? "You fail" : "You fail";
			SingletonMonoBehaviour<DialogMessage>.Instance.OpenDialog(title, message, "Cancel", "Watch", delegate
			{
                aaaa = isTimesup;
   //             Advertisements.Instance.ShowRewardedVideo(CompleteMethod);
               
			}, delegate
			{
				ShowLevelFailed(isTimesup);
			});
			revieveCount++;
		}
		else
		{
			ShowLevelFailed(isTimesup);
		}
	}
    //private void CompleteMethod(bool completed, string advertiser)
    //{
    //    Debug.Log("Closed rewarded from: " + advertiser + " -> Completed " + completed);
    //    if (completed == true)
    //    {
    //        //give the reward
    //        Revive(aaaa);
    //    }
    //    else
    //    {
    //        //no reward
    //    }
    //}

    private void ShowLevelFailed(bool isTimesup)
	{
		//AdsManager.Instance.ShowInterstitialAd();
        float value = SingletonMonoBehaviour<AudioManager>.Instance.musicVolume;
		SingletonMonoBehaviour<AudioManager>.Instance.musicVolume = 0f;
		//SingletonMonoBehaviour<API>.Instance.ShowFull(delegate
		//{
			SingletonMonoBehaviour<AudioManager>.Instance.musicVolume = value;
			SingletonMonoBehaviour<AudioManager>.Instance.Shot("failed");
			SingletonMonoBehaviour<UIManager>.Instance.ShowPage("LevelFailed");
			SingletonMonoBehaviour<LevelFailedPanel>.Instance.SetData((currentLevel.ID + 1).ToString(), scoreTotal, isTimesup);
		//});
	}
}
