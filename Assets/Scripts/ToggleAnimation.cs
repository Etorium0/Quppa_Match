using UnityEngine;
using UnityEngine.UI;

public class ToggleAnimation : ButtonAnimationController
{
	[SerializeField]
	private Sprite activeSprite;

	[SerializeField]
	private Sprite deactiveSprite;

	[SerializeField]
	private Image targetImage;

	private void OnEnable()
	{
		UpdateSprite();
	}

	public void OnClick()
	{
		SingletonMonoBehaviour<AudioManager>.Instance.Mute(SingletonMonoBehaviour<AudioManager>.Instance.IsMute());
		UpdateSprite();
	}

	private void UpdateSprite()
	{
		targetImage.overrideSprite = ((!SingletonMonoBehaviour<AudioManager>.Instance.IsMute()) ? activeSprite : deactiveSprite);
	}
}
