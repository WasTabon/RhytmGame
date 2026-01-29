using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class LevelSelectUI : MonoBehaviour
{
    public static LevelSelectUI Instance { get; private set; }

    [Header("Panel")]
    [SerializeField] private RectTransform panelRect;
    [SerializeField] private CanvasGroup panelCanvasGroup;

    [Header("Scroll")]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform content;
    [SerializeField] private RectTransform viewport;

    [Header("Card Prefab")]
    [SerializeField] private LevelCard cardPrefab;

    [Header("Layout")]
    [SerializeField] private float cardHeight = 180f;
    [SerializeField] private float cardSpacing = 20f;
    [SerializeField] private float topPadding = 20f;
    [SerializeField] private float bottomPadding = 20f;
    [SerializeField] private float sidePadding = 40f;

    [Header("Data")]
    [SerializeField] private LevelData levelData;

    [Header("Animation")]
    [SerializeField] private float panelSlideDuration = 0.4f;
    [SerializeField] private float cardAppearDelay = 0.05f;

    [Header("Buttons")]
    [SerializeField] private Button backButton;

    private List<LevelCard> cards = new List<LevelCard>();
    private Vector2 hiddenPosition;
    private Vector2 shownPosition;
    private bool isShown;
    private float lastScrollPosition;

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

        RefreshLevelList();

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

    private void RefreshLevelList()
    {
        ClearCards();

        if (levelData == null || levelData.LevelCount == 0)
        {
            Debug.LogWarning("[LevelSelectUI] No LevelData assigned!");
            return;
        }

        int unlockedCount = GetUnlockedLevelCount();

        float totalHeight = topPadding + bottomPadding + 
            (levelData.LevelCount * cardHeight) + 
            ((levelData.LevelCount - 1) * cardSpacing);

        content.sizeDelta = new Vector2(content.sizeDelta.x, totalHeight);

        for (int i = 0; i < levelData.LevelCount; i++)
        {
            CreateCard(i, unlockedCount);
        }

        scrollRect.verticalNormalizedPosition = 1f;
    }

    private void CreateCard(int index, int unlockedCount)
    {
        if (cardPrefab == null)
        {
            Debug.LogError("[LevelSelectUI] Card prefab not assigned!");
            return;
        }

        LevelCard card = Instantiate(cardPrefab, content);
        card.name = $"LevelCard_{index + 1}";

        RectTransform cardRect = card.GetComponent<RectTransform>();
        cardRect.anchorMin = new Vector2(0, 1);
        cardRect.anchorMax = new Vector2(1, 1);
        cardRect.pivot = new Vector2(0.5f, 1);

        float yPos = -topPadding - (index * (cardHeight + cardSpacing));
        cardRect.anchoredPosition = new Vector2(0, yPos);
        cardRect.sizeDelta = new Vector2(-sidePadding * 2, cardHeight);

        card.SetOriginalPosition(cardRect.anchoredPosition);

        LevelInfo levelInfo = levelData.GetLevel(index);
        bool isLocked = index >= unlockedCount;
        bool isCompleted = index < unlockedCount - 1 || (index < unlockedCount && HasCompletedLevel(index));

        int bestScore = GetBestScore(index);
        int bestCombo = GetBestCombo(index);

        card.Setup(index, levelInfo, isLocked, isCompleted, bestScore, bestCombo);
        card.OnCardClicked += HandleCardClicked;
        card.HideInstant();

        cards.Add(card);
    }

    private void ClearCards()
    {
        foreach (var card in cards)
        {
            if (card != null)
            {
                card.OnCardClicked -= HandleCardClicked;
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

    private bool IsCardInViewport(LevelCard card)
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

    private float GetCardViewportPosition(LevelCard card)
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

    private void HandleCardClicked(int levelIndex)
    {
        LevelCard card = cards.Find(c => c.LevelIndex == levelIndex);
        if (card != null)
        {
            card.PlayPressAnimation();
        }

        DOVirtual.DelayedCall(0.2f, () =>
        {
            StartLevel(levelIndex);
        });
    }

    private void StartLevel(int levelIndex)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetGameMode(GameMode.Levels);
            GameManager.Instance.SetSelectedLevel(levelIndex);
        }

        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadGame();
        }
    }

    private void HandleBackClicked()
    {
        Hide();
        OnBackClicked?.Invoke();
    }

    private int GetUnlockedLevelCount()
    {
        return PlayerPrefs.GetInt("UnlockedLevel", 0) + 1;
    }

    private bool HasCompletedLevel(int index)
    {
        return PlayerPrefs.GetInt($"Level_{index}_Completed", 0) == 1;
    }

    private int GetBestScore(int index)
    {
        return PlayerPrefs.GetInt($"Level_{index}_BestScore", 0);
    }

    private int GetBestCombo(int index)
    {
        return PlayerPrefs.GetInt($"Level_{index}_BestCombo", 0);
    }

    public static void SaveLevelResult(int levelIndex, int score, int maxCombo)
    {
        int bestScore = PlayerPrefs.GetInt($"Level_{levelIndex}_BestScore", 0);
        int bestCombo = PlayerPrefs.GetInt($"Level_{levelIndex}_BestCombo", 0);

        if (score > bestScore)
        {
            PlayerPrefs.SetInt($"Level_{levelIndex}_BestScore", score);
        }

        if (maxCombo > bestCombo)
        {
            PlayerPrefs.SetInt($"Level_{levelIndex}_BestCombo", maxCombo);
        }

        PlayerPrefs.SetInt($"Level_{levelIndex}_Completed", 1);
        PlayerPrefs.Save();
    }

    public void SetLevelData(LevelData data)
    {
        levelData = data;
    }

    public void SetCardPrefab(LevelCard prefab)
    {
        cardPrefab = prefab;
    }
}
