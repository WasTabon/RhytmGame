using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private RectTransform mainMenuPanel;
    [SerializeField] private RectTransform modeSelectPanel;
    [SerializeField] private RectTransform settingsPanel;

    [Header("Main Menu Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    [Header("Mode Select Buttons")]
    [SerializeField] private Button levelsButton;
    [SerializeField] private Button infiniteButton;
    [SerializeField] private Button timeAttackButton;
    [SerializeField] private Button modeBackButton;

    [Header("Settings Buttons")]
    [SerializeField] private Button settingsBackButton;

    [Header("Animation Settings")]
    [SerializeField] private float panelSlideDuration = 0.4f;
    [SerializeField] private float buttonScaleDuration = 0.1f;

    private Vector2 hiddenLeft;
    private Vector2 hiddenRight;
    private Vector2 centerPosition;

    private void Start()
    {
        EnsureGameManager();
        SetupPositions();
        SetupButtons();
        ShowMainMenu(false);
    }

    private void EnsureGameManager()
    {
        if (GameManager.Instance == null)
        {
            GameObject go = new GameObject("GameManager");
            go.AddComponent<GameManager>();
        }
    }

    private void SetupPositions()
    {
        centerPosition = Vector2.zero;
        hiddenLeft = new Vector2(-Screen.width, 0);
        hiddenRight = new Vector2(Screen.width, 0);

        modeSelectPanel.anchoredPosition = hiddenRight;
        settingsPanel.anchoredPosition = hiddenRight;
    }

    private void SetupButtons()
    {
        playButton.onClick.AddListener(OnPlayClicked);
        settingsButton.onClick.AddListener(OnSettingsClicked);
        quitButton.onClick.AddListener(OnQuitClicked);

        levelsButton.onClick.AddListener(() => OnModeSelected(GameMode.Levels));
        infiniteButton.onClick.AddListener(() => OnModeSelected(GameMode.Infinite));
        timeAttackButton.onClick.AddListener(() => OnModeSelected(GameMode.TimeAttack));
        modeBackButton.onClick.AddListener(OnModeBackClicked);

        settingsBackButton.onClick.AddListener(OnSettingsBackClicked);

        SetupButtonAnimation(playButton);
        SetupButtonAnimation(settingsButton);
        SetupButtonAnimation(quitButton);
        SetupButtonAnimation(levelsButton);
        SetupButtonAnimation(infiniteButton);
        SetupButtonAnimation(timeAttackButton);
        SetupButtonAnimation(modeBackButton);
        SetupButtonAnimation(settingsBackButton);
    }

    private void SetupButtonAnimation(Button button)
    {
        var trigger = button.gameObject.AddComponent<ButtonAnimationTrigger>();
        trigger.Initialize(buttonScaleDuration);
    }

    private void ShowMainMenu(bool animated)
    {
        if (animated)
        {
            mainMenuPanel.DOAnchorPos(centerPosition, panelSlideDuration).SetEase(Ease.OutCubic);
        }
        else
        {
            mainMenuPanel.anchoredPosition = centerPosition;
        }
    }

    private void OnPlayClicked()
    {
        mainMenuPanel.DOAnchorPos(hiddenLeft, panelSlideDuration).SetEase(Ease.InCubic);
        modeSelectPanel.DOAnchorPos(centerPosition, panelSlideDuration).SetEase(Ease.OutCubic);
    }

    private void OnSettingsClicked()
    {
        mainMenuPanel.DOAnchorPos(hiddenLeft, panelSlideDuration).SetEase(Ease.InCubic);
        settingsPanel.DOAnchorPos(centerPosition, panelSlideDuration).SetEase(Ease.OutCubic);
    }

    private void OnQuitClicked()
    {
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.QuitGame();
        }
    }

    private void OnModeSelected(GameMode mode)
    {
        EnsureGameManager();

        GameManager.Instance.SetGameMode(mode);

        if (mode == GameMode.Levels)
        {
            GameManager.Instance.SetSelectedLevel(0);
        }

        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadGame();
        }
    }

    private void OnModeBackClicked()
    {
        modeSelectPanel.DOAnchorPos(hiddenRight, panelSlideDuration).SetEase(Ease.InCubic);
        mainMenuPanel.DOAnchorPos(centerPosition, panelSlideDuration).SetEase(Ease.OutCubic);
    }

    private void OnSettingsBackClicked()
    {
        settingsPanel.DOAnchorPos(hiddenRight, panelSlideDuration).SetEase(Ease.InCubic);
        mainMenuPanel.DOAnchorPos(centerPosition, panelSlideDuration).SetEase(Ease.OutCubic);
    }
}
