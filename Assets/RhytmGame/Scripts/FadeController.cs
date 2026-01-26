using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class FadeController : MonoBehaviour
{
    public static FadeController Instance { get; private set; }

    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 0.5f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (fadeImage != null)
        {
            fadeImage.raycastTarget = true;
            FadeIn(null);
        }
    }

    public void FadeIn(Action onComplete)
    {
        if (fadeImage == null) return;
        
        fadeImage.raycastTarget = true;
        fadeImage.color = new Color(0, 0, 0, 1);
        fadeImage.DOFade(0, fadeDuration).OnComplete(() =>
        {
            fadeImage.raycastTarget = false;
            onComplete?.Invoke();
        });
    }

    public void FadeOut(Action onComplete)
    {
        if (fadeImage == null) return;
        
        fadeImage.raycastTarget = true;
        fadeImage.color = new Color(0, 0, 0, 0);
        fadeImage.DOFade(1, fadeDuration).OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }

    public void SetFadeImage(Image image)
    {
        fadeImage = image;
    }
}
