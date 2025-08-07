using System.Collections;
using UnityEngine;

public class UIPanel : MonoBehaviour
{
	public static int uiAnimation;

	public bool freez;

	private bool _isPlaying;

	public string hide;

	public string show;

	private string currentClip = string.Empty;

	private Animation anim;

	public bool isPlaying
	{
		get
		{
			return _isPlaying;
		}
		set
		{
			if (_isPlaying != value)
			{
				_isPlaying = value;
				uiAnimation += (_isPlaying ? 1 : (-1));
			}
		}
	}

	private void Awake()
	{
		anim = GetComponent<Animation>();
	}

	private void OnEnable()
	{
		freez = false;
	}

	public void SetVisible(bool visible, bool immediate = false)
	{
		if (base.gameObject.activeSelf == visible)
		{
			return;
		}
		currentClip = string.Empty;
		if (!visible)
		{
			if (!(hide != string.Empty))
			{
				base.gameObject.SetActive(value: false);
				return;
			}
			currentClip = hide;
		}
		if (visible)
		{
			base.gameObject.SetActive(value: true);
			if (!(show != string.Empty))
			{
				return;
			}
			currentClip = show;
		}
		if (!(currentClip == string.Empty))
		{
			if (immediate)
			{
				anim[currentClip].time = anim[currentClip].length;
			}
			else
			{
				Play(currentClip);
			}
		}
	}

	private void Play(string clip)
	{
		StartCoroutine(PlayClipRoutine(clip));
	}

	public void PlayClip(string clip)
	{
		if (!isPlaying)
		{
			Play(clip);
		}
	}

	private IEnumerator PlayClipRoutine(string clip)
	{
		isPlaying = true;
		anim.Play(clip);
		anim[clip].time = 0f;
		while (anim[clip].time < anim[clip].length)
		{
			anim[clip].enabled = true;
			anim[clip].time += Mathf.Min(Time.unscaledDeltaTime, Time.maximumDeltaTime);
			anim.Sample();
			anim[clip].enabled = false;
			yield return 0;
		}
		isPlaying = false;
		if (clip == hide)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
