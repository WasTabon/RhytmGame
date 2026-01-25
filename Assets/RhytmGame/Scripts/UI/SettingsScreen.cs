using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class SettingsScreen : ScreenBase
{
    [Header("Header")]
    [SerializeField] private RectTransform headerRect;
    [SerializeField] private Button backButton;

    [Header("Content")]
    [SerializeField] private RectTransform contentRect;

    [Header("Music Volume")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private TextMeshProUGUI musicValueText;

    [Header("SFX Volume")]
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TextMeshProUGUI sfxValueText;

    [Header("Vibration")]
    [SerializeField] private Toggle vibrationToggle;
    [SerializeField] private Image vibrationCheckmark;

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

        if (musicSlider != null)
        {
            musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            musicSlider.value = AudioManager.Instance?.MusicVolume ?? 1f;
            UpdateMusicValueText(musicSlider.value);
        }

        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
            sfxSlider.value = AudioManager.Instance?.SFXVolume ?? 1f;
            UpdateSFXValueText(sfxSlider.value);
        }

        if (vibrationToggle != null)
        {
            vibrationToggle.onValueChanged.AddListener(OnVibrationChanged);
            vibrationToggle.isOn = VibrationManager.Instance?.VibrationEnabled ?? true;
            UpdateVibrationVisual(vibrationToggle.isOn);
        }
    }

    private void OnEnable()
    {
        RefreshSettings();
    }

    private void RefreshSettings()
    {
        if (musicSlider != null && AudioManager.Instance != null)
        {
            musicSlider.SetValueWithoutNotify(AudioManager.Instance.MusicVolume);
            UpdateMusicValueText(AudioManager.Instance.MusicVolume);
        }

        if (sfxSlider != null && AudioManager.Instance != null)
        {
            sfxSlider.SetValueWithoutNotify(AudioManager.Instance.SFXVolume);
            UpdateSFXValueText(AudioManager.Instance.SFXVolume);
        }

        if (vibrationToggle != null && VibrationManager.Instance != null)
        {
            vibrationToggle.SetIsOnWithoutNotify(VibrationManager.Instance.VibrationEnabled);
            UpdateVibrationVisual(VibrationManager.Instance.VibrationEnabled);
        }
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

    private void OnMusicVolumeChanged(float value)
    {
        AudioManager.Instance?.SetMusicVolume(value);
        UpdateMusicValueText(value);
    }

    private void OnSFXVolumeChanged(float value)
    {
        AudioManager.Instance?.SetSFXVolume(value);
        UpdateSFXValueText(value);
    }

    private void OnVibrationChanged(bool isOn)
    {
        VibrationManager.Instance?.SetVibrationEnabled(isOn);
        UpdateVibrationVisual(isOn);
        
        if (isOn)
        {
            VibrationManager.Instance?.Selection();
        }
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

    private void UpdateVibrationVisual(bool isOn)
    {
        if (vibrationCheckmark != null)
        {
            vibrationCheckmark.DOFade(isOn ? 1f : 0f, 0.2f);
        }
    }

    private void OnBackClicked()
    {
        VibrationManager.Instance?.Selection();
        mainMenuUI?.ShowMainMenu();
    }
}
