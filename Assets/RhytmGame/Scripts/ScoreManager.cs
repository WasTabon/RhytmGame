using UnityEngine;
using System;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Score Settings")]
    [SerializeField] private int perfectScore = 100;
    [SerializeField] private int goodScore = 50;
    [SerializeField] private float comboMultiplierStep = 0.1f;

    [Header("References")]
    [SerializeField] private LockMechanic lockMechanic;

    [Header("Current State (Read Only)")]
    [SerializeField] private int currentScore;
    [SerializeField] private int currentCombo;
    [SerializeField] private int maxCombo;
    [SerializeField] private int perfectCount;
    [SerializeField] private int goodCount;
    [SerializeField] private int missCount;

    public event Action<int, int> OnScoreChanged;
    public event Action<int> OnComboChanged;
    public event Action OnComboReset;

    public int CurrentScore => currentScore;
    public int CurrentCombo => currentCombo;
    public int MaxCombo => maxCombo;
    public int PerfectCount => perfectCount;
    public int GoodCount => goodCount;
    public int MissCount => missCount;

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
        if (lockMechanic == null)
            lockMechanic = LockMechanic.Instance;

        if (lockMechanic != null)
        {
            lockMechanic.OnLock += HandleLock;
        }

        ResetStats();
    }

    private void OnDestroy()
    {
        if (lockMechanic != null)
        {
            lockMechanic.OnLock -= HandleLock;
        }
    }

    private void HandleLock(LockResult result)
    {
        int scoreToAdd = 0;

        switch (result)
        {
            case LockResult.Perfect:
                perfectCount++;
                currentCombo++;
                scoreToAdd = CalculateScore(perfectScore);
                break;

            case LockResult.Good:
                goodCount++;
                currentCombo++;
                scoreToAdd = CalculateScore(goodScore);
                break;

            case LockResult.Miss:
                missCount++;
                if (currentCombo > 0)
                {
                    currentCombo = 0;
                    OnComboReset?.Invoke();
                }
                break;
        }

        if (currentCombo > maxCombo)
        {
            maxCombo = currentCombo;
        }

        if (scoreToAdd > 0)
        {
            currentScore += scoreToAdd;
            OnScoreChanged?.Invoke(currentScore, scoreToAdd);
        }

        OnComboChanged?.Invoke(currentCombo);
    }

    private int CalculateScore(int baseScore)
    {
        float multiplier = 1f + (currentCombo * comboMultiplierStep);
        return Mathf.RoundToInt(baseScore * multiplier);
    }

    public void ResetStats()
    {
        currentScore = 0;
        currentCombo = 0;
        maxCombo = 0;
        perfectCount = 0;
        goodCount = 0;
        missCount = 0;

        OnScoreChanged?.Invoke(currentScore, 0);
        OnComboChanged?.Invoke(currentCombo);
    }

    public float GetCurrentMultiplier()
    {
        return 1f + (currentCombo * comboMultiplierStep);
    }

    public int GetTotalAttempts()
    {
        return perfectCount + goodCount + missCount;
    }

    public float GetAccuracy()
    {
        int total = GetTotalAttempts();
        if (total == 0) return 0f;
        return (float)(perfectCount + goodCount) / total * 100f;
    }
}
