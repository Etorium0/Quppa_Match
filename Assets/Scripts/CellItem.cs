using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CellItem : MonoBehaviour
{
	public int posX;

	public int posY;

	public int cellType;

	private GameController gameController;

	private bool isSelected;

	[SerializeField]
	private GameObject selectObj;

	[SerializeField]
	private Image itemImage;

	public void Init(GameController _gameController, int _posx, int _posy, int _cellType, Sprite sprite)
	{
		selectObj.SetActive(value: false);
		selectObj.transform.DOKill();
		posX = _posx;
		posY = _posy;
		cellType = _cellType;
		gameController = _gameController;
		itemImage.sprite = sprite;
		isSelected = false;
		Vector2 sizeDelta = GetComponent<RectTransform>().sizeDelta;
		SetPos();
	}

	private void SetPos()
	{
		Vector2 pos = gameController.GetPos(posX, posY);
		base.transform.localPosition = pos;
	}

	public void OnPressItem()
	{
		if (SingletonMonoBehaviour<GameMgr>.Instance.isPlay)
		{
			selectObj.transform.DOKill();
			selectObj.SetActive(value: false);
			isSelected = !isSelected;
			if (isSelected)
			{
				OnSelected();
			}
			else
			{
				OnDeselected();
			}
			gameController.OnPressItem(this);
		}
	}

	public void Hint()
	{
		if (SingletonMonoBehaviour<GameMgr>.Instance.isPlay)
		{
			selectObj.SetActive(value: true);
			selectObj.transform.DOScale(Vector2.one * 0.9f, 0.5f).SetLoops(-1, LoopType.Yoyo);
		}
	}

	public void UpdateSprite(Sprite sprite)
	{
		selectObj.SetActive(value: false);
		itemImage.sprite = sprite;
	}

	public void OnSelected()
	{
		selectObj.SetActive(value: true);
	}

	public void OnDeselected()
	{
		selectObj.SetActive(value: false);
		isSelected = false;
	}

	public void MoveItem(Move move)
	{
		switch (move)
		{
		case Move.Up:
			posY++;
			break;
		case Move.Dow:
			posY--;
			break;
		case Move.Right:
			posX++;
			break;
		case Move.Left:
			posX--;
			break;
		}
		MoveAnimation();
	}

	private void MoveAnimation()
	{
		Vector2 pos = gameController.GetPos(posX, posY);
		base.transform.DOLocalMove(pos, 0.3f);
	}
}
