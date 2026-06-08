using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{
    public static ScreenFade Instance;

    [Header("Referenzen")]
    [SerializeField] private RawImage fadeImage;

    [Header("Zeiten")]
    [SerializeField] private float fadeToBlackDuration = 1f;
    [SerializeField] private float blackScreenDuration = 2f;
    [SerializeField] private float fadeToClearDuration = 1f;

    [Header("Events")]
    [Tooltip("Get's invoked when screen turned completly to black")]
    public UnityEvent OnBlackScreenEvent;
    public UnityEvent OnStartFadingToTransparentEvent;
    [Tooltip("Get's invoked when screen turned completly to transparent again")]
    public UnityEvent OnTransparentScreenEvent;
    private Coroutine currentFade;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape) || OVRInput.GetDown(OVRInput.Button.One))
        {
            StartFade();
        }
    }

    public void StartFade()
    {
        if (currentFade != null)
            StopCoroutine(currentFade);

        currentFade = StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        yield return FadeAlpha(0f, 1f, fadeToBlackDuration);
        OnBlackScreenEvent.Invoke();
        yield return new WaitForSeconds(blackScreenDuration);
        OnStartFadingToTransparentEvent.Invoke();
        yield return FadeAlpha(1f, 0f, fadeToClearDuration);
        OnTransparentScreenEvent.Invoke();
    }

    private IEnumerator FadeAlpha(float startAlpha, float endAlpha, float duration)
    {
        Color color = fadeImage.color;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float alpha = Mathf.Lerp(startAlpha, endAlpha, timer / duration);
            color.a = alpha;
            fadeImage.color = color;

            yield return null;
        }

        color.a = endAlpha;
        fadeImage.color = color;
    }
}