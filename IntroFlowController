using System.Collections;
using UnityEngine;

public class IntroFlowController : MonoBehaviour
{
    public CanvasGroup openingPage;
    public CanvasGroup openingIntroPage;
    public CanvasGroup introPage;

    public float fadeDuration = 1f;
    public float pageDisplayTime = 3f;

    void Start()
    {
        StartCoroutine(PlayIntroSequence());
    }

    IEnumerator PlayIntroSequence()
    {
        yield return StartCoroutine(FadeInOut(openingPage));
        yield return StartCoroutine(FadeInOut(openingIntroPage));
        yield return StartCoroutine(FadeIn(introPage));
    }

    IEnumerator FadeInOut(CanvasGroup cg)
    {
        yield return StartCoroutine(FadeIn(cg));
        yield return new WaitForSeconds(pageDisplayTime);
        yield return StartCoroutine(FadeOut(cg));
    }

    IEnumerator FadeIn(CanvasGroup cg)
    {
        cg.gameObject.SetActive(true);
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
        cg.gameObject.SetActive(false);
    }
}
