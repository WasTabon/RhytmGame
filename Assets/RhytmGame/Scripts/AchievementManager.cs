using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance { get; private set; }

    [Header("Data")]
    [SerializeField] private AchievementData achievementData;

    [Header("References")]
    [SerializeField] private StatsManager statsManager;
    [SerializeField] private LevelData levelData;

    [Header("Debug (Read Only)")]
    [SerializeField] private int unlockedCount;
    [SerializeField] private int totalStars;

    public event Action<AchievementInfo> OnAchievementUnlocked;

    public int UnlockedCount => unlockedCount;
    public int TotalStars => totalStars;
    public int TotalAchievements => achievementData != null ? achievementData.AchievementCount : 0;

    private HashSet<string> unlockedAchievements = new HashSet<string>();
    private bool firstPerfectTriggered;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadUnlockedAchievements();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UnsubscribeFromEvents();
        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        if (statsManager == null)
            statsManager = StatsManager.Instance;

        if (statsManager != null)
        {
            statsManager.OnStatChanged -= CheckAchievements;
            statsManager.OnStatChanged += CheckAchievements;
        }

        if (LockMechanic.Instance != null)
        {
            LockMechanic.Instance.OnLock -= HandleLock;
            LockMechanic.Instance.OnLock += HandleLock;
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (statsManager != null)
        {
            statsManager.OnStatChanged -= CheckAchievements;
        }

        if (LockMechanic.Instance != null)
        {
            LockMechanic.Instance.OnLock -= HandleLock;
        }
    }

    public void LateSubscribe()
    {
        UnsubscribeFromEvents();
        SubscribeToEvents();
    }

    private void HandleLock(LockResult result)
    {
        if (result == LockResult.Perfect && !firstPerfectTriggered)
        {
            firstPerfectTriggered = true;
            CheckAchievements("FirstPerfect", 1);
        }
    }

    private void CheckAchievements(string statName, int value)
    {
        if (achievementData == null)
            return;

        foreach (var achievement in achievementData.Achievements)
        {
            if (IsUnlocked(achievement.id))
                continue;

            bool shouldUnlock = CheckCondition(achievement, statName, value);

            if (shouldUnlock)
            {
                UnlockAchievement(achievement);
            }
        }
    }

    private bool CheckCondition(AchievementInfo achievement, string statName, int value)
    {
        switch (achievement.condition)
        {
            case AchievementCondition.FirstPerfect:
                return statName == "FirstPerfect";

            case AchievementCondition.FirstLevelComplete:
                return statName == "LevelsCompleted" && value >= 1;

            case AchievementCondition.PerfectsInRow:
                return statName == "PerfectsInRow" && value >= achievement.targetValue;

            case AchievementCondition.ComboReached:
                return statName == "ComboReached" && value >= achievement.targetValue;

            case AchievementCondition.TotalShapesCompleted:
                return statName == "TotalShapesCompleted" && value >= achievement.targetValue;

            case AchievementCondition.TotalScoreReached:
                return statName == "TotalScore" && value >= achievement.targetValue;

            case AchievementCondition.TotalPerfects:
                return statName == "TotalPerfects" && value >= achievement.targetValue;

            case AchievementCondition.LevelsCompleted:
                return statName == "LevelsCompleted" && value >= achievement.targetValue;

            case AchievementCondition.AllLevelsCompleted:
                if (statName != "LevelsCompleted")
                    return false;
                int totalLevels = levelData != null ? levelData.LevelCount : 30;
                int unlockedLevels = PlayerPrefs.GetInt("UnlockedLevel", 0) + 1;
                return unlockedLevels >= totalLevels;

            case AchievementCondition.PerfectAccuracyOnLevel:
                return statName == "PerfectAccuracyOnLevel";

            case AchievementCondition.NoMissesOnLevel:
                return statName == "NoMissesOnLevel";

            case AchievementCondition.PlayTimeMinutes:
                return statName == "PlayTimeMinutes" && value >= achievement.targetValue;

            default:
                return false;
        }
    }

    private void UnlockAchievement(AchievementInfo achievement)
    {
        if (unlockedAchievements.Contains(achievement.id))
            return;

        unlockedAchievements.Add(achievement.id);
        unlockedCount++;
        totalStars += achievement.starReward;

        SaveUnlockedAchievements();

        Debug.Log($"[Achievement] Unlocked: {achievement.title}");
        OnAchievementUnlocked?.Invoke(achievement);
    }

    public bool IsUnlocked(string achievementId)
    {
        return unlockedAchievements.Contains(achievementId);
    }

    public List<AchievementInfo> GetAllAchievements()
    {
        if (achievementData == null)
            return new List<AchievementInfo>();

        return new List<AchievementInfo>(achievementData.Achievements);
    }

    public List<AchievementInfo> GetUnlockedAchievements()
    {
        var list = new List<AchievementInfo>();
        if (achievementData == null)
            return list;

        foreach (var achievement in achievementData.Achievements)
        {
            if (IsUnlocked(achievement.id))
            {
                list.Add(achievement);
            }
        }
        return list;
    }

    public List<AchievementInfo> GetLockedAchievements()
    {
        var list = new List<AchievementInfo>();
        if (achievementData == null)
            return list;

        foreach (var achievement in achievementData.Achievements)
        {
            if (!IsUnlocked(achievement.id))
            {
                list.Add(achievement);
            }
        }
        return list;
    }

    public List<AchievementInfo> GetAchievementsByCategory(AchievementCategory category)
    {
        var list = new List<AchievementInfo>();
        if (achievementData == null)
            return list;

        foreach (var achievement in achievementData.Achievements)
        {
            if (achievement.category == category)
            {
                list.Add(achievement);
            }
        }
        return list;
    }

    private void LoadUnlockedAchievements()
    {
        unlockedAchievements.Clear();
        unlockedCount = 0;
        totalStars = 0;

        string data = PlayerPrefs.GetString("UnlockedAchievements", "");
        if (!string.IsNullOrEmpty(data))
        {
            string[] ids = data.Split(',');
            foreach (var id in ids)
            {
                if (!string.IsNullOrEmpty(id))
                {
                    unlockedAchievements.Add(id);
                    unlockedCount++;

                    if (achievementData != null)
                    {
                        var achievement = achievementData.GetAchievementById(id);
                        if (achievement != null)
                        {
                            totalStars += achievement.starReward;
                        }
                    }
                }
            }
        }
    }

    private void SaveUnlockedAchievements()
    {
        string data = string.Join(",", unlockedAchievements);
        PlayerPrefs.SetString("UnlockedAchievements", data);
        PlayerPrefs.Save();
    }

    public void SetAchievementData(AchievementData data)
    {
        achievementData = data;
        LoadUnlockedAchievements();
    }

    public static void ResetAllAchievements()
    {
        PlayerPrefs.DeleteKey("UnlockedAchievements");
        PlayerPrefs.Save();

        if (Instance != null)
        {
            Instance.LoadUnlockedAchievements();
            Instance.firstPerfectTriggered = false;
        }
    }
}
