using UnityEngine;

public class YoutubeOpener : MonoBehaviour
{
    // �������A�Q���઺ YouTube �v���s��
    public string youtubeURL = "https://youtu.be/dQw4w9WgXcQ?si=958HQQaCaLjAmno8";

    public void OpenYoutube()
    {
        Application.OpenURL(youtubeURL);
    }
}
