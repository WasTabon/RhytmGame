using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonAnimationTrigger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    private float duration = 0.1f;
    private Vector3 normalScale = Vector3.one;
    private Vector3 pressedScale = new Vector3(0.95f, 0.95f, 1f);
    private Vector3 hoverScale = new Vector3(1.05f, 1.05f, 1f);

    public void Initialize(float animDuration)
    {
        duration = animDuration;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.DOScale(pressedScale, duration).SetEase(Ease.OutCubic);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.DOScale(normalScale, duration).SetEase(Ease.OutCubic);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(hoverScale, duration).SetEase(Ease.OutCubic);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(normalScale, duration).SetEase(Ease.OutCubic);
    }
}
