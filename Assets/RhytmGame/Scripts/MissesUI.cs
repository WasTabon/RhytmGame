using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MissesUI : MonoBehaviour
{
    public static MissesUI Instance { get; private set; }

    [Header("References")]
    [SerializeField] private TextMeshProUGUI missesText;
    [SerializeField] private Image[] heartIcons;
    [SerializeField] private GameModeController gameModeController;

    [Header("Settings")]
    [SerializeField] private bool useHearts = false;
    [SerializeField] private Color fullHeartColor = new Color(1f, 0.3f, 0.3f);
    [SerializeField] private Color emptyHeartColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);

    [Header("Animation")]
    [SerializeField] private float shakeStrength = 10f;
    [SerializeField] private float shakeDuration = 0.3f;

    [Header("Format")]
    [SerializeField] private string missesFormat = "♥ {0} / {1}";

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
            gameModeController.OnMissesChanged += UpdateMisses;
            gameModeController.OnGameStart += HandleGameStart;
        }
    }

    private void OnDestroy()
    {
        if (gameModeController != null)
        {
            gameModeController.OnMissesChanged -= UpdateMisses;
            gameModeController.OnGameStart -= HandleGameStart;
        }
    }

    private void HandleGameStart()
    {
        if (gameModeController != null && gameModeController.CurrentMode == GameMode.Infinite)
        {
            Show();
            UpdateDisplay(0, gameModeController.MaxMisses);
        }
        else
        {
            Hide();
        }
    }

    private void UpdateMisses(int current, int max)
    {
        UpdateDisplay(current, max);
        AnimateShake();
    }

    private void UpdateDisplay(int current, int max)
    {
        int remaining = max - current;

        if (useHearts && heartIcons != null && heartIcons.Length > 0)
        {
            for (int i = 0; i < heartIcons.Length; i++)
            {
                if (heartIcons[i] != null)
                {
                    heartIcons[i].color = i < remaining ? fullHeartColor : emptyHeartColor;
                }
            }
        }
        else if (missesText != null)
        {
            missesText.text = string.Format(missesFormat, remaining, max);
        }
    }

    private void AnimateShake()
    {
        if (missesText != null)
        {
            missesText.transform.DOKill();
            missesText.transform.DOShakePosition(shakeDuration, shakeStrength, 20, 90f, false, true);
        }

        if (useHearts && heartIcons != null)
        {
            foreach (var heart in heartIcons)
            {
                if (heart != null)
                {
                    heart.transform.DOKill();
                    heart.transform.DOShakePosition(shakeDuration, shakeStrength, 20, 90f, false, true);
                }
            }
        }
    }

    public void Show()
    {
        if (missesText != null)
        {
            missesText.gameObject.SetActive(true);
        }

        if (heartIcons != null)
        {
            foreach (var heart in heartIcons)
            {
                if (heart != null)
                {
                    heart.gameObject.SetActive(useHearts);
                }
            }
        }
    }

    public void Hide()
    {
        if (missesText != null)
        {
            missesText.gameObject.SetActive(false);
        }

        if (heartIcons != null)
        {
            foreach (var heart in heartIcons)
            {
                if (heart != null)
                {
                    heart.gameObject.SetActive(false);
                }
            }
        }
    }

    public void SetMissesText(TextMeshProUGUI text)
    {
        missesText = text;
    }
}
