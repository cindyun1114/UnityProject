using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Networking;

public class VRLessonManager : MonoBehaviour
{
    [Header("額外 UI 元件")]
    public GameObject continuePanel;  // **繼續上課確認視窗**
    public GameObject reviewPage;     // **複習頁面**
    public TMP_Text reviewCourseName; // **複習頁面 - 課程名稱**
    public TMP_Text reviewCreatedAt;  // **複習頁面 - 課程創建時間**
    public Button confirmContinueButton; // **確認繼續按鈕**

    private string baseUrl = "https://feyndora-api.onrender.com"; // Flask API 伺服器
    private int selectedCourseId;  // **目前選中的課程 ID**
    private string selectedCourseName; // **目前選中的課程名稱**
    private string selectedCourseCreatedAt; // **目前選中的課程建立時間**

    void Start()
    {
        if (confirmContinueButton != null)
        {
            confirmContinueButton.onClick.AddListener(() => StartCoroutine(ConfirmContinueCourse()));
        }
    }

    // **🔹 當課程被點擊時**
    public void OnCourseClicked(int courseId, string courseName, string createdAt, float progress)
    {
        selectedCourseId = courseId;
        selectedCourseName = courseName;
        selectedCourseCreatedAt = createdAt;

        if (progress >= 100)
        {
            // **✅ 進度 100%，顯示複習頁**
            reviewCourseName.text = courseName;
            reviewCreatedAt.text = $"建立時間: {createdAt}";
            reviewPage.SetActive(true);
        }
        else
        {
            // **✅ 進度未達 100%，顯示確認視窗**
            continuePanel.SetActive(true);
        }
    }

    // **🚀 確認繼續上課**
    IEnumerator ConfirmContinueCourse()
    {
        if (selectedCourseId == 0)
        {
            Debug.LogError("❌ 沒有選擇課程，無法繼續！");
            yield break;
        }

        string url = $"{baseUrl}/continue_course";
        string jsonData = $"{{\"course_id\": {selectedCourseId}}}";
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"✅ 成功通知後端，開始課程: {selectedCourseName}");
                continuePanel.SetActive(false);
            }
            else
            {
                Debug.LogError($"❌ 失敗: {request.downloadHandler.text}");
            }
        }
    }
}