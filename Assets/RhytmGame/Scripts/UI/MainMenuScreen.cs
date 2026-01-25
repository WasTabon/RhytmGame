using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class MainMenuScreen : ScreenBase
{
    [Header("Title")]
    [SerializeField] private RectTransform titleContainer;
    [SerializeField] private TextMeshProUGUI titleText;

    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button infiniteButton;
    [SerializeField] private Button timeAttackButton;
    [SerializeField] private Button achievementsButton;
    [SerializeField] private Button settingsButton;

    [Header("Button Containers")]
    [SerializeField] private RectTransform playButtonRect;
    [SerializeField] private RectTransform infiniteButtonRect;
    [SerializeField] private RectTransform timeAttackButtonRect;
    [SerializeField] private RectTransform achievementsButtonRect;
    [SerializeField] private RectTransform settingsButtonRect;

    [Header("Animation Settings")]
    [SerializeField] private float buttonStaggerDelay = 0.08f;
    [SerializeField] private float titleAnimDuration = 0.5f;

    private MainMenuUI mainMenuUI;

    protected override void Awake()
    {
        base.Awake();
        hideOnStart = false;
        
        SetupButtons();
    }

    public void Initialize(MainMenuUI menuUI)
    {
        mainMenuUI = menuUI;
    }

    private void SetupButtons()
    {
        playButton?.onClick.AddListener(OnPlayClicked);
        infiniteButton?.onClick.AddListener(OnInfiniteClicked);
        timeAttackButton?.onClick.AddListener(OnTimeAttackClicked);
        achievementsButton?.onClick.AddListener(OnAchievementsClicked);
        settingsButton?.onClick.AddListener(OnSettingsClicked);

        playButton?.SetupButtonAnimation();
        infiniteButton?.SetupButtonAnimation();
        timeAttackButton?.SetupButtonAnimation();
        achievementsButton?.SetupButtonAnimation();
        settingsButton?.SetupButtonAnimation();
    }

    protected override void PlayShowAnimation()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        if (titleContainer != null)
        {
            titleContainer.localScale = Vector3.zero;
            titleContainer.DOScale(1f, titleAnimDuration).SetEase(Ease.OutBack);
        }

        RectTransform[] buttons = { playButtonRect, infiniteButtonRect, timeAttackButtonRect, achievementsButtonRect, settingsButtonRect };

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] != null)
            {
                buttons[i].localScale = Vector3.zero;
                buttons[i].DOScale(1f, showDuration)
                    .SetEase(Ease.OutBack)
                    .SetDelay(0.15f + i * buttonStaggerDelay);
            }
        }

        DOVirtual.DelayedCall(0.15f + buttons.Length * buttonStaggerDelay + showDuration, OnShowComplete);
    }

    protected override void PlayHideAnimation(System.Action onComplete)
    {
        RectTransform[] buttons = { settingsButtonRect, achievementsButtonRect, timeAttackButtonRect, infiniteButtonRect, playButtonRect };

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] != null)
            {
                buttons[i].DOScale(0f, hideDuration * 0.7f)
                    .SetEase(Ease.InBack)
                    .SetDelay(i * buttonStaggerDelay * 0.5f);
            }
        }

        if (titleContainer != null)
        {
            titleContainer.DOScale(0f, hideDuration)
                .SetEase(Ease.InBack)
                .SetDelay(buttons.Length * buttonStaggerDelay * 0.5f);
        }

        float totalDuration = buttons.Length * buttonStaggerDelay * 0.5f + hideDuration;
        DOVirtual.DelayedCall(totalDuration, () =>
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            onComplete?.Invoke();
        });
    }

    private void OnPlayClicked()
    {
        VibrationManager.Instance?.Selection();
        mainMenuUI?.ShowLevelSelect();
    }

    private void OnInfiniteClicked()
    {
        VibrationManager.Instance?.Selection();
        GameManager.Instance?.StartGame(GameManager.GameMode.InfiniteMode);
    }

    private void OnTimeAttackClicked()
    {
        VibrationManager.Instance?.Selection();
        GameManager.Instance?.StartGame(GameManager.GameMode.TimeAttack);
    }

    private void OnAchievementsClicked()
    {
        VibrationManager.Instance?.Selection();
        mainMenuUI?.ShowAchievements();
    }

    private void OnSettingsClicked()
    {
        VibrationManager.Instance?.Selection();
        mainMenuUI?.ShowSettings();
    }
}
