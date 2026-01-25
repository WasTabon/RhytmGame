using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] private MainMenuScreen mainMenuScreen;
    [SerializeField] private SettingsScreen settingsScreen;
    [SerializeField] private LevelSelectScreen levelSelectScreen;
    [SerializeField] private AchievementsScreen achievementsScreen;

    private ScreenBase currentScreen;

    private void Start()
    {
        InitializeScreens();
        ShowMainMenu(immediate: true);
    }

    private void InitializeScreens()
    {
        mainMenuScreen?.Initialize(this);
        settingsScreen?.Initialize(this);
        levelSelectScreen?.Initialize(this);
        achievementsScreen?.Initialize(this);

        settingsScreen?.SetVisibleImmediate(false);
        levelSelectScreen?.SetVisibleImmediate(false);
        achievementsScreen?.SetVisibleImmediate(false);
    }

    public void ShowMainMenu(bool immediate = false)
    {
        SwitchScreen(mainMenuScreen, immediate);
    }

    public void ShowSettings()
    {
        SwitchScreen(settingsScreen);
    }

    public void ShowLevelSelect()
    {
        SwitchScreen(levelSelectScreen);
    }

    public void ShowAchievements()
    {
        SwitchScreen(achievementsScreen);
    }

    private void SwitchScreen(ScreenBase newScreen, bool immediate = false)
    {
        if (newScreen == null || newScreen == currentScreen) return;

        if (immediate)
        {
            currentScreen?.SetVisibleImmediate(false);
            newScreen.SetVisibleImmediate(true);
            currentScreen = newScreen;
        }
        else
        {
            var previousScreen = currentScreen;
            currentScreen = newScreen;

            if (previousScreen != null && previousScreen.IsVisible)
            {
                previousScreen.Hide();
            }

            newScreen.Show();
        }
    }
}
