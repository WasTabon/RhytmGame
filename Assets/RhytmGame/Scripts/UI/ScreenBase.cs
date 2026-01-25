using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public abstract class ScreenBase : MonoBehaviour
{
    [Header("Screen Settings")]
    [SerializeField] protected float showDuration = 0.3f;
    [SerializeField] protected float hideDuration = 0.25f;
    [SerializeField] protected bool hideOnStart = true;

    protected CanvasGroup canvasGroup;
    protected RectTransform rectTransform;

    public bool IsVisible { get; protected set; }

    protected virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();

        if (hideOnStart)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            IsVisible = false;
        }
        else
        {
            IsVisible = true;
        }
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
        IsVisible = true;
        OnShowStart();
        PlayShowAnimation();
    }

    public virtual void Hide()
    {
        OnHideStart();
        PlayHideAnimation(() =>
        {
            IsVisible = false;
            gameObject.SetActive(false);
            OnHideComplete();
        });
    }

    protected virtual void PlayShowAnimation()
    {
        canvasGroup.DOKill();
        canvasGroup.FadeIn(showDuration).OnComplete(OnShowComplete);
    }

    protected virtual void PlayHideAnimation(System.Action onComplete)
    {
        canvasGroup.DOKill();
        canvasGroup.FadeOut(hideDuration, onComplete);
    }

    protected virtual void OnShowStart() { }
    protected virtual void OnShowComplete() { }
    protected virtual void OnHideStart() { }
    protected virtual void OnHideComplete() { }

    public void SetVisibleImmediate(bool visible)
    {
        canvasGroup.alpha = visible ? 1f : 0f;
        canvasGroup.blocksRaycasts = visible;
        canvasGroup.interactable = visible;
        IsVisible = visible;
        gameObject.SetActive(visible);
    }
}
