using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollViewEventBlocker : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ScrollRect parentScrollRect; // 指向外部的 Scroll View

    // 當開始拖動內部 Scroll View 時
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (parentScrollRect != null)
            parentScrollRect.enabled = false; // 禁用外部 Scroll View
    }

    // 當拖動時，內部 Scroll View 正常運作（不需要特別處理）
    public void OnDrag(PointerEventData eventData) { }

    // 當拖動結束時
    public void OnEndDrag(PointerEventData eventData)
    {
        if (parentScrollRect != null)
            parentScrollRect.enabled = true; // 重新啟用外部 Scroll View
    }
}
