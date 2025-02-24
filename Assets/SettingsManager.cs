using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Networking;

public class SettingsManager : MonoBehaviour
{
    [Header("UI 元件")]
    public TMP_Text usernameText;  // 顯示用戶名稱
    public TMP_Text emailText;     // 顯示用戶 Email
    public TMP_InputField newNicknameInput; // 修改暱稱輸入框
    public Button updateNicknameButton; // 更新暱稱按鈕
    public Button deleteAccountButton; // 刪除帳號按鈕
    public GameObject confirmationPanel; // 確認刪除視窗
    public Button confirmDeleteButton; // 確認刪除按鈕
    public Button cancelDeleteButton;  // 取消刪除按鈕
    public GameObject loginPanel; // 登入畫面 Panel（用於刪除帳號後跳轉）

    private string baseUrl = "https://feyndora-api.onrender.com"; // Flask API 伺服器

    void OnEnable()
    {
        LoadUserData(); // **✅ 當 Settings 頁面開啟時，自動載入用戶資料**
    }

    void Start()
    {
        // **✅ 設定更新暱稱按鈕**
        updateNicknameButton.onClick.AddListener(() => StartCoroutine(UpdateNickname()));

        // **✅ 設定刪除帳號按鈕**
        deleteAccountButton.onClick.AddListener(ShowDeleteConfirmation);
        confirmDeleteButton.onClick.AddListener(() => StartCoroutine(DeleteAccount()));
        cancelDeleteButton.onClick.AddListener(HideDeleteConfirmation);
    }

    // **📌 載入用戶資訊**
    void LoadUserData()
    {
        int userID = PlayerPrefs.GetInt("UserID", -1);
        if (userID == -1)
        {
            Debug.LogError("❌ 無法獲取用戶 ID");
            return;
        }

        string savedUsername = PlayerPrefs.GetString("Username", "未知用戶");
        string savedEmail = PlayerPrefs.GetString("UserEmail", "未綁定 Email");

        usernameText.text = savedUsername;
        emailText.text = savedEmail; // ✅ 確保 Email 正確顯示

        Debug.Log($"📢 載入用戶資料: 用戶名: {savedUsername}, Email: {savedEmail}");
    }

    // **📝 更新暱稱 API**
    IEnumerator UpdateNickname()
    {
        int userID = PlayerPrefs.GetInt("UserID", -1);
        if (userID == -1)
        {
            Debug.LogError("❌ 無法獲取用戶 ID");
            yield break;
        }

        string newNickname = newNicknameInput.text.Trim();
        if (string.IsNullOrEmpty(newNickname))
        {
            Debug.LogError("❌ 暱稱不能為空");
            yield break;
        }

        string jsonData = $"{{\"nickname\": \"{newNickname}\"}}";
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = new UnityWebRequest(baseUrl + "/update_nickname/" + userID, "PUT"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("✅ 暱稱更新成功");
                PlayerPrefs.SetString("Username", newNickname); // **✅ 更新本地暱稱**
                usernameText.text = newNickname; // **✅ 立即更新 UI**

                // **✅ 讓 APIManager 重新獲取數據，確保 HomePagePanel 也更新**
                if (APIManager.Instance != null)
                {
                    APIManager.Instance.StartCoroutine(APIManager.Instance.FetchUserData());
                }
                else
                {
                    Debug.LogError("❌ APIManager.Instance 為 null，請確保 APIManager 存在於場景中！");
                }
            }
            else
            {
                Debug.LogError("❌ 暱稱更新失敗：" + request.downloadHandler.text);
            }
        }
    }

    // **🗑️ 顯示確認刪除彈窗**
    void ShowDeleteConfirmation()
    {
        confirmationPanel.SetActive(true);
    }

    // **❌ 隱藏確認刪除彈窗**
    void HideDeleteConfirmation()
    {
        confirmationPanel.SetActive(false);
    }

    // **🚀 刪除帳號 API**
    IEnumerator DeleteAccount()
    {
        int userID = PlayerPrefs.GetInt("UserID", -1);
        if (userID == -1)
        {
            Debug.LogError("❌ 無法獲取用戶 ID");
            yield break;
        }

        using (UnityWebRequest request = UnityWebRequest.Delete(baseUrl + "/delete_user/" + userID))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("🗑️ 帳號刪除成功");

                // **✅ 清除本地用戶資訊**
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();

                // **✅ 確保 APIManager 存在並執行 Logout（清除 UI & 回到登入頁）**
                if (APIManager.Instance != null)
                {
                    APIManager.Instance.Logout();
                }
                else
                {
                    Debug.LogError("❌ APIManager.Instance 為 null，請確保 APIManager 存在於場景中！");
                }
            }
            else
            {
                Debug.LogError("❌ 刪除帳號失敗：" + request.downloadHandler.text);
            }
        }
    }
}