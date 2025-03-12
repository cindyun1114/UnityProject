using UnityEngine;
using UnityEngine.UI;

public class ScrollIndicator : MonoBehaviour
{
    public ScrollRect scrollRect; // Scroll View
    public Image[] dots; // �������ܾ��I�I
    public Color activeColor = Color.white; // ��e�����I�I�C��
    public Color inactiveColor = Color.gray; // ��L�I�I�C��
    public int totalPages = 4; // �`����

    private void Update()
    {
        float scrollPos = scrollRect.horizontalNormalizedPosition; // ���o��e�u�ʦ�m
        int currentPage = Mathf.RoundToInt(scrollPos * (totalPages - 1)); // �p���e��������

        // �T�O�������ަb�X�z�d��
        currentPage = Mathf.Clamp(currentPage, 0, totalPages - 1);

        // ��s�I�I�C��
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].color = (i == currentPage) ? activeColor : inactiveColor;
        }
    }
}
