using UnityEngine;
using UnityEngine.UI;

public class SceneOrientationManager : MonoBehaviour
{
    // ��ҼҦ��T�O�u���@�Ӻ޲z�����
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

    // ������ݫ�UI�Ҧ�
    public void SwitchToPortrait()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        // �p�G�ϥΤFUI�۾��A�i�H�b�o�̿E��
        // uiCamera.gameObject.SetActive(true);
        // vrCamera.gameObject.SetActive(false);
    }

    // ������VR��̼Ҧ�
    public void SwitchToLandscapeVR()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        // �p�G�ϥΤFVR�۾��A�i�H�b�o�̿E��
        // uiCamera.gameObject.SetActive(false);
        // vrCamera.gameObject.SetActive(true);
    }
}