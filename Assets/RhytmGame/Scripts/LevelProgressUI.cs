using UnityEngine;
using TMPro;
using DG.Tweening;

public class LevelProgressUI : MonoBehaviour
{
    public static LevelProgressUI Instance { get; private set; }

    [Header("References")]
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private GameModeController gameModeController;

    [Header("Format")]
    [SerializeField] private string progressFormat = "{0} / {1}";

    [Header("Animation")]
    [SerializeField] private float punchScale = 1.2f;
    [SerializeField] private float punchDuration = 0.2f;

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

        if (gameModeController != null)
        {
            gameModeController.OnShapeProgress += UpdateProgress;
            gameModeController.OnGameStart += HandleGameStart;
        }

        Hide();
    }

    private void OnDestroy()
    {
        if (gameModeController != null)
        {
            gameModeController.OnShapeProgress -= UpdateProgress;
            gameModeController.OnGameStart -= HandleGameStart;
        }
    }

    private void HandleGameStart()
    {
        if (gameModeController != null && gameModeController.CurrentMode == GameMode.Levels)
        {
            Show();
            UpdateProgress(0, gameModeController.ShapesRequired);
        }
        else
        {
            Hide();
        }
    }

    private void UpdateProgress(int completed, int required)
    {
        if (progressText == null)
            return;

        progressText.text = string.Format(progressFormat, completed, required);
        AnimatePunch();
    }

    private void AnimatePunch()
    {
        if (progressText == null)
            return;

        progressText.transform.DOKill();
        progressText.transform.localScale = Vector3.one;
        progressText.transform.DOPunchScale(Vector3.one * (punchScale - 1f), punchDuration, 5, 0.5f);
    }

    public void Show()
    {
        if (progressText != null)
        {
            progressText.gameObject.SetActive(true);
        }
    }

    public void Hide()
    {
        if (progressText != null)
        {
            progressText.gameObject.SetActive(false);
        }
    }

    public void SetProgressText(TextMeshProUGUI text)
    {
        progressText = text;
    }
}
