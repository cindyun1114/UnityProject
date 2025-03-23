using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelector : MonoBehaviour
{
    public GameObject levelButtonPrefab;  // 預製按鈕
    public Transform levelGroup;          // 按鈕父物件
    public int levelCount = 10;           // 關卡數量
    public float radius = 250f;           // 按鈕圓形排列半徑
    public float rotationSpeed = 2f;      // 旋轉速度

    private List<GameObject> levelButtons = new List<GameObject>();
    private List<int> sortOrders = new List<int>();

    private void Start()
    {
        if (levelButtonPrefab == null)
        {
            Debug.LogError("levelButtonPrefab 尚未在 Inspector 指定！");
            return;
        }

        // 生成關卡按鈕
        for (int i = 0; i < levelCount; i++)
        {
            GameObject levelButton = Instantiate(levelButtonPrefab, levelGroup);

            if (levelButton == null)
            {
                Debug.LogError($"關卡 {i} 按鈕無法實例化！");
                continue;
            }

            // 嘗試取得 TextMeshPro 組件
            TextMeshProUGUI buttonText = levelButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText == null)
            {
                Debug.LogError($"關卡 {i} 按鈕缺少 TextMeshProUGUI 元件！");
            }
            else
            {
                buttonText.text = (i + 1).ToString();
            }

            levelButtons.Add(levelButton);
            sortOrders.Add(i);
        }

        ArrangeButtons();
    }

    // 將按鈕排列成圓形
    private void ArrangeButtons()
    {
        float angleStep = 360f / levelCount;

        for (int i = 0; i < levelCount; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            levelButtons[i].transform.localPosition = new Vector3(x, y, 0);
        }
    }

    // 旋轉按鈕
    public void RotateButtons(bool clockwise)
    {
        int shiftAmount = clockwise ? 1 : -1;
        List<int> newSortOrders = new List<int>();

        for (int i = 0; i < sortOrders.Count; i++)
        {
            int newIndex = (i + shiftAmount + sortOrders.Count) % sortOrders.Count;
            newSortOrders.Add(sortOrders[newIndex]);
        }

        sortOrders = newSortOrders;
        ArrangeButtons();
    }
}
