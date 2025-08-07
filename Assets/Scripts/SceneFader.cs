using EventManager;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFader : SingletonMonoBehaviour<SceneFader>
{
	public enum FadeDirection
	{
		In,
		Out
	}

	public Image fadeOutUIImage;

	public float fadeSpeed = 0.8f;

	private void OnEnable()
	{
		StartCoroutine(Fade(FadeDirection.Out));
	}

	private IEnumerator Fade(FadeDirection fadeDirection)
	{
		float alpha = (fadeDirection == FadeDirection.Out) ? 1 : 0;
		float fadeEndValue = (fadeDirection != FadeDirection.Out) ? 1 : 0;
		if (fadeDirection == FadeDirection.Out)
		{
			while (alpha >= fadeEndValue)
			{
				SetColorImage(ref alpha, fadeDirection);
				yield return null;
			}
			fadeOutUIImage.enabled = false;
		}
		else
		{
			fadeOutUIImage.enabled = true;
			while (alpha <= fadeEndValue)
			{
				SetColorImage(ref alpha, fadeDirection);
				yield return null;
			}
		}
	}

	public IEnumerator FadeAndLoadScene(FadeDirection fadeDirection, string sceneToLoad)
	{
		yield return Fade(fadeDirection);
		EventDispatcher.ClearAllListener();
		SceneManager.LoadScene(sceneToLoad);
	}

	private void SetColorImage(ref float alpha, FadeDirection fadeDirection)
	{
		Image image = fadeOutUIImage;
		Color color = fadeOutUIImage.color;
		float r = color.r;
		Color color2 = fadeOutUIImage.color;
		float g = color2.g;
		Color color3 = fadeOutUIImage.color;
		image.color = new Color(r, g, color3.b, alpha);
		alpha += Time.deltaTime * (1f / fadeSpeed) * (float)((fadeDirection != FadeDirection.Out) ? 1 : (-1));
	}

	public void LoadScene(string sceneName)
	{
		StartCoroutine(FadeAndLoadScene(FadeDirection.In, sceneName));
	}
}
