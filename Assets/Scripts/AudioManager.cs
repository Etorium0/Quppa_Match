using Berry.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioListener))]
[RequireComponent(typeof(AudioSource))]
public class AudioManager : SingletonMonoBehaviour<AudioManager>
{
	[Serializable]
	public class MusicTrack
	{
		public string name;

		public AudioClip track;
	}

	[Serializable]
	public class Sound
	{
		public string name;

		public List<AudioClip> clips = new List<AudioClip>();
	}

	private AudioSource music;

	private AudioSource sfx;

	public List<MusicTrack> tracks = new List<MusicTrack>();

	public List<Sound> sounds = new List<Sound>();

	private static List<string> mixBuffer = new List<string>();

	private static float mixBufferClearDelay = 0.05f;

	private float music_volume_max = 1f;

	internal string currentTrack;

	public float musicVolume
	{
		get
		{
			return PlayerPrefs.GetFloat("Music Volume", 0f);
		}
		set
		{
			PlayerPrefs.SetFloat("Music Volume", value);
		}
	}

	public float sfxVolume
	{
		get
		{
			return PlayerPrefs.GetFloat("SFX Volume", 0f);
		}
		set
		{
			PlayerPrefs.SetFloat("SFX Volume", value);
		}
	}

	private Sound GetSoundByName(string name)
	{
		return sounds.Find((Sound x) => x.name == name);
	}

	public override void Awake()
	{
		AudioSource[] components = GetComponents<AudioSource>();
		sfx = components[0];
		GameObject gameObject = GameObject.Find("BGMusic");
		if ((bool)gameObject)
		{
			music = gameObject.GetComponent<AudioSource>();
		}
		else
		{
			music = base.gameObject.AddComponent<AudioSource>();
		}
		sfxVolume = sfxVolume;
		musicVolume = musicVolume;
		ChangeMusicVolume(musicVolume);
		ChangeSFXVolume(sfxVolume);
		StartCoroutine(MixBufferRoutine());
		base.Awake();
	}

	private IEnumerator MixBufferRoutine()
	{
		float time = 0f;
		while (true)
		{
			time += Time.unscaledDeltaTime;
			yield return 0;
			if (time >= mixBufferClearDelay)
			{
				mixBuffer.Clear();
				time = 0f;
			}
		}
	}

	public void PlayMusic(string trackName)
	{
		if (trackName != string.Empty)
		{
			currentTrack = trackName;
		}
		AudioClip audioClip = null;
		foreach (MusicTrack track in tracks)
		{
			if (track.name == trackName)
			{
				audioClip = track.track;
			}
		}
	}

	private IEnumerator CrossFade(AudioClip to)
	{
		float delay2 = 0.3f;
		if (music.clip != null)
		{
			while (delay2 > 0f)
			{
				music.volume = delay2 * musicVolume * music_volume_max;
				delay2 -= Time.unscaledDeltaTime;
				yield return 0;
			}
		}
		music.clip = to;
		if (to == null)
		{
			music.Stop();
			yield break;
		}
		delay2 = 0f;
		if (!music.isPlaying)
		{
			music.Play();
		}
		while (delay2 < 0.3f)
		{
			music.volume = delay2 * musicVolume * music_volume_max;
			delay2 += Time.unscaledDeltaTime;
			yield return 0;
		}
		music.volume = musicVolume * music_volume_max;
		music.loop = true;
	}

	public void Shot(string clip)
	{
		Sound soundByName = GetSoundByName(clip);
		if (soundByName != null && !mixBuffer.Contains(clip) && soundByName.clips.Count != 0)
		{
			mixBuffer.Add(clip);
			sfx.PlayOneShot(soundByName.clips.GetRandom());
		}
	}

	public void MusicOn(bool value)
	{
		ChangeMusicVolume((!value) ? 0.5f : 0f);
	}

	public void EffectOn(bool value)
	{
		ChangeSFXVolume((!value) ? 1f : 0f);
	}

	public void ChangeMusicVolume(float v)
	{
		musicVolume = v;
		music.volume = musicVolume * music_volume_max;
	}

	public void Mute(bool value)
	{
		ChangeMusicVolume((!value) ? 1f : 0f);
		ChangeSFXVolume((!value) ? 1f : 0f);
	}

	public bool IsMute()
	{
		return musicVolume == 0f && sfxVolume == 0f;
	}

	public void ChangeSFXVolume(float v)
	{
		sfxVolume = v;
		sfx.volume = sfxVolume;
	}
}
