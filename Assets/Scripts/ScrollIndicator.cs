using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ScrollIndicator : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    public ScrollRect scrollRect;
    public Image[] dots;
    public Color activeColor = Color.white;
    public Color inactiveColor = Color.gray;
    public int totalPages = 4;
    public float snapSpeed = 10f;

    private float[] pagePositions;
    private bool isLerping = false;
    private bool dragging = false;

    // 判斷滑動距離
    private Vector2 dragStartPos;
    private float dragThreshold = 0.2f; // 滑動寬度 20% 才翻頁

    void Start()
    {
        pagePositions = new float[totalPages];
        for (int i = 0; i < totalPages; i++)
        {
            pagePositions[i] = (float)i / (totalPages - 1);
        }
    }

    void Update()
    {
        // 更新點點狀態
        float scrollPos = scrollRect.horizontalNormalizedPosition;
        int currentPage = Mathf.RoundToInt(scrollPos * (totalPages - 1));
        currentPage = Mathf.Clamp(currentPage, 0, totalPages - 1);

        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].color = (i == currentPage) ? activeColor : inactiveColor;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragging = true;
        dragStartPos = eventData.pressPosition;
        StopAllCoroutines();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragging = false;

        float dragDelta = eventData.position.x - dragStartPos.x;
        float dragPercent = dragDelta / scrollRect.GetComponent<RectTransform>().rect.width;

        float currentPos = scrollRect.horizontalNormalizedPosition;
        int currentPage = Mathf.RoundToInt(currentPos * (totalPages - 1));

        // 判斷要不要翻頁
        if (Mathf.Abs(dragPercent) > dragThreshold)
        {
            if (dragPercent < 0 && currentPage < totalPages - 1) currentPage++; // 向左翻頁
            if (dragPercent > 0 && currentPage > 0) currentPage--;             // 向右翻頁
        }

        // 計算目標位置
        float target = (float)currentPage / (totalPages - 1);
        StartCoroutine(SmoothScrollTo(target));
    }

    IEnumerator SmoothScrollTo(float target)
    {
        isLerping = true;

        while (Mathf.Abs(scrollRect.horizontalNormalizedPosition - target) > 0.001f)
        {
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(
                scrollRect.horizontalNormalizedPosition,
                target,
                Time.deltaTime * snapSpeed
            );
            yield return null;
        }

        scrollRect.horizontalNormalizedPosition = target;
        isLerping = false;
    }
}
