using UnityEngine;
using UnityEngine.EventSystems;

public class DragMove : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public RectTransform whitePanel;
    public float moveDistance =500f;
    private bool isMoved = false;

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.delta.y > 20 && !isMoved) // 向上滑
        {
            whitePanel.anchoredPosition += new Vector2(0, moveDistance);
            isMoved = true;
        }
        else if (eventData.delta.y < -20 && isMoved) // 向下滑
        {
            whitePanel.anchoredPosition -= new Vector2(0, moveDistance);
            isMoved = false;
        }
    }

    public void OnEndDrag(PointerEventData eventData) { }
}
