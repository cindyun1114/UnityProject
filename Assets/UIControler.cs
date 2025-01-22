using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour  // 確保類名與文件名完全一致
{
    // 由於這是按鈕事件，我們加上 [SerializeField] 來確保Unity能看到這個方法
    [SerializeField]
    public void OnEnterVRButtonClick()
    {
        SceneOrientationManager.Instance.SwitchToLandscapeVR();
        SceneManager.LoadScene("TeacherScene");
    }

    [SerializeField]
    public void OnExitVRButtonClick()
    {
        SceneOrientationManager.Instance.SwitchToPortrait();
        SceneManager.LoadScene("PreviewPageScene");
    }
}
