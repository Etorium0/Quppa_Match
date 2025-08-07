using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonAnimationController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
{
	[SerializeField]
	private Transform buttonImg;

	public float imgHeight;

	public void OnPointerDown(PointerEventData data)
	{
		buttonImg.DOLocalMove(new Vector3(0f, 0f, 0f), 0.2f);
	}

	public void OnPointerUp(PointerEventData data)
	{
		buttonImg.DOLocalMove(new Vector3(0f, imgHeight, 0f), 0.2f);
	}
}
