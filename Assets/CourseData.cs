using UnityEngine;

public class CourseData : MonoBehaviour
{
    public int courseId;
    public string courseName;
    public string category;
    public GameObject coursePage;
    public Sprite courseImage;

    private PresetCoursesManager presetCoursesManager;
    private bool isOpeningCourse = false;

    void Start()
    {
        presetCoursesManager = PresetCoursesManager.Instance;
    }

    public void OpenCourseDetail()
    {
        if (isOpeningCourse)
        {
            Debug.Log($"⚠️ 正在開啟課程 {courseName}，跳過重複請求");
            return;
        }

        isOpeningCourse = true;

        try
        {
            Debug.Log($"🎯 嘗試進入課程 {courseName}");

            if (coursePage != null)
            {
                Debug.Log($"✅ 進入課程 {courseName}，顯示 Panel：{coursePage.name}");
                presetCoursesManager.ShowCoursePage(coursePage);
            }
            else
            {
                Debug.LogError($"❌ 沒有設定對應的 Panel，請確保手動拖入！");
            }
        }
        finally
        {
            Invoke("ResetOpeningState", 0.5f);
        }
    }

    private void ResetOpeningState()
    {
        isOpeningCourse = false;
    }
}