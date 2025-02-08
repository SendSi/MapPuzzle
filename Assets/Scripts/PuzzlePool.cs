using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PieceType
{
    Grid,
    TL,
    TR,
    T1,
    T2,
    BL,
    BR,
    B1,
    B2,
    L1,
    L2,
    R1,
    R2,
    C1,
    C2
}

public class PuzzlePool : MonoBehaviour
{
    public static PuzzlePool Instance;
    [SerializeField] GameObject grid, TL, TR, T1, T2, BL, BR, B1, B2, L1, L2, R1, R2, C1, C2;
    [SerializeField] private Transform puzzlePanel;
    [SerializeField] private Transform canvasTrans;
    List<GameObject> gridList = new List<GameObject>();

    [SerializeField] Vector2 textureStartPos, offsetX, offsetY;

    private Vector2 tempV2;

    private Texture2D tex;

    private void Awake()
    {
        Instance = this;
    }

    private Vector2 size;

    /// <summary> 左上角为0,0   向右,向下为正方向 </summary>
    public void CreatePuzzle(int row, int col, Texture2D pTex, float interval)
    {
        tex = pTex;
        tempV2 = textureStartPos;
        int totalGridNum = row * col;
        size = new Vector2(col * interval, row * interval);

        GameObject gridObj = null;
        for (int i = 0; i < totalGridNum; i++)
        {
            gridObj = GetPiece(PieceType.Grid, i);
            gridObj.transform.SetParent(puzzlePanel);
            gridList.Add(gridObj);
            recyclePiece.Add(gridObj);
        }

        StartCoroutine(IECreate(row, col));
    }

    IEnumerator IECreate(int row, int col)
    {
        yield return new WaitForSeconds(0.7f);
        CreatePieces(row, col);
    }

    void CreatePieces(int row, int col)
    {
        int index = 0;
        RectTransform temp;
        GameObject picese = null;
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (CheckCorner(i, j, row, col, index, ref picese))
                {
                }
                else if (CheckSide(i, j, row, col, index, ref picese))
                {
                }
                else
                {
                    if ((i % 2 == 0 && j % 2 == 0) || (i % 2 != 0 && j % 2 != 0))
                    {
                        picese = GetPiece(PieceType.C1, index);
                    }
                    else
                    {
                        picese = GetPiece(PieceType.C2, index);
                    }
                }

                picese.transform.SetParent(canvasTrans);
                picese.GetComponent<RectTransform>().anchoredPosition = gridList[index].transform.position;

                picese.transform.Find("Mask/texture").GetComponent<RawImage>().texture = tex;
                temp = picese.transform.Find("Mask/texture").GetComponent<RectTransform>();
                temp.sizeDelta = size;

                temp.anchoredPosition = tempV2; //图片的偏移
                tempV2 += offsetX;
                index++;
                recyclePiece.Add(picese);
            }

            tempV2.x = textureStartPos.x; //换行做图片的偏移
            tempV2 += offsetY;
        }
    }

    /// <summary> 四周 非角 </summary>
    /// <param name="row">行数</param>
    /// <param name="col">列数</param>
    bool CheckSide(int xx, int yy, int row, int col, int index, ref GameObject obj)
    {
        if (xx == 0 && yy != 0 && yy != col - 1) //上边  非角
        {
            obj = GetPiece(yy % 2 != 0 ? PieceType.T1 : PieceType.T2, index);
            return true;
        }
        else if (xx == row - 1 && yy != 0 && yy != col - 1) //下边  非角
        {
            obj = GetPiece(yy % 2 != 0 ? PieceType.B1 : PieceType.B2, index);
            return true;
        }
        else if (yy == 0 && xx != 0 && yy != col - 1) //左边
        {
            obj = GetPiece(xx % 2 != 0 ? PieceType.L1 : PieceType.L2, index);
            return true;
        }
        else if (yy == col - 1 && xx != 0 && xx != col - 1) //右边
        {
            obj = GetPiece(xx % 2 != 0 ? PieceType.R1 : PieceType.R2, index);
            return true;
        }

        return false;
    }

    /// <summary> 四角 </summary>
    /// <param name="row">行数</param>
    /// <param name="col">列数</param>
    bool CheckCorner(int x, int y, int row, int col, int index, ref GameObject obj)
    {
        if (x == 0 && y == 0) //左上角
        {
            obj = GetPiece(PieceType.TL, index);
            return true;
        }
        else if (x == 0 && y == col - 1) //右上角
        {
            obj = GetPiece(PieceType.TR, index);
            return true;
        }
        else if (x == row - 1 && y == 0) //左下角
        {
            obj = GetPiece(PieceType.BL, index);
            return true;
        }
        else if (x == row - 1 && y == col - 1) //右下角
        {
            obj = GetPiece(PieceType.BR, index);
            return true;
        }

        return false;
    }

    GameObject GetPiece(PieceType pType, int id)
    {
        string str = pType.ToString();
        GameObject obj = null;

        if (pieceDic.TryGetValue(str, out var list))
        {
            bool find = false;
            foreach (var item in list)
            {
                if (item.activeSelf == false)
                {
                    obj = item;
                    find = true;
                    break;
                }
            }

            if (!find)
            {
                obj = CreatePriece__2(pType);
                list.Add(obj);
            }
        }
        else
        {
            list = new List<GameObject>();
            obj = CreatePriece__2(pType);
            list.Add(obj);
            pieceDic[str] = list;
        }

        obj.name = id.ToString();
        obj.SetActive(true);

        return obj;
    }


    GameObject CreatePriece__2(PieceType pType)
    {
        switch (pType)
        {
            case PieceType.Grid:
                return Instantiate(grid);
            case PieceType.TL:
                return Instantiate(TL);
            case PieceType.TR:
                return Instantiate(TR);
            case PieceType.T1:
                return Instantiate(T1);
            case PieceType.T2:
                return Instantiate(T2);
            case PieceType.BL:
                return Instantiate(BL);
            case PieceType.BR:
                return Instantiate(BR);
            case PieceType.B1:
                return Instantiate(B1);
            case PieceType.B2:
                return Instantiate(B2);
            case PieceType.L1:
                return Instantiate(L1);
            case PieceType.L2:
                return Instantiate(L2);
            case PieceType.R1:
                return Instantiate(R1);
            case PieceType.R2:
                return Instantiate(R2);
            case PieceType.C1:
                return Instantiate(C1);
            case PieceType.C2:
                return Instantiate(C2);
            default:
                return null;
        }

        return null;
    }

    List<GameObject> recyclePiece = new List<GameObject>(); //回收
    Dictionary<string, List<GameObject>> pieceDic = new Dictionary<string, List<GameObject>>(); //缓存

    [ContextMenu("Recycle")]
    public void Recycle()
    {
        foreach (var item in recyclePiece)
        {
            item.SetActive(false);
        }

        gridList.Clear();
        recyclePiece.Clear();
    }
}