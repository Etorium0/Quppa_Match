using EventManager;
using UnityEngine;
using UnityEngine.UI;

public class CoinCount : MonoBehaviour
{
	private Text coinText;

	public CountType countType;

	private void Awake()
	{
		coinText = GetComponent<Text>();
	}

	private void OnEnable()
	{
		UpdateText();
		this.RegisterListener(EventID.UpdateCoin, delegate
		{
			UpdateText();
		});
		this.RegisterListener(EventID.UpdateHint, delegate
		{
			UpdateText();
		});
		this.RegisterListener(EventID.UpdateSwap, delegate
		{
			UpdateText();
		});
	}

	private void OnDisable()
	{
		this.RemoveListener(EventID.UpdateCoin, delegate
		{
			UpdateText();
		});
		this.RemoveListener(EventID.UpdateHint, delegate
		{
			UpdateText();
		});
		this.RemoveListener(EventID.UpdateSwap, delegate
		{
			UpdateText();
		});
	}

	private void UpdateText()
	{
		switch (countType)
		{
		case CountType.Coin:
			coinText.text = ((GameDataMgr.Coin <= 0) ? "0" : (GameDataMgr.Coin + string.Empty));
			break;
		case CountType.Swap:
			coinText.text = ((GameDataMgr.Swap <= 0) ? "0" : (GameDataMgr.Swap + string.Empty));
			break;
		case CountType.Hint:
			coinText.text = ((GameDataMgr.Hint <= 0) ? "0" : (GameDataMgr.Hint + string.Empty));
			break;
		}
	}
}
