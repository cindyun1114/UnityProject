using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

public class AvatarManager : MonoBehaviour
{
    [Header("頭像按鈕（6個）")]
    public GameObject[] avatarButtons;   // 放6個頭像按鈕
    public Sprite[] avatarSprites;       // 6個頭像的圖片

    [Header("顯示的頭像區域")]
    public Image settingPageAvatar;      // 設定頁的頭像
    public Image homePageAvatar;         // 主頁的頭像

    private int selectedAvatarId = 1;    // 預設選第一個
    private string baseUrl = "https://feyndora-api.onrender.com"; // Flask API

    void Start()
    {
        // **每個頭像按鈕都加上點擊事件**
        for (int i = 0; i < avatarButtons.Length; i++)
        {
            int avatarId = i + 1;  // 頭像ID從1開始
            avatarButtons[i].GetComponent<Button>().onClick.AddListener(() => SelectAvatar(avatarId));
        }

        LoadCurrentAvatar();  // 進入時載入當前頭像
    }

    // **載入當前用戶的頭像（從PlayerPrefs取）**
    public void LoadCurrentAvatar()
    {
        int savedAvatarId = PlayerPrefs.GetInt("AvatarID", 1);  // 預設1
        SelectAvatar(savedAvatarId, true);
    }

    // **選擇頭像**
    public void SelectAvatar(int avatarId, bool isInit = false)
    {
        selectedAvatarId = avatarId;

        // **先清除所有選擇框**
        foreach (var button in avatarButtons)
        {
            Transform selectionIndicator = button.transform.Find("SelectionIndicator");
            if (selectionIndicator != null)
            {
                selectionIndicator.gameObject.SetActive(false);
            }
        }

        // **選中的頭像顯示框框**
        Transform selectedIndicator = avatarButtons[avatarId - 1].transform.Find("SelectionIndicator");
        if (selectedIndicator != null)
        {
            selectedIndicator.gameObject.SetActive(true);
        }

        // **設定頁面即時更新頭像**
        settingPageAvatar.sprite = avatarSprites[avatarId - 1];

        // **如果是初始化，就不更新資料庫，單純載入UI即可**
        if (!isInit)
        {
            Debug.Log($"📸 選擇了新頭像: {avatarId}");
        }
    }

    // **確認選擇頭像，送到資料庫**
    public void ConfirmSelection()
    {
        StartCoroutine(UpdateAvatarInDatabase(selectedAvatarId));
    }

    // **送API更新資料庫的頭像**
    IEnumerator UpdateAvatarInDatabase(int avatarId)
    {
        int userId = PlayerPrefs.GetInt("UserID");
        string url = $"{baseUrl}/update_avatar/{userId}";
        string jsonData = $"{{\"avatar_id\": {avatarId}}}";

        using (UnityWebRequest request = new UnityWebRequest(url, "PUT"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("✅ 頭像更新成功");

                PlayerPrefs.SetInt("AvatarID", avatarId);
                PlayerPrefs.Save();

                UpdateHomePageAvatar(avatarId);  // 立即刷新主頁頭像
            }
            else
            {
                Debug.LogError("❌ 頭像更新失敗: " + request.downloadHandler.text);
            }
        }
    }

    // **更新主頁的頭像**
    public void UpdateHomePageAvatar(int avatarId)
    {
        if (homePageAvatar != null)
        {
            homePageAvatar.sprite = avatarSprites[avatarId - 1];
        }
    }
}