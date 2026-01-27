using UnityEngine;
using TMPro;
using DG.Tweening;

public class FeedbackManager : MonoBehaviour
{
    public static FeedbackManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private LockMechanic lockMechanic;

    [Header("Colors")]
    [SerializeField] private Color perfectColor = new Color(0.3f, 1f, 0.3f);
    [SerializeField] private Color goodColor = new Color(1f, 1f, 0.3f);
    [SerializeField] private Color missColor = new Color(1f, 0.3f, 0.3f);

    [Header("Animation")]
    [SerializeField] private float showDuration = 0.8f;
    [SerializeField] private float punchScale = 1.3f;

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
        if (lockMechanic == null)
            lockMechanic = LockMechanic.Instance;

        if (lockMechanic != null)
        {
            lockMechanic.OnLock += ShowFeedback;
        }

        if (feedbackText != null)
        {
            feedbackText.alpha = 0f;
        }
    }

    private void OnDestroy()
    {
        if (lockMechanic != null)
        {
            lockMechanic.OnLock -= ShowFeedback;
        }
    }

    private void ShowFeedback(LockResult result)
    {
        if (feedbackText == null)
            return;

        feedbackText.DOKill();
        feedbackText.transform.DOKill();

        switch (result)
        {
            case LockResult.Perfect:
                feedbackText.text = "PERFECT!";
                feedbackText.color = perfectColor;
                break;
            case LockResult.Good:
                feedbackText.text = "GOOD";
                feedbackText.color = goodColor;
                break;
            case LockResult.Miss:
                feedbackText.text = "MISS";
                feedbackText.color = missColor;
                break;
        }

        feedbackText.transform.localScale = Vector3.one;
        feedbackText.alpha = 1f;

        feedbackText.transform.DOPunchScale(Vector3.one * (punchScale - 1f), 0.3f, 5, 0.5f);

        feedbackText.DOFade(0f, showDuration).SetDelay(0.2f);
    }

    public void SetFeedbackText(TextMeshProUGUI text)
    {
        feedbackText = text;
    }
}
