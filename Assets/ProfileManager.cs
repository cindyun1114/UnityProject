using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class ProfileManager : MonoBehaviour
{
    public static ProfileManager Instance;  // Singleton

    [Header("AvatarManager (請在Inspector設定)")]
    public AvatarManager avatarManager;

    [Header("用戶資料 UI")]
    public Image avatarImage;
    public Image teacherCardImage;  // 添加老師卡片圖片引用

    [Header("卡片圖片資源")]
    public Sprite goblinSprite;      // 哥布林
    public Sprite athenaSprite;      // 雅典娜
    public Sprite pirateSprite;      // 海盜
    public Sprite teacherSprite;     // 盛惟老師
    public Sprite santaSprite;       // 聖誕老公公
    public Sprite popeSprite;        // 教宗

    public TMP_Text usernameText;
    public TMP_Text emailText;
    public TMP_Text totalPointsText;
    public TMP_Text diamondsText;
    public TMP_Text coinsText;
    public TMP_Text coursesCountText;  // 顯示用戶課程數量
    public TMP_Text signinDaysText; // 顯示累積簽到天數

    [Header("分數統計 - 長條圖 (Bar Chart)")]
    public Image[] barImages;  // 7 個 Bar (代表最近 7 天)
    public float baseHeight = 50f;       // 50 高度對應 125 分數
    public float heightPerPoint = 50f / 125f;  // 每 1 分數的高度
    public float maxBarHeight = 240f;      // 長條最大高度

    private string baseUrl = "https://feyndora-api.onrender.com";

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        // 頁面啟動時從本地資料更新 UI（避免不必要的 API 呼叫）
        LoadUserDataFromPrefs();
        // 確保即時刷新數據
        StartCoroutine(RefreshAll());
    }

    /// <summary>
    /// 讓 APIManager 呼叫此方法刷新 Profile 頁面（從伺服器取得最新資料）
    /// </summary>
    public void RefreshProfile()
    {
        if (!gameObject.activeInHierarchy) return; // 避免 Coroutine 啟動失敗
        StartCoroutine(RefreshAll());
        UpdateTeacherCard();  // 更新老師卡片
    }

    /// <summary>
    /// 讀取本地 PlayerPrefs 資料更新 UI（初次顯示用）
    /// </summary>
    public void LoadUserDataFromPrefs()
    {
        int avatarId = PlayerPrefs.GetInt("AvatarID", 1);
        string username = PlayerPrefs.GetString("Username", "未知用戶");
        string email = PlayerPrefs.GetString("UserEmail", "未綁定 Email");
        int totalPoints = PlayerPrefs.GetInt("TotalPoints", 0);
        int coins = PlayerPrefs.GetInt("Coins", 0);
        int diamonds = PlayerPrefs.GetInt("Diamonds", 0);
        int total_signin_days = PlayerPrefs.GetInt("TotalSigninDays", 0); // 讀取簽到天數

        avatarImage.sprite = avatarManager.GetAvatarSprite(avatarId);
        usernameText.text = username;
        emailText.text = email;
        totalPointsText.text = totalPoints.ToString();
        coinsText.text = coins.ToString();
        diamondsText.text = diamonds.ToString();
        signinDaysText.text = total_signin_days.ToString(); // 顯示在UI上 
    }

    /// <summary>
    /// 更新老師卡片圖片
    /// </summary>
    public void UpdateTeacherCard()
    {
        if (teacherCardImage == null)
        {
            Debug.LogError("Teacher Card Image component not assigned!");
            return;
        }

        int userId = PlayerPrefs.GetInt("UserID");
        APIManager.Instance.GetUserCards(userId, (cards) =>
        {
            if (cards != null && cards.Count > 0)
            {
                // 尋找被選中的卡片
                var selectedCard = cards.Find(card => card.is_selected);
                if (selectedCard != null)
                {
                    // 根據卡片名稱設置對應的圖片
                    Sprite cardSprite = GetTeacherCardSprite(selectedCard.name);
                    if (cardSprite != null)
                    {
                        teacherCardImage.sprite = cardSprite;
                        Debug.Log($"更新Profile頁面老師卡片為：{selectedCard.name}");
                    }
                }
                else
                {
                    Debug.LogWarning("用戶沒有選中的卡片");
                    teacherCardImage.sprite = null;
                }
            }
            else
            {
                Debug.LogWarning("用戶沒有卡片");
                teacherCardImage.sprite = null;
            }
        });
    }

    /// <summary>
    /// 根據卡片名稱獲取對應的Sprite
    /// </summary>
    private Sprite GetTeacherCardSprite(string cardName)
    {
        Sprite cardSprite = null;
        switch (cardName)
        {
            case "哥布林":
                cardSprite = goblinSprite;
                break;
            case "雅典娜":
                cardSprite = athenaSprite;
                break;
            case "海盜":
                cardSprite = pirateSprite;
                break;
            case "盛惟老師":
                cardSprite = teacherSprite;
                break;
            case "聖誕老公公":
                cardSprite = santaSprite;
                break;
            case "教宗":
                cardSprite = popeSprite;
                break;
            default:
                Debug.LogError($"未知的卡片名稱：{cardName}");
                break;
        }

        if (cardSprite == null)
        {
            Debug.LogError($"卡片 {cardName} 的圖片資源未設置");
        }

        return cardSprite;
    }

    /// <summary>
    /// 清空 Profile 頁面所有 UI 資料（用於登出時）
    /// </summary>
    public void ClearProfileUI()
    {
        avatarImage.sprite = avatarManager.GetAvatarSprite(1); // 預設頭像
        if (teacherCardImage != null)
        {
            teacherCardImage.sprite = null;  // 清空老師卡片
        }
        usernameText.text = "";
        emailText.text = "";
        totalPointsText.text = "0";
        coinsText.text = "0";
        diamondsText.text = "0";
        coursesCountText.text = "0";
        signinDaysText.text = "0";
        foreach (Image bar in barImages)
        {
            RectTransform rt = bar.rectTransform;
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, 0);
        }
    }

    /// <summary>
    /// 一次刷新：從伺服器取得用戶資料、課程數量、以及最近 7 天每日積分（更新長條圖）
    /// </summary>
    IEnumerator RefreshAll()
    {
        yield return StartCoroutine(GetUserProfile());
        yield return StartCoroutine(GetCoursesCount());
        yield return StartCoroutine(GetWeeklyPoints());
    }

    /// <summary>
    /// 從 /user/{user_id} 取得用戶資料並更新 UI
    /// </summary>
    IEnumerator GetUserProfile()
    {
        int userID = PlayerPrefs.GetInt("UserID", 0);
        if (userID == 0)
        {
            Debug.LogError("❌ 未登入，無法取得用戶資料");
            yield break;
        }

        string url = $"{baseUrl}/user/{userID}";
        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            req.SetRequestHeader("Content-Type", "application/json");
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                UserData user = JsonUtility.FromJson<UserData>(req.downloadHandler.text);

                // 更新 UI 元件
                avatarImage.sprite = avatarManager.GetAvatarSprite(user.avatar_id);
                usernameText.text = user.username;
                emailText.text = user.email;
                totalPointsText.text = user.total_learning_points.ToString();
                coinsText.text = user.coins.ToString();
                diamondsText.text = user.diamonds.ToString();

                // 更新本地資料
                PlayerPrefs.SetString("Username", user.username);
                PlayerPrefs.SetString("UserEmail", user.email);
                PlayerPrefs.SetInt("TotalPoints", user.total_learning_points);
                PlayerPrefs.SetInt("Coins", user.coins);
                PlayerPrefs.SetInt("Diamonds", user.diamonds);
                PlayerPrefs.SetInt("AvatarID", user.avatar_id);
                PlayerPrefs.Save();
            }
            else
            {
                Debug.LogError("❌ 取得用戶資料失敗: " + req.error);
            }
        }
    }

    /// <summary>
    /// 從 /courses_count/{user_id} 取得用戶的課程數量並更新 UI
    /// </summary>
    IEnumerator GetCoursesCount()
    {
        int userID = PlayerPrefs.GetInt("UserID", 0);
        if (userID == 0) yield break;

        string url = $"{baseUrl}/courses_count/{userID}";
        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            req.SetRequestHeader("Content-Type", "application/json");
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                CoursesCountResponse resp = JsonUtility.FromJson<CoursesCountResponse>(req.downloadHandler.text);
                if (coursesCountText != null)
                {
                    coursesCountText.text = resp.courses_count.ToString();
                }
            }
            else
            {
                Debug.LogError("❌ 取得課程數量失敗: " + req.error);
            }
        }
    }

    /// <summary>
    /// 從 /weekly_points/{user_id} 取得最近 7 天的每日積分，根據固定換算公式更新長條圖高度
    /// </summary>
    IEnumerator GetWeeklyPoints()
    {
        int userID = PlayerPrefs.GetInt("UserID", 0);
        if (userID == 0) yield break;

        string url = $"{baseUrl}/weekly_points/{userID}";
        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                WeeklyPointsResponse resp = JsonUtility.FromJson<WeeklyPointsResponse>(req.downloadHandler.text);

                // 確保取得 7 天數據，不足補 0
                List<int> points = resp.weekly_points;
                while (points.Count < 7)
                {
                    points.Add(0);
                }

                // 設定換算公式：固定 50 高度代表 125 分，超過 240 高度顯示 240
                float heightPerPoint = 50f / 125f;
                float maxBarHeight = 240f;

                for (int i = 0; i < barImages.Length && i < points.Count; i++)
                {
                    float newHeight = points[i] * heightPerPoint;
                    newHeight = Mathf.Min(newHeight, maxBarHeight);
                    RectTransform rt = barImages[i].rectTransform;
                    rt.sizeDelta = new Vector2(rt.sizeDelta.x, newHeight);
                    Debug.Log($"📊 Bar {i}: Points = {points[i]}, NewHeight = {newHeight}");
                }
            }
            else
            {
                Debug.LogError("❌ 取得最近7天分數失敗: " + req.error);
            }
        }
    }

    // --- 資料結構 ---
    [System.Serializable]
    public class UserData
    {
        public int user_id;
        public string username;
        public string email;
        public int total_learning_points;
        public int coins;
        public int diamonds;
        public int avatar_id;
    }

    [System.Serializable]
    public class CoursesCountResponse
    {
        public int courses_count;
    }

    [System.Serializable]
    public class WeeklyPointsResponse
    {
        public List<int> weekly_points;
    }
}