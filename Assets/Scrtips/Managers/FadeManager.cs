using System.Collections;
using UnityEngine;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private CanvasGroup fadeScreen;

    [Header("Settings")]
    [SerializeField] private float defaultFadeDuration = 0.25f;

    private Coroutine currentFadeCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        fadeScreen.alpha = 0f;
        fadeScreen.blocksRaycasts = false;
        fadeScreen.interactable = false;
        fadeScreen.gameObject.SetActive(false);
    }

    public void FadeOut(System.Action onComplete = null)
    {
        StartFade(1f, defaultFadeDuration, onComplete);
    }

    public void FadeIn(System.Action onComplete = null)
    {
        StartFade(0f, defaultFadeDuration, onComplete);
    }

    public void FadeOut(float duration, System.Action onComplete = null)
    {
        StartFade(1f, duration, onComplete);
    }

    public void FadeIn(float duration, System.Action onComplete = null)
    {
        StartFade(0f, duration, onComplete);
    }

    private void StartFade(float targetAlpha, float duration, System.Action onComplete)
    {
        if (currentFadeCoroutine != null)
            StopCoroutine(currentFadeCoroutine);

        currentFadeCoroutine = StartCoroutine(FadeRoutine(targetAlpha, duration, onComplete));
    }

    private IEnumerator FadeRoutine(float targetAlpha, float duration, System.Action onComplete)
    {
        fadeScreen.gameObject.SetActive(true);
        fadeScreen.blocksRaycasts = true;

        float startAlpha = fadeScreen.alpha;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            fadeScreen.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / duration);

            yield return null;
        }

        fadeScreen.alpha = targetAlpha;

        if (targetAlpha <= 0f)
        {
            fadeScreen.blocksRaycasts = false;
            fadeScreen.interactable = false;
            fadeScreen.gameObject.SetActive(false);
        }

        currentFadeCoroutine = null;
        onComplete?.Invoke();
    }
}
