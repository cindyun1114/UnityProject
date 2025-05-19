using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ShareManager : MonoBehaviour
{
    private Dictionary<string, string> achievementMessages = new Dictionary<string, string>()
    {
        {"小試身手", "初次嘗試即獲徽章"},
        {"知識射手", "答題命中率極高"},
        {"學霸附體", "展現優異學習表現"},
        {"學習狂熱", "持續投入學習挑戰"},
        {"百發百中", "答題正確率極高"},
        {"榮耀戰神", "完成高難度挑戰"},
        {"卡牌大亨", "集滿多項學習成就"},
        {"好奇寶寶", "勇於發問、探索知識"},
        {"學習強者", "堅持不懈穩定學習"}
    };

    private List<string> promoPhrases = new List<string>()
    {
        "打造不無聊的沉浸式學習體驗",
        "用遊戲化任務探索費曼學習法",
        "結合互動設計與理解力的學習方式",
        "讓知識內化成為你的超能力",
        "學習就該又沉浸又有趣"
    };

    public void ShareAchievementFromButton(GameObject buttonObject)
    {
        Transform parent = buttonObject.transform.parent;
        TextMeshProUGUI[] texts = parent.GetComponentsInChildren<TextMeshProUGUI>();
        string achievementName = null;

        foreach (TextMeshProUGUI tmp in texts)
        {
            if (!string.IsNullOrEmpty(tmp.text))
            {
                achievementName = tmp.text;
                break;
            }
        }

        if (string.IsNullOrEmpty(achievementName))
        {
            Debug.LogWarning("❌ 無法找到成就名稱（Text TMP 為空）");
            return;
        }

        string description = achievementMessages.ContainsKey(achievementName)
            ? achievementMessages[achievementName]
            : "我剛解鎖了新的徽章！";

        string promo = promoPhrases[Random.Range(0, promoPhrases.Count)];

        string message = $"🎉 我在 Feyndora 解鎖「{achievementName}」徽章！{description}\n" +
                         $"👉 {promo}：https://www.youtube.com/watch?v=YoT0wXPW_18";

        Debug.Log("📤 分享內容：\n" + message);
        ShareBridge.Share(message);
    }
}
