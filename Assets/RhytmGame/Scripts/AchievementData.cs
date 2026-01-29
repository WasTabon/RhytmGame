using UnityEngine;

public enum AchievementCategory
{
    Beginner,
    Skill,
    Endurance,
    Mastery
}

public enum AchievementCondition
{
    FirstPerfect,
    FirstLevelComplete,
    PerfectsInRow,
    ComboReached,
    TotalShapesCompleted,
    TotalScoreReached,
    TotalPerfects,
    LevelsCompleted,
    AllLevelsCompleted,
    PerfectAccuracyOnLevel,
    NoMissesOnLevel,
    PlayTimeMinutes
}

[CreateAssetMenu(fileName = "AchievementData", menuName = "RhythmGame/Achievement Data")]
public class AchievementData : ScriptableObject
{
    [SerializeField] private AchievementInfo[] achievements;

    public AchievementInfo[] Achievements => achievements;
    public int AchievementCount => achievements != null ? achievements.Length : 0;

    public AchievementInfo GetAchievement(int index)
    {
        if (achievements == null || index < 0 || index >= achievements.Length)
            return null;
        return achievements[index];
    }

    public AchievementInfo GetAchievementById(string id)
    {
        if (achievements == null)
            return null;

        foreach (var achievement in achievements)
        {
            if (achievement.id == id)
                return achievement;
        }
        return null;
    }
}

[System.Serializable]
public class AchievementInfo
{
    public string id;
    public string title;
    [TextArea(2, 4)]
    public string description;
    public AchievementCategory category;
    public AchievementCondition condition;
    public int targetValue;
    public int starReward = 1;
    public Sprite icon;
}
