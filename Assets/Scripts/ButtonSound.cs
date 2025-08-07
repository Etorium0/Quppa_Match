using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
	private Toggle toggle;

	[SerializeField]
	private bool isMusic;

	private void Awake()
	{
		Button component = GetComponent<Button>();
		toggle = GetComponent<Toggle>();
		if (component != null)
		{
			component.onClick.AddListener(delegate
			{
				SingletonMonoBehaviour<AudioManager>.Instance.Shot("button");
			});
		}
	}

	private void OnEnable()
	{
		if (toggle != null)
		{
			toggle.isOn = ((!isMusic) ? (SingletonMonoBehaviour<AudioManager>.Instance.sfxVolume == 0f) : (SingletonMonoBehaviour<AudioManager>.Instance.musicVolume == 0f));
		}
	}
}
