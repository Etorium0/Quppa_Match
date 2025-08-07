using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class AndroidBackButton : MonoBehaviour
{
	private Button button;

	private void Awake()
	{
		button = GetComponent<Button>();
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
		{
			button.onClick.Invoke();
		}
	}
}
