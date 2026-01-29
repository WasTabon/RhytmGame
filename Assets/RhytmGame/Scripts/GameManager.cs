using UnityEngine;

public enum GameMode
{
    Levels,
    Infinite,
    TimeAttack
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public GameMode CurrentMode { get; private set; }
    public int SelectedLevelIndex { get; private set; }
    public float TimeAttackDuration { get; private set; } = 60f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetGameMode(GameMode mode)
    {
        CurrentMode = mode;
    }

    public void SetSelectedLevel(int index)
    {
        SelectedLevelIndex = index;
    }

    public void SetTimeAttackDuration(float duration)
    {
        TimeAttackDuration = duration;
    }
}
