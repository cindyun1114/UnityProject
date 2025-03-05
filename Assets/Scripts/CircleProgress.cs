using UnityEngine;
using UnityEngine.UI;

public class CircleProgress : MonoBehaviour
{
    public Image progressImage;  // 前景進度條
    public Text percentageText;  // 顯示數值
    [Range(0, 1)] public float progress = 0f; // 進度（0~1）

    void Update()
    {
        // 更新填充進度
        progressImage.fillAmount = progress;

        // 更新顯示數字
        percentageText.text = (progress * 100).ToString("0") + "%";
    }

    // 設定進度
    public void SetProgress(float value)
    {
        progress = Mathf.Clamp01(value); // 限制範圍 0~1
    }
}