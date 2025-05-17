using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Networking;
using System.Text;

public class VRLessonManager : MonoBehaviour
{
    [Header("評分系統")]
    public Slider accuracySlider;
    public Slider understandingSlider;
    public Slider expressionSlider;
    public Slider interactionSlider;

    [Header("評分文字")]
    public TMP_Text accuracyText;
    public TMP_Text understandingText;
    public TMP_Text expressionText;
    public TMP_Text interactionText;

    [Header("評語區塊")]
    public TMP_Text teacherCommentText;
    public TMP_Text[] studentFeedbackTexts;
    public TMP_Text pointsText;

    [Header("重點回顧")]
    public TMP_Text reviewText;    // PartTwo 下的 reviewText

    [Header("額外 UI 元件")]
    public GameObject continuePanel;  // **繼續上課確認視窗**
    public GameObject reviewPagePanel;     // **複習頁面**

    public GameObject loadingPagePanel;
    public TMP_Text reviewCourseName; // **複習頁面 - 課程名稱**
    public TMP_Text reviewCreatedAt;  // **複習頁面 - 課程創建時間**
    public Button confirmContinueButton; // **確認繼續按鈕**

    [Header("目錄 UI 元件")]
    public Transform chapterListContainer; //放章節的List
    public GameObject chapterPrefab; //章節Prefab

    private reviewChat reviewChatScript;

    private string baseUrl = "https://feyndora-api.onrender.com"; // Flask API 伺服器
    private string apiFetchUrl = "https://feynman-server.onrender.com/fetch";
    public string apiUrl = "https://feynman-server.onrender.com/get_chapters";

    public string apiFetchCloudLink = "https://feynman-server.onrender.com/get_cloud_link";
    private int selectedCourseId;  // **目前選中的課程 ID**
    private string selectedCourseName; // **目前選中的課程名稱**
    private string selectedCourseCreatedAt; // **目前選中的課程建立時間**

    void Start()
    {
        if (confirmContinueButton != null)
        {
            confirmContinueButton.onClick.AddListener(() => StartCoroutine(ConfirmContinueCourse()));
        }

        // 初始化所有滑块
        InitializeSliders();
    }

    private void InitializeSliders()
    {
        // 设置滑块的最小值和最大值
        accuracySlider.minValue = 0;
        accuracySlider.maxValue = 100;
        accuracySlider.wholeNumbers = true;
        accuracySlider.interactable = false;  // 设置为不可交互

        understandingSlider.minValue = 0;
        understandingSlider.maxValue = 100;
        understandingSlider.wholeNumbers = true;
        understandingSlider.interactable = false;

        expressionSlider.minValue = 0;
        expressionSlider.maxValue = 100;
        expressionSlider.wholeNumbers = true;
        expressionSlider.interactable = false;

        interactionSlider.minValue = 0;
        interactionSlider.maxValue = 100;
        interactionSlider.wholeNumbers = true;
        interactionSlider.interactable = false;
    }

