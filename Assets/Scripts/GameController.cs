using Battlehub.Dispatcher;
using DG.Tweening;
using EventManager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : SingletonMonoBehaviour<GameController>
{
    public Transform gridParent;

    private CellData[,] cellDatas;

    private List<CellItem> cellItems = new List<CellItem>();

    [SerializeField]
    private LineConnect line;

    public int width;

    public int height;

    private CellItem firstSelected;

    public float cellWidth;

    public float cellHeight;

    private MoveMode moveMode;

    [SerializeField]
    private RectTransform rectGrid;

    [SerializeField]
    private Sprite[] sprites;

    [SerializeField]
    private Transform hintButton;

    [SerializeField]
    private Transform swapButton;

    private float TimeDefault = 15f;

    private bool isSugget;

    private bool isCountTime;

    private float countTime = 5f;

    private void SuggetHint()
    {
        isSugget = true;
        if (GameDataMgr.Hint > 0)
        {
            hintButton.DOScale(Vector3.one * 1.1f, 0.5f).SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            swapButton.DOScale(Vector3.one * 1.1f, 0.5f).SetLoops(-1, LoopType.Yoyo);
        }
    }

    private void DestroySugget()
    {
        isSugget = false;
        hintButton.DOKill();
        swapButton.DOKill();
        hintButton.localScale = Vector3.one;
        swapButton.localScale = Vector3.one;
    }

    public void InitMap(Level levelData, MoveMode _moveMode)
    {
        isCountTime = true;
        countTime = TimeDefault;
        Reset();
        width = levelData.height + 2;
        height = levelData.width + 2;
        moveMode = _moveMode;
        CheckShowBanner(string.Empty);
        GenMap();
        SortList();
    }

    private void Reset()
    {
        firstSelected = null;
        line.gameObject.SetActive(value: false);
        if (cellItems == null)
        {
            cellItems = new List<CellItem>();
        }
        foreach (CellItem cellItem in cellItems)
        {
            if (cellItem.gameObject.activeSelf)
            {
                SingletonMonoBehaviour<ContentMgr>.Instance.Despaw(cellItem.gameObject);
            }
        }
        cellItems.Clear();
    }

    public Vector2 GetPos(int posX, int posY)
    {
        float num = 0f;
        if (height == 9 || height == 8)
        {
            num = cellHeight / 2f;
        }
        float x = (cellWidth - 13f) * ((float)posX - (float)(width - 2) / 2f - 0.5f);
        float y = (cellHeight - 13f) * ((float)posY - (float)(height - 2) / 2f - 0.5f) + num;
        return new Vector2(x, y);
    }

    private void GenMap()
    {
        if (width * height % 2 != 0)
        {
            UnityEngine.Debug.LogError("Error");
            return;
        }
        cellDatas = new CellData[width, height];
        cellWidth = 130f;
        cellHeight = 130f;
        int num = (width - 2) * (height - 2);
        List<int> list = new List<int>();
        int num2 = (num / 2 < sprites.Length) ? 1 : (num / 2 / sprites.Length + 1);
        for (int i = 0; i < num / 2; i++)
        {
            int value = -1;
            do
            {
                value = UnityEngine.Random.Range(0, sprites.Length);
            }
            while (list.FindAll((int x) => x == value).Count() > num2);
            list.Add(value);
            list.Insert(0, value);
        }
        for (int j = 0; j < num; j++)
        {
            int index = UnityEngine.Random.Range(0, num);
            int index2 = UnityEngine.Random.Range(0, num);
            int value2 = list[index];
            list[index] = list[index2];
            list[index2] = value2;
        }
        for (int k = 0; k < width; k++)
        {
            for (int l = 0; l < height; l++)
            {
                Vector2 pos = GetPos(k, l);
                cellDatas[k, l] = new CellData(k, l, -1, pos);
            }
        }
        int num3 = 0;
        for (int m = 1; m < height - 1; m++)
        {
            for (int num4 = width - 2; num4 > 0; num4--)
            {
                int num5 = list[num3];
                num3++;
                CellItem item = CreateItem(num5, num4, m);
                cellItems.Add(item);
                cellDatas[num4, m].cellType = num5;
            }
        }
        if (!CheckAnswer())
        {
            OnSwap();
        }
    }

    private void SortList()
    {
        cellItems = (from x in cellItems
                     orderby x.posY descending, x.posX
                     select x).ToList();
        for (int i = 0; i < cellItems.Count; i++)
        {
            cellItems[i].transform.SetSiblingIndex(i);
        }
    }

    private CellItem CreateItem(int type, int i, int j)
    {
        GameObject item = SingletonMonoBehaviour<ContentMgr>.Instance.GetItem("Cell");
        item.transform.SetParent(gridParent);
        item.GetComponent<RectTransform>().sizeDelta = new Vector2(1f * cellWidth, 1f * cellHeight);
        item.transform.localScale = Vector3.one;
        item.name = (i + string.Empty + j).ToString();
        CellItem component = item.GetComponent<CellItem>();
        Sprite sprite = sprites[type];
        component.Init(this, i, j, type, sprite);
        return component;
    }

    public void OnPressItem(CellItem item)
    {
        if (firstSelected == null)
        {
            firstSelected = item;
            return;
        }
        if (firstSelected.posX == item.posX && firstSelected.posY == item.posY)
        {
            firstSelected = null;
            return;
        }
        List<CellData> list = CheckPath(firstSelected, item);
        if (list != null)
        {
            OnPair(list, firstSelected, item);
            return;
        }
        SingletonMonoBehaviour<AudioManager>.Instance.Shot("false");
        firstSelected.OnDeselected();
        item.OnDeselected();
        firstSelected = null;
    }

    private IEnumerator UpdateCellData()
    {
        yield return new WaitForSeconds(0.5f);
        CellData[,] array = cellDatas;
        int length = array.GetLength(0);
        int length2 = array.GetLength(1);
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < length2; j++)
            {
                CellData item = array[i, j];
                CellItem cellItem = cellItems.Find((CellItem x) => x.posX == item.posX && x.posY == item.posY);
                if ((bool)cellItem)
                {
                    item.cellType = cellItem.cellType;
                }
                else
                {
                    item.cellType = -1;
                }
            }
        }
        if (!CheckAnswer())
        {
            this.PostEvent(EventID.NothingMatch);
            yield return new WaitForSeconds(1f);
            SingletonMonoBehaviour<GameMgr>.Instance.UpdateResetCount();
            OnSwap();
        }
    }

    private IEnumerator MoveItem(CellItem firstItem, CellItem secondItem, float time)
    {
        yield return new WaitForSeconds(time);
        float mid = (float)(width - 1) / 2f;
        switch (moveMode)
        {
            case MoveMode.Up:
                {
                    List<CellItem> list2;
                    if (firstItem.posY > secondItem.posY)
                    {
                        list2 = cellItems.FindAll((CellItem x) => x.posY < secondItem.posY && x.posX == secondItem.posX);
                        list2.AddRange(cellItems.FindAll((CellItem x) => x.posY < firstItem.posY && x.posX == firstItem.posX));
                    }
                    else
                    {
                        list2 = cellItems.FindAll((CellItem x) => x.posY < firstItem.posY && x.posX == firstItem.posX);
                        list2.AddRange(cellItems.FindAll((CellItem x) => x.posY < secondItem.posY && x.posX == secondItem.posX));
                    }
                    MoveItemList(list2, Move.Up);
                    break;
                }
            case MoveMode.Right:
                {
                    List<CellItem> list2;
                    if (firstItem.posX > secondItem.posX)
                    {
                        list2 = cellItems.FindAll((CellItem x) => x.posX < secondItem.posX && x.posY == secondItem.posY);
                        list2.AddRange(cellItems.FindAll((CellItem x) => x.posX < firstItem.posX && x.posY == firstItem.posY));
                    }
                    else
                    {
                        list2 = cellItems.FindAll((CellItem x) => x.posX < firstItem.posX && x.posY == firstItem.posY);
                        list2.AddRange(cellItems.FindAll((CellItem x) => x.posX < secondItem.posX && x.posY == secondItem.posY));
                    }
                    MoveItemList(list2, Move.Right);
                    break;
                }
            case MoveMode.Dow:
                {
                    List<CellItem> list2;
                    if (firstItem.posY > secondItem.posY)
                    {
                        list2 = cellItems.FindAll((CellItem x) => x.posY > secondItem.posY && x.posX == secondItem.posX);
                        list2.AddRange(cellItems.FindAll((CellItem x) => x.posY > firstItem.posY && x.posX == firstItem.posX));
                    }
                    else
                    {
                        list2 = cellItems.FindAll((CellItem x) => x.posY > firstItem.posY && x.posX == firstItem.posX);
                        list2.AddRange(cellItems.FindAll((CellItem x) => x.posY > secondItem.posY && x.posX == secondItem.posX));
                    }
                    MoveItemList(list2, Move.Dow);
                    break;
                }
            case MoveMode.Left:
                {
                    List<CellItem> list2;
                    if (firstItem.posX > secondItem.posX)
                    {
                        list2 = cellItems.FindAll((CellItem x) => x.posX > secondItem.posX && x.posY == secondItem.posY);
                        list2.AddRange(cellItems.FindAll((CellItem x) => x.posX > firstItem.posX && x.posY == firstItem.posY));
                    }
                    else
                    {
                        list2 = cellItems.FindAll((CellItem x) => x.posX > firstItem.posX && x.posY == firstItem.posY);
                        list2.AddRange(cellItems.FindAll((CellItem x) => x.posX > secondItem.posX && x.posY == secondItem.posY));
                    }
                    MoveItemList(list2, Move.Left);
                    break;
                }
            case MoveMode.InToOut:
                if ((float)firstItem.posX > mid && (float)secondItem.posX > mid)
                {
                    if (firstItem.posX > secondItem.posX)
                    {
                        MoveInToOut(secondItem);
                        MoveInToOut(firstItem);
                    }
                    else
                    {
                        MoveInToOut(firstItem);
                        MoveInToOut(secondItem);
                    }
                }
                else if ((float)firstItem.posX < mid && (float)secondItem.posX < mid)
                {
                    if (firstItem.posX < secondItem.posX)
                    {
                        MoveInToOut(secondItem);
                        MoveInToOut(firstItem);
                    }
                    else
                    {
                        MoveInToOut(firstItem);
                        MoveInToOut(secondItem);
                    }
                }
                else
                {
                    MoveInToOut(firstItem);
                    MoveInToOut(secondItem);
                }
                break;
            case MoveMode.OutToIn:
                if ((float)firstItem.posX < mid && (float)secondItem.posX < mid)
                {
                    if (firstItem.posX > secondItem.posX)
                    {
                        MoveOutToIn(secondItem);
                        MoveOutToIn(firstItem);
                    }
                    else
                    {
                        MoveOutToIn(firstItem);
                        MoveOutToIn(secondItem);
                    }
                }
                else if ((float)firstItem.posX > mid && (float)secondItem.posX > mid)
                {
                    if (firstItem.posX < secondItem.posX)
                    {
                        MoveOutToIn(secondItem);
                        MoveOutToIn(firstItem);
                    }
                    else
                    {
                        MoveOutToIn(firstItem);
                        MoveOutToIn(secondItem);
                    }
                }
                else
                {
                    MoveOutToIn(firstItem);
                    MoveOutToIn(secondItem);
                }
                break;
        }
        StartCoroutine(UpdateCellData());
        SortList();
    }

    private void MoveOutToIn(CellItem cellItem)
    {
        float num = (float)(width - 1) / 2f;
        List<CellItem> list = null;
        if ((float)cellItem.posX >= num)
        {
            list = cellItems.FindAll((CellItem x) => x.posX > cellItem.posX && x.posY == cellItem.posY);
            MoveItemList(list, Move.Left);
        }
        else if ((float)cellItem.posX < num)
        {
            list = cellItems.FindAll((CellItem x) => x.posX < cellItem.posX && x.posY == cellItem.posY);
            MoveItemList(list, Move.Right);
        }
    }

    private void MoveInToOut(CellItem cellItem)
    {
        float mid = (float)(width - 1) / 2f;
        List<CellItem> list = null;
        if ((float)cellItem.posX >= mid)
        {
            list = cellItems.FindAll((CellItem x) => (float)x.posX >= mid && x.posX < cellItem.posX && x.posY == cellItem.posY);
            MoveItemList(list, Move.Right);
        }
        else if ((float)cellItem.posX < mid)
        {
            list = cellItems.FindAll((CellItem x) => x.posX > cellItem.posX && (float)x.posX < mid && x.posY == cellItem.posY);
            MoveItemList(list, Move.Left);
        }
    }

    private void MoveItemList(List<CellItem> cellItems, Move move)
    {
        foreach (CellItem cellItem in cellItems)
        {
            cellItem.MoveItem(move);
        }
    }

    private void Update()
    {
        if (isCountTime)
        {
            countTime -= Time.deltaTime;
            if (countTime < 0f)
            {
                SuggetHint();
                isCountTime = false;
            }
        }
    }

    private void OnPair(List<CellData> path, CellItem firstItem, CellItem secondItem)
    {
        isCountTime = true;
        countTime = TimeDefault;
        if (isSugget)
        {
            DestroySugget();
        }
        SingletonMonoBehaviour<AudioManager>.Instance.Shot("true");
        CellData item = new CellData(firstItem);
        CellData item2 = new CellData(secondItem);
        cellItems.Remove(firstItem);
        cellItems.Remove(secondItem);
        SingletonMonoBehaviour<ContentMgr>.Instance.Despaw(firstItem.gameObject, 0.2f);
        SingletonMonoBehaviour<ContentMgr>.Instance.Despaw(secondItem.gameObject, 0.2f);
        StartCoroutine(MoveItem(firstItem, secondItem, 0.25f));
        path.Add(item2);
        path.Insert(0, item);
        DrawLineConnect(path);
        firstSelected = null;
        cellDatas[firstItem.posX, firstItem.posY].cellType = -1;
        cellDatas[secondItem.posX, secondItem.posY].cellType = -1;
        this.PostEvent(EventID.UpdateScore, 10);
        if (firstItem.cellType == 0)
        {
            GameDataMgr.PikachuCount += 2;
            SingletonMonoBehaviour<GameMgr>.Instance.AchievementPikachu();
        }
        CheckWin();
        CheckShowBanner(string.Empty);
    }

    private void OnEnable()
    {
        UIManager.onShowPage = (Action<string>)Delegate.Combine(UIManager.onShowPage, new Action<string>(CheckShowBanner));
    }

    private void OnDisable()
    {
        UIManager.onShowPage = (Action<string>)Delegate.Remove(UIManager.onShowPage, new Action<string>(CheckShowBanner));
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && SingletonMonoBehaviour<UIManager>.Instance.GetCurrentPage() == "GamePage")
        {
            SingletonMonoBehaviour<UIManager>.Instance.ShowPage("PausePage");
            SingletonMonoBehaviour<GameMgr>.Instance.PauseGame();
        }
    }

    private void CheckShowBanner(string page)
    {
        if (SingletonMonoBehaviour<UIManager>.Instance.GetCurrentPage() == "GamePage" && height > 9 && cellItems.FindAll((CellItem x) => x.posY < 2).Count <= 0)
        {
        }
    }

    private void DrawLineConnect(List<CellData> path)
    {
        line.DrawLine(path);
    }

    private void CheckWin()
    {
        if (cellItems.Count <= 0)
        {
            SingletonMonoBehaviour<GameMgr>.Instance.LeveFinish();
        }
    }

    private bool CheckAnswer()
    {
        if (cellItems.Count <= 0)
        {
            return true;
        }
        for (int i = 0; i < cellItems.Count - 1; i++)
        {
            for (int j = i + 1; j < cellItems.Count; j++)
            {
                List<CellData> list = CheckPath(cellItems[i], cellItems[j]);
                if (list != null)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void Trick()
    {
        if (cellItems.Count <= 0)
        {
            return;
        }
        for (int i = 0; i < cellItems.Count - 1; i++)
        {
            for (int j = i + 1; j < cellItems.Count; j++)
            {
                List<CellData> list = CheckPath(cellItems[i], cellItems[j]);
                if (list != null)
                {
                    OnPair(list, cellItems[i], cellItems[j]);
                    return;
                }
            }
        }
    }

    public void Hint()
    {
        DestroySugget();
        if (GameDataMgr.Hint <= 0)
        {
            SingletonMonoBehaviour<GameMgr>.Instance.isPlay = false;
            SingletonMonoBehaviour<UIManager>.Instance.ShowPage("StorePage");
            return;
        }
        SingletonMonoBehaviour<GameMgr>.Instance.isPlay = true;
        if (cellItems.Count <= 0)
        {
            return;
        }
        for (int i = 0; i < cellItems.Count - 1; i++)
        {
            for (int j = i + 1; j < cellItems.Count; j++)
            {
                List<CellData> list = CheckPath(cellItems[i], cellItems[j]);
                if (list != null)
                {
                    cellItems[i].Hint();
                    cellItems[j].Hint();
                    GameDataMgr.Hint--;
                    this.PostEvent(EventID.UpdateHint);
                    return;
                }
            }
        }
    }

    public void Swap()
    {
        DestroySugget();
        if (GameDataMgr.Swap <= 0)
        {
            // SingletonMonoBehaviour<GameMgr>.Instance.isPlay = false;
            // SingletonMonoBehaviour<UIManager>.Instance.ShowPage("StorePage");
            return;
        }
        SingletonMonoBehaviour<GameMgr>.Instance.isPlay = true;
        GameDataMgr.Swap--;
        this.PostEvent(EventID.UpdateSwap, GameDataMgr.Swap);
        OnSwap();
    }

    private void OnSwap()
    {
        Dispatcher.Current.BeginInvoke(delegate
        {
            SingletonMonoBehaviour<AudioManager>.Instance.Shot("random");
        });
        do
        {
            int num = cellItems.Count;
            while (num > 1)
            {
                num--;
                int index = UnityEngine.Random.Range(0, num + 1);
                int cellType = cellItems[num].cellType;
                cellItems[num].cellType = cellItems[index].cellType;
                cellItems[index].cellType = cellType;
            }
        }
        while (!CheckAnswer());
        foreach (CellItem cellItem in cellItems)
        {
            cellItem.UpdateSprite(sprites[cellItem.cellType]);
            cellDatas[cellItem.posX, cellItem.posY].cellType = cellItem.cellType;
        }
    }

    private List<CellData> CheckPath(CellItem firstItem, CellItem secondItem)
    {
        CellData cellData = new CellData(firstItem);
        CellData cellData2 = new CellData(secondItem);
        List<CellData> list = new List<CellData>();
        if (cellData.cellType != cellData2.cellType)
        {
            return null;
        }
        if (CheckNearby(cellData, cellData2))
        {
            list.Add(cellData);
            list.Add(cellData2);
            return list;
        }
        float num = width * height;
        bool flag = false;
        CellData item = null;
        CellData item2 = null;
        List<CellData> cellsEnableFromCell = GetCellsEnableFromCell(cellData);
        List<CellData> cellsEnableFromCell2 = GetCellsEnableFromCell(cellData2);
        for (int i = 0; i < cellsEnableFromCell.Count; i++)
        {
            for (int j = 0; j < cellsEnableFromCell2.Count; j++)
            {
                if (CheckPathLine(cellsEnableFromCell[i], cellsEnableFromCell2[j]) && Distance(cellsEnableFromCell[i], cellsEnableFromCell2[j]) < num)
                {
                    item = cellsEnableFromCell[i];
                    item2 = cellsEnableFromCell2[j];
                    flag = true;
                    num = Distance(cellsEnableFromCell[i], cellsEnableFromCell2[j]);
                }
            }
        }
        if (flag)
        {
            list.Add(item);
            list.Add(item2);
            return list;
        }
        return null;
    }

    private float Distance(CellData first, CellData second)
    {
        return Vector2.Distance(new Vector2(first.posX, first.posY), new Vector2(second.posX, second.posY));
    }

    private bool CheckNearby(CellData first, CellData second)
    {
        if (first.posX == second.posX && Mathf.Abs(first.posY - second.posY) == 1)
        {
            return true;
        }
        if (first.posY == second.posY && Mathf.Abs(first.posX - second.posX) == 1)
        {
            return true;
        }
        return false;
    }

    private List<CellData> GetCellsEnableFromCell(CellData from)
    {
        List<CellData> list = new List<CellData>();
        for (int i = from.posX + 1; i < width; i++)
        {
            CellData cellData = cellDatas[i, from.posY];
            if (cellData.cellType != -1)
            {
                break;
            }
            if (CheckPathLine(from, cellData))
            {
                list.Add(cellData);
            }
        }
        for (int num = from.posX - 1; num >= 0; num--)
        {
            CellData cellData2 = cellDatas[num, from.posY];
            if (cellData2.cellType != -1)
            {
                break;
            }
            if (CheckPathLine(from, cellData2))
            {
                list.Add(cellData2);
            }
        }
        for (int j = from.posY + 1; j < height; j++)
        {
            CellData cellData3 = cellDatas[from.posX, j];
            if (cellData3.cellType != -1)
            {
                break;
            }
            if (CheckPathLine(from, cellData3))
            {
                list.Add(cellData3);
            }
        }
        for (int num2 = from.posY - 1; num2 >= 0; num2--)
        {
            CellData cellData4 = cellDatas[from.posX, num2];
            if (cellData4.cellType != -1)
            {
                break;
            }
            if (CheckPathLine(from, cellData4))
            {
                list.Add(cellData4);
            }
        }
        return list;
    }

    private bool CheckPathLine(CellData to, CellData from)
    {
        if (to.posX == from.posX)
        {
            int num = Mathf.Min(to.posY, from.posY);
            int num2 = Mathf.Max(to.posY, from.posY);
            for (int i = num; i <= num2; i++)
            {
                if (i != to.posY && cellDatas[to.posX, i].cellType != -1)
                {
                    return false;
                }
            }
        }
        else
        {
            if (to.posY != from.posY)
            {
                return false;
            }
            int num3 = Mathf.Min(to.posX, from.posX);
            int num4 = Mathf.Max(to.posX, from.posX);
            for (int j = num3; j <= num4; j++)
            {
                if (j != to.posX && cellDatas[j, to.posY].cellType != -1)
                {
                    return false;
                }
            }
        }
        return true;
    }
}
