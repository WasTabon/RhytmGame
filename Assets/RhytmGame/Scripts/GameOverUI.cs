using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GameOverUI : MonoBehaviour
{
    public static GameOverUI Instance { get; private set; }

    [Header("Panel")]
    [SerializeField] private GameObject panel;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI maxComboText;
    [SerializeField] private TextMeshProUGUI accuracyText;
    [SerializeField] private TextMeshProUGUI shapesText;

    [Header("Buttons")]
    [SerializeField] private Button retryButton;
    [SerializeField] private Button menuButton;

    [Header("Animation")]
    [SerializeField] private float fadeInDuration = 0.3f;
    [SerializeField] private float showDelay = 0.5f;

    [Header("References")]
    [SerializeField] private GameModeController gameModeController;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private SceneLoader sceneLoader;

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
        if (gameModeController == null)
            gameModeController = GameModeController.Instance;

        if (scoreManager == null)
            scoreManager = ScoreManager.Instance;

        if (sceneLoader == null)
            sceneLoader = FindObjectOfType<SceneLoader>();

        if (gameModeController != null)
        {
            gameModeController.OnGameOver += ShowGameOver;
            gameModeController.OnLevelComplete += ShowLevelComplete;
        }

        if (retryButton != null)
        {
            retryButton.onClick.AddListener(OnRetryClicked);
        }

        if (menuButton != null)
        {
            menuButton.onClick.AddListener(OnMenuClicked);
        }

        Hide();
    }

    private void OnDestroy()
    {
        if (gameModeController != null)
        {
            gameModeController.OnGameOver -= ShowGameOver;
            gameModeController.OnLevelComplete -= ShowLevelComplete;
        }
    }

    private void ShowGameOver()
    {
        if (titleText != null)
        {
            titleText.text = "GAME OVER";
        }

        DOVirtual.DelayedCall(showDelay, () =>
        {
            UpdateStats();
            ShowPanel();
        });
    }

    private void ShowLevelComplete()
    {
        if (titleText != null)
        {
            string levelName = gameModeController != null ? 
                gameModeController.GetCurrentLevelName() : "Level";
            titleText.text = $"{levelName}\nCOMPLETE!";
        }

        DOVirtual.DelayedCall(showDelay, () =>
        {
            UpdateStats();
            ShowPanel();
        });
    }

    private void UpdateStats()
    {
        if (scoreManager == null)
            return;

        if (scoreText != null)
        {
            scoreText.text = $"Score: {scoreManager.CurrentScore}";
        }

        if (maxComboText != null)
        {
            maxComboText.text = $"Max Combo: {scoreManager.MaxCombo}";
        }

        if (accuracyText != null)
        {
            accuracyText.text = $"Accuracy: {scoreManager.GetAccuracy():F1}%";
        }

        if (shapesText != null && gameModeController != null)
        {
            if (gameModeController.CurrentMode == GameMode.Levels)
            {
                shapesText.text = $"Shapes: {gameModeController.ShapesCompleted}/{gameModeController.ShapesRequired}";
                shapesText.gameObject.SetActive(true);
            }
            else
            {
                shapesText.text = $"Shapes: {gameModeController.ShapesCompleted}";
                shapesText.gameObject.SetActive(true);
            }
        }
    }

    private void ShowPanel()
    {
        if (panel != null)
        {
            panel.SetActive(true);
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.DOFade(1f, fadeInDuration).SetEase(Ease.OutQuad);
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }

    public void Hide()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    private void OnRetryClicked()
    {
        Hide();

        if (gameModeController != null)
        {
            gameModeController.RestartGame();
        }
    }

    private void OnMenuClicked()
    {
        if (sceneLoader != null)
        {
            sceneLoader.LoadScene("MainMenu");
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }

    public void SetReferences(TextMeshProUGUI title, TextMeshProUGUI score, 
        TextMeshProUGUI combo, TextMeshProUGUI accuracy, TextMeshProUGUI shapes,
        Button retry, Button menu)
    {
        titleText = title;
        scoreText = score;
        maxComboText = combo;
        accuracyText = accuracy;
        shapesText = shapes;
        retryButton = retry;
        menuButton = menu;
    }
}
