using UnityEngine;

public class CellData
{
	public int posX;

	public int posY;

	public int cellType;

	public Vector3 posWord;

	public CellData(int _posx, int _posy, int _cellType, Vector3 _posWord)
	{
		posX = _posx;
		posY = _posy;
		cellType = _cellType;
		posWord = _posWord;
	}

	public CellData(CellItem item)
	{
		posX = item.posX;
		posY = item.posY;
		cellType = item.cellType;
		posWord = item.transform.localPosition;
	}
}
