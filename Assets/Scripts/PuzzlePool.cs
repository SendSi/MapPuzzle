using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzlePool : MonoBehaviour
{
    public static PuzzlePool Instance;
    [SerializeField] GameObject grid, TL, TR, T1, T2, BL, BR, B1, B2, L1, L2, R1, R2, C1, C2;
    [SerializeField] private Transform puzzlePanel;
    [SerializeField] private Transform canvasTrans;
    List<GameObject> puzzleList = new List<GameObject>();

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
        size = new Vector2(col * interval, row * interval);
        tempV2 = textureStartPos;

        int totalGridNum = row * col;
        for (int i = 0; i < totalGridNum; i++)
        {
            GameObject gridObj = Instantiate(grid, puzzlePanel);
            puzzleList.Add(gridObj);
        }

        StartCoroutine(IECreate(row, col));
    }

    IEnumerator IECreate(int row, int col)
    {
        yield return new WaitForSeconds(0.1f);
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
                if (CheckCorner(i, j, row, col, ref picese))
                {
                }
                else if (CheckSide(i, j, row, col, ref picese))
                {
                }
                else
                {
                    if ((i % 2 == 0 && j % 2 == 0) || (i % 2 != 0 && j % 2 != 0))
                    {
                        picese = Instantiate(C1);
                    }
                    else
                    {
                        picese = Instantiate(C2);
                    }
                }

                picese.GetComponent<RectTransform>().anchoredPosition = puzzleList[index].transform.position;
                picese.transform.SetParent(canvasTrans);
                picese.transform.Find("Mask/texture").GetComponent<RawImage>().texture = tex;
                temp = picese.transform.Find("Mask/texture").GetComponent<RectTransform>();
                temp.sizeDelta = size;
                temp.anchoredPosition = tempV2;
                index++;
                tempV2 += offsetX;
            }

            tempV2.x = textureStartPos.x;
            tempV2 += offsetY;
        }
    }

    /// <summary> 四周 非角 </summary>
    /// <param name="row">行数</param>
    /// <param name="col">列数</param>
    bool CheckSide(int xx, int yy, int row, int col, ref GameObject obj)
    {
        if (xx == 0 && yy != 0 && yy != col - 1) //上边  非角
        {
            obj = Instantiate(yy % 2 != 0 ? T1 : T2);
            return true;
        }
        else if (xx == row - 1 && yy != 0 && yy != col - 1) //下边  非角
        {
            obj = Instantiate(yy % 2 != 0 ? B1 : B2);
            return true;
        }
        else if (yy == 0 && xx != 0 && yy != col - 1) //左边
        {
            obj = Instantiate(xx % 2 != 0 ? L1 : L2);
            return true;
        }
        else if (yy == col - 1 && xx != 0 && xx != col - 1) //右边
        {
            obj = Instantiate(xx % 2 != 0 ? R1 : R2);
            return true;
        }

        return false;
    }

    /// <summary> 四角 </summary>
    /// <param name="row">行数</param>
    /// <param name="col">列数</param>
    bool CheckCorner(int x, int y, int row, int col, ref GameObject obj)
    {
        if (x == 0 && y == 0) //左上角
        {
            obj = Instantiate(TL);
            return true;
        }
        else if (x == 0 && y == col - 1) //右上角
        {
            obj = Instantiate(TR);
            return true;
        }
        else if (x == row - 1 && y == 0) //左下角
        {
            obj = Instantiate(BL);
            return true;
        }
        else if (x == row - 1 && y == col - 1) //右下角
        {
            obj = Instantiate(BR);
            return true;
        }

        return false;
    }
}