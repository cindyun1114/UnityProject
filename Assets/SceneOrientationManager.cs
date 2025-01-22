using UnityEngine;
using UnityEngine.UI;

public class SceneOrientationManager : MonoBehaviour
{
    // 單例模式確保只有一個管理器實例
    public static SceneOrientationManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 切換到豎屏UI模式
    public void SwitchToPortrait()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        // 如果使用了UI相機，可以在這裡激活
        // uiCamera.gameObject.SetActive(true);
        // vrCamera.gameObject.SetActive(false);
    }

    // 切換到VR橫屏模式
    public void SwitchToLandscapeVR()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        // 如果使用了VR相機，可以在這裡激活
        // uiCamera.gameObject.SetActive(false);
        // vrCamera.gameObject.SetActive(true);
    }
}