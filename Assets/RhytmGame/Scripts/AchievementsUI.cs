using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;

public class AchievementsUI : MonoBehaviour
{
    public static AchievementsUI Instance { get; private set; }

    [Header("Panel")]
    [SerializeField] private RectTransform panelRect;
    [SerializeField] private CanvasGroup panelCanvasGroup;

    [Header("Header")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private TextMeshProUGUI starsText;

    [Header("Scroll")]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform content;
    [SerializeField] private RectTransform viewport;

    [Header("Card Prefab")]
    [SerializeField] private AchievementCard cardPrefab;

    [Header("Layout")]
    [SerializeField] private float cardHeight = 140f;
    [SerializeField] private float cardSpacing = 15f;
    [SerializeField] private float topPadding = 20f;
    [SerializeField] private float bottomPadding = 20f;
    [SerializeField] private float sidePadding = 30f;

    [Header("Animation")]
    [SerializeField] private float panelSlideDuration = 0.4f;
    [SerializeField] private float cardAppearDelay = 0.03f;

    [Header("Buttons")]
    [SerializeField] private Button backButton;

    private List<AchievementCard> cards = new List<AchievementCard>();
    private Vector2 hiddenPosition;
    private Vector2 shownPosition;
    private bool isShown;

    public System.Action OnBackClicked;

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
        SetupPositions();

        if (backButton != null)
        {
            backButton.onClick.AddListener(HandleBackClicked);
        }

        if (scrollRect != null)
        {
            scrollRect.onValueChanged.AddListener(OnScrollChanged);
        }

        HideInstant();
    }

    private void SetupPositions()
    {
        shownPosition = Vector2.zero;
        hiddenPosition = new Vector2(Screen.width, 0);

        if (panelRect != null)
        {
            panelRect.anchoredPosition = hiddenPosition;
        }
    }

    public void Show()
    {
        if (isShown)
            return;

        isShown = true;
        gameObject.SetActive(true);

        RefreshAchievementList();
        UpdateHeader();

        panelRect.DOKill();
        panelCanvasGroup.DOKill();

        panelRect.anchoredPosition = hiddenPosition;
        panelCanvasGroup.alpha = 0f;

        panelRect.DOAnchorPos(shownPosition, panelSlideDuration).SetEase(Ease.OutCubic);
        panelCanvasGroup.DOFade(1f, panelSlideDuration).SetEase(Ease.OutQuad);

        AnimateCardsAppear();
    }

    public void Hide()
    {
        if (!isShown)
            return;

        isShown = false;

        panelRect.DOKill();
        panelCanvasGroup.DOKill();

        panelRect.DOAnchorPos(hiddenPosition, panelSlideDuration).SetEase(Ease.InCubic);
        panelCanvasGroup.DOFade(0f, panelSlideDuration).SetEase(Ease.InQuad).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }

    public void HideInstant()
    {
        isShown = false;

        if (panelRect != null)
            panelRect.anchoredPosition = hiddenPosition;

        if (panelCanvasGroup != null)
            panelCanvasGroup.alpha = 0f;

        gameObject.SetActive(false);
    }

    private void UpdateHeader()
    {
        if (AchievementManager.Instance == null)
            return;

        int unlocked = AchievementManager.Instance.UnlockedCount;
        int total = AchievementManager.Instance.TotalAchievements;
        int stars = AchievementManager.Instance.TotalStars;

        if (progressText != null)
        {
            progressText.text = $"{unlocked} / {total}";
        }

        if (starsText != null)
        {
            starsText.text = $"{stars} ★";
        }
    }

    private void RefreshAchievementList()
    {
        ClearCards();

        if (AchievementManager.Instance == null)
        {
            Debug.LogWarning("[AchievementsUI] AchievementManager not found!");
            return;
        }

        var achievements = AchievementManager.Instance.GetAllAchievements();

        achievements.Sort((a, b) =>
        {
            bool aUnlocked = AchievementManager.Instance.IsUnlocked(a.id);
            bool bUnlocked = AchievementManager.Instance.IsUnlocked(b.id);

            if (aUnlocked != bUnlocked)
                return aUnlocked ? -1 : 1;

            if (a.category != b.category)
                return a.category.CompareTo(b.category);

            return 0;
        });

        float totalHeight = topPadding + bottomPadding +
            (achievements.Count * cardHeight) +
            ((achievements.Count - 1) * cardSpacing);

        content.sizeDelta = new Vector2(content.sizeDelta.x, totalHeight);

        for (int i = 0; i < achievements.Count; i++)
        {
            CreateCard(i, achievements[i]);
        }

        scrollRect.verticalNormalizedPosition = 1f;
    }

