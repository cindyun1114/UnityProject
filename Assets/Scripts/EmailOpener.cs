using UnityEngine;

public class EmailOpener : MonoBehaviour
{
    public void OpenGmailCompose()
    {
        string email = "crayoneaterccu@gmail.com";
        string subject = EscapeURL("關於Feyndora的使用問題");

        string gmailUrl = $"https://mail.google.com/mail/?view=cm&fs=1&to={email}&su={subject}";
        Application.OpenURL(gmailUrl);
    }

    private string EscapeURL(string url)
    {
        return WWW.EscapeURL(url).Replace("+", "%20");
    }
}
