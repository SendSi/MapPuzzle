using System.Windows.Forms;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Application = UnityEngine.Application;

public class PuzzleMgr : MonoBehaviour
{
    public static PuzzleMgr Instance;
    public RawImage rawImage;
    private RectTransform rawImage_RT;
    public Vector2 basicSize;
    private float sizeWidth;
    private float sizeHeight;
    private int intervalSize = 170;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        rawImage_RT = rawImage.GetComponent<RectTransform>();
    }

    public void OnClickOpenFindImg()
    {
        OpenFileDialog ofd = new OpenFileDialog();
        ofd.Filter = "Image Files(*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";
        if (ofd.ShowDialog() == DialogResult.OK)
        {
            string filePath = ofd.FileName;
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(System.IO.File.ReadAllBytes(filePath));

            SetInPic(texture);
        }
    }

    public void LoadInternalPic(string picName)
    {
        var texture = Resources.Load<Texture2D>(picName);
        SetInPic(texture);
    }


    void SetInPic(Texture2D texture)
    {
        rawImage.texture = texture;
        TextureScaleResize(texture);
        int cellNumX = (int)(sizeWidth / intervalSize);
        int cellNumY = (int)(sizeHeight / intervalSize);
        if (cellNumX % 2 != 0)
        {
            cellNumX++;
        }
        if (cellNumY % 2 != 0)
        {
            cellNumY++;
        }
        PuzzlePool.Instance.CreatePuzzle(cellNumY, cellNumX, texture, intervalSize);

        sizeWidth = cellNumX * intervalSize; //保证图片不超过基本尺寸
        sizeHeight = cellNumY * intervalSize;
        rawImage_RT.sizeDelta = new Vector2(sizeWidth, sizeHeight);
        InitPuzzleGrid(cellNumY, cellNumX);
    }
    

    public void SwitchToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    void TextureScaleResize(Texture2D tex)
    {
        bool isWidth = tex.width > tex.height; //判断图片宽是否大于高
        float longSide = isWidth ? tex.width : tex.height;

        bool expand;
        float ratio;
        if (isWidth)
        {
            expand = longSide > basicSize.x ? false : true;
            ratio = expand ? longSide / basicSize.x : basicSize.x / longSide;
        }
        else
        {
            expand = longSide > basicSize.y ? false : true;
            ratio = expand ? longSide / basicSize.y : basicSize.y / longSide;
        }

        sizeWidth = expand ? tex.width / ratio : tex.width * ratio;
        sizeHeight = expand ? tex.height / ratio : tex.height * ratio;

        if (sizeHeight > basicSize.y) //伪正方形图片
        {
            ratio = basicSize.y / sizeHeight;
            sizeWidth *= ratio;
            sizeHeight = basicSize.y;
        }

        sizeWidth = (int)(sizeWidth);
        sizeHeight = (int)(sizeHeight); //得到 width 和 height 后，设置图片的 sizeDelta
    }

    /// <summary> 格子碎片 </summary>
    private int[] puzzle;

    /// <summary> 初始化格子 </summary>
    void InitPuzzleGrid(int cellNumY, int cellNumX)
    {
        puzzle = null;
        puzzle = new int[cellNumY * cellNumX];
        for (int i = 0; i < puzzle.Length; i++)
        {
            puzzle[i] = 99999;
        }
    }

    /// <summary> 检测该格子下 是否有碎片 </summary>
    public bool CheckHasvePiece(int gridId)
    {
        if (puzzle.Length <= gridId)
        {
            Debug.LogError("检查格子碎片时,超时索引");
            return false;
        }

        if (puzzle[gridId] != 99999)
        {
            return true;
        }

        return false;
    }

    /// <summary> 网格填 进碎片 </summary>
    public void SetPiece(int gridId, int pieceId)
    {
        puzzle[gridId] = pieceId;
    }

    /// <summary> 取出碎片 </summary>
    public void OutPiece(int gridId)
    {
        puzzle[gridId] = 99999;
    }

    public bool IsFinish()
    {
        for (int i = 0; i < puzzle.Length - 1; i++)
        {
            if (puzzle[i] >= puzzle[i + 1])
            {
                return false;
            }
        }

        Debug.Log("拼图完成了");
        return true;
    }
}