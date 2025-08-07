using UnityEngine;

public class MoreGameButton : MonoBehaviour
{
	[SerializeField]
	private GameObject anim;

	public int Clicked
	{
		get
		{
			return PlayerPrefs.GetInt("MoreGameClick", 0);
		}
		set
		{
			PlayerPrefs.SetInt("MoreGameClick", value);
		}
	}

	private void OnEnable()
	{
		anim.SetActive(Clicked != 1);
	}

	private void OnDisable()
	{
	}

	private void UpdateMoreGame(string link)
	{
		Clicked = 0;
	}

	public void OnPressMoreGameButton()
	{
		
		Clicked = 1;
		anim.SetActive(Clicked != 1);
	}
}
