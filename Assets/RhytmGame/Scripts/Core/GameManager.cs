using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameMode { None, LevelMode, InfiniteMode, TimeAttack }
    public enum GameState { Menu, Playing, Paused, GameOver, Results }

    public GameMode CurrentMode { get; private set; } = GameMode.None;
    public GameState CurrentState { get; private set; } = GameState.Menu;

    public event Action<GameState> OnGameStateChanged;
    public event Action<GameMode> OnGameModeChanged;

    [Header("Scene Names")]
    [SerializeField] private string mainMenuScene = "MainMenu";
    [SerializeField] private string gameScene = "Game";

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
        OnGameModeChanged?.Invoke(mode);
    }

    public void SetGameState(GameState state)
    {
        CurrentState = state;
        OnGameStateChanged?.Invoke(state);
    }

    public void StartGame(GameMode mode)
    {
        SetGameMode(mode);
        SceneTransition.Instance.LoadScene(gameScene);
    }

    public void GoToMainMenu()
    {
        SetGameState(GameState.Menu);
        SetGameMode(GameMode.None);
        SceneTransition.Instance.LoadScene(mainMenuScene);
    }

    public void PauseGame()
    {
        if (CurrentState == GameState.Playing)
        {
            SetGameState(GameState.Paused);
            Time.timeScale = 0f;
        }
    }

    public void ResumeGame()
    {
        if (CurrentState == GameState.Paused)
        {
            SetGameState(GameState.Playing);
            Time.timeScale = 1f;
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneTransition.Instance.LoadScene(gameScene);
    }
}
