using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance; // Singleton

    private string baseUrl = "https://feyndora-api.onrender.com";

    [Header("徽章 UI（Badge）")]
    public GameObject courseAddedBadge;       // 新增一門課程的徽章
    public GameObject courseCompletedBadge;   // 完整上完一門課的徽章
    public GameObject pointsBadge;            // 學習積分達到 500 分的徽章

    [Header("成就詳細 UI（Panel）")]
    public GameObject courseAddedPanel;
    public GameObject courseCompletedPanel;
    public GameObject pointsPanel;

    [Header("成就紅點")]
    public Image achievementRedDotImage;    // 第一個紅點
    public Image achievementRedDotImage2;   // 第二個紅點

    private const string ACHIEVEMENT_RED_DOT_STATE_KEY = "AchievementRedDotState";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 讓成就管理器在場景切換時不被刪除
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 加載紅點狀態
        LoadRedDotState();
    }

    private void LoadRedDotState()
    {
        bool redDotState = PlayerPrefs.GetInt(ACHIEVEMENT_RED_DOT_STATE_KEY, 0) == 1;
        if (achievementRedDotImage != null)
            achievementRedDotImage.gameObject.SetActive(redDotState);
        if (achievementRedDotImage2 != null)
            achievementRedDotImage2.gameObject.SetActive(redDotState);
    }

    private void SaveRedDotState(bool state)
    {
        PlayerPrefs.SetInt(ACHIEVEMENT_RED_DOT_STATE_KEY, state ? 1 : 0);
        PlayerPrefs.Save();
    }

    // 登入後等待 APIManager 設定 UserID，再初始化成就資料
    void Start()
    {
        StartCoroutine(WaitForUserIDAndInitializeAchievements());
    }
    public void ReinitializeAchievements()
    {
        StopAllCoroutines();
        StartCoroutine(WaitForUserIDAndInitializeAchievements());
    }
    private IEnumerator WaitForUserIDAndInitializeAchievements()
    {
        // 等待 UserID 可用（由 APIManager 設定）
        while (!PlayerPrefs.HasKey("UserID"))
        {
            yield return null;
        }

        int userID = PlayerPrefs.GetInt("UserID");

        // 先檢查是否有新解鎖的成就
        CheckAchievements();

        // 再一次性從後端拉取用戶所有成就狀態
        yield return StartCoroutine(UpdateAchievementStatuses(userID));

        // 最後根據本地快取更新 UI
        UpdateAllAchievementsUI();
    }

    // 呼叫後端 /check_achievements，取得新解鎖的成就
    public void CheckAchievements()
    {
        int userID = PlayerPrefs.GetInt("UserID");
        StartCoroutine(CheckAchievementsCoroutine(userID));
    }

    private IEnumerator CheckAchievementsCoroutine(int userID)
    {
        string jsonData = "{}"; // POST 所需空 body
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = new UnityWebRequest($"{baseUrl}/check_achievements/{userID}", "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var jsonResponse = JsonUtility.FromJson<CheckAchievementResponse>(request.downloadHandler.text);

                if (jsonResponse.new_achievements.Length > 0)
                {
                    Debug.Log("✅ 用戶獲得了新的成就：" + string.Join(", ", jsonResponse.new_achievements));
                    foreach (string achievement in jsonResponse.new_achievements)
                    {
                        // 標記新成就為已解鎖（未領取）到本地
                        PlayerPrefs.SetInt($"AchievementUnlocked_{achievement}", 1);
                        PlayerPrefs.Save();
                        // 更新該成就的 UI：隱藏遮罩，顯示 Claim 按鈕
                        UnlockAchievementUI(achievement);
                    }

                    // 顯示紅點並保存狀態
                    if (achievementRedDotImage != null)
                        achievementRedDotImage.gameObject.SetActive(true);
                    if (achievementRedDotImage2 != null)
                        achievementRedDotImage2.gameObject.SetActive(true);
                    SaveRedDotState(true);
                }
                else
                {
                    Debug.Log("📌 目前沒有新的成就，但可能有未領取的獎勵");
                }
            }
            else
            {
                Debug.LogError("❌ 檢查成就失敗：" + request.downloadHandler.text);
            }
        }
    }

    // 一次性從後端拉取用戶所有擁有的成就（透過 /get_user_achievements）
    private IEnumerator UpdateAchievementStatuses(int userID)
    {
        using (UnityWebRequest request = UnityWebRequest.Get($"{baseUrl}/get_user_achievements/{userID}"))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var jsonResponse = JsonUtility.FromJson<AchievementsResponse>(request.downloadHandler.text);

                foreach (var achievement in jsonResponse.achievements)
                {
                    string badgeName = achievement.badge_name;
                    bool isClaimed = achievement.is_claimed;

                    if (isClaimed)
                    {
                        PlayerPrefs.SetInt($"AchievementClaimed_{badgeName}", 1);
                        PlayerPrefs.DeleteKey($"AchievementUnlocked_{badgeName}");
                    }
                    else
                    {
                        PlayerPrefs.SetInt($"AchievementUnlocked_{badgeName}", 1);
                    }
                }
                PlayerPrefs.Save();
                UpdateAllAchievementsUI();
            }
            else
            {
                Debug.LogError("❌ 無法獲取用戶的成就：" + request.downloadHandler.text);
            }
        }
    }

    // 更新單一成就的 UI（新解鎖時呼叫）
    private void UnlockAchievementUI(string badgeName)
    {
        GameObject badge = GetBadgeObject(badgeName);
        GameObject panel = GetPanelObject(badgeName);
        if (badge == null || panel == null)
            return;

        // 隱藏徽章遮罩
        GameObject coverImage = badge.transform.Find("AchievementCoverImage")?.gameObject;
        if (coverImage != null)
        {
            coverImage.SetActive(false);
            Debug.Log($"✅ 已隱藏 {badgeName} 的遮罩！");
        }
        else
        {
            Debug.LogError($"❌ 找不到 {badgeName} 的 AchievementCoverImage！");
        }

        // 更新面板：顯示 Claim 按鈕（未領取）
        SetPanelButtons(panel, false, true, false);
    }

    // 點擊成就圖示時呼叫，檢查該成就是否已領取（單筆查詢，可保留以便用戶查看詳情）
    public void ShowAchievementDetails(string badgeName)
    {
        int userID = PlayerPrefs.GetInt("UserID");
        StartCoroutine(CheckIfClaimed(userID, badgeName));
    }

    // 檢查單項成就是否已領取（使用 /check_achievement_status API）
    private IEnumerator CheckIfClaimed(int userID, string badgeName)
    {
        GameObject panel = GetPanelObject(badgeName);
        if (panel == null)
            yield break;

        using (UnityWebRequest request = UnityWebRequest.Get($"{baseUrl}/check_achievement_status/{userID}?badge_name={UnityWebRequest.EscapeURL(badgeName)}"))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var jsonResponse = JsonUtility.FromJson<AchievementStatusResponse>(request.downloadHandler.text);
                if (jsonResponse.is_claimed)
                {
                    PlayerPrefs.SetInt($"AchievementClaimed_{badgeName}", 1);
                    PlayerPrefs.DeleteKey($"AchievementUnlocked_{badgeName}");
                    PlayerPrefs.Save();
                    SetPanelButtons(panel, false, false, true);
                }
                else
                {
                    PlayerPrefs.SetInt($"AchievementUnlocked_{badgeName}", 1);
                    PlayerPrefs.Save();
                    SetPanelButtons(panel, false, true, false);
                }
            }
            else
            {
                SetPanelButtons(panel, true, false, false);
            }
        }
    }

    // 領取成就獎勵
    public void ClaimReward(string badgeName)
    {
        int userID = PlayerPrefs.GetInt("UserID");
        StartCoroutine(ClaimRewardCoroutine(userID, badgeName));
    }

    private IEnumerator ClaimRewardCoroutine(int userID, string badgeName)
    {
        Debug.Log($"🚀 嘗試領取成就：{badgeName}");

        GameObject panel = GetPanelObject(badgeName);
        if (panel == null)
            yield break;

        string jsonData = $"{{\"badge_name\": \"{badgeName}\"}}";
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = new UnityWebRequest($"{baseUrl}/claim_achievement/{userID}", "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            Debug.Log($"🚀 正在發送領取請求：{jsonData}");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"✅ 成就 {badgeName} 領取成功！回應：{request.downloadHandler.text}");
                PlayerPrefs.SetInt($"AchievementClaimed_{badgeName}", 1);
                PlayerPrefs.DeleteKey($"AchievementUnlocked_{badgeName}");
                PlayerPrefs.Save();
                SetPanelButtons(panel, false, false, true);

                // 檢查是否還有未領取的成就
                bool hasUnclaimedAchievements = false;
                string[] achievements = new string[] { "新增一門課程", "完整上完一門課", "學習積分達到 500 分" };
                foreach (string achievement in achievements)
                {
                    if (PlayerPrefs.GetInt($"AchievementUnlocked_{achievement}", 0) == 1)
                    {
                        hasUnclaimedAchievements = true;
                        break;
                    }
                }

                // 如果沒有未領取的成就，隱藏紅點
                if (!hasUnclaimedAchievements)
                {
                    if (achievementRedDotImage != null)
                        achievementRedDotImage.gameObject.SetActive(false);
                    if (achievementRedDotImage2 != null)
                        achievementRedDotImage2.gameObject.SetActive(false);
                    SaveRedDotState(false);
                }
            }
            else
            {
                Debug.LogError("❌ 領取失敗：" + request.downloadHandler.text);
            }
        }
    }

    // 設定面板上各按鈕的顯示狀態
    // showDisabled：未解鎖；showClaim：解鎖但未領取；showShare：已領取
    private void SetPanelButtons(GameObject panel, bool showDisabled, bool showClaim, bool showShare)
    {
        panel.transform.Find("DisabledButton")?.gameObject.SetActive(showDisabled);
        panel.transform.Find("ClaimButton")?.gameObject.SetActive(showClaim);
        panel.transform.Find("ShareButton")?.gameObject.SetActive(showShare);
    }

    // 根據本地快取更新所有成就的 UI（包含徽章遮罩與面板按鈕）
    private void UpdateAllAchievementsUI()
    {
        string[] achievements = new string[] { "新增一門課程", "完整上完一門課", "學習積分達到 500 分" };
        foreach (string achievement in achievements)
        {
            GameObject badge = GetBadgeObject(achievement);
            GameObject panel = GetPanelObject(achievement);
            if (badge == null || panel == null)
                continue;

            GameObject coverImage = badge.transform.Find("AchievementCoverImage")?.gameObject;
            if (coverImage != null)
            {
                // 若本地標記為已解鎖或已領取，則隱藏遮罩；否則顯示遮罩
                if (PlayerPrefs.GetInt($"AchievementUnlocked_{achievement}", 0) == 1 ||
                    PlayerPrefs.GetInt($"AchievementClaimed_{achievement}", 0) == 1)
                {
                    coverImage.SetActive(false);
                }
                else
                {
                    coverImage.SetActive(true);
                }
            }

            // 根據本地狀態設定面板按鈕：
            // 若已領取 → 僅顯示 Share；
            // 若解鎖但未領取 → 顯示 Claim；
            // 否則顯示 Disabled
            if (PlayerPrefs.GetInt($"AchievementClaimed_{achievement}", 0) == 1)
            {
                SetPanelButtons(panel, false, false, true);
            }
            else if (PlayerPrefs.GetInt($"AchievementUnlocked_{achievement}", 0) == 1)
            {
                SetPanelButtons(panel, false, true, false);
            }
            else
            {
                SetPanelButtons(panel, true, false, false);
            }
        }
    }

    // 登出時呼叫，清除所有本地與該用戶相關的成就快取
    public void ClearUserAchievementData()
    {
        string[] achievements = new string[] { "新增一門課程", "完整上完一門課", "學習積分達到 500 分" };
        foreach (string achievement in achievements)
        {
            PlayerPrefs.DeleteKey($"AchievementUnlocked_{achievement}");
            PlayerPrefs.DeleteKey($"AchievementClaimed_{achievement}");
        }
        PlayerPrefs.DeleteKey(ACHIEVEMENT_RED_DOT_STATE_KEY);
        PlayerPrefs.Save();

        // 隱藏紅點
        if (achievementRedDotImage != null)
            achievementRedDotImage.gameObject.SetActive(false);
        if (achievementRedDotImage2 != null)
            achievementRedDotImage2.gameObject.SetActive(false);

        UpdateAllAchievementsUI();
    }

    // 透過成就名稱取得對應的徽章 GameObject
    private GameObject GetBadgeObject(string badgeName)
    {
        switch (badgeName)
        {
            case "新增一門課程":
                return courseAddedBadge;
            case "完整上完一門課":
                return courseCompletedBadge;
            case "學習積分達到 500 分":
                return pointsBadge;
            default:
                Debug.LogError($"❌ 找不到對應的徽章: {badgeName}");
                return null;
        }
    }

    // 透過成就名稱取得對應的 Panel
    private GameObject GetPanelObject(string badgeName)
    {
        switch (badgeName)
        {
            case "新增一門課程":
                return courseAddedPanel;
            case "完整上完一門課":
                return courseCompletedPanel;
            case "學習積分達到 500 分":
                return pointsPanel;
            default:
                Debug.LogError($"❌ 找不到對應的 Panel: {badgeName}");
                return null;
        }
    }

    // 新增：顯示成就頁面時隱藏紅點
    public void ShowAchievementPage()
    {
        // 隱藏兩個紅點並保存狀態
        if (achievementRedDotImage != null)
            achievementRedDotImage.gameObject.SetActive(false);
        if (achievementRedDotImage2 != null)
            achievementRedDotImage2.gameObject.SetActive(false);

        SaveRedDotState(false);
    }
}

[System.Serializable]
public class CheckAchievementResponse
{
    public string[] new_achievements;
}

[System.Serializable]
public class AchievementStatusResponse
{
    public bool is_claimed;
}

[System.Serializable]
public class AchievementsResponse
{
    public AchievementData[] achievements;
}

[System.Serializable]
public class AchievementData
{
    public string badge_name;
    public bool is_claimed;
}