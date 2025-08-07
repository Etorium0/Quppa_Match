using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LevelCompletePanel : SingletonMonoBehaviour<LevelCompletePanel>
{
	[SerializeField]
	private Text levelText;

	[SerializeField]
	private Text scoreText;

	private int score;

	[SerializeField]
	private GameObject star1;

	[SerializeField]
	private GameObject star2;

	[SerializeField]
	private GameObject star3;

	public void SetData(string level, int score, LevelState levelState)
	{
		levelText.text = level;
		int _score = 0;
		DOTween.To(() => _score, delegate(int x)
		{
			scoreText.text = x.ToString();
		}, score, 1f);
		star3.SetActive(value: false);
		star2.SetActive(value: false);
		star1.SetActive(value: false);
		star1.transform.localScale = Vector3.one * 4f;
		star2.transform.localScale = Vector3.one * 4f;
		star3.transform.localScale = Vector3.one * 4f;
		switch (levelState)
		{
		case LevelState.IsThreeStar:
			star1.SetActive(value: true);
			star1.transform.DOScale(Vector3.one, 0.5f).OnComplete(delegate
			{
				star2.SetActive(value: true);
			});
			star2.transform.DOScale(Vector3.one, 0.5f).SetDelay(0.5f).OnComplete(delegate
			{
				star3.SetActive(value: true);
			});
			star3.transform.DOScale(Vector3.one, 0.5f).SetDelay(1f);
			break;
		case LevelState.IsTwoStar:
			star2.SetActive(value: true);
			star1.SetActive(value: true);
			star1.transform.DOScale(Vector3.one, 0.5f).OnComplete(delegate
			{
				star2.SetActive(value: true);
			});
			star2.transform.DOScale(Vector3.one, 0.5f).SetDelay(0.5f);
			break;
		case LevelState.IsOneStar:
			star1.SetActive(value: true);
			star1.transform.DOScale(Vector3.one, 0.5f);
			break;
		}
	}
}
