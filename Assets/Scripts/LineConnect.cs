using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

[RequireComponent(typeof(UILineRenderer))]
public class LineConnect : MonoBehaviour
{
	[SerializeField]
	private UILineRenderer line;

	public void DrawLine(List<CellData> data)
	{
		line.Points = new Vector2[data.Count];
		for (int i = 0; i < data.Count; i++)
		{
			line.Points[i] = data[i].posWord;
		}
		base.gameObject.SetActive(value: true);
		StartCoroutine(DeActive());
	}

	private IEnumerator DeActive()
	{
		yield return new WaitForSeconds(0.2f);
		base.gameObject.SetActive(value: false);
	}
}
