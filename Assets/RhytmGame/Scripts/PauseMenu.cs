using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance { get; private set; }

    [Header("Panel")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform panelRect;

    [Header("Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;

    [Header("Sliders")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI musicValueText;
    [SerializeField] private TextMeshProUGUI sfxValueText;

    [Header("Animation")]
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private float scaleDuration = 0.3f;
    [SerializeField] private Ease showEase = Ease.OutBack;
    [SerializeField] private Ease hideEase = Ease.InBack;

    [Header("References")]
    [SerializeField] private RotationController rotationController;
    [SerializeField] private LockMechanic lockMechanic;

    private bool isPaused;
    private bool isAnimating;

    public bool IsPaused => isPaused;

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
        SetupButtons();
        SetupSliders();
        HideInstant();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isAnimating)
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    private void SetupButtons()
    {
        if (resumeButton != null)
            resumeButton.onClick.AddListener(Resume);

        if (restartButton != null)
            restartButton.onClick.AddListener(Restart);

        if (menuButton != null)
            menuButton.onClick.AddListener(GoToMenu);
    }

    private void SetupSliders()
    {
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        if (musicSlider != null)
        {
            musicSlider.value = musicVolume;
            musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            UpdateMusicValueText(musicVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = sfxVolume;
            sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
            UpdateSFXValueText(sfxVolume);
        }

        ApplyVolumes();
    }

    private void OnMusicVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
        UpdateMusicValueText(value);
        ApplyVolumes();
    }

    private void OnSFXVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
        UpdateSFXValueText(value);
        ApplyVolumes();
    }

    private void UpdateMusicValueText(float value)
    {
        if (musicValueText != null)
            musicValueText.text = Mathf.RoundToInt(value * 100) + "%";
    }

    private void UpdateSFXValueText(float value)
    {
        if (sfxValueText != null)
            sfxValueText.text = Mathf.RoundToInt(value * 100) + "%";
    }

    private void ApplyVolumes()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume", 1f));
            SoundManager.Instance.SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume", 1f));
        }
    }

    public void Pause()
    {
        if (isPaused || isAnimating)
            return;

        if (GameModeController.Instance != null && !GameModeController.Instance.IsGameActive)
            return;

        isPaused = true;
        isAnimating = true;

        Time.timeScale = 0f;

        if (rotationController == null)
            rotationController = FindObjectOfType<RotationController>();
        
        if (lockMechanic == null)
            lockMechanic = LockMechanic.Instance;

        if (lockMechanic != null)
            lockMechanic.SetCanLock(false);

        pausePanel.SetActive(true);
        canvasGroup.alpha = 0f;
        panelRect.localScale = Vector3.one * 0.8f;

        canvasGroup.DOFade(1f, fadeDuration).SetUpdate(true);
        panelRect.DOScale(1f, scaleDuration).SetEase(showEase).SetUpdate(true).OnComplete(() =>
        {
            isAnimating = false;
        });
    }

    public void Resume()
    {
        if (!isPaused || isAnimating)
            return;

        isAnimating = true;

        canvasGroup.DOFade(0f, fadeDuration).SetUpdate(true);
        panelRect.DOScale(0.8f, scaleDuration).SetEase(hideEase).SetUpdate(true).OnComplete(() =>
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1f;
            isPaused = false;
            isAnimating = false;

            if (lockMechanic != null && GameModeController.Instance != null && GameModeController.Instance.IsGameActive)
                lockMechanic.SetCanLock(true);
        });
    }

    private void Restart()
    {
        Time.timeScale = 1f;
        isPaused = false;
        HideInstant();

        if (GameModeController.Instance != null)
        {
            GameModeController.Instance.RestartGame();
        }
    }

    private void GoToMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
        HideInstant();

        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadScene("MainMenu");
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }

    public void HideInstant()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;
    }

    public void TogglePause()
    {
        if (isPaused)
            Resume();
        else
            Pause();
    }
}
