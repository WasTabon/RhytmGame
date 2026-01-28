using UnityEngine;
using TMPro;
using DG.Tweening;

public class GameHUD : MonoBehaviour
{
    public static GameHUD Instance { get; private set; }

    [Header("Text References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private TextMeshProUGUI roundText;

    [Header("Score Animation")]
    [SerializeField] private float scorePunchScale = 1.2f;
    [SerializeField] private float scorePunchDuration = 0.2f;

    [Header("Combo Animation")]
    [SerializeField] private float comboPunchScale = 1.3f;
    [SerializeField] private float comboPunchDuration = 0.15f;
    [SerializeField] private float comboShakeDuration = 0.3f;
    [SerializeField] private float comboShakeStrength = 10f;

    [Header("Combo Colors")]
    [SerializeField] private Color comboColor0 = Color.white;
    [SerializeField] private Color comboColor5 = new Color(0.3f, 1f, 0.3f);
    [SerializeField] private Color comboColor10 = new Color(1f, 1f, 0.3f);
    [SerializeField] private Color comboColor20 = new Color(1f, 0.5f, 0f);
    [SerializeField] private Color comboColor50 = new Color(1f, 0.3f, 0.3f);

    [Header("Format")]
    [SerializeField] private string scoreFormat = "SCORE: {0}";
    [SerializeField] private string comboFormat = "COMBO: x{0}";
    [SerializeField] private string roundFormat = "ROUND: {0}";

    private ScoreManager scoreManager;
    private RoundManager roundManager;

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
        scoreManager = ScoreManager.Instance;
        roundManager = RoundManager.Instance;

        if (scoreManager != null)
        {
            scoreManager.OnScoreChanged += UpdateScore;
            scoreManager.OnComboChanged += UpdateCombo;
            scoreManager.OnComboReset += OnComboReset;
        }

        if (roundManager != null)
        {
            roundManager.OnRoundStart += UpdateRound;
        }

        UpdateScoreText(0);
        UpdateComboText(0);
        UpdateRoundText(1);
    }

    private void OnDestroy()
    {
        if (scoreManager != null)
        {
            scoreManager.OnScoreChanged -= UpdateScore;
            scoreManager.OnComboChanged -= UpdateCombo;
            scoreManager.OnComboReset -= OnComboReset;
        }

        if (roundManager != null)
        {
            roundManager.OnRoundStart -= UpdateRound;
        }
    }

    private void UpdateScore(int totalScore, int addedScore)
    {
        UpdateScoreText(totalScore);
        AnimateScorePunch();
    }

    private void UpdateCombo(int combo)
    {
        UpdateComboText(combo);
        UpdateComboColor(combo);
        
        if (combo > 0)
        {
            AnimateComboPunch();
        }
    }

    private void UpdateRound(int round)
    {
        UpdateRoundText(round);
    }

    private void OnComboReset()
    {
        AnimateComboShake();
    }

    private void UpdateScoreText(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = string.Format(scoreFormat, score);
        }
    }

    private void UpdateComboText(int combo)
    {
        if (comboText != null)
        {
            if (combo > 0)
            {
                comboText.text = string.Format(comboFormat, combo);
                comboText.gameObject.SetActive(true);
            }
            else
            {
                comboText.gameObject.SetActive(false);
            }
        }
    }

    private void UpdateRoundText(int round)
    {
        if (roundText != null)
        {
            roundText.text = string.Format(roundFormat, round);
        }
    }

    private void UpdateComboColor(int combo)
    {
        if (comboText == null) return;

        Color targetColor;

        if (combo >= 50)
            targetColor = comboColor50;
        else if (combo >= 20)
            targetColor = comboColor20;
        else if (combo >= 10)
            targetColor = comboColor10;
        else if (combo >= 5)
            targetColor = comboColor5;
        else
            targetColor = comboColor0;

        comboText.color = targetColor;
    }

    private void AnimateScorePunch()
    {
        if (scoreText == null) return;

        scoreText.transform.DOKill();
        scoreText.transform.localScale = Vector3.one;
        scoreText.transform.DOPunchScale(Vector3.one * (scorePunchScale - 1f), scorePunchDuration, 5, 0.5f);
    }

    private void AnimateComboPunch()
    {
        if (comboText == null) return;

        comboText.transform.DOKill();
        comboText.transform.localScale = Vector3.one;
        comboText.transform.DOPunchScale(Vector3.one * (comboPunchScale - 1f), comboPunchDuration, 5, 0.5f);
    }

    private void AnimateComboShake()
    {
        if (comboText == null) return;

        comboText.transform.DOKill();
        comboText.transform.localScale = Vector3.one;
        comboText.transform.DOShakePosition(comboShakeDuration, comboShakeStrength, 20, 90f, false, true);
    }

    public void SetReferences(TextMeshProUGUI score, TextMeshProUGUI combo, TextMeshProUGUI round)
    {
        scoreText = score;
        comboText = combo;
        roundText = round;
    }
}
