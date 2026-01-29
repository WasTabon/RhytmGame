using UnityEngine;
using System;

public class GameModeController : MonoBehaviour
{
    public static GameModeController Instance { get; private set; }

    [Header("References")]
    [SerializeField] private LevelData levelData;
    [SerializeField] private LockMechanic lockMechanic;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private RoundManager roundManager;
    [SerializeField] private TimeAttackTimer timeAttackTimer;

    [Header("Time Attack Settings")]
    [SerializeField] private float timeAttackDuration = 60f;

    [Header("Infinite Mode Settings")]
    [SerializeField] private int maxMisses = 5;

    [Header("Current State (Read Only)")]
    [SerializeField] private GameMode currentMode;
    [SerializeField] private int currentLevelIndex;
    [SerializeField] private int shapesCompleted;
    [SerializeField] private int shapesRequired;
    [SerializeField] private int currentMisses;
    [SerializeField] private bool isGameActive;

    public event Action OnGameStart;
    public event Action OnGameOver;
    public event Action OnLevelComplete;
    public event Action<int, int> OnShapeProgress;
    public event Action<int, int> OnMissesChanged;

    public GameMode CurrentMode => currentMode;
    public int CurrentLevelIndex => currentLevelIndex;
    public int ShapesCompleted => shapesCompleted;
    public int ShapesRequired => shapesRequired;
    public int CurrentMisses => currentMisses;
    public int MaxMisses => maxMisses;
    public bool IsGameActive => isGameActive;

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

        if (scoreManager == null)
            scoreManager = ScoreManager.Instance;

        if (roundManager == null)
            roundManager = RoundManager.Instance;

        if (timeAttackTimer == null)
            timeAttackTimer = TimeAttackTimer.Instance;

        if (lockMechanic != null)
        {
            lockMechanic.OnLock += HandleLock;
        }

        if (timeAttackTimer != null)
        {
            timeAttackTimer.OnTimerEnd += HandleTimerEnd;
        }

        InitializeMode();
    }

    private void OnDestroy()
    {
        if (lockMechanic != null)
        {
            lockMechanic.OnLock -= HandleLock;
        }

        if (timeAttackTimer != null)
        {
            timeAttackTimer.OnTimerEnd -= HandleTimerEnd;
        }
    }

    private void InitializeMode()
    {
        if (GameManager.Instance != null)
        {
            currentMode = GameManager.Instance.CurrentMode;
            currentLevelIndex = GameManager.Instance.SelectedLevelIndex;
            timeAttackDuration = GameManager.Instance.TimeAttackDuration;
            Debug.Log($"[GameModeController] Mode: {currentMode}, Level: {currentLevelIndex}, TimeAttack Duration: {timeAttackDuration}");
        }
        else
        {
            currentMode = GameMode.Infinite;
            currentLevelIndex = 0;
            Debug.LogWarning("[GameModeController] GameManager not found! Defaulting to Infinite mode.");
        }

        StartGame();
    }

    public void StartGame()
    {
        shapesCompleted = 0;
        currentMisses = 0;
        isGameActive = true;

        if (scoreManager != null)
        {
            scoreManager.ResetStats();
        }

        switch (currentMode)
        {
            case GameMode.Infinite:
                Debug.Log($"[GameModeController] Starting INFINITE mode (max {maxMisses} misses)");
                StartInfiniteMode();
                break;

            case GameMode.TimeAttack:
                Debug.Log($"[GameModeController] Starting TIME ATTACK mode ({timeAttackDuration} sec)");
                StartTimeAttackMode();
                break;

            case GameMode.Levels:
                Debug.Log($"[GameModeController] Starting LEVELS mode (Level {currentLevelIndex + 1})");
                StartLevelMode();
                break;
        }

        OnGameStart?.Invoke();
    }

    private void StartInfiniteMode()
    {
        shapesRequired = -1;

        if (timeAttackTimer != null)
        {
            timeAttackTimer.Hide();
        }
    }

    private void StartTimeAttackMode()
    {
        shapesRequired = -1;

        if (timeAttackTimer != null)
        {
            timeAttackTimer.StartTimer(timeAttackDuration);
        }
    }

    private void StartLevelMode()
    {
        if (levelData != null && currentLevelIndex < levelData.LevelCount)
        {
            LevelInfo level = levelData.GetLevel(currentLevelIndex);
            shapesRequired = level.shapesToComplete;
        }
        else
        {
            shapesRequired = 5;
        }

        if (timeAttackTimer != null)
        {
            timeAttackTimer.Hide();
        }

        OnShapeProgress?.Invoke(shapesCompleted, shapesRequired);
    }

    private void HandleLock(LockResult result)
    {
        if (!isGameActive)
            return;

        if (result == LockResult.Perfect || result == LockResult.Good)
        {
            shapesCompleted++;

            if (currentMode == GameMode.Levels)
            {
                OnShapeProgress?.Invoke(shapesCompleted, shapesRequired);

                if (shapesCompleted >= shapesRequired)
                {
                    CompleteLevel();
                }
            }
        }
        else if (result == LockResult.Miss)
        {
            if (currentMode == GameMode.Infinite)
            {
                currentMisses++;
                OnMissesChanged?.Invoke(currentMisses, maxMisses);

                if (currentMisses >= maxMisses)
                {
                    EndGame();
                }
            }
        }
    }

    private void HandleTimerEnd()
    {
        if (currentMode == GameMode.TimeAttack)
        {
            EndGame();
        }
    }

    private void CompleteLevel()
    {
        isGameActive = false;

        if (lockMechanic != null)
        {
            lockMechanic.SetCanLock(false);
        }

        SaveLevelProgress();

        OnLevelComplete?.Invoke();
    }

    private void EndGame()
    {
        isGameActive = false;

        if (lockMechanic != null)
        {
            lockMechanic.SetCanLock(false);
        }

        OnGameOver?.Invoke();
    }

    private void SaveLevelProgress()
    {
        if (currentMode != GameMode.Levels)
            return;

        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 0);
        if (currentLevelIndex >= unlockedLevel)
        {
            PlayerPrefs.SetInt("UnlockedLevel", currentLevelIndex + 1);
            PlayerPrefs.Save();
        }
    }

    public void RestartGame()
    {
        StartGame();

        if (roundManager != null)
        {
            roundManager.StartNewRound();
        }
    }

    public void SetTimeAttackDuration(float duration)
    {
        timeAttackDuration = duration;
    }

    public void SetLevelData(LevelData data)
    {
        levelData = data;
    }

    public void SetCurrentLevel(int index)
    {
        currentLevelIndex = index;
    }

    public string GetCurrentLevelName()
    {
        if (levelData != null && currentLevelIndex < levelData.LevelCount)
        {
            return levelData.GetLevel(currentLevelIndex).levelName;
        }
        return $"Level {currentLevelIndex + 1}";
    }

    public static int GetUnlockedLevelCount()
    {
        return PlayerPrefs.GetInt("UnlockedLevel", 0) + 1;
    }
}
