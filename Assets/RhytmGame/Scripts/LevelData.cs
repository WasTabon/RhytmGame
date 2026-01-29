using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "RhythmGame/Level Data")]
public class LevelData : ScriptableObject
{
    [SerializeField] private LevelInfo[] levels;

    public LevelInfo[] Levels => levels;
    public int LevelCount => levels != null ? levels.Length : 0;

    public LevelInfo GetLevel(int index)
    {
        if (levels == null || levels.Length == 0)
            return null;

        index = Mathf.Clamp(index, 0, levels.Length - 1);
        return levels[index];
    }
}

[System.Serializable]
public class LevelInfo
{
    public string levelName = "Level 1";
    public int shapesToComplete = 3;
}
