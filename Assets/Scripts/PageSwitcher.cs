using System.Collections;
using UnityEngine;

public class PageSwitcher : MonoBehaviour
{
    public CanvasGroup introPage;     // �ĤT���G���Э�
    public CanvasGroup loginPage;     // �ĥ|���G�n�J��
    public CanvasGroup registerPage;  // �Ĥ����G���U��
    public float fadeDuration = 1f;

    // ���u�n�J�v���s�ɩI�s
    public void GoToLogin()
    {
        StartCoroutine(SwitchWithFade(introPage, loginPage));
    }

    // ���u���U�v���s�ɩI�s
    public void GoToRegister()
    {
        StartCoroutine(SwitchWithFade(introPage, registerPage));
    }

    IEnumerator SwitchWithFade(CanvasGroup fromPage, CanvasGroup toPage)
    {
        yield return StartCoroutine(FadeOut(fromPage));
        yield return StartCoroutine(FadeIn(toPage));
    }

    IEnumerator FadeOut(CanvasGroup cg)
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            cg.alpha = Mathf.Lerp(1, 0, timer / fadeDuration);
            yield return null;
        }
        cg.alpha = 0;
        cg.gameObject.SetActive(false); // �O�o����
    }

    IEnumerator FadeIn(CanvasGroup cg)
    {
        cg.gameObject.SetActive(true);  // �}�ҭ���
        cg.alpha = 0;
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            cg.alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
            yield return null;
        }
        cg.alpha = 1;
    }
}
