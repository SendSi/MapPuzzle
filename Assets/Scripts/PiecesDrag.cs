using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PiecesDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Image maskImage; //遮罩的图片
    private Vector3 offset; //鼠标点击偏移
    private Vector3 originPos; //挪动起始点 移动失败会回到原来的位置 上
    private RectTransform rectTrans; //
    private Vector3 outPos; //转换坐标系 输出坐标


    void Start()
    {
        maskImage = GetComponent<Image>();
        maskImage.alphaHitTestMinimumThreshold = 0.1f; //透明度小于0.1将无法触发射线 
        rectTrans = transform.parent.parent.GetComponent<RectTransform>();
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.LogError("开始拖");
        var tPoint = RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTrans, Input.mousePosition,
            eventData.enterEventCamera, out outPos);
        if (tPoint)
        {
            originPos = rectTrans.anchoredPosition;
            rectTrans.SetAsLastSibling();
            offset = (Vector3)rectTrans.anchoredPosition - outPos;

            var hiss2D = Physics2D.RaycastAll(eventData.position, Vector2.zero);
            foreach (var hit in hiss2D)
            {
                var go = hit.collider.gameObject;
                if (go.layer == 9)
                {
                    int gridId = (int.Parse)(go.name);
                    if (PuzzleMgr.Instance.CheckHasvePiece(gridId))
                    {
                        PuzzleMgr.Instance.OutPiece(gridId);
                        Debug.LogError("取 碎");
                    }
                }
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTrans.anchoredPosition = offset + Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.LogError("拖结束");
        var hiss2D = Physics2D.RaycastAll(eventData.position, Vector2.zero);
        if (hiss2D.Length > 1)
        {
            foreach (var hit in hiss2D)
            {
                var go = hit.collider.gameObject;
                if (go.layer == 9)
                {
                    int gridId = (int.Parse)(go.name);
                    if (PuzzleMgr.Instance.CheckHasvePiece(gridId) == false)
                    {
                        rectTrans.anchoredPosition = go.GetComponent<RectTransform>().position;
                        PuzzleMgr.Instance.SetPiece(gridId,int.Parse(rectTrans.name));
                        rectTrans.SetAsFirstSibling();
                        if (PuzzleMgr.Instance.IsFinish())
                        {
                            Debug.LogError("finish");
                        }
                    }
                    else
                    {
                        rectTrans.anchoredPosition = originPos;
                        Debug.LogError("放入失败");
                        return;
                    }
                }
            }
        }
    }
}