using DG.Tweening;
using EventManager;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : MonoBehaviour
{
	[SerializeField]
	private Image timeProgress;

	[SerializeField]
	private Text scoreText;

	[SerializeField]
	private Text resetCountText;

	[SerializeField]
	private Text levelText;

	[SerializeField]
	private Transform arrow;

	[SerializeField]
	private Transform textMes;

	private void Awake()
	{
		this.RegisterListener(EventID.UpdateTime, delegate(Component sender, object param)
		{
			UpdateTime((float)param);
		});
		this.RegisterListener(EventID.UpdateLevel, delegate(Component sender, object param)
		{
			UpdateLevel((int)param);
		});
		this.RegisterListener(EventID.UpdateScoreTotal, delegate(Component sender, object param)
		{
			UpdateScoreTotal((int)param);
		});
		this.RegisterListener(EventID.UpdateResetCount, delegate(Component sender, object param)
		{
			UpdateResetCount((int)param);
		});
		this.RegisterListener(EventID.NothingMatch, delegate
		{
			NothingMatch();
		});
	}

	private void OnDestroy()
	{
		this.RemoveListener(EventID.UpdateTime, delegate(Component sender, object param)
		{
			UpdateTime((float)param);
		});
		this.RemoveListener(EventID.UpdateLevel, delegate(Component sender, object param)
		{
			UpdateLevel((int)param);
		});
		this.RemoveListener(EventID.UpdateScoreTotal, delegate(Component sender, object param)
		{
			UpdateScoreTotal((int)param);
		});
		this.RemoveListener(EventID.UpdateResetCount, delegate(Component sender, object param)
		{
			UpdateResetCount((int)param);
		});
		this.RemoveListener(EventID.NothingMatch, delegate
		{
			NothingMatch();
		});
	}

	private void NothingMatch()
	{
		textMes.localPosition = Vector3.zero;
		textMes.localScale = Vector3.one;
		textMes.gameObject.SetActive(value: true);
		textMes.DOMove(resetCountText.transform.position, 0.5f).SetDelay(1f);
		textMes.DOScale(Vector3.zero, 0.5f).SetDelay(1f);
	}

	private void UpdateTime(float time)
	{
		timeProgress.fillAmount = time;
	}

	private void UpdateScoreTotal(int score)
	{
		scoreText.text = score.ToString("D4");
	}

	private void UpdateLevel(int level)
	{
		levelText.text = "LEVEL " + level.ToString();
	}

	private void UpdateResetCount(int count)
	{
		resetCountText.text = count.ToString();
	}

	private void Update()
	{
		if (SingletonMonoBehaviour<GameMgr>.Instance.isPlay)
		{
			arrow.Rotate(new Vector3(0f, 0f, -5f));
		}
	}
}
