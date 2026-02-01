using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;

public class AchievementPopup : MonoBehaviour
{
    public static AchievementPopup Instance { get; private set; }

    [Header("References")]
    [SerializeField] private RectTransform popupRect;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image iconImage;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI starsText;

    [Header("Animation")]
    [SerializeField] private float slideDistance = 200f;
    [SerializeField] private float slideDuration = 0.4f;
    [SerializeField] private float displayDuration = 3f;
    [SerializeField] private float punchScale = 1.1f;
    [SerializeField] private Ease slideInEase = Ease.OutBack;
    [SerializeField] private Ease slideOutEase = Ease.InCubic;

    [Header("Colors")]
    [SerializeField] private Color beginnerColor = new Color(0.4f, 0.7f, 0.4f, 1f);
    [SerializeField] private Color skillColor = new Color(0.4f, 0.5f, 0.9f, 1f);
    [SerializeField] private Color enduranceColor = new Color(0.9f, 0.6f, 0.3f, 1f);
    [SerializeField] private Color masteryColor = new Color(0.9f, 0.4f, 0.9f, 1f);

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip unlockSound;

    private Queue<AchievementInfo> pendingPopups = new Queue<AchievementInfo>();
    private bool isShowing;
    private Vector2 hiddenPosition;
    private Vector2 shownPosition;
    private bool isSubscribed;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(transform.root.gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        SetupPositions();
        HideInstant();
        SubscribeToAchievements();
    }

    private void OnDestroy()
    {
        UnsubscribeFromAchievements();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SubscribeToAchievements();
    }

    private void SubscribeToAchievements()
    {
        if (isSubscribed)
            return;

        if (AchievementManager.Instance != null)
        {
            AchievementManager.Instance.OnAchievementUnlocked += QueuePopup;
            isSubscribed = true;
        }
    }

    private void UnsubscribeFromAchievements()
    {
        if (!isSubscribed)
            return;

        if (AchievementManager.Instance != null)
        {
            AchievementManager.Instance.OnAchievementUnlocked -= QueuePopup;
        }
        isSubscribed = false;
    }

    public void LateSubscribe()
    {
        SubscribeToAchievements();
    }

    private void SetupPositions()
    {
        shownPosition = new Vector2(0, -100);
        hiddenPosition = new Vector2(0, slideDistance);
    }

    private void QueuePopup(AchievementInfo achievement)
    {
        pendingPopups.Enqueue(achievement);

        if (!isShowing)
        {
            ShowNextPopup();
        }
    }

    private void ShowNextPopup()
    {
        if (pendingPopups.Count == 0)
        {
            isShowing = false;
            return;
        }

        isShowing = true;
        AchievementInfo achievement = pendingPopups.Dequeue();
        ShowPopup(achievement);
    }

    private void ShowPopup(AchievementInfo achievement)
    {
        if (titleText != null)
            titleText.text = achievement.title;

        if (descriptionText != null)
            descriptionText.text = achievement.description;

        if (starsText != null)
            starsText.text = $"+{achievement.starReward}";

        if (iconImage != null)
        {
            if (achievement.icon != null)
            {
                iconImage.sprite = achievement.icon;
                iconImage.color = Color.white;
            }
            else
            {
                iconImage.color = GetCategoryColor(achievement.category);
            }
        }

        if (backgroundImage != null)
        {
            backgroundImage.color = GetCategoryColor(achievement.category);
        }

        if (audioSource != null && unlockSound != null)
        {
            audioSource.PlayOneShot(unlockSound);
        }

        AnimateShow();
    }

    private Color GetCategoryColor(AchievementCategory category)
    {
        switch (category)
        {
            case AchievementCategory.Beginner:
                return beginnerColor;
            case AchievementCategory.Skill:
                return skillColor;
            case AchievementCategory.Endurance:
                return enduranceColor;
            case AchievementCategory.Mastery:
                return masteryColor;
            default:
                return Color.white;
        }
    }

    private void AnimateShow()
    {
        if (popupRect == null || canvasGroup == null)
            return;

        popupRect.DOKill();
        canvasGroup.DOKill();

        popupRect.anchoredPosition = hiddenPosition;
        canvasGroup.alpha = 0f;
        popupRect.localScale = Vector3.one;

        gameObject.SetActive(true);

        Sequence sequence = DOTween.Sequence();

        sequence.Append(popupRect.DOAnchorPos(shownPosition, slideDuration).SetEase(slideInEase));
        sequence.Join(canvasGroup.DOFade(1f, slideDuration * 0.5f));
        sequence.Append(popupRect.DOPunchScale(Vector3.one * (punchScale - 1f), 0.3f, 5, 0.5f));

        sequence.AppendInterval(displayDuration);

        sequence.Append(popupRect.DOAnchorPos(hiddenPosition, slideDuration * 0.7f).SetEase(slideOutEase));
        sequence.Join(canvasGroup.DOFade(0f, slideDuration * 0.5f));

        sequence.OnComplete(() =>
        {
            ShowNextPopup();
        });
    }

    public void HideInstant()
    {
        if (popupRect != null)
            popupRect.anchoredPosition = hiddenPosition;

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

        //gameObject.SetActive(false);
    }

    public void ForceHide()
    {
        if (popupRect != null)
            popupRect.DOKill();
        if (canvasGroup != null)
            canvasGroup.DOKill();
        HideInstant();
        isShowing = false;
        ShowNextPopup();
    }
}
