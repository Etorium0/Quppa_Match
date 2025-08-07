using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T instance;

	public static T Instance
	{
		get
		{
			if ((Object)instance == (Object)null)
			{
				instance = (T)UnityEngine.Object.FindObjectOfType(typeof(T));
				if ((Object)instance == (Object)null)
				{
					GameObject gameObject = new GameObject();
					instance = gameObject.AddComponent<T>();
					gameObject.name = typeof(T).ToString();
				}
			}
			return instance;
		}
	}

	public virtual void Awake()
	{
		CheckInstance();
	}

	protected bool CheckInstance()
	{
		if (this == Instance)
		{
			return true;
		}
		UnityEngine.Object.Destroy(this);
		return false;
	}
}
