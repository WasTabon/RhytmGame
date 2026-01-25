using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using TMPro;
using System.IO;

public class Iteration01_SetupEditor : EditorWindow
{
    private static Color backgroundColor = new Color(0.1f, 0.1f, 0.12f, 1f);
    private static Color buttonColor = new Color(0.2f, 0.2f, 0.25f, 1f);
    private static Color buttonTextColor = Color.white;
    private static Color titleColor = Color.white;
    private static Color sliderFillColor = new Color(0.4f, 0.6f, 1f, 1f);
    private static Color toggleOnColor = new Color(0.4f, 0.8f, 0.4f, 1f);

    private static RectTransform GetOrAddRectTransform(GameObject go)
    {
        var rect = go.GetComponent<RectTransform>();
        if (rect == null)
            rect = go.AddComponent<RectTransform>();
        return rect;
    }

    private static GameObject CreateUIObject(string name, Transform parent)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        return go;
    }

    [MenuItem("Starlock/Iteration 01 - Setup All")]
    public static void SetupAll()
    {
        if (!EditorUtility.DisplayDialog("Setup Iteration 01",
            "This will create:\n" +
            "- Folder structure\n" +
            "- Manager prefabs\n" +
            "- MainMenu scene with all UI\n" +
            "- Game scene (placeholder)\n\n" +
            "Continue?", "Yes", "Cancel"))
        {
            return;
        }

        CreateFolderStructure();
        CreateManagerPrefabs();
        CreateMainMenuScene();
        CreateGameScene();
        SetupBuildSettings();

        EditorUtility.DisplayDialog("Setup Complete",
            "Iteration 01 setup complete!\n\n" +
            "1. Open MainMenu scene\n" +
            "2. Press Play to test", "OK");
    }

    [MenuItem("Starlock/Iteration 01 - Create Folders Only")]
    public static void CreateFolderStructure()
    {
        string[] folders = new string[]
        {
            "Assets/Scenes",
            "Assets/Prefabs",
            "Assets/Prefabs/UI",
            "Assets/Prefabs/Managers",
            "Assets/Audio/Music",
            "Assets/Audio/SFX",
            "Assets/Resources/Audio/SFX"
        };

        foreach (string folder in folders)
        {
            if (!AssetDatabase.IsValidFolder(folder))
            {
                string[] parts = folder.Split('/');
                string currentPath = parts[0];
                for (int i = 1; i < parts.Length; i++)
                {
                    string newPath = currentPath + "/" + parts[i];
                    if (!AssetDatabase.IsValidFolder(newPath))
                    {
                        AssetDatabase.CreateFolder(currentPath, parts[i]);
                    }
                    currentPath = newPath;
                }
            }
        }

        AssetDatabase.Refresh();
        Debug.Log("[Starlock] Folder structure created");
    }

    [MenuItem("Starlock/Iteration 01 - Create Manager Prefabs")]
    public static void CreateManagerPrefabs()
    {
        CreateFolderStructure();

        CreateGameManagerPrefab();
        CreateAudioManagerPrefab();
        CreateVibrationManagerPrefab();
        CreateSceneTransitionPrefab();

        AssetDatabase.Refresh();
        Debug.Log("[Starlock] Manager prefabs created");
    }

    private static void CreateGameManagerPrefab()
    {
        string path = "Assets/Prefabs/Managers/GameManager.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null) return;

        GameObject go = new GameObject("GameManager");
        go.AddComponent<GameManager>();

        PrefabUtility.SaveAsPrefabAsset(go, path);
        DestroyImmediate(go);
    }

    private static void CreateAudioManagerPrefab()
    {
        string path = "Assets/Prefabs/Managers/AudioManager.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null) return;

        GameObject go = new GameObject("AudioManager");
        var audioManager = go.AddComponent<AudioManager>();

        GameObject musicGO = new GameObject("MusicSource");
        musicGO.transform.SetParent(go.transform);
        var musicSource = musicGO.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;

        GameObject sfxGO = new GameObject("SFXSource");
        sfxGO.transform.SetParent(go.transform);
        var sfxSource = sfxGO.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;

        SerializedObject so = new SerializedObject(audioManager);
        so.FindProperty("musicSource").objectReferenceValue = musicSource;
        so.FindProperty("sfxSource").objectReferenceValue = sfxSource;
        so.ApplyModifiedPropertiesWithoutUndo();

        PrefabUtility.SaveAsPrefabAsset(go, path);
        DestroyImmediate(go);
    }

    private static void CreateVibrationManagerPrefab()
    {
        string path = "Assets/Prefabs/Managers/VibrationManager.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null) return;

        GameObject go = new GameObject("VibrationManager");
        go.AddComponent<VibrationManager>();

        PrefabUtility.SaveAsPrefabAsset(go, path);
        DestroyImmediate(go);
    }

    private static void CreateSceneTransitionPrefab()
    {
        string path = "Assets/Prefabs/Managers/SceneTransition.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null) return;

        GameObject go = new GameObject("SceneTransition");
        var sceneTransition = go.AddComponent<SceneTransition>();

        GameObject canvasGO = new GameObject("FadeCanvas");
        canvasGO.transform.SetParent(go.transform);

        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;

        var canvasScaler = canvasGO.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1080, 1920);
        canvasScaler.matchWidthOrHeight = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();

        var canvasGroup = canvasGO.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;

        GameObject imageGO = CreateUIObject("FadeImage", canvasGO.transform);
        var image = imageGO.AddComponent<Image>();
        image.color = Color.black;

        var imageRect = GetOrAddRectTransform(imageGO);
        imageRect.anchorMin = Vector2.zero;
        imageRect.anchorMax = Vector2.one;
        imageRect.offsetMin = Vector2.zero;
        imageRect.offsetMax = Vector2.zero;

        SerializedObject so = new SerializedObject(sceneTransition);
        so.FindProperty("fadeCanvas").objectReferenceValue = canvasGroup;
        so.FindProperty("fadeImage").objectReferenceValue = image;
        so.ApplyModifiedPropertiesWithoutUndo();

        PrefabUtility.SaveAsPrefabAsset(go, path);
        DestroyImmediate(go);
    }

    [MenuItem("Starlock/Iteration 01 - Create MainMenu Scene")]
    public static void CreateMainMenuScene()
    {
        CreateFolderStructure();
        CreateManagerPrefabs();

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        CreateCamera();
        CreateEventSystem();
        GameObject bootstrapGO = CreateBootstrap();
        GameObject canvasGO = CreateMainCanvas();
        GameObject safeAreaGO = CreateSafeArea(canvasGO);
        
        var mainMenuScreen = CreateMainMenuScreen(safeAreaGO);
        var settingsScreen = CreateSettingsScreen(safeAreaGO);
        var levelSelectScreen = CreateLevelSelectScreen(safeAreaGO);
        var achievementsScreen = CreateAchievementsScreen(safeAreaGO);

        GameObject mainMenuUIGO = new GameObject("MainMenuUI");
        var mainMenuUI = mainMenuUIGO.AddComponent<MainMenuUI>();

        SerializedObject uiSO = new SerializedObject(mainMenuUI);
        uiSO.FindProperty("mainMenuScreen").objectReferenceValue = mainMenuScreen;
        uiSO.FindProperty("settingsScreen").objectReferenceValue = settingsScreen;
        uiSO.FindProperty("levelSelectScreen").objectReferenceValue = levelSelectScreen;
        uiSO.FindProperty("achievementsScreen").objectReferenceValue = achievementsScreen;
        uiSO.ApplyModifiedPropertiesWithoutUndo();

        string scenePath = "Assets/Scenes/MainMenu.unity";
        EditorSceneManager.SaveScene(scene, scenePath);
        Debug.Log("[Starlock] MainMenu scene created");
    }

    private static void CreateCamera()
    {
        GameObject camGO = new GameObject("Main Camera");
        camGO.tag = "MainCamera";
        var cam = camGO.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = backgroundColor;
        cam.orthographic = true;
        cam.orthographicSize = 5;
        camGO.AddComponent<AudioListener>();
    }

    private static void CreateEventSystem()
    {
        GameObject eventSystemGO = new GameObject("EventSystem");
        eventSystemGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystemGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
    }

    private static GameObject CreateBootstrap()
    {
        GameObject bootstrapGO = new GameObject("Bootstrap");
        var bootstrap = bootstrapGO.AddComponent<Bootstrap>();

        var gameManagerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Managers/GameManager.prefab");
        var audioManagerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Managers/AudioManager.prefab");
        var vibrationManagerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Managers/VibrationManager.prefab");
        var sceneTransitionPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Managers/SceneTransition.prefab");

        SerializedObject so = new SerializedObject(bootstrap);
        so.FindProperty("gameManagerPrefab").objectReferenceValue = gameManagerPrefab;
        so.FindProperty("audioManagerPrefab").objectReferenceValue = audioManagerPrefab;
        so.FindProperty("vibrationManagerPrefab").objectReferenceValue = vibrationManagerPrefab;
        so.FindProperty("sceneTransitionPrefab").objectReferenceValue = sceneTransitionPrefab;
        so.ApplyModifiedPropertiesWithoutUndo();

        return bootstrapGO;
    }

    private static GameObject CreateMainCanvas()
    {
        GameObject canvasGO = new GameObject("Canvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var canvasScaler = canvasGO.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1080, 1920);
        canvasScaler.matchWidthOrHeight = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();

        GameObject bgGO = CreateUIObject("Background", canvasGO.transform);
        var bgImage = bgGO.AddComponent<Image>();
        bgImage.color = backgroundColor;
        var bgRect = GetOrAddRectTransform(bgGO);
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        return canvasGO;
    }

    private static GameObject CreateSafeArea(GameObject canvas)
    {
        GameObject safeAreaGO = CreateUIObject("SafeArea", canvas.transform);
        
        var rect = GetOrAddRectTransform(safeAreaGO);
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
        safeAreaGO.AddComponent<SafeArea>();

        return safeAreaGO;
    }

    private static MainMenuScreen CreateMainMenuScreen(GameObject parent)
    {
        GameObject screenGO = CreateUIObject("MainMenuScreen", parent.transform);

        var canvasGroup = screenGO.AddComponent<CanvasGroup>();
        var mainMenuScreen = screenGO.AddComponent<MainMenuScreen>();

        var screenRect = GetOrAddRectTransform(screenGO);
        screenRect.anchorMin = Vector2.zero;
        screenRect.anchorMax = Vector2.one;
        screenRect.offsetMin = Vector2.zero;
        screenRect.offsetMax = Vector2.zero;

        GameObject titleContainer = CreateUIObject("TitleContainer", screenGO.transform);
        var titleContainerRect = GetOrAddRectTransform(titleContainer);
        titleContainerRect.anchorMin = new Vector2(0.5f, 1f);
        titleContainerRect.anchorMax = new Vector2(0.5f, 1f);
        titleContainerRect.pivot = new Vector2(0.5f, 1f);
        titleContainerRect.anchoredPosition = new Vector2(0, -150);
        titleContainerRect.sizeDelta = new Vector2(800, 150);

        GameObject titleGO = CreateUIObject("TitleText", titleContainer.transform);
        var titleText = titleGO.AddComponent<TextMeshProUGUI>();
        titleText.text = "STARLOCK";
        titleText.fontSize = 96;
        titleText.fontStyle = FontStyles.Bold;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = titleColor;
        var titleRect = GetOrAddRectTransform(titleGO);
        titleRect.anchorMin = Vector2.zero;
        titleRect.anchorMax = Vector2.one;
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;

        GameObject buttonsContainer = CreateUIObject("ButtonsContainer", screenGO.transform);
        var buttonsRect = GetOrAddRectTransform(buttonsContainer);
        buttonsRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonsRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonsRect.pivot = new Vector2(0.5f, 0.5f);
        buttonsRect.anchoredPosition = new Vector2(0, -50);
        buttonsRect.sizeDelta = new Vector2(700, 700);

        var verticalLayout = buttonsContainer.AddComponent<VerticalLayoutGroup>();
        verticalLayout.spacing = 30;
        verticalLayout.childAlignment = TextAnchor.MiddleCenter;
        verticalLayout.childControlWidth = true;
        verticalLayout.childControlHeight = false;
        verticalLayout.childForceExpandWidth = true;
        verticalLayout.childForceExpandHeight = false;

        var playBtn = CreateMenuButton(buttonsContainer.transform, "PlayButton", "PLAY");
        var infiniteBtn = CreateMenuButton(buttonsContainer.transform, "InfiniteButton", "INFINITE");
        var timeAttackBtn = CreateMenuButton(buttonsContainer.transform, "TimeAttackButton", "TIME ATTACK");
        var achievementsBtn = CreateMenuButton(buttonsContainer.transform, "AchievementsButton", "ACHIEVEMENTS");
        var settingsBtn = CreateMenuButton(buttonsContainer.transform, "SettingsButton", "SETTINGS");

        SerializedObject so = new SerializedObject(mainMenuScreen);
        so.FindProperty("titleContainer").objectReferenceValue = titleContainerRect;
        so.FindProperty("titleText").objectReferenceValue = titleText;
        so.FindProperty("playButton").objectReferenceValue = playBtn.GetComponent<Button>();
        so.FindProperty("infiniteButton").objectReferenceValue = infiniteBtn.GetComponent<Button>();
        so.FindProperty("timeAttackButton").objectReferenceValue = timeAttackBtn.GetComponent<Button>();
        so.FindProperty("achievementsButton").objectReferenceValue = achievementsBtn.GetComponent<Button>();
        so.FindProperty("settingsButton").objectReferenceValue = settingsBtn.GetComponent<Button>();
        so.FindProperty("playButtonRect").objectReferenceValue = playBtn.GetComponent<RectTransform>();
        so.FindProperty("infiniteButtonRect").objectReferenceValue = infiniteBtn.GetComponent<RectTransform>();
        so.FindProperty("timeAttackButtonRect").objectReferenceValue = timeAttackBtn.GetComponent<RectTransform>();
        so.FindProperty("achievementsButtonRect").objectReferenceValue = achievementsBtn.GetComponent<RectTransform>();
        so.FindProperty("settingsButtonRect").objectReferenceValue = settingsBtn.GetComponent<RectTransform>();
        so.FindProperty("hideOnStart").boolValue = false;
        so.ApplyModifiedPropertiesWithoutUndo();

        return mainMenuScreen;
    }

    private static GameObject CreateMenuButton(Transform parent, string name, string text)
    {
        GameObject btnGO = CreateUIObject(name, parent);

        var image = btnGO.AddComponent<Image>();
        image.color = buttonColor;

        var button = btnGO.AddComponent<Button>();
        var colors = button.colors;
        colors.normalColor = buttonColor;
        colors.highlightedColor = buttonColor * 1.2f;
        colors.pressedColor = buttonColor * 0.8f;
        button.colors = colors;

        btnGO.AddComponent<AnimatedButton>();

        var btnRect = GetOrAddRectTransform(btnGO);
        var layoutElement = btnGO.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = 100;
        layoutElement.minHeight = 100;

        GameObject textGO = CreateUIObject("Text", btnGO.transform);
        var tmpText = textGO.AddComponent<TextMeshProUGUI>();
        tmpText.text = text;
        tmpText.fontSize = 42;
        tmpText.fontStyle = FontStyles.Bold;
        tmpText.alignment = TextAlignmentOptions.Center;
        tmpText.color = buttonTextColor;

        var textRect = GetOrAddRectTransform(textGO);
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        return btnGO;
    }

    private static SettingsScreen CreateSettingsScreen(GameObject parent)
    {
        GameObject screenGO = CreateUIObject("SettingsScreen", parent.transform);

        var canvasGroup = screenGO.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;

        var settingsScreen = screenGO.AddComponent<SettingsScreen>();

        var screenRect = GetOrAddRectTransform(screenGO);
        screenRect.anchorMin = Vector2.zero;
        screenRect.anchorMax = Vector2.one;
        screenRect.offsetMin = Vector2.zero;
        screenRect.offsetMax = Vector2.zero;

        var headerGO = CreateScreenHeader(screenGO.transform, "SETTINGS");
        var backBtn = headerGO.transform.Find("BackButton").GetComponent<Button>();
        var headerRect = GetOrAddRectTransform(headerGO);

        GameObject contentGO = CreateUIObject("Content", screenGO.transform);
        var contentRect = GetOrAddRectTransform(contentGO);
        contentRect.anchorMin = new Vector2(0, 0);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.offsetMin = new Vector2(60, 200);
        contentRect.offsetMax = new Vector2(-60, -200);

        var verticalLayout = contentGO.AddComponent<VerticalLayoutGroup>();
        verticalLayout.spacing = 60;
        verticalLayout.childAlignment = TextAnchor.UpperCenter;
        verticalLayout.childControlWidth = true;
        verticalLayout.childControlHeight = false;
        verticalLayout.childForceExpandWidth = true;
        verticalLayout.childForceExpandHeight = false;
        verticalLayout.padding = new RectOffset(40, 40, 40, 40);

        var musicRow = CreateSliderRow(contentGO.transform, "MusicRow", "MUSIC");
        var sfxRow = CreateSliderRow(contentGO.transform, "SFXRow", "SOUND");
        var vibrationRow = CreateToggleRow(contentGO.transform, "VibrationRow", "VIBRATION");

        var musicSlider = musicRow.GetComponentInChildren<Slider>();
        var musicValue = musicRow.transform.Find("Value").GetComponent<TextMeshProUGUI>();
        var sfxSlider = sfxRow.GetComponentInChildren<Slider>();
        var sfxValue = sfxRow.transform.Find("Value").GetComponent<TextMeshProUGUI>();
        var vibrationToggle = vibrationRow.GetComponentInChildren<Toggle>();
        var vibrationCheckmark = vibrationToggle.graphic as Image;

        SerializedObject so = new SerializedObject(settingsScreen);
        so.FindProperty("headerRect").objectReferenceValue = headerRect;
        so.FindProperty("backButton").objectReferenceValue = backBtn;
        so.FindProperty("contentRect").objectReferenceValue = contentRect;
        so.FindProperty("musicSlider").objectReferenceValue = musicSlider;
        so.FindProperty("musicValueText").objectReferenceValue = musicValue;
        so.FindProperty("sfxSlider").objectReferenceValue = sfxSlider;
        so.FindProperty("sfxValueText").objectReferenceValue = sfxValue;
        so.FindProperty("vibrationToggle").objectReferenceValue = vibrationToggle;
        so.FindProperty("vibrationCheckmark").objectReferenceValue = vibrationCheckmark;
        so.ApplyModifiedPropertiesWithoutUndo();

        return settingsScreen;
    }

    private static GameObject CreateScreenHeader(Transform parent, string title)
    {
        GameObject headerGO = CreateUIObject("Header", parent);

        var headerRect = GetOrAddRectTransform(headerGO);
        headerRect.anchorMin = new Vector2(0, 1);
        headerRect.anchorMax = new Vector2(1, 1);
        headerRect.pivot = new Vector2(0.5f, 1);
        headerRect.anchoredPosition = new Vector2(0, -50);
        headerRect.sizeDelta = new Vector2(0, 120);

        GameObject backBtnGO = CreateUIObject("BackButton", headerGO.transform);

        var backImage = backBtnGO.AddComponent<Image>();
        backImage.color = buttonColor;

        var backBtn = backBtnGO.AddComponent<Button>();
        backBtnGO.AddComponent<AnimatedButton>();

        var backRect = GetOrAddRectTransform(backBtnGO);
        backRect.anchorMin = new Vector2(0, 0.5f);
        backRect.anchorMax = new Vector2(0, 0.5f);
        backRect.pivot = new Vector2(0, 0.5f);
        backRect.anchoredPosition = new Vector2(40, 0);
        backRect.sizeDelta = new Vector2(100, 80);

        GameObject backTextGO = CreateUIObject("Text", backBtnGO.transform);
        var backText = backTextGO.AddComponent<TextMeshProUGUI>();
        backText.text = "←";
        backText.fontSize = 48;
        backText.alignment = TextAlignmentOptions.Center;
        backText.color = buttonTextColor;
        var backTextRect = GetOrAddRectTransform(backTextGO);
        backTextRect.anchorMin = Vector2.zero;
        backTextRect.anchorMax = Vector2.one;
        backTextRect.offsetMin = Vector2.zero;
        backTextRect.offsetMax = Vector2.zero;

        GameObject titleGO = CreateUIObject("TitleText", headerGO.transform);
        var titleText = titleGO.AddComponent<TextMeshProUGUI>();
        titleText.text = title;
        titleText.fontSize = 56;
        titleText.fontStyle = FontStyles.Bold;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = titleColor;
        var titleRect = GetOrAddRectTransform(titleGO);
        titleRect.anchorMin = Vector2.zero;
        titleRect.anchorMax = Vector2.one;
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;

        return headerGO;
    }

    private static GameObject CreateSliderRow(Transform parent, string name, string label)
    {
        GameObject rowGO = CreateUIObject(name, parent);

        var rowRect = GetOrAddRectTransform(rowGO);
        var layoutElement = rowGO.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = 100;
        layoutElement.minHeight = 100;

        GameObject labelGO = CreateUIObject("Label", rowGO.transform);
        var labelText = labelGO.AddComponent<TextMeshProUGUI>();
        labelText.text = label;
        labelText.fontSize = 36;
        labelText.alignment = TextAlignmentOptions.Left;
        labelText.color = titleColor;
        var labelRect = GetOrAddRectTransform(labelGO);
        labelRect.anchorMin = new Vector2(0, 0);
        labelRect.anchorMax = new Vector2(0.35f, 1);
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        GameObject sliderGO = CreateUIObject("Slider", rowGO.transform);
        var sliderRect = GetOrAddRectTransform(sliderGO);
        sliderRect.anchorMin = new Vector2(0.35f, 0.3f);
        sliderRect.anchorMax = new Vector2(0.8f, 0.7f);
        sliderRect.offsetMin = Vector2.zero;
        sliderRect.offsetMax = Vector2.zero;

        GameObject bgGO = CreateUIObject("Background", sliderGO.transform);
        var bgImage = bgGO.AddComponent<Image>();
        bgImage.color = new Color(0.15f, 0.15f, 0.18f, 1f);
        var bgRect = GetOrAddRectTransform(bgGO);
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        GameObject fillAreaGO = CreateUIObject("Fill Area", sliderGO.transform);
        var fillAreaRect = GetOrAddRectTransform(fillAreaGO);
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.offsetMin = new Vector2(5, 5);
        fillAreaRect.offsetMax = new Vector2(-5, -5);

        GameObject fillGO = CreateUIObject("Fill", fillAreaGO.transform);
        var fillImage = fillGO.AddComponent<Image>();
        fillImage.color = sliderFillColor;
        var fillRect = GetOrAddRectTransform(fillGO);
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        var slider = sliderGO.AddComponent<Slider>();
        slider.fillRect = fillRect;
        slider.targetGraphic = bgImage;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 1f;

        GameObject valueGO = CreateUIObject("Value", rowGO.transform);
        var valueText = valueGO.AddComponent<TextMeshProUGUI>();
        valueText.text = "100%";
        valueText.fontSize = 32;
        valueText.alignment = TextAlignmentOptions.Right;
        valueText.color = titleColor;
        var valueRect = GetOrAddRectTransform(valueGO);
        valueRect.anchorMin = new Vector2(0.82f, 0);
        valueRect.anchorMax = new Vector2(1f, 1);
        valueRect.offsetMin = Vector2.zero;
        valueRect.offsetMax = Vector2.zero;

        return rowGO;
    }

    private static GameObject CreateToggleRow(Transform parent, string name, string label)
    {
        GameObject rowGO = CreateUIObject(name, parent);

        var rowRect = GetOrAddRectTransform(rowGO);
        var layoutElement = rowGO.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = 100;
        layoutElement.minHeight = 100;

        GameObject labelGO = CreateUIObject("Label", rowGO.transform);
        var labelText = labelGO.AddComponent<TextMeshProUGUI>();
        labelText.text = label;
        labelText.fontSize = 36;
        labelText.alignment = TextAlignmentOptions.Left;
        labelText.color = titleColor;
        var labelRect = GetOrAddRectTransform(labelGO);
        labelRect.anchorMin = new Vector2(0, 0);
        labelRect.anchorMax = new Vector2(0.7f, 1);
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        GameObject toggleGO = CreateUIObject("Toggle", rowGO.transform);
        var toggleRect = GetOrAddRectTransform(toggleGO);
        toggleRect.anchorMin = new Vector2(1, 0.5f);
        toggleRect.anchorMax = new Vector2(1, 0.5f);
        toggleRect.pivot = new Vector2(1, 0.5f);
        toggleRect.anchoredPosition = new Vector2(0, 0);
        toggleRect.sizeDelta = new Vector2(80, 80);

        GameObject bgGO = CreateUIObject("Background", toggleGO.transform);
        var bgImage = bgGO.AddComponent<Image>();
        bgImage.color = new Color(0.15f, 0.15f, 0.18f, 1f);
        var bgRect = GetOrAddRectTransform(bgGO);
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        GameObject checkGO = CreateUIObject("Checkmark", bgGO.transform);
        var checkImage = checkGO.AddComponent<Image>();
        checkImage.color = toggleOnColor;
        var checkRect = GetOrAddRectTransform(checkGO);
        checkRect.anchorMin = new Vector2(0.2f, 0.2f);
        checkRect.anchorMax = new Vector2(0.8f, 0.8f);
        checkRect.offsetMin = Vector2.zero;
        checkRect.offsetMax = Vector2.zero;

        var toggle = toggleGO.AddComponent<Toggle>();
        toggle.targetGraphic = bgImage;
        toggle.graphic = checkImage;
        toggle.isOn = true;

        return rowGO;
    }

    private static LevelSelectScreen CreateLevelSelectScreen(GameObject parent)
    {
        GameObject screenGO = CreateUIObject("LevelSelectScreen", parent.transform);

        var canvasGroup = screenGO.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;

        var levelSelectScreen = screenGO.AddComponent<LevelSelectScreen>();

        var screenRect = GetOrAddRectTransform(screenGO);
        screenRect.anchorMin = Vector2.zero;
        screenRect.anchorMax = Vector2.one;
        screenRect.offsetMin = Vector2.zero;
        screenRect.offsetMax = Vector2.zero;

        var headerGO = CreateScreenHeader(screenGO.transform, "LEVELS");
        var backBtn = headerGO.transform.Find("BackButton").GetComponent<Button>();
        var headerRect = GetOrAddRectTransform(headerGO);

        GameObject contentGO = CreateUIObject("Content", screenGO.transform);
        var contentRect = GetOrAddRectTransform(contentGO);
        contentRect.anchorMin = new Vector2(0, 0);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.offsetMin = new Vector2(60, 200);
        contentRect.offsetMax = new Vector2(-60, -200);

        GameObject textGO = CreateUIObject("ComingSoonText", contentGO.transform);
        var text = textGO.AddComponent<TextMeshProUGUI>();
        text.text = "Coming Soon\n\n(Iteration 07)";
        text.fontSize = 48;
        text.alignment = TextAlignmentOptions.Center;
        text.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        var textRect = GetOrAddRectTransform(textGO);
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        SerializedObject so = new SerializedObject(levelSelectScreen);
        so.FindProperty("headerRect").objectReferenceValue = headerRect;
        so.FindProperty("backButton").objectReferenceValue = backBtn;
        so.FindProperty("contentRect").objectReferenceValue = contentRect;
        so.FindProperty("comingSoonText").objectReferenceValue = text;
        so.ApplyModifiedPropertiesWithoutUndo();

        return levelSelectScreen;
    }

    private static AchievementsScreen CreateAchievementsScreen(GameObject parent)
    {
        GameObject screenGO = CreateUIObject("AchievementsScreen", parent.transform);

        var canvasGroup = screenGO.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;

        var achievementsScreen = screenGO.AddComponent<AchievementsScreen>();

        var screenRect = GetOrAddRectTransform(screenGO);
        screenRect.anchorMin = Vector2.zero;
        screenRect.anchorMax = Vector2.one;
        screenRect.offsetMin = Vector2.zero;
        screenRect.offsetMax = Vector2.zero;

        var headerGO = CreateScreenHeader(screenGO.transform, "ACHIEVEMENTS");
        var backBtn = headerGO.transform.Find("BackButton").GetComponent<Button>();
        var headerRect = GetOrAddRectTransform(headerGO);

        GameObject contentGO = CreateUIObject("Content", screenGO.transform);
        var contentRect = GetOrAddRectTransform(contentGO);
        contentRect.anchorMin = new Vector2(0, 0);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.offsetMin = new Vector2(60, 200);
        contentRect.offsetMax = new Vector2(-60, -200);

        GameObject textGO = CreateUIObject("ComingSoonText", contentGO.transform);
        var text = textGO.AddComponent<TextMeshProUGUI>();
        text.text = "Coming Soon\n\n(Iteration 09)";
        text.fontSize = 48;
        text.alignment = TextAlignmentOptions.Center;
        text.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        var textRect = GetOrAddRectTransform(textGO);
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        SerializedObject so = new SerializedObject(achievementsScreen);
        so.FindProperty("headerRect").objectReferenceValue = headerRect;
        so.FindProperty("backButton").objectReferenceValue = backBtn;
        so.FindProperty("contentRect").objectReferenceValue = contentRect;
        so.FindProperty("comingSoonText").objectReferenceValue = text;
        so.ApplyModifiedPropertiesWithoutUndo();

        return achievementsScreen;
    }

    [MenuItem("Starlock/Iteration 01 - Create Game Scene")]
    public static void CreateGameScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        GameObject camGO = new GameObject("Main Camera");
        camGO.tag = "MainCamera";
        var cam = camGO.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = backgroundColor;
        cam.orthographic = true;
        camGO.AddComponent<AudioListener>();

        CreateEventSystem();

        GameObject canvasGO = new GameObject("Canvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var canvasScaler = canvasGO.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1080, 1920);
        canvasScaler.matchWidthOrHeight = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();

        GameObject textGO = CreateUIObject("PlaceholderText", canvasGO.transform);
        var text = textGO.AddComponent<TextMeshProUGUI>();
        text.text = "Game Scene\n\nComing in Iteration 04";
        text.fontSize = 48;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        var textRect = GetOrAddRectTransform(textGO);
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        string scenePath = "Assets/Scenes/Game.unity";
        EditorSceneManager.SaveScene(scene, scenePath);
        Debug.Log("[Starlock] Game scene created");
    }

    private static void SetupBuildSettings()
    {
        var scenes = new EditorBuildSettingsScene[]
        {
            new EditorBuildSettingsScene("Assets/Scenes/MainMenu.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/Game.unity", true)
        };

        EditorBuildSettings.scenes = scenes;
        Debug.Log("[Starlock] Build settings configured");
    }
}
