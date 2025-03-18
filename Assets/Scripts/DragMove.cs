using UnityEngine;
using UnityEngine.EventSystems;

public class DragMove : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public RectTransform whitePanel;  // �i��ʪ��ؼ�
    public float moveDistance = 500f;
    private bool isMoved = false;

    // �s�W�G�x�s��l��m
    private Vector2 originalPosition;

    void Start()
    {
        // �i�J�����ɡA���O�� whitePanel �쥻�� anchoredPosition
        if (whitePanel != null)
            originalPosition = whitePanel.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (whitePanel == null) return;

        // ���W��ʡ]y > 20�^
        if (eventData.delta.y > 20 && !isMoved)
        {
            whitePanel.anchoredPosition += new Vector2(0, moveDistance);
            isMoved = true;
        }
        // ���U��ʡ]y < -20�^
        else if (eventData.delta.y < -20 && isMoved)
        {
            whitePanel.anchoredPosition -= new Vector2(0, moveDistance);
            isMoved = false;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // ������ʮɡA�p�G�٦��n�����ƥi�H�g�b�o��
    }

    // ������Ψ������Q�]�� inactive �ɡA�|�I�s OnDisable
    void OnDisable()
    {
        // �u�n�����Q�����γo�Ӫ���Q���ΡA�N�۰ʧ�ժO��m���m�^���I
        if (whitePanel != null)
            whitePanel.anchoredPosition = originalPosition;
        isMoved = false;
    }
}