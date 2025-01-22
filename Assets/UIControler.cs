using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour  // �T�O���W�P���W�����@�P
{
    // �ѩ�o�O���s�ƥ�A�ڭ̥[�W [SerializeField] �ӽT�OUnity��ݨ�o�Ӥ�k
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
