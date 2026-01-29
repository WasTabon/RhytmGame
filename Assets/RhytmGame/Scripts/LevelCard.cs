using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;

public class LevelCard : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform cardRect;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image lockIcon;
    [SerializeField] private Image completedIcon;
    [SerializeField] private TextMeshProUGUI numberText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI shapesText;
    [SerializeField] private TextMeshProUGUI bestScoreText;
    [SerializeField] private TextMeshProUGUI bestComboText;
    [SerializeField] private Button cardButton;

    [Header("Colors")]
    [SerializeField] private Color unlockedColor = new Color(0.2f, 0.25f, 0.35f, 1f);
    [SerializeField] private Color lockedColor = new Color(0.15f, 0.15f, 0.2f, 0.7f);
    [SerializeField] private Color completedColor = new Color(0.2f, 0.35f, 0.25f, 1f);

    [Header("Animation")]
    [SerializeField] private float appearDuration = 0.3f;
    [SerializeField] private float disappearDuration = 0.2f;
    [SerializeField] private float slideDistance = 100f;
    [SerializeField] private Ease appearEase = Ease.OutBack;
    [SerializeField] private Ease disappearEase = Ease.InCubic;

    private int levelIndex;
    private bool isLocked;
    private bool isCompleted;
    private bool isVisible;
    private bool isAnimating;
    private Vector2 originalPosition;

    public event Action<int> OnCardClicked;

    public int LevelIndex => levelIndex;
    public bool IsVisible => isVisible;

    private void Awake()
    {
        if (cardRect == null)
            cardRect = GetComponent<RectTransform>();

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (cardButton != null)
        {
            cardButton.onClick.AddListener(HandleClick);
        }
    }

    public void Setup(int index, LevelInfo levelInfo, bool locked, bool completed, int bestScore, int bestCombo)
    {
        levelIndex = index;
        isLocked = locked;
        isCompleted = completed;

        if (numberText != null)
            numberText.text = (index + 1).ToString();

        if (nameText != null)
            nameText.text = levelInfo.levelName;

        if (shapesText != null)
            shapesText.text = $"{levelInfo.shapesToComplete} shapes";

        if (bestScoreText != null)
        {
            if (bestScore > 0)
                bestScoreText.text = $"Best: {bestScore}";
            else
                bestScoreText.text = "Best: ---";
        }

        if (bestComboText != null)
        {
            if (bestCombo > 0)
                bestComboText.text = $"Combo: x{bestCombo}";
            else
                bestComboText.text = "Combo: ---";
        }

        UpdateVisualState();
    }

    private void UpdateVisualState()
    {
        if (lockIcon != null)
            lockIcon.gameObject.SetActive(isLocked);

        if (completedIcon != null)
            completedIcon.gameObject.SetActive(isCompleted && !isLocked);

        if (backgroundImage != null)
        {
            if (isLocked)
                backgroundImage.color = lockedColor;
            else if (isCompleted)
                backgroundImage.color = completedColor;
            else
                backgroundImage.color = unlockedColor;
        }

        if (cardButton != null)
            cardButton.interactable = !isLocked;

        if (numberText != null)
            numberText.alpha = isLocked ? 0.5f : 1f;

        if (nameText != null)
            nameText.alpha = isLocked ? 0.5f : 1f;

        if (shapesText != null)
            shapesText.alpha = isLocked ? 0.5f : 1f;

        if (bestScoreText != null)
            bestScoreText.gameObject.SetActive(!isLocked);

        if (bestComboText != null)
            bestComboText.gameObject.SetActive(!isLocked);
    }

    private void HandleClick()
    {
        if (!isLocked)
        {
            OnCardClicked?.Invoke(levelIndex);
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

        float startOffset = fromTop ? slideDistance : -slideDistance;
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

        float endOffset = toTop ? slideDistance : -slideDistance;
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

    public void PlayPressAnimation()
    {
        cardRect.DOKill();
        cardRect.localScale = Vector3.one;
        cardRect.DOPunchScale(Vector3.one * 0.05f, 0.2f, 5, 0.5f);
    }
}
