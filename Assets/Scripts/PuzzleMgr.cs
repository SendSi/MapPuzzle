using System.Windows.Forms;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleMgr : MonoBehaviour
{
    public static PuzzleMgr Instance;
    public RawImage rawImage;
    private RectTransform rawImage_RT;
    public Vector2 basicSize;
    private float newWidth;
    private float newHeight;
    private int intervalSize = 170;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
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
            rawImage.texture = texture;
            TextureScaleResize(texture);
            int cellNumX = (int)(newWidth / intervalSize);
            int cellNumY = (int)(newHeight / intervalSize);
            if (cellNumX % 2 != 0)
            {
                cellNumX++;
            }

            if (cellNumY % 2 != 0)
            {
                cellNumY++;
            }

            PuzzlePool.Instance.CreatePuzzle(cellNumY, cellNumX, texture, intervalSize);

            newWidth = cellNumX * intervalSize; //保证图片不超过基本尺寸
            newHeight = cellNumY * intervalSize;
            rawImage_RT.sizeDelta = new Vector2(newWidth, newHeight);
        }
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

        newWidth = expand ? tex.width / ratio : tex.width * ratio;
        newHeight = expand ? tex.height / ratio : tex.height * ratio;

        if (newHeight > basicSize.y) //伪正方形图片
        {
            ratio = basicSize.y / newHeight;
            newWidth *= ratio;
            newHeight = basicSize.y;
        }

        newWidth = (int)(newWidth);
        newHeight = (int)(newHeight); //得到 width 和 height 后，设置图片的 sizeDelta
    }
}