using EventManager;
using UnityEngine;
using UnityEngine.UI;

public class WinPanel : MonoBehaviour
{
	[SerializeField]
	private Text scoreText;

	private void OnEnable()
	{
		this.RegisterListener(EventID.UpdateScoreTotal, delegate(Component sender, object param)
		{
			scoreText.text = string.Empty + (int)param;
		});
	}

	private void OnDisable()
	{
		this.RemoveListener(EventID.UpdateScoreTotal, delegate(Component sender, object param)
		{
			scoreText.text = string.Empty + (int)param;
		});
	}
}
