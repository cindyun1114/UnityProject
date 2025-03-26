using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LevelButton : MonoBehaviour
{
    private LevelSelector levelSelector;
    [SerializeField] private Button levelButton;
    [SerializeField] private TextMeshProUGUI descriptionText; // 顯示描述的 UI

    private static Dictionary<string, string> levelDescriptions = new Dictionary<string, string>()
    {
        { "0", "這是一張魔法卡片" },
        { "1", "這是一位優雅的貴族" },
        { "2", "神秘的宇宙探索者" },
        { "3", "擁有古老智慧的賢者" },
        { "4", "勇敢的劍士" },
        { "5", "傳說中的占星師" }
    };

    public void Setup(int levelIndex)
    {
        gameObject.name = levelIndex.ToString(); // 設定名稱為 Index，方便管理
        if (levelDescriptions.ContainsKey(gameObject.name))
        {
            descriptionText.text = levelDescriptions[gameObject.name]; // 設定對應的描述
        }
        else
        {
            descriptionText.text = "未知的冒險者"; // 預設描述
        }
    }

    public void RegisterLevelSelector(LevelSelector levelSelector)
    {
        this.levelSelector = levelSelector;
        levelButton.onClick.AddListener(() =>
        {
            levelSelector.MoveLevelToBottom(int.Parse(gameObject.name));
        });
    }
}
