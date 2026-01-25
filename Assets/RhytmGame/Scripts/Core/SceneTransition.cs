using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 0.4f;
    [SerializeField] private Ease fadeEase = Ease.InOutQuad;

    [Header("References")]
    [SerializeField] private CanvasGroup fadeCanvas;
    [SerializeField] private Image fadeImage;

    private bool isTransitioning = false;

    public event Action OnTransitionStart;
    public event Action OnTransitionMiddle;
    public event Action OnTransitionEnd;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (fadeCanvas != null)
        {
            fadeCanvas.alpha = 0f;
            fadeCanvas.blocksRaycasts = false;
        }
    }

    public void LoadScene(string sceneName, Action onMiddle = null)
    {
        if (isTransitioning) return;
        StartCoroutine(TransitionCoroutine(sceneName, onMiddle));
    }

    public void LoadScene(int sceneIndex, Action onMiddle = null)
    {
        if (isTransitioning) return;
        StartCoroutine(TransitionCoroutine(sceneIndex, onMiddle));
    }

    private System.Collections.IEnumerator TransitionCoroutine(string sceneName, Action onMiddle)
    {
        isTransitioning = true;
        OnTransitionStart?.Invoke();

        fadeCanvas.blocksRaycasts = true;
        yield return fadeCanvas.DOFade(1f, fadeDuration).SetEase(fadeEase).SetUpdate(true).WaitForCompletion();

        OnTransitionMiddle?.Invoke();
        onMiddle?.Invoke();

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        yield return new WaitForSecondsRealtime(0.1f);

        yield return fadeCanvas.DOFade(0f, fadeDuration).SetEase(fadeEase).SetUpdate(true).WaitForCompletion();
        fadeCanvas.blocksRaycasts = false;

        isTransitioning = false;
        OnTransitionEnd?.Invoke();
    }

    private System.Collections.IEnumerator TransitionCoroutine(int sceneIndex, Action onMiddle)
    {
        isTransitioning = true;
        OnTransitionStart?.Invoke();

        fadeCanvas.blocksRaycasts = true;
        yield return fadeCanvas.DOFade(1f, fadeDuration).SetEase(fadeEase).SetUpdate(true).WaitForCompletion();

        OnTransitionMiddle?.Invoke();
        onMiddle?.Invoke();

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        yield return new WaitForSecondsRealtime(0.1f);

        yield return fadeCanvas.DOFade(0f, fadeDuration).SetEase(fadeEase).SetUpdate(true).WaitForCompletion();
        fadeCanvas.blocksRaycasts = false;

        isTransitioning = false;
        OnTransitionEnd?.Invoke();
    }

    public void FadeIn(Action onComplete = null)
    {
        fadeCanvas.blocksRaycasts = true;
        fadeCanvas.DOFade(1f, fadeDuration).SetEase(fadeEase).SetUpdate(true).OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }

    public void FadeOut(Action onComplete = null)
    {
        fadeCanvas.DOFade(0f, fadeDuration).SetEase(fadeEase).SetUpdate(true).OnComplete(() =>
        {
            fadeCanvas.blocksRaycasts = false;
            onComplete?.Invoke();
        });
    }
}
