using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollViewEventBlocker : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ScrollRect parentScrollRect; // ���V�~���� Scroll View

    // ��}�l��ʤ��� Scroll View ��
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (parentScrollRect != null)
            parentScrollRect.enabled = false; // �T�Υ~�� Scroll View
    }

    // ���ʮɡA���� Scroll View ���`�B�@�]���ݭn�S�O�B�z�^
    public void OnDrag(PointerEventData eventData) { }

    // ���ʵ�����
    public void OnEndDrag(PointerEventData eventData)
    {
        if (parentScrollRect != null)
            parentScrollRect.enabled = true; // ���s�ҥΥ~�� Scroll View
    }
}
