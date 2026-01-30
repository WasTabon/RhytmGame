using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class FinalPolishSetupWindow : EditorWindow
{
    private Canvas targetCanvas;
    private bool isGameScene = true;

    private Color panelColor = new Color(0f, 0f, 0f, 0.9f);
    private Color buttonColor = new Color(0.25f, 0.25f, 0.35f, 1f);

    [MenuItem("RhythmGame/Setup Final Polish")]
    public static void ShowWindow()
    {
        GetWindow<FinalPolishSetupWindow>("Final Polish Setup");
    }

    private void OnGUI()
    {
        GUILayout.Label("Final Polish Setup", EditorStyles.boldLabel);
        GUILayout.Space(10);

        targetCanvas = (Canvas)EditorGUILayout.ObjectField("Target Canvas", targetCanvas, typeof(Canvas), true);
        isGameScene = EditorGUILayout.Toggle("Is Game Scene", isGameScene);

        GUILayout.Space(20);

        if (isGameScene)
        {
            GUILayout.Label("Game Scene Setup:", EditorStyles.boldLabel);
            
            EditorGUI.BeginDisabledGroup(targetCanvas == null);
            if (GUILayout.Button("Create Pause Menu", GUILayout.Height(35)))
            {
                CreatePauseMenu();
            }

            if (GUILayout.Button("Create Pause Button (Mobile)", GUILayout.Height(30)))
            {
                CreatePauseButton();
            }
            EditorGUI.EndDisabledGroup();
        }
        else
        {
            GUILayout.Label("Main Menu Setup:", EditorStyles.boldLabel);

            EditorGUI.BeginDisabledGroup(targetCanvas == null);
            if (GUILayout.Button("Add Settings Sliders", GUILayout.Height(30)))
            {
                AddSettingsSliders();
            }

            if (GUILayout.Button("Add Achievements Button", GUILayout.Height(30)))
            {
                AddAchievementsButton();
            }
            EditorGUI.EndDisabledGroup();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Create Sound Manager", GUILayout.Height(30)))
        {
            CreateSoundManager();
        }

        GUILayout.Space(10);

        if (targetCanvas == null)
        {
            EditorGUILayout.HelpBox("Drag a Canvas to create UI elements", MessageType.Info);
        }
    }

    private void CreatePauseMenu()
    {
        GameObject panel = new GameObject("PausePanel", typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
        panel.transform.SetParent(targetCanvas.transform, false);

        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image panelImage = panel.GetComponent<Image>();
        panelImage.color = panelColor;

        GameObject titleGO = CreateText(panel.transform, "TitleText", "PAUSED", 64, new Vector2(0, 250));
        titleGO.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        GameObject musicLabelGO = CreateText(panel.transform, "MusicLabel", "Music", 28, new Vector2(-200, 100));
        musicLabelGO.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Right;
        RectTransform musicLabelRect = musicLabelGO.GetComponent<RectTransform>();
        musicLabelRect.sizeDelta = new Vector2(150, 40);

        GameObject musicSliderGO = CreateSlider(panel.transform, "MusicSlider", new Vector2(50, 100));
        
        GameObject musicValueGO = CreateText(panel.transform, "MusicValue", "100%", 24, new Vector2(250, 100));
        RectTransform musicValueRect = musicValueGO.GetComponent<RectTransform>();
        musicValueRect.sizeDelta = new Vector2(80, 40);

        GameObject sfxLabelGO = CreateText(panel.transform, "SFXLabel", "SFX", 28, new Vector2(-200, 30));
        sfxLabelGO.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Right;
        RectTransform sfxLabelRect = sfxLabelGO.GetComponent<RectTransform>();
        sfxLabelRect.sizeDelta = new Vector2(150, 40);

        GameObject sfxSliderGO = CreateSlider(panel.transform, "SFXSlider", new Vector2(50, 30));
        
        GameObject sfxValueGO = CreateText(panel.transform, "SFXValue", "100%", 24, new Vector2(250, 30));
        RectTransform sfxValueRect = sfxValueGO.GetComponent<RectTransform>();
        sfxValueRect.sizeDelta = new Vector2(80, 40);

        Button resumeButton = CreateButton(panel.transform, "ResumeButton", "RESUME", new Vector2(0, -80));
        Button restartButton = CreateButton(panel.transform, "RestartButton", "RESTART", new Vector2(0, -160));
        Button menuButton = CreateButton(panel.transform, "MenuButton", "MENU", new Vector2(0, -240));

        GameObject pauseMenuGO = new GameObject("PauseMenu");
        PauseMenu pauseMenu = pauseMenuGO.AddComponent<PauseMenu>();

        SetPrivateField(pauseMenu, "pausePanel", panel);
        SetPrivateField(pauseMenu, "canvasGroup", panel.GetComponent<CanvasGroup>());
        SetPrivateField(pauseMenu, "panelRect", panelRect);
        SetPrivateField(pauseMenu, "resumeButton", resumeButton);
        SetPrivateField(pauseMenu, "restartButton", restartButton);
        SetPrivateField(pauseMenu, "menuButton", menuButton);
        SetPrivateField(pauseMenu, "musicSlider", musicSliderGO.GetComponent<Slider>());
        SetPrivateField(pauseMenu, "sfxSlider", sfxSliderGO.GetComponent<Slider>());
        SetPrivateField(pauseMenu, "musicValueText", musicValueGO.GetComponent<TextMeshProUGUI>());
        SetPrivateField(pauseMenu, "sfxValueText", sfxValueGO.GetComponent<TextMeshProUGUI>());

        Undo.RegisterCreatedObjectUndo(pauseMenuGO, "Create PauseMenu");
        Undo.RegisterCreatedObjectUndo(panel, "Create PausePanel");

        Debug.Log("Pause Menu created!");
    }

    private void CreatePauseButton()
    {
        GameObject buttonGO = new GameObject("PauseButton", typeof(RectTransform), typeof(Image), typeof(Button), typeof(PauseButton));
        buttonGO.transform.SetParent(targetCanvas.transform, false);

        RectTransform buttonRect = buttonGO.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(1, 1);
        buttonRect.anchorMax = new Vector2(1, 1);
        buttonRect.pivot = new Vector2(1, 1);
        buttonRect.anchoredPosition = new Vector2(-20, -20);
        buttonRect.sizeDelta = new Vector2(60, 60);

        Image buttonImage = buttonGO.GetComponent<Image>();
        buttonImage.color = new Color(0.3f, 0.3f, 0.4f, 0.8f);

        GameObject iconGO = new GameObject("Icon", typeof(RectTransform), typeof(Image));
        iconGO.transform.SetParent(buttonGO.transform, false);

        RectTransform iconRect = iconGO.GetComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0.2f, 0.2f);
        iconRect.anchorMax = new Vector2(0.8f, 0.8f);
        iconRect.offsetMin = Vector2.zero;
        iconRect.offsetMax = Vector2.zero;

        Image iconImage = iconGO.GetComponent<Image>();
        iconImage.color = Color.white;

        PauseButton pauseButton = buttonGO.GetComponent<PauseButton>();
        SetPrivateField(pauseButton, "button", buttonGO.GetComponent<Button>());
        SetPrivateField(pauseButton, "iconImage", iconImage);

        Undo.RegisterCreatedObjectUndo(buttonGO, "Create PauseButton");

        Debug.Log("Pause Button created! You may want to assign a pause icon sprite.");
    }

    private void AddSettingsSliders()
    {
        Transform settingsPanel = null;
        foreach (Transform child in targetCanvas.transform)
        {
            if (child.name.Contains("Settings"))
            {
                settingsPanel = child;
                break;
            }
        }

        if (settingsPanel == null)
        {
            Debug.LogWarning("Settings Panel not found in Canvas!");
            return;
        }

        GameObject musicLabelGO = CreateText(settingsPanel, "MusicLabel", "Music", 32, new Vector2(-150, 50));
        musicLabelGO.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Right;
        RectTransform musicLabelRect = musicLabelGO.GetComponent<RectTransform>();
        musicLabelRect.sizeDelta = new Vector2(150, 50);

        GameObject musicSliderGO = CreateSlider(settingsPanel, "MusicSlider", new Vector2(100, 50));

        GameObject sfxLabelGO = CreateText(settingsPanel, "SFXLabel", "SFX", 32, new Vector2(-150, -30));
        sfxLabelGO.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Right;
        RectTransform sfxLabelRect = sfxLabelGO.GetComponent<RectTransform>();
        sfxLabelRect.sizeDelta = new Vector2(150, 50);

        GameObject sfxSliderGO = CreateSlider(settingsPanel, "SFXSlider", new Vector2(100, -30));

        MainMenuController mainMenu = FindObjectOfType<MainMenuController>();
        if (mainMenu != null)
        {
            SetPrivateField(mainMenu, "musicSlider", musicSliderGO.GetComponent<Slider>());
            SetPrivateField(mainMenu, "sfxSlider", sfxSliderGO.GetComponent<Slider>());
            EditorUtility.SetDirty(mainMenu);
        }

        Debug.Log("Settings sliders added!");
    }

    private void AddAchievementsButton()
    {
        Transform settingsPanel = null;
        foreach (Transform child in targetCanvas.transform)
        {
            if (child.name.Contains("Settings"))
            {
                settingsPanel = child;
                break;
            }
        }

        if (settingsPanel == null)
        {
            Debug.LogWarning("Settings Panel not found in Canvas!");
            return;
        }

        Button achievementsButton = CreateButton(settingsPanel, "AchievementsButton", "ACHIEVEMENTS", new Vector2(0, -120));

        MainMenuController mainMenu = FindObjectOfType<MainMenuController>();
        if (mainMenu != null)
        {
            SetPrivateField(mainMenu, "achievementsButton", achievementsButton);
            EditorUtility.SetDirty(mainMenu);
        }

        Debug.Log("Achievements button added to Settings panel!");
    }

    private void CreateSoundManager()
    {
        if (FindObjectOfType<SoundManager>() != null)
        {
            Debug.Log("SoundManager already exists!");
            return;
        }

        GameObject soundManagerGO = new GameObject("SoundManager");
        SoundManager soundManager = soundManagerGO.AddComponent<SoundManager>();

        GameObject sfxSourceGO = new GameObject("SFXSource");
        sfxSourceGO.transform.SetParent(soundManagerGO.transform);
        AudioSource sfxSource = sfxSourceGO.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;

        SetPrivateField(soundManager, "sfxSource", sfxSource);

        Undo.RegisterCreatedObjectUndo(soundManagerGO, "Create SoundManager");

        Debug.Log("SoundManager created! Assign AudioClips for sound effects in the inspector.");
    }

    private GameObject CreateText(Transform parent, string name, string text, int fontSize, Vector2 position)
    {
        GameObject textGO = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        textGO.transform.SetParent(parent, false);

        RectTransform rect = textGO.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(400, fontSize + 20);

        TextMeshProUGUI tmp = textGO.GetComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;

        return textGO;
    }

    private Button CreateButton(Transform parent, string name, string text, Vector2 position)
    {
        GameObject buttonGO = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        buttonGO.transform.SetParent(parent, false);

        RectTransform buttonRect = buttonGO.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.pivot = new Vector2(0.5f, 0.5f);
        buttonRect.anchoredPosition = position;
        buttonRect.sizeDelta = new Vector2(300, 60);

        Image buttonImage = buttonGO.GetComponent<Image>();
        buttonImage.color = buttonColor;

        GameObject textGO = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        textGO.transform.SetParent(buttonGO.transform, false);

        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        TextMeshProUGUI tmp = textGO.GetComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 32;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;

        return buttonGO.GetComponent<Button>();
    }

    private GameObject CreateSlider(Transform parent, string name, Vector2 position)
    {
        GameObject sliderGO = new GameObject(name, typeof(RectTransform), typeof(Slider));
        sliderGO.transform.SetParent(parent, false);

        RectTransform sliderRect = sliderGO.GetComponent<RectTransform>();
        sliderRect.anchorMin = new Vector2(0.5f, 0.5f);
        sliderRect.anchorMax = new Vector2(0.5f, 0.5f);
        sliderRect.pivot = new Vector2(0.5f, 0.5f);
        sliderRect.anchoredPosition = position;
        sliderRect.sizeDelta = new Vector2(250, 30);

        GameObject bgGO = new GameObject("Background", typeof(RectTransform), typeof(Image));
        bgGO.transform.SetParent(sliderGO.transform, false);

        RectTransform bgRect = bgGO.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        Image bgImage = bgGO.GetComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.25f, 1f);

        GameObject fillAreaGO = new GameObject("Fill Area", typeof(RectTransform));
        fillAreaGO.transform.SetParent(sliderGO.transform, false);

        RectTransform fillAreaRect = fillAreaGO.GetComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.offsetMin = new Vector2(5, 5);
        fillAreaRect.offsetMax = new Vector2(-5, -5);

        GameObject fillGO = new GameObject("Fill", typeof(RectTransform), typeof(Image));
        fillGO.transform.SetParent(fillAreaGO.transform, false);

        RectTransform fillRect = fillGO.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        Image fillImage = fillGO.GetComponent<Image>();
        fillImage.color = new Color(0.4f, 0.6f, 1f, 1f);

        Slider slider = sliderGO.GetComponent<Slider>();
        slider.fillRect = fillRect;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 1f;

        return sliderGO;
    }

    private void SetPrivateField(object target, string fieldName, object value)
    {
        var field = target.GetType().GetField(fieldName,
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance);

        if (field != null)
        {
            field.SetValue(target, value);
            if (target is Object unityObj)
            {
                EditorUtility.SetDirty(unityObj);
            }
        }
    }
}
