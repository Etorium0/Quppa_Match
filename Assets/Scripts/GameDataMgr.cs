using UnityEngine;

public class GameDataMgr
{
	private const string HighScorekey = "HighScoreKEY";

	private const string Hintkey = "HintKEY";

	private const string SwapKey = "SwapKey";

	private const string CoinKey = "CoinKey";

	private const string scoreTotalClassickKEY = "scoreTotalClassickKEY";

	private const string pikachuKEY = "pikachuKEY";

	public static int HighScore
	{
		get
		{
			return GameDataManager.Instance.userData.data.Data[DataType.HIGH_SCORE];
		}
		set
		{
			if (value > GameDataManager.Instance.userData.data.Data[DataType.HIGH_SCORE])
			{
                GameDataManager.Instance.UpdateData(DataType.HIGH_SCORE, value);
			}
		}
	}

	public static int Hint
	{
		get
		{
			return GameDataManager.Instance.userData.data.Data[DataType.HINT];
		}
		set
		{
            GameDataManager.Instance.UpdateData(DataType.HINT, value);
        }
	}

	public static int Swap
	{
		get
		{
			return GameDataManager.Instance.userData.data.Data[DataType.SWAP];
        }
		set
		{
            GameDataManager.Instance.UpdateData(DataType.SWAP, value);
        }
	}

	public static int Coin
	{
		get
		{
			return GameDataManager.Instance.userData.balance;
		}
		set
		{
			GameDataManager.Instance.userData.balance = value;
			GameDataManager.Instance.UpdateData();

        }
	}

	public static int scoreTotalClassic
	{
		get
		{
			return GameDataManager.Instance.userData.data.Data[DataType.TOTAL_SCORE];
        }
		set
		{
			GameDataManager.Instance.UpdateData(DataType.TOTAL_SCORE, value);
		}
	}

	public static int PikachuCount
	{
		get
		{
			return GameDataManager.Instance.userData.data.Data[DataType.PIKACHU];
		}
		set
		{
			GameDataManager.Instance.UpdateData(DataType.PIKACHU, value);
		}
	}
}
