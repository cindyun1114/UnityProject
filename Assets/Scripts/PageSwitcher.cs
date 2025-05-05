using System.Collections;
using UnityEngine;

public class PageSwitcher : MonoBehaviour
{
    public CanvasGroup introPage;     // 第三頁：介紹頁
    public CanvasGroup loginPage;     // 第四頁：登入頁
    public CanvasGroup registerPage;  // 第五頁：註冊頁
    public float fadeDuration = 1f;

    // 按「登入」按鈕時呼叫
    public void GoToLogin()
    {
        StartCoroutine(SwitchWithFade(introPage, loginPage));
    }

    // 按「註冊」按鈕時呼叫
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
        cg.gameObject.SetActive(false); // 記得關掉
    }

    IEnumerator FadeIn(CanvasGroup cg)
    {
        cg.gameObject.SetActive(true);  // 開啟頁面
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
