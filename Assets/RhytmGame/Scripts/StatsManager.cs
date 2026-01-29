using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance { get; private set; }

    [Header("Session Stats (Read Only)")]
    [SerializeField] private int sessionPerfects;
    [SerializeField] private int sessionGoods;
    [SerializeField] private int sessionMisses;
    [SerializeField] private int sessionMaxCombo;
    [SerializeField] private int sessionScore;
    [SerializeField] private int sessionPerfectsInRow;

    [Header("Lifetime Stats (Read Only)")]
    [SerializeField] private int totalPerfects;
    [SerializeField] private int totalGoods;
    [SerializeField] private int totalMisses;
    [SerializeField] private int totalShapesCompleted;
    [SerializeField] private int totalScore;
    [SerializeField] private int bestComboEver;
    [SerializeField] private int levelsCompleted;
    [SerializeField] private float totalPlayTimeMinutes;

    public event Action<string, int> OnStatChanged;

    public int SessionPerfects => sessionPerfects;
    public int SessionGoods => sessionGoods;
    public int SessionMisses => sessionMisses;
    public int SessionMaxCombo => sessionMaxCombo;
    public int SessionScore => sessionScore;
    public int SessionPerfectsInRow => sessionPerfectsInRow;

    public int TotalPerfects => totalPerfects;
    public int TotalGoods => totalGoods;
    public int TotalMisses => totalMisses;
    public int TotalShapesCompleted => totalShapesCompleted;
    public int TotalScore => totalScore;
    public int BestComboEver => bestComboEver;
    public int LevelsCompleted => levelsCompleted;
    public float TotalPlayTimeMinutes => totalPlayTimeMinutes;

    private float sessionStartTime;
    private int currentPerfectsInRow;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadStats();
        sessionStartTime = Time.realtimeSinceStartup;
        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        UnsubscribeFromEvents();
        SaveStats();
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TrySubscribeToSceneEvents();
    }

    private void TrySubscribeToSceneEvents()
    {   
        UnsubscribeFromEvents();

        if (LockMechanic.Instance != null)
            LockMechanic.Instance.OnLock += HandleLock;

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged += HandleScoreChanged;
            ScoreManager.Instance.OnComboChanged += HandleComboChanged;
        }

        if (GameModeController.Instance != null)
        {
            GameModeController.Instance.OnLevelComplete += HandleLevelComplete;
            GameModeController.Instance.OnGameStart += HandleGameStart;
        }
    }


    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            UpdatePlayTime();
            SaveStats();
        }
        else
        {
            sessionStartTime = Time.realtimeSinceStartup;
        }
    }

    private void OnApplicationQuit()
    {
        UpdatePlayTime();
        SaveStats();
    }

    private void SubscribeToEvents()
    {
        if (LockMechanic.Instance != null)
            LockMechanic.Instance.OnLock += HandleLock;

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged += HandleScoreChanged;
            ScoreManager.Instance.OnComboChanged += HandleComboChanged;
        }

        if (GameModeController.Instance != null)
        {
            GameModeController.Instance.OnLevelComplete += HandleLevelComplete;
            GameModeController.Instance.OnGameStart += HandleGameStart;
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (LockMechanic.Instance != null)
        {
            LockMechanic.Instance.OnLock -= HandleLock;
        }

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged -= HandleScoreChanged;
            ScoreManager.Instance.OnComboChanged -= HandleComboChanged;
        }

        if (GameModeController.Instance != null)
        {
            GameModeController.Instance.OnLevelComplete -= HandleLevelComplete;
            GameModeController.Instance.OnGameStart -= HandleGameStart;
        }
    }

    public void LateSubscribe()
    {
        SubscribeToEvents();
    }

    private void HandleGameStart()
    {
        ResetSessionStats();
    }

    private void HandleLock(LockResult result)
    {
        switch (result)
        {
            case LockResult.Perfect:
                sessionPerfects++;
                totalPerfects++;
                totalShapesCompleted++;
                currentPerfectsInRow++;
                if (currentPerfectsInRow > sessionPerfectsInRow)
                {
                    sessionPerfectsInRow = currentPerfectsInRow;
                    OnStatChanged?.Invoke("PerfectsInRow", sessionPerfectsInRow);
                }
                OnStatChanged?.Invoke("TotalPerfects", totalPerfects);
                OnStatChanged?.Invoke("TotalShapesCompleted", totalShapesCompleted);
                break;

            case LockResult.Good:
                sessionGoods++;
                totalGoods++;
                totalShapesCompleted++;
                currentPerfectsInRow = 0;
                OnStatChanged?.Invoke("TotalShapesCompleted", totalShapesCompleted);
                break;

            case LockResult.Miss:
                sessionMisses++;
                totalMisses++;
                currentPerfectsInRow = 0;
                break;
        }
    }

    private void HandleScoreChanged(int total, int added)
    {
        sessionScore = total;
        totalScore += added;
        OnStatChanged?.Invoke("TotalScore", totalScore);
    }

    private void HandleComboChanged(int combo)
    {
        if (combo > sessionMaxCombo)
        {
            sessionMaxCombo = combo;
            OnStatChanged?.Invoke("ComboReached", sessionMaxCombo);
        }

        if (combo > bestComboEver)
        {
            bestComboEver = combo;
            OnStatChanged?.Invoke("BestComboEver", bestComboEver);
        }
    }

    private void HandleLevelComplete()
    {
        levelsCompleted++;
        OnStatChanged?.Invoke("LevelsCompleted", levelsCompleted);

        if (sessionMisses == 0)
        {
            OnStatChanged?.Invoke("NoMissesOnLevel", 1);
        }

        if (sessionMisses == 0 && sessionGoods == 0 && sessionPerfects > 0)
        {
            OnStatChanged?.Invoke("PerfectAccuracyOnLevel", 1);
        }
    }

    private void ResetSessionStats()
    {
        sessionPerfects = 0;
        sessionGoods = 0;
        sessionMisses = 0;
        sessionMaxCombo = 0;
        sessionScore = 0;
        sessionPerfectsInRow = 0;
        currentPerfectsInRow = 0;
    }

    private void UpdatePlayTime()
    {
        float sessionTime = (Time.realtimeSinceStartup - sessionStartTime) / 60f;
        totalPlayTimeMinutes += sessionTime;
        sessionStartTime = Time.realtimeSinceStartup;
        OnStatChanged?.Invoke("PlayTimeMinutes", Mathf.FloorToInt(totalPlayTimeMinutes));
    }

    private void LoadStats()
    {
        totalPerfects = PlayerPrefs.GetInt("Stats_TotalPerfects", 0);
        totalGoods = PlayerPrefs.GetInt("Stats_TotalGoods", 0);
        totalMisses = PlayerPrefs.GetInt("Stats_TotalMisses", 0);
        totalShapesCompleted = PlayerPrefs.GetInt("Stats_TotalShapesCompleted", 0);
        totalScore = PlayerPrefs.GetInt("Stats_TotalScore", 0);
        bestComboEver = PlayerPrefs.GetInt("Stats_BestComboEver", 0);
        levelsCompleted = PlayerPrefs.GetInt("Stats_LevelsCompleted", 0);
        totalPlayTimeMinutes = PlayerPrefs.GetFloat("Stats_TotalPlayTime", 0f);
    }

    private void SaveStats()
    {
        PlayerPrefs.SetInt("Stats_TotalPerfects", totalPerfects);
        PlayerPrefs.SetInt("Stats_TotalGoods", totalGoods);
        PlayerPrefs.SetInt("Stats_TotalMisses", totalMisses);
        PlayerPrefs.SetInt("Stats_TotalShapesCompleted", totalShapesCompleted);
        PlayerPrefs.SetInt("Stats_TotalScore", totalScore);
        PlayerPrefs.SetInt("Stats_BestComboEver", bestComboEver);
        PlayerPrefs.SetInt("Stats_LevelsCompleted", levelsCompleted);
        PlayerPrefs.SetFloat("Stats_TotalPlayTime", totalPlayTimeMinutes);
        PlayerPrefs.Save();
    }

    public static void ResetAllStats()
    {
        PlayerPrefs.DeleteKey("Stats_TotalPerfects");
        PlayerPrefs.DeleteKey("Stats_TotalGoods");
        PlayerPrefs.DeleteKey("Stats_TotalMisses");
        PlayerPrefs.DeleteKey("Stats_TotalShapesCompleted");
        PlayerPrefs.DeleteKey("Stats_TotalScore");
        PlayerPrefs.DeleteKey("Stats_BestComboEver");
        PlayerPrefs.DeleteKey("Stats_LevelsCompleted");
        PlayerPrefs.DeleteKey("Stats_TotalPlayTime");
        PlayerPrefs.Save();

        if (Instance != null)
        {
            Instance.LoadStats();
        }
    }
}
