using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class AchievementCard : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform cardRect;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private Image categoryBadge;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI starsText;
    [SerializeField] private GameObject lockedOverlay;

    [Header("Colors")]
    [SerializeField] private Color unlockedBgColor = new Color(0.2f, 0.25f, 0.35f, 1f);
    [SerializeField] private Color lockedBgColor = new Color(0.12f, 0.12f, 0.15f, 0.9f);
    [SerializeField] private Color beginnerColor = new Color(0.4f, 0.7f, 0.4f, 1f);
    [SerializeField] private Color skillColor = new Color(0.4f, 0.5f, 0.9f, 1f);
    [SerializeField] private Color enduranceColor = new Color(0.9f, 0.6f, 0.3f, 1f);
    [SerializeField] private Color masteryColor = new Color(0.9f, 0.4f, 0.9f, 1f);

    [Header("Animation")]
    [SerializeField] private float appearDuration = 0.3f;
    [SerializeField] private float disappearDuration = 0.2f;
    [SerializeField] private float slideDistance = 100f;
    [SerializeField] private Ease appearEase = Ease.OutBack;
    [SerializeField] private Ease disappearEase = Ease.InCubic;

    private string achievementId;
    private bool isUnlocked;
    private bool isVisible;
    private bool isAnimating;
    private Vector2 originalPosition;

    public bool IsVisible => isVisible;

    private void Awake()
    {
        if (cardRect == null)
            cardRect = GetComponent<RectTransform>();

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Setup(AchievementInfo achievement, bool unlocked)
    {
        achievementId = achievement.id;
        isUnlocked = unlocked;

        if (titleText != null)
        {
            titleText.text = unlocked ? achievement.title : "???";
            titleText.alpha = unlocked ? 1f : 0.5f;
        }

        if (descriptionText != null)
        {
            descriptionText.text = unlocked ? achievement.description : "Locked achievement";
            descriptionText.alpha = unlocked ? 0.8f : 0.4f;
        }

        if (starsText != null)
        {
            starsText.text = $"{achievement.starReward} ★";
            starsText.alpha = unlocked ? 1f : 0.3f;
        }

        if (iconImage != null)
        {
            if (achievement.icon != null && unlocked)
            {
                iconImage.sprite = achievement.icon;
                iconImage.color = Color.white;
            }
            else
            {
                iconImage.color = unlocked ? GetCategoryColor(achievement.category) : new Color(0.3f, 0.3f, 0.3f, 0.5f);
            }
        }

        if (categoryBadge != null)
        {
            categoryBadge.color = GetCategoryColor(achievement.category);
            categoryBadge.gameObject.SetActive(unlocked);
        }

        if (backgroundImage != null)
        {
            backgroundImage.color = unlocked ? unlockedBgColor : lockedBgColor;
        }

        if (lockedOverlay != null)
        {
            lockedOverlay.SetActive(!unlocked);
        }
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

    public void SetOriginalPosition(Vector2 position)
    {
        originalPosition = position;
    }

    public void AnimateAppear(bool fromTop)
    {
        if (isVisible || isAnimating)
            return;

        isAnimating = true;
        isVisible = true;

        cardRect.DOKill();
        canvasGroup.DOKill();

        cardRect.anchoredPosition = new Vector2(originalPosition.x + slideDistance, originalPosition.y);
        canvasGroup.alpha = 0f;

        cardRect.DOAnchorPos(originalPosition, appearDuration).SetEase(appearEase);
        canvasGroup.DOFade(1f, appearDuration).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            isAnimating = false;
        });
    }

    public void AnimateDisappear(bool toTop)
    {
        if (!isVisible || isAnimating)
            return;

        isAnimating = true;
        isVisible = false;

        cardRect.DOKill();
        canvasGroup.DOKill();

        Vector2 targetPos = new Vector2(originalPosition.x - slideDistance, originalPosition.y);

        cardRect.DOAnchorPos(targetPos, disappearDuration).SetEase(disappearEase);
        canvasGroup.DOFade(0f, disappearDuration).SetEase(Ease.InQuad).OnComplete(() =>
        {
            isAnimating = false;
        });
    }

    public void ShowInstant()
    {
        isVisible = true;
        cardRect.anchoredPosition = originalPosition;
        canvasGroup.alpha = 1f;
    }

    public void HideInstant()
    {
        isVisible = false;
        canvasGroup.alpha = 0f;
    }

    public void AnimateUnlock()
    {
        cardRect.DOKill();
        cardRect.localScale = Vector3.one;

        Sequence sequence = DOTween.Sequence();

        sequence.Append(cardRect.DOScale(1.1f, 0.2f).SetEase(Ease.OutQuad));

        if (backgroundImage != null)
        {
            sequence.Join(backgroundImage.DOColor(Color.white, 0.15f));
            sequence.Append(backgroundImage.DOColor(unlockedBgColor, 0.3f));
        }

        sequence.Join(cardRect.DOScale(1f, 0.3f).SetEase(Ease.OutBounce));
    }
}
