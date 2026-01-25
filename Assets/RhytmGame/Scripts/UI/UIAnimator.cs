using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public static class UIAnimator
{
    public static float DefaultDuration = 0.3f;
    public static Ease DefaultEase = Ease.OutBack;
    public static Ease DefaultFadeEase = Ease.OutQuad;

    public static Tweener PopIn(this Transform transform, float duration = -1f, float delay = 0f)
    {
        if (duration < 0) duration = DefaultDuration;
        transform.localScale = Vector3.zero;
        return transform.DOScale(1f, duration)
            .SetEase(DefaultEase)
            .SetDelay(delay)
            .SetUpdate(true);
    }

    public static Tweener PopOut(this Transform transform, float duration = -1f, System.Action onComplete = null)
    {
        if (duration < 0) duration = DefaultDuration * 0.7f;
        return transform.DOScale(0f, duration)
            .SetEase(Ease.InBack)
            .SetUpdate(true)
            .OnComplete(() => onComplete?.Invoke());
    }

    public static Tweener FadeIn(this CanvasGroup canvasGroup, float duration = -1f, float delay = 0f)
    {
        if (duration < 0) duration = DefaultDuration;
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        return canvasGroup.DOFade(1f, duration)
            .SetEase(DefaultFadeEase)
            .SetDelay(delay)
            .SetUpdate(true);
    }

    public static Tweener FadeOut(this CanvasGroup canvasGroup, float duration = -1f, System.Action onComplete = null)
    {
        if (duration < 0) duration = DefaultDuration;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        return canvasGroup.DOFade(0f, duration)
            .SetEase(DefaultFadeEase)
            .SetUpdate(true)
            .OnComplete(() => onComplete?.Invoke());
    }

    public static Tweener SlideInFromBottom(this RectTransform rectTransform, float duration = -1f, float delay = 0f)
    {
        if (duration < 0) duration = DefaultDuration;
        Vector2 startPos = rectTransform.anchoredPosition;
        rectTransform.anchoredPosition = new Vector2(startPos.x, startPos.y - Screen.height);
        return rectTransform.DOAnchorPos(startPos, duration)
            .SetEase(Ease.OutCubic)
            .SetDelay(delay)
            .SetUpdate(true);
    }

    public static Tweener SlideOutToBottom(this RectTransform rectTransform, float duration = -1f, System.Action onComplete = null)
    {
        if (duration < 0) duration = DefaultDuration;
        Vector2 targetPos = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y - Screen.height);
        return rectTransform.DOAnchorPos(targetPos, duration)
            .SetEase(Ease.InCubic)
            .SetUpdate(true)
            .OnComplete(() => onComplete?.Invoke());
    }

    public static Tweener SlideInFromTop(this RectTransform rectTransform, float duration = -1f, float delay = 0f)
    {
        if (duration < 0) duration = DefaultDuration;
        Vector2 startPos = rectTransform.anchoredPosition;
        rectTransform.anchoredPosition = new Vector2(startPos.x, startPos.y + Screen.height);
        return rectTransform.DOAnchorPos(startPos, duration)
            .SetEase(Ease.OutCubic)
            .SetDelay(delay)
            .SetUpdate(true);
    }

    public static Tweener SlideInFromLeft(this RectTransform rectTransform, float duration = -1f, float delay = 0f)
    {
        if (duration < 0) duration = DefaultDuration;
        Vector2 startPos = rectTransform.anchoredPosition;
        rectTransform.anchoredPosition = new Vector2(startPos.x - Screen.width, startPos.y);
        return rectTransform.DOAnchorPos(startPos, duration)
            .SetEase(Ease.OutCubic)
            .SetDelay(delay)
            .SetUpdate(true);
    }

    public static Tweener SlideInFromRight(this RectTransform rectTransform, float duration = -1f, float delay = 0f)
    {
        if (duration < 0) duration = DefaultDuration;
        Vector2 startPos = rectTransform.anchoredPosition;
        rectTransform.anchoredPosition = new Vector2(startPos.x + Screen.width, startPos.y);
        return rectTransform.DOAnchorPos(startPos, duration)
            .SetEase(Ease.OutCubic)
            .SetDelay(delay)
            .SetUpdate(true);
    }

    public static Sequence PunchScale(this Transform transform, float punch = 0.2f, float duration = 0.3f)
    {
        return DOTween.Sequence()
            .Append(transform.DOScale(1f + punch, duration * 0.5f).SetEase(Ease.OutQuad))
            .Append(transform.DOScale(1f, duration * 0.5f).SetEase(Ease.OutElastic))
            .SetUpdate(true);
    }

    public static Sequence PunchRotation(this Transform transform, float punch = 10f, float duration = 0.3f)
    {
        return DOTween.Sequence()
            .Append(transform.DORotate(new Vector3(0, 0, punch), duration * 0.25f).SetEase(Ease.OutQuad))
            .Append(transform.DORotate(new Vector3(0, 0, -punch * 0.5f), duration * 0.25f).SetEase(Ease.OutQuad))
            .Append(transform.DORotate(Vector3.zero, duration * 0.5f).SetEase(Ease.OutElastic))
            .SetUpdate(true);
    }

    public static Tweener CountTo(this TextMeshProUGUI text, int from, int to, float duration, string format = "{0}")
    {
        int current = from;
        return DOTween.To(() => current, x =>
        {
            current = x;
            text.text = string.Format(format, current);
        }, to, duration).SetEase(Ease.OutQuad).SetUpdate(true);
    }

    public static Sequence Shake(this Transform transform, float duration = 0.5f, float strength = 10f)
    {
        return DOTween.Sequence()
            .Append(transform.DOShakePosition(duration, strength, 20, 90, false, true))
            .SetUpdate(true);
    }

    public static void ButtonPress(this Transform transform)
    {
        transform.DOKill();
        transform.localScale = Vector3.one;
        transform.DOPunchScale(Vector3.one * -0.1f, 0.15f, 10, 1).SetUpdate(true);
    }

    public static void SetupButtonAnimation(this Button button)
    {
        button.onClick.AddListener(() => button.transform.ButtonPress());
    }
}
