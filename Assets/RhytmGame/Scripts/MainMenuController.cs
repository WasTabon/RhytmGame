using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private RectTransform mainMenuPanel;
    [SerializeField] private RectTransform modeSelectPanel;
    [SerializeField] private RectTransform settingsPanel;
    [SerializeField] private LevelSelectUI levelSelectUI;
    [SerializeField] private AchievementsUI achievementsUI;

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
    [SerializeField] private Button achievementsButton;

    [Header("Settings Sliders")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Animation Settings")]
    [SerializeField] private float panelSlideDuration = 0.4f;
    [SerializeField] private float buttonScaleDuration = 0.1f;

    private Vector2 hiddenLeft;
    private Vector2 hiddenRight;
    private Vector2 centerPosition;

    private void Start()
    {
        EnsureGameManager();
        EnsureSoundManager();
        SetupPositions();
        SetupButtons();
        SetupSliders();
        SetupLevelSelect();
        SetupAchievements();
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

    private void EnsureSoundManager()
    {
        if (SoundManager.Instance == null)
        {
            GameObject go = new GameObject("SoundManager");
            go.AddComponent<SoundManager>();
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

        levelsButton.onClick.AddListener(OnLevelsClicked);
        infiniteButton.onClick.AddListener(() => OnModeSelected(GameMode.Infinite));
        timeAttackButton.onClick.AddListener(() => OnModeSelected(GameMode.TimeAttack));
        modeBackButton.onClick.AddListener(OnModeBackClicked);

        settingsBackButton.onClick.AddListener(OnSettingsBackClicked);

        if (achievementsButton != null)
        {
            achievementsButton.onClick.AddListener(OnAchievementsClicked);
            SetupButtonAnimation(achievementsButton);
        }

        SetupButtonAnimation(playButton);
        SetupButtonAnimation(settingsButton);
        SetupButtonAnimation(quitButton);
        SetupButtonAnimation(levelsButton);
        SetupButtonAnimation(infiniteButton);
        SetupButtonAnimation(timeAttackButton);
        SetupButtonAnimation(modeBackButton);
        SetupButtonAnimation(settingsBackButton);
    }

    private void SetupSliders()
    {
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        if (musicSlider != null)
        {
            musicSlider.value = musicVolume;
            musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = sfxVolume;
            sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }
    }

    private void OnMusicVolumeChanged(float value)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetMusicVolume(value);
        }
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
    }

    private void OnSFXVolumeChanged(float value)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetSFXVolume(value);
        }
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
    }

    private void SetupLevelSelect()
    {
        if (levelSelectUI != null)
        {
            levelSelectUI.OnBackClicked += OnLevelSelectBackClicked;
        }
    }

    private void SetupAchievements()
    {
        if (achievementsUI != null)
        {
            achievementsUI.OnBackClicked += OnAchievementsBackClicked;
        }
    }

    private void SetupButtonAnimation(Button button)
    {
        if (button == null)
            return;

        if (button.GetComponent<ButtonAnimationTrigger>() == null)
        {
            var trigger = button.gameObject.AddComponent<ButtonAnimationTrigger>();
            trigger.Initialize(buttonScaleDuration);
        }
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
        PlayButtonSound();
        mainMenuPanel.DOAnchorPos(hiddenLeft, panelSlideDuration).SetEase(Ease.InCubic);
        modeSelectPanel.DOAnchorPos(centerPosition, panelSlideDuration).SetEase(Ease.OutCubic);
    }

    private void OnSettingsClicked()
    {
        PlayButtonSound();
        mainMenuPanel.DOAnchorPos(hiddenLeft, panelSlideDuration).SetEase(Ease.InCubic);
        settingsPanel.DOAnchorPos(centerPosition, panelSlideDuration).SetEase(Ease.OutCubic);
    }

    private void OnQuitClicked()
    {
        PlayButtonSound();
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.QuitGame();
        }
    }

    private void OnLevelsClicked()
    {
        PlayButtonSound();
        modeSelectPanel.DOAnchorPos(hiddenLeft, panelSlideDuration).SetEase(Ease.InCubic);
        
        if (levelSelectUI != null)
        {
            levelSelectUI.Show();
        }
    }

    private void OnLevelSelectBackClicked()
    {
        modeSelectPanel.DOAnchorPos(centerPosition, panelSlideDuration).SetEase(Ease.OutCubic);
    }

    private void OnAchievementsClicked()
    {
        PlayButtonSound();
        settingsPanel.DOAnchorPos(hiddenLeft, panelSlideDuration).SetEase(Ease.InCubic);
        
        if (achievementsUI != null)
        {
            achievementsUI.Show();
        }
    }

    private void OnAchievementsBackClicked()
    {
        settingsPanel.DOAnchorPos(centerPosition, panelSlideDuration).SetEase(Ease.OutCubic);
    }

    private void OnModeSelected(GameMode mode)
    {
        PlayButtonSound();
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
        PlayButtonSound();
        modeSelectPanel.DOAnchorPos(hiddenRight, panelSlideDuration).SetEase(Ease.InCubic);
        mainMenuPanel.DOAnchorPos(centerPosition, panelSlideDuration).SetEase(Ease.OutCubic);
    }

    private void OnSettingsBackClicked()
    {
        PlayButtonSound();
        settingsPanel.DOAnchorPos(hiddenRight, panelSlideDuration).SetEase(Ease.InCubic);
        mainMenuPanel.DOAnchorPos(centerPosition, panelSlideDuration).SetEase(Ease.OutCubic);
    }

    private void PlayButtonSound()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayButtonClick();
        }
    }
}