    // **🔹 當課程被點擊時**
    public void OnCourseClicked(int courseId, string courseName, string createdAt, float progress)
    {
        selectedCourseId = courseId;
        selectedCourseName = courseName;
        selectedCourseCreatedAt = createdAt;

        if (progress >= 100)
        {
            loadingPagePanel.SetActive(true);
            // **✅ 進度 100%，顯示複習頁**
            reviewCourseName.text = courseName;

            // 解析時間字串並格式化為 yyyy/MM/dd HH:mm:ss
            if (System.DateTime.TryParse(createdAt, out System.DateTime dateTime))
            {
                reviewCreatedAt.text = dateTime.ToString("yyyy/MM/dd HH:mm:ss");
            }
            else
            {
                reviewCreatedAt.text = createdAt;  // 如果解析失敗，顯示原始時間
            }

            reviewChatScript = reviewPagePanel.GetComponent<reviewChat>();
            if (reviewChatScript != null)
            {
                reviewChatScript.InitReviewPagePanel();
                Debug.Log("Cleaning ReviewPagePanel...");
            }

            // 使用协程等待所有数据加载完成
            StartCoroutine(LoadAllCourseData(courseId));
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

    // 新增：加载所有课程数据的协程
    private IEnumerator LoadAllCourseData(int courseId)
    {
        Debug.Log("🔄 开始加载所有课程数据...");
        bool reviewLoaded = false;
        bool tocLoaded = false;
        bool assistantLoaded = false;
        bool cloudLinkLoaded = false;

        // 启动所有加载协程
        StartCoroutine(LoadCourseReviewWithCallback(courseId, () => reviewLoaded = true));
        StartCoroutine(LoadToCWithCallback(courseId, () => tocLoaded = true));
        StartCoroutine(LoadAssistantWithCallback(courseId, () => assistantLoaded = true));
        StartCoroutine(LoadCloudLinkWithCallback(courseId, () => cloudLinkLoaded = true));

        // 等待所有数据加载完成
        while (!reviewLoaded || !tocLoaded || !assistantLoaded || !cloudLinkLoaded)
        {
            yield return null;
        }

        Debug.Log("✅ 所有课程数据加载完成");
        loadingPagePanel.SetActive(false);
        reviewPagePanel.SetActive(true);
    }

    // 修改现有的加载方法，添加回调
    private IEnumerator LoadCourseReviewWithCallback(int courseId, System.Action onComplete)
    {
        yield return StartCoroutine(LoadCourseReview(courseId));
        onComplete?.Invoke();
    }

    private IEnumerator LoadToCWithCallback(int courseId, System.Action onComplete)
    {
        yield return StartCoroutine(LoadToC(courseId));
        onComplete?.Invoke();
    }

    private IEnumerator LoadAssistantWithCallback(int courseId, System.Action onComplete)
    {
        yield return StartCoroutine(LoadAssistant(courseId));
        onComplete?.Invoke();
    }

    private IEnumerator LoadCloudLinkWithCallback(int courseId, System.Action onComplete)
    {
        yield return StartCoroutine(LoadCloudLink(courseId));
        onComplete?.Invoke();
    }

    private IEnumerator LoadCourseReview(int courseId)
    {
        Debug.Log($"🔍 开始加载课程回顾 - CourseID: {courseId}");
        string url = $"{baseUrl}/course_review/{courseId}";
        Debug.Log($"🌐 请求URL: {url}");

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.timeout = 30; // 设置30秒超时
            request.SetRequestHeader("Content-Type", "application/json");

            Debug.Log("⏳ 发送请求...");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;
                Debug.Log($"✅ 请求成功，响应内容: {responseText}");

                try
                {
                    CourseReviewData reviewData = JsonUtility.FromJson<CourseReviewData>(responseText);
                    if (reviewData == null)
                    {
                        Debug.LogError("❌ JSON解析失败：返回的数据格式不正确");
                        yield break;
                    }
                    UpdateReviewUI(reviewData);
                    Debug.Log("✅ 课程回顾数据更新成功");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"❌ JSON解析错误: {e.Message}\n响应内容: {responseText}");
                }
            }
            else
            {
                string errorMessage = $"❌ 加载课程回顾失败: {request.error}\n状态码: {request.responseCode}\n响应内容: {request.downloadHandler.text}";
                Debug.LogError(errorMessage);

                // 显示用户友好的错误信息
                if (teacherCommentText != null)
                {
                    teacherCommentText.text = "加载课程回顾时发生错误，请稍后重试";
                }
            }
        }
    }

    IEnumerator LoadToC(int courseId)
    {
        Debug.Log("抓取目錄中......");
        string apiUrl = $"{this.apiUrl}?course_id={courseId}&chapter_type={"one_to_one"}";

        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("API Response: " + request.downloadHandler.text);
                string jsonResponse = "{\"chapters\":" + request.downloadHandler.text + "}";
                ChapterList chapterList = JsonUtility.FromJson<ChapterList>(jsonResponse);

                if (chapterList == null || chapterList.chapters == null)
                {
                    Debug.LogError("Error: Parsed JSON is null. Check API response format.");
                }

                DisplayChapters(chapterList.chapters);

            }
            else
            {
                Debug.LogError("Error fetching chapters: " + request.error);
            }
        }
        Debug.Log("抓取目錄完成");
    }

    void DisplayChapters(Chapter[] chapters)
    {
        if (chapterPrefab == null)
        {
            Debug.LogError("Error: chapterPrefab is null!");
            return;
        }

        if (chapterListContainer == null)
        {
            Debug.LogError("Error: chapterListContainer is null!");
            return;
        }

        foreach (Chapter chapter in chapters)
        {
            GameObject newChapter = Instantiate(chapterPrefab, chapterListContainer);

            TMP_Text textComponent = newChapter.GetComponentInChildren<TMP_Text>();
            if (textComponent == null)
            {
                Debug.LogError("Error: TMP_Text component is missing on prefab!");
                return;
            }

            textComponent.text = chapter.chapter_name;
        }
    }

    public IEnumerator LoadAssistant(int courseId)
    {
        Debug.Log("找Assistant和thread...");
        Debug.Log($"CourseId: {courseId}");
        PlayerPrefs.SetInt("Course_ID", courseId);

        string jsonData = JsonUtility.ToJson(new fetchRequest
        {
            action = "fetch_assistant_and_thread",
            course_id = courseId,
            role = "teacher"
        });

        using (UnityWebRequest request = new UnityWebRequest(apiFetchUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonUtility.FromJson<fetchResponse>(request.downloadHandler.text);
                PlayerPrefs.SetString("Assistant1_ID", response.assistant_id);
                PlayerPrefs.SetString("Thread1_ID", response.thread_id);
                Debug.Log("assistant_Id and thread_id fetched successfully");
                Debug.Log("assistant_Id: " + response.assistant_id);
                Debug.Log("thread_id: " + response.thread_id);
            }
            else
            {
                Debug.Log("Error: " + request.error);
            }
        }
    }

    public IEnumerator LoadCloudLink(int courseId)
    {
        Debug.Log("找Cloud Url...");

        string jsonData = JsonUtility.ToJson(new fetchRequest
        {
            action = "get_cloud",
            course_id = courseId,
            role = ""
        });

        using (UnityWebRequest request = new UnityWebRequest(apiFetchCloudLink, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonUtility.FromJson<fetchCloudLink>(request.downloadHandler.text);
                PlayerPrefs.SetString("Cloud_Link", response.cloud_link);
                Debug.Log("assistant_Id and thread_id fetched successfully");
                Debug.Log("Fetched Cloud_Link: " + response.cloud_link);
            }
            else
            {
                Debug.Log("Error: " + request.error);
            }
        }
    }

    [System.Serializable]
    private class CourseReviewData
    {
        public int accuracy_score;
        public int understanding_score;
        public int expression_score;
        public int interaction_score;
        public string teacher_comment;
        public string student1_feedback;
        public string student2_feedback;
        public string student3_feedback;
        public int earned_points;
        public string[] good_points;      // 做得好的點
        public string[] improvement_points; // 需要加強的點
    }
    [System.Serializable]
    public class Chapter
    {
        public int chapter_id;
        public string chapter_name;
        public int order_index;
    }

    [System.Serializable]
    public class ChapterList
    {
        public Chapter[] chapters;
    }

    private void UpdateReviewUI(CourseReviewData data)
    {
        // 更新評分
        accuracySlider.value = data.accuracy_score;
        understandingSlider.value = data.understanding_score;
        expressionSlider.value = data.expression_score;
        interactionSlider.value = data.interaction_score;

        // 更新評分文字
        accuracyText.text = $"{data.accuracy_score}%";
        understandingText.text = $"{data.understanding_score}%";
        expressionText.text = $"{data.expression_score}%";
        interactionText.text = $"{data.interaction_score}%";

        // 更新評語
        if (teacherCommentText != null)
        {
            teacherCommentText.text = data.teacher_comment;
            Debug.Log($"✅ 更新老師評語: {data.teacher_comment}");
        }
        else
        {
            Debug.LogError("❌ teacherCommentText 未設置");
        }

        studentFeedbackTexts[0].text = data.student1_feedback;
        studentFeedbackTexts[1].text = data.student2_feedback;
        studentFeedbackTexts[2].text = data.student3_feedback;

        // 更新積分
        pointsText.text = $"{data.earned_points}";

        // 更新重點回顧
        StringBuilder reviewContent = new StringBuilder();

        // 添加做得好的點
        if (data.good_points != null && data.good_points.Length > 0)
        {
            reviewContent.AppendLine("<color=#4CAF50>做得好的點：</color>");
            foreach (var point in data.good_points)
            {
                reviewContent.AppendLine($"<color=#4CAF50>{point}</color>");
            }
            reviewContent.AppendLine();
        }

        // 添加需要加強的點
        if (data.improvement_points != null && data.improvement_points.Length > 0)
        {
            reviewContent.AppendLine("<color=#FF9800>需要加強的點：</color>");
            foreach (var point in data.improvement_points)
            {
                reviewContent.AppendLine($"<color=#FF9800>{point}</color>");
            }
        }

        // 更新文字內容
        reviewText.text = reviewContent.ToString();

        // 計算文字所需的高度
        float textHeight = reviewText.preferredHeight;

        // 獲取所有需要的組件
        RectTransform imageRect = reviewText.transform.parent.GetComponent<RectTransform>();
        RectTransform partTwoRect = imageRect.transform.parent.GetComponent<RectTransform>();
        LayoutElement partTwoLayout = partTwoRect.GetComponent<LayoutElement>();

        if (partTwoLayout == null)
        {
            partTwoLayout = partTwoRect.gameObject.AddComponent<LayoutElement>();
        }

        if (imageRect != null && partTwoRect != null)
        {
            // 設置固定的間距值
            float topPadding = 40f;      // Image 與 PartTwo 頂部的間距
            float bottomPadding = 40f;    // Image 與 PartTwo 底部的間距
            float textPadding = 60f;      // 文字與 Image 邊緣的間距
            float titleHeight = 60f;      // 標題預留的高度

            // 1. 計算 Image 需要的高度（加上標題空間）
            float imageHeight = titleHeight + textHeight + (textPadding * 2);

            // 2. 計算 PartTwo 需要的總高度
            float partTwoHeight = imageHeight + topPadding + bottomPadding;

            // 3. 設置 Image 的大小
            imageRect.sizeDelta = new Vector2(imageRect.sizeDelta.x, imageHeight);

            // 4. 使用 Layout Element 來控制 PartTwo 的高度
            partTwoLayout.preferredWidth = partTwoRect.sizeDelta.x;  // 保持原有寬度
            partTwoLayout.preferredHeight = partTwoHeight;

            // 5. 確保 Text 在 Image 內部正確位置（從標題下方開始）
            reviewText.rectTransform.anchoredPosition = new Vector2(
                reviewText.rectTransform.anchoredPosition.x,
                -(titleHeight + textPadding)  // 從標題下方開始
            );
        }
    }
}

[System.Serializable]
public class fetchRequest
{
    public string action;
    public int course_id;
    public string role;
}
[System.Serializable]
public class fetchResponse
{
    public string action;
    public string assistant_id;
    public string thread_id;
}

[System.Serializable]
public class fetchCloudLink
{
    public string action;
    public string cloud_link;
}