using UnityEngine;
using UnityEngine.UI;

public class LevelItem : MonoBehaviour
{
	[SerializeField]
	private GameObject start1;

	[SerializeField]
	private GameObject start2;

	[SerializeField]
	private GameObject start3;

	[SerializeField]
	private Sprite lockImage;

	[SerializeField]
	private Sprite currentImage;

	[SerializeField]
	private Sprite passImage;

	[SerializeField]
	private Image imageBG;

	[SerializeField]
	private Text levelText;

	private Level level;

	private LevelState levelState;

	//private Color lockColor = new Color(0.9411765f, 0.9411765f, 0.9411765f);

	//private Color passColor = new Color(107f / 255f, 242f / 255f, 209f / 255f);

	//private Color currentColor = new Color(166f / 255f, 1f, 46f / 51f);

	public void OnClick()
	{
		SingletonMonoBehaviour<MissionManager>.Instance.LevelCurrentID = level.ID;
		StartCoroutine(SingletonMonoBehaviour<SceneFader>.Instance.FadeAndLoadScene(SceneFader.FadeDirection.In, "Game"));
	}

	public void SetData(Level level)
	{
		levelText.text = (level.ID + 1).ToString("D2");
		this.level = level;
		UpdateState();
	}

	private void OnEnable()
	{
		UpdateState();
	}

	private void UpdateState()
	{
		if (level != null)
		{
			levelState = SingletonMonoBehaviour<MissionManager>.Instance.GetLevelState(level.ID, level.MissionID);
			start3.SetActive(value: false);
			start2.SetActive(value: false);
			start1.SetActive(value: false);
			levelText.gameObject.SetActive(value: true);
			//levelText.color = passColor;
			switch (levelState)
			{
			case LevelState.IsOpen:
				imageBG.overrideSprite = currentImage;
				//levelText.color = currentColor;
				break;
			case LevelState.IsLock:
				imageBG.overrideSprite = lockImage;
				//levelText.color = lockColor;
				break;
			case LevelState.IsThreeStar:
				imageBG.overrideSprite = passImage;
				start3.SetActive(value: true);
				start2.SetActive(value: true);
				start1.SetActive(value: true);
				break;
			case LevelState.IsTwoStar:
				imageBG.overrideSprite = passImage;
				start3.SetActive(value: false);
				start2.SetActive(value: true);
				start1.SetActive(value: true);
				break;
			case LevelState.IsOneStar:
				imageBG.overrideSprite = passImage;
				start3.SetActive(value: false);
				start2.SetActive(value: false);
				start1.SetActive(value: true);
				break;
			}
		}
	}
}
