using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CircleProgress : MonoBehaviour
{
    [SerializeField] private Image progressImage;
    [SerializeField] private TextMeshProUGUI percentageText;
    [Range(0, 1)] private float progress = 0f;  // 設為 private，確保只能透過 SetProgress() 設定

    void Start()
    {
        UpdateUI();
    }

    void Update()
    {
        Debug.Log("Update 執行中, progress = " + progress); // 確保 Update 在執行
        UpdateUI();
    }

    // ✅ 提供外部設定 progress
    public void SetProgress(float value)
    {
        progress = Mathf.Clamp01(value);  // 限制範圍 0 ~ 1
        UpdateUI();
    }

    // ✅ 更新 UI
    private void UpdateUI()
    {
        if (progressImage != null)
        {
            progressImage.fillAmount = progress;
        }

        if (percentageText != null)
        {
            percentageText.text = Mathf.RoundToInt(progress * 100).ToString();
            percentageText.ForceMeshUpdate();  // ✅ 確保文字即時更新
            Debug.Log("更新 UI：fillAmount = " + progressImage.fillAmount + ", 文字 = " + percentageText.text);
        }
    }
}