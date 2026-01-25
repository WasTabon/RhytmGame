using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class LevelSelectScreen : ScreenBase
{
    [Header("Header")]
    [SerializeField] private RectTransform headerRect;
    [SerializeField] private Button backButton;

    [Header("Content")]
    [SerializeField] private RectTransform contentRect;
    [SerializeField] private TextMeshProUGUI comingSoonText;

    private MainMenuUI mainMenuUI;
    private Vector2 contentStartPos;

    protected override void Awake()
    {
        base.Awake();

        if (contentRect != null)
            contentStartPos = contentRect.anchoredPosition;

        SetupUI();
    }

    public void Initialize(MainMenuUI menuUI)
    {
        mainMenuUI = menuUI;
    }

    private void SetupUI()
    {
        backButton?.onClick.AddListener(OnBackClicked);
        backButton?.SetupButtonAnimation();
    }

    protected override void PlayShowAnimation()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        canvasGroup.DOFade(1f, showDuration * 0.5f);

        if (headerRect != null)
        {
            headerRect.anchoredPosition = new Vector2(0, 100);
            headerRect.DOAnchorPosY(0, showDuration).SetEase(Ease.OutCubic);
        }

        if (contentRect != null)
        {
            contentRect.anchoredPosition = new Vector2(contentStartPos.x, contentStartPos.y - 200);
            contentRect.DOAnchorPos(contentStartPos, showDuration).SetEase(Ease.OutCubic).SetDelay(0.05f)
                .OnComplete(OnShowComplete);
        }
        else
        {
            DOVirtual.DelayedCall(showDuration, OnShowComplete);
        }
    }

    protected override void PlayHideAnimation(System.Action onComplete)
    {
        if (headerRect != null)
        {
            headerRect.DOAnchorPosY(100, hideDuration).SetEase(Ease.InCubic);
        }

        if (contentRect != null)
        {
            contentRect.DOAnchorPosY(contentStartPos.y - 200, hideDuration).SetEase(Ease.InCubic);
        }

        canvasGroup.DOFade(0f, hideDuration).OnComplete(() =>
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            onComplete?.Invoke();
        });
    }

    private void OnBackClicked()
    {
        VibrationManager.Instance?.Selection();
        mainMenuUI?.ShowMainMenu();
    }
}
