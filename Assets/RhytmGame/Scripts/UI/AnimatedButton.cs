using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

[RequireComponent(typeof(Button))]
public class AnimatedButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Animation Settings")]
    [SerializeField] private float pressScale = 0.95f;
    [SerializeField] private float pressDuration = 0.1f;
    [SerializeField] private float releaseDuration = 0.15f;
    [SerializeField] private bool useVibration = true;

    private Button button;
    private Vector3 originalScale;
    private Tweener currentTween;

    private void Awake()
    {
        button = GetComponent<Button>();
        originalScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!button.interactable) return;

        currentTween?.Kill();
        currentTween = transform.DOScale(originalScale * pressScale, pressDuration)
            .SetEase(Ease.OutQuad)
            .SetUpdate(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!button.interactable) return;

        currentTween?.Kill();
        currentTween = transform.DOScale(originalScale, releaseDuration)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);

        if (useVibration)
        {
            VibrationManager.Instance?.Selection();
        }
    }

    private void OnDisable()
    {
        currentTween?.Kill();
        transform.localScale = originalScale;
    }
}
