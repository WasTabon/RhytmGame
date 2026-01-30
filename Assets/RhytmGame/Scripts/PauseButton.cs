using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PauseButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button button;
    [SerializeField] private Image iconImage;

    [Header("Animation")]
    [SerializeField] private float punchScale = 1.15f;
    [SerializeField] private float punchDuration = 0.15f;

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();
    }

    private void Start()
    {
        if (button != null)
        {
            button.onClick.AddListener(OnPauseClicked);
        }
    }

    private void OnPauseClicked()
    {
        if (iconImage != null)
        {
            iconImage.transform.DOKill();
            iconImage.transform.localScale = Vector3.one;
            iconImage.transform.DOPunchScale(Vector3.one * (punchScale - 1f), punchDuration, 5, 0.5f);
        }

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayButtonClick();
        }

        if (PauseMenu.Instance != null)
        {
            PauseMenu.Instance.Pause();
        }
    }
}