    private void CreateCard(int index, AchievementInfo achievement)
    {
        if (cardPrefab == null)
        {
            Debug.LogError("[AchievementsUI] Card prefab not assigned!");
            return;
        }

        AchievementCard card = Instantiate(cardPrefab, content);
        card.name = $"AchievementCard_{achievement.id}";

        RectTransform cardRect = card.GetComponent<RectTransform>();
        cardRect.anchorMin = new Vector2(0, 1);
        cardRect.anchorMax = new Vector2(1, 1);
        cardRect.pivot = new Vector2(0.5f, 1);

        float yPos = -topPadding - (index * (cardHeight + cardSpacing));
        cardRect.anchoredPosition = new Vector2(0, yPos);
        cardRect.sizeDelta = new Vector2(-sidePadding * 2, cardHeight);

        card.SetOriginalPosition(cardRect.anchoredPosition);

        bool isUnlocked = AchievementManager.Instance.IsUnlocked(achievement.id);
        card.Setup(achievement, isUnlocked);
        card.HideInstant();

        cards.Add(card);
    }

    private void ClearCards()
    {
        foreach (var card in cards)
        {
            if (card != null)
            {
                Destroy(card.gameObject);
            }
        }
        cards.Clear();
    }

    private void AnimateCardsAppear()
    {
        float delay = 0f;
        foreach (var card in cards)
        {
            if (IsCardInViewport(card))
            {
                DOVirtual.DelayedCall(delay, () =>
                {
                    if (card != null)
                        card.AnimateAppear(true);
                });
                delay += cardAppearDelay;
            }
        }
    }

    private void OnScrollChanged(Vector2 position)
    {
        UpdateCardVisibility();
    }

    private void UpdateCardVisibility()
    {
        foreach (var card in cards)
        {
            if (card == null)
                continue;

            bool inViewport = IsCardInViewport(card);

            if (inViewport && !card.IsVisible)
            {
                bool fromTop = GetCardViewportPosition(card) > 0.5f;
                card.AnimateAppear(fromTop);
            }
            else if (!inViewport && card.IsVisible)
            {
                bool toTop = GetCardViewportPosition(card) > 0.5f;
                card.AnimateDisappear(toTop);
            }
        }
    }

    private bool IsCardInViewport(AchievementCard card)
    {
        if (viewport == null || card == null)
            return false;

        RectTransform cardRect = card.GetComponent<RectTransform>();

        Vector3[] cardCorners = new Vector3[4];
        Vector3[] viewportCorners = new Vector3[4];

        cardRect.GetWorldCorners(cardCorners);
        viewport.GetWorldCorners(viewportCorners);

        float cardTop = cardCorners[1].y;
        float cardBottom = cardCorners[0].y;
        float viewportTop = viewportCorners[1].y;
        float viewportBottom = viewportCorners[0].y;

        float buffer = cardHeight * 0.5f;

        return cardBottom < viewportTop + buffer && cardTop > viewportBottom - buffer;
    }

    private float GetCardViewportPosition(AchievementCard card)
    {
        if (viewport == null || card == null)
            return 0.5f;

        RectTransform cardRect = card.GetComponent<RectTransform>();

        Vector3[] cardCorners = new Vector3[4];
        Vector3[] viewportCorners = new Vector3[4];

        cardRect.GetWorldCorners(cardCorners);
        viewport.GetWorldCorners(viewportCorners);

        float cardCenter = (cardCorners[0].y + cardCorners[1].y) / 2f;
        float viewportCenter = (viewportCorners[0].y + viewportCorners[1].y) / 2f;
        float viewportHeight = viewportCorners[1].y - viewportCorners[0].y;

        return 0.5f + (cardCenter - viewportCenter) / viewportHeight;
    }

    private void HandleBackClicked()
    {
        Hide();
        OnBackClicked?.Invoke();
    }

    public void SetCardPrefab(AchievementCard prefab)
    {
        cardPrefab = prefab;
    }
}
