using UnityEngine;
using UnityEngine.UI;

public class LevelFailedPanel : SingletonMonoBehaviour<LevelFailedPanel>
{
	[SerializeField]
	private Text levelText;

	[SerializeField]
	private Text scoreText;

	[SerializeField]
	private Text titleText;

	public void SetData(string level, int score, bool isTimeUp)
	{
		scoreText.text = score.ToString();
	}
}
