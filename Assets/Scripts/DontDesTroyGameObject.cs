using UnityEngine;

public class DontDesTroyGameObject : MonoBehaviour
{
	private void Awake()
	{
		Object.DontDestroyOnLoad(base.transform.gameObject);
	}
}
