using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class MainMenuSetupWindow : EditorWindow
{
    private Canvas targetCanvas;

    private Color backgroundColor = new Color(0.08f, 0.08f, 0.12f, 1f);
    private Color panelColor = new Color(0.12f, 0.12f, 0.18f, 1f);
    private Color buttonColor = new Color(0.18f, 0.18f, 0.25f, 1f);
    private Color buttonHighlightColor = new Color(0.25f, 0.25f, 0.35f, 1f);
    private Color accentColor = new Color(0.4f, 0.6f, 1f, 1f);
    private Color textColor = new Color(0.95f, 0.95f, 0.95f, 1f);
    private Color textSecondaryColor = new Color(0.6f, 0.6f, 0.7f, 1f);

    [MenuItem("RhythmGame/Setup MainMenu Scene")]
    public static void ShowWindow()
    {
        GetWindow<MainMenuSetupWindow>("MainMenu Setup");
    }

    private void OnGUI()
    {
        GUILayout.Label("MainMenu Scene Setup", EditorStyles.boldLabel);
        GUILayout.Space(10);

        targetCanvas = (Canvas)EditorGUILayout.ObjectField("Target Canvas", targetCanvas, typeof(Canvas), true);

        GUILayout.Space(20);

        EditorGUI.BeginDisabledGroup(targetCanvas == null);
        if (GUILayout.Button("Create MainMenu UI", GUILayout.Height(40)))
        {
            CreateMainMenuUI();
        }
        EditorGUI.EndDisabledGroup();

        if (targetCanvas == null)
        {
            EditorGUILayout.HelpBox("Drag a Canvas here to setup MainMenu UI", MessageType.Info);
        }
    }

    private void CreateMainMenuUI()
    {
        Undo.RegisterCompleteObjectUndo(targetCanvas.gameObject, "Create MainMenu UI");

        CreateBackground(targetCanvas.transform);
        var fadePanel = CreateFadePanel(targetCanvas.transform);
        var mainMenuPanel = CreateMainMenuPanel(targetCanvas.transform);
        var modeSelectPanel = CreateModeSelectPanel(targetCanvas.transform);
        var settingsPanel = CreateSettingsPanel(targetCanvas.transform);

        var initGO = new GameObject("MainMenuInit");
        var init = initGO.AddComponent<MainMenuInit>();
        SetPrivateField(init, "fadeImage", fadePanel.GetComponent<Image>());
        Undo.RegisterCreatedObjectUndo(initGO, "Create MainMenuInit");

        var controllerGO = new GameObject("MainMenuController");
        var controller = controllerGO.AddComponent<MainMenuController>();
        
        SetPrivateField(controller, "mainMenuPanel", mainMenuPanel);
        SetPrivateField(controller, "modeSelectPanel", modeSelectPanel);
        SetPrivateField(controller, "settingsPanel", settingsPanel);
        
        SetPrivateField(controller, "playButton", mainMenuPanel.Find("ButtonsContainer/PlayButton").GetComponent<Button>());
        SetPrivateField(controller, "settingsButton", mainMenuPanel.Find("ButtonsContainer/SettingsButton").GetComponent<Button>());
        SetPrivateField(controller, "quitButton", mainMenuPanel.Find("ButtonsContainer/QuitButton").GetComponent<Button>());
        
        SetPrivateField(controller, "levelsButton", modeSelectPanel.Find("ButtonsContainer/LevelsButton").GetComponent<Button>());
        SetPrivateField(controller, "infiniteButton", modeSelectPanel.Find("ButtonsContainer/InfiniteButton").GetComponent<Button>());
        SetPrivateField(controller, "timeAttackButton", modeSelectPanel.Find("ButtonsContainer/TimeAttackButton").GetComponent<Button>());
        SetPrivateField(controller, "modeBackButton", modeSelectPanel.Find("Footer/BackButton").GetComponent<Button>());
        
        SetPrivateField(controller, "settingsBackButton", settingsPanel.Find("Footer/BackButton").GetComponent<Button>());
        
        Undo.RegisterCreatedObjectUndo(controllerGO, "Create MainMenuController");

        Selection.activeGameObject = targetCanvas.gameObject;
        EditorUtility.SetDirty(targetCanvas);

        Debug.Log("MainMenu UI created successfully!");
    }

    private void CreateBackground(Transform parent)
    {
        var go = new GameObject("Background", typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        go.transform.SetAsFirstSibling();
        
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
        var image = go.GetComponent<Image>();
        image.color = backgroundColor;
        image.raycastTarget = false;

        Undo.RegisterCreatedObjectUndo(go, "Create Background");
    }

    private GameObject CreateFadePanel(Transform parent)
    {
        var go = new GameObject("FadePanel", typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        go.transform.SetAsLastSibling();
        
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
        var image = go.GetComponent<Image>();
        image.color = Color.black;
        image.raycastTarget = true;

        Undo.RegisterCreatedObjectUndo(go, "Create FadePanel");
        return go;
    }

    private RectTransform CreateMainMenuPanel(Transform parent)
    {
        var panel = CreatePanel("MainMenuPanel", parent);
        
        var header = CreateHeader(panel);
        CreateLogo(header, "STARLOCK");
        CreateSubtitle(header, "RHYTHM CORE");

        var buttonsContainer = CreateButtonsContainer(panel, 0);
        CreateMenuButton("PlayButton", buttonsContainer, "PLAY", "Start the game", true);
        CreateMenuButton("SettingsButton", buttonsContainer, "SETTINGS", "Adjust preferences", false);
        CreateMenuButton("QuitButton", buttonsContainer, "QUIT", "Exit game", false);

        var footer = CreateFooter(panel);
        CreateFooterText(footer, "v1.0.0");

        return panel;
    }

    private RectTransform CreateModeSelectPanel(Transform parent)
    {
        var panel = CreatePanel("ModeSelectPanel", parent);

        var header = CreateHeader(panel);
        CreateTitle(header, "SELECT MODE");

        var buttonsContainer = CreateButtonsContainer(panel, 50);
        CreateModeButton("LevelsButton", buttonsContainer, "LEVELS", "Progress through chapters", "3-7 rounds per level");
        CreateModeButton("InfiniteButton", buttonsContainer, "INFINITE", "Endless challenge", "How far can you go?");
        CreateModeButton("TimeAttackButton", buttonsContainer, "TIME ATTACK", "Beat the clock", "60 / 90 / 120 seconds");

        var footer = CreateFooter(panel);
        CreateBackButton("BackButton", footer);

        return panel;
    }

    private RectTransform CreateSettingsPanel(Transform parent)
    {
        var panel = CreatePanel("SettingsPanel", parent);

        var header = CreateHeader(panel);
        CreateTitle(header, "SETTINGS");

        var content = CreateSettingsContent(panel);
        CreateSettingsPlaceholder(content);

        var footer = CreateFooter(panel);
        CreateBackButton("BackButton", footer);

        return panel;
    }

    private RectTransform CreatePanel(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Undo.RegisterCreatedObjectUndo(go, "Create Panel");
        return rect;
    }

    private RectTransform CreateHeader(RectTransform parent)
    {
        var go = new GameObject("Header", typeof(RectTransform));
        go.transform.SetParent(parent, false);
        
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(0.5f, 1);
        rect.anchoredPosition = new Vector2(0, -120);
        rect.sizeDelta = new Vector2(0, 300);

        Undo.RegisterCreatedObjectUndo(go, "Create Header");
        return rect;
    }

    private void CreateLogo(RectTransform parent, string text)
    {
        var go = new GameObject("LogoText", typeof(RectTransform), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0.5f);
        rect.anchorMax = new Vector2(1, 1);
        rect.offsetMin = new Vector2(60, 0);
        rect.offsetMax = new Vector2(-60, -20);
        
        var tmp = go.GetComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 110;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = textColor;
        tmp.enableAutoSizing = false;

        Undo.RegisterCreatedObjectUndo(go, "Create Logo");
    }

    private void CreateSubtitle(RectTransform parent, string text)
    {
        var go = new GameObject("SubtitleText", typeof(RectTransform), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(1, 0.5f);
        rect.offsetMin = new Vector2(60, 20);
        rect.offsetMax = new Vector2(-60, 0);
        
        var tmp = go.GetComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 36;
        tmp.fontStyle = FontStyles.Normal;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = accentColor;
        tmp.characterSpacing = 20;

        Undo.RegisterCreatedObjectUndo(go, "Create Subtitle");
    }

    private void CreateTitle(RectTransform parent, string text)
    {
        var go = new GameObject("TitleText", typeof(RectTransform), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(60, 0);
        rect.offsetMax = new Vector2(-60, 0);
        
        var tmp = go.GetComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 64;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = textColor;

        Undo.RegisterCreatedObjectUndo(go, "Create Title");
    }

    private RectTransform CreateButtonsContainer(RectTransform parent, float yOffset)
    {
        var go = new GameObject("ButtonsContainer", typeof(RectTransform));
        go.transform.SetParent(parent, false);
        
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0, yOffset);
        rect.sizeDelta = new Vector2(900, 700);

        Undo.RegisterCreatedObjectUndo(go, "Create ButtonsContainer");
        return rect;
    }

    private void CreateMenuButton(string name, RectTransform parent, string title, string subtitle, bool isPrimary)
    {
        int childCount = parent.childCount;
        float yPos = -childCount * 170;

        var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1);
        rect.anchorMax = new Vector2(0.5f, 1);
        rect.pivot = new Vector2(0.5f, 1);
        rect.anchoredPosition = new Vector2(0, yPos);
        rect.sizeDelta = new Vector2(750, 140);
        
        var image = go.GetComponent<Image>();
        image.color = isPrimary ? accentColor : buttonColor;
        image.type = Image.Type.Sliced;
        image.pixelsPerUnitMultiplier = 2;

        var button = go.GetComponent<Button>();
        var colors = button.colors;
        colors.normalColor = isPrimary ? accentColor : buttonColor;
        colors.highlightedColor = isPrimary ? new Color(0.5f, 0.7f, 1f, 1f) : buttonHighlightColor;
        colors.pressedColor = isPrimary ? new Color(0.3f, 0.5f, 0.9f, 1f) : new Color(0.15f, 0.15f, 0.2f, 1f);
        colors.fadeDuration = 0.1f;
        button.colors = colors;

        var titleGO = new GameObject("Title", typeof(RectTransform), typeof(TextMeshProUGUI));
        titleGO.transform.SetParent(go.transform, false);
        
        var titleRect = titleGO.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.5f);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.offsetMin = new Vector2(40, 0);
        titleRect.offsetMax = new Vector2(-40, -15);
        
        var titleTmp = titleGO.GetComponent<TextMeshProUGUI>();
        titleTmp.text = title;
        titleTmp.fontSize = 44;
        titleTmp.fontStyle = FontStyles.Bold;
        titleTmp.alignment = TextAlignmentOptions.Center;
        titleTmp.color = isPrimary ? new Color(0.05f, 0.05f, 0.1f, 1f) : textColor;

        var subtitleGO = new GameObject("Subtitle", typeof(RectTransform), typeof(TextMeshProUGUI));
        subtitleGO.transform.SetParent(go.transform, false);
        
        var subtitleRect = subtitleGO.GetComponent<RectTransform>();
        subtitleRect.anchorMin = new Vector2(0, 0);
        subtitleRect.anchorMax = new Vector2(1, 0.5f);
        subtitleRect.offsetMin = new Vector2(40, 15);
        subtitleRect.offsetMax = new Vector2(-40, 0);
        
        var subtitleTmp = subtitleGO.GetComponent<TextMeshProUGUI>();
        subtitleTmp.text = subtitle;
        subtitleTmp.fontSize = 26;
        subtitleTmp.alignment = TextAlignmentOptions.Center;
        subtitleTmp.color = isPrimary ? new Color(0.1f, 0.1f, 0.2f, 1f) : textSecondaryColor;

        Undo.RegisterCreatedObjectUndo(go, "Create Button");
    }

    private void CreateModeButton(string name, RectTransform parent, string title, string description, string info)
    {
        int childCount = parent.childCount;
        float yPos = -childCount * 210;

        var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1);
        rect.anchorMax = new Vector2(0.5f, 1);
        rect.pivot = new Vector2(0.5f, 1);
        rect.anchoredPosition = new Vector2(0, yPos);
        rect.sizeDelta = new Vector2(900, 180);
        
        var image = go.GetComponent<Image>();
        image.color = panelColor;

        var button = go.GetComponent<Button>();
        var colors = button.colors;
        colors.normalColor = panelColor;
        colors.highlightedColor = buttonHighlightColor;
        colors.pressedColor = new Color(0.1f, 0.1f, 0.15f, 1f);
        colors.fadeDuration = 0.1f;
        button.colors = colors;

        var accent = new GameObject("Accent", typeof(RectTransform), typeof(Image));
        accent.transform.SetParent(go.transform, false);
        
        var accentRect = accent.GetComponent<RectTransform>();
        accentRect.anchorMin = new Vector2(0, 0);
        accentRect.anchorMax = new Vector2(0, 1);
        accentRect.pivot = new Vector2(0, 0.5f);
        accentRect.anchoredPosition = Vector2.zero;
        accentRect.sizeDelta = new Vector2(8, 0);
        
        accent.GetComponent<Image>().color = accentColor;

        var titleGO = new GameObject("Title", typeof(RectTransform), typeof(TextMeshProUGUI));
        titleGO.transform.SetParent(go.transform, false);
        
        var titleRect = titleGO.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.6f);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.offsetMin = new Vector2(50, 0);
        titleRect.offsetMax = new Vector2(-40, -15);
        
        var titleTmp = titleGO.GetComponent<TextMeshProUGUI>();
        titleTmp.text = title;
        titleTmp.fontSize = 42;
        titleTmp.fontStyle = FontStyles.Bold;
        titleTmp.alignment = TextAlignmentOptions.Left;
        titleTmp.color = textColor;

        var descGO = new GameObject("Description", typeof(RectTransform), typeof(TextMeshProUGUI));
        descGO.transform.SetParent(go.transform, false);
        
        var descRect = descGO.GetComponent<RectTransform>();
        descRect.anchorMin = new Vector2(0, 0.3f);
        descRect.anchorMax = new Vector2(1, 0.6f);
        descRect.offsetMin = new Vector2(50, 0);
        descRect.offsetMax = new Vector2(-40, 0);
        
        var descTmp = descGO.GetComponent<TextMeshProUGUI>();
        descTmp.text = description;
        descTmp.fontSize = 28;
        descTmp.alignment = TextAlignmentOptions.Left;
        descTmp.color = textSecondaryColor;

        var infoGO = new GameObject("Info", typeof(RectTransform), typeof(TextMeshProUGUI));
        infoGO.transform.SetParent(go.transform, false);
        
        var infoRect = infoGO.GetComponent<RectTransform>();
        infoRect.anchorMin = new Vector2(0, 0);
        infoRect.anchorMax = new Vector2(1, 0.3f);
        infoRect.offsetMin = new Vector2(50, 10);
        infoRect.offsetMax = new Vector2(-40, 0);
        
        var infoTmp = infoGO.GetComponent<TextMeshProUGUI>();
        infoTmp.text = info;
        infoTmp.fontSize = 24;
        infoTmp.fontStyle = FontStyles.Italic;
        infoTmp.alignment = TextAlignmentOptions.Left;
        infoTmp.color = accentColor;

        Undo.RegisterCreatedObjectUndo(go, "Create Mode Button");
    }

    private RectTransform CreateFooter(RectTransform parent)
    {
        var go = new GameObject("Footer", typeof(RectTransform));
        go.transform.SetParent(parent, false);
        
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(1, 0);
        rect.pivot = new Vector2(0.5f, 0);
        rect.anchoredPosition = new Vector2(0, 100);
        rect.sizeDelta = new Vector2(0, 120);

        Undo.RegisterCreatedObjectUndo(go, "Create Footer");
        return rect;
    }

    private void CreateFooterText(RectTransform parent, string text)
    {
        var go = new GameObject("VersionText", typeof(RectTransform), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
        var tmp = go.GetComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 24;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = textSecondaryColor;

        Undo.RegisterCreatedObjectUndo(go, "Create Footer Text");
    }

    private void CreateBackButton(string name, RectTransform parent)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(300, 80);
        
        var image = go.GetComponent<Image>();
        image.color = new Color(0, 0, 0, 0);

        var button = go.GetComponent<Button>();
        var colors = button.colors;
        colors.normalColor = new Color(1, 1, 1, 0);
        colors.highlightedColor = new Color(1, 1, 1, 0.1f);
        colors.pressedColor = new Color(1, 1, 1, 0.05f);
        colors.fadeDuration = 0.1f;
        button.colors = colors;

        var textGO = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        textGO.transform.SetParent(go.transform, false);
        
        var textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        var tmp = textGO.GetComponent<TextMeshProUGUI>();
        tmp.text = "← BACK";
        tmp.fontSize = 32;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = textSecondaryColor;

        Undo.RegisterCreatedObjectUndo(go, "Create Back Button");
    }

    private RectTransform CreateSettingsContent(RectTransform parent)
    {
        var go = new GameObject("Content", typeof(RectTransform));
        go.transform.SetParent(parent, false);
        
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0.2f);
        rect.anchorMax = new Vector2(1, 0.8f);
        rect.offsetMin = new Vector2(60, 0);
        rect.offsetMax = new Vector2(-60, 0);

        Undo.RegisterCreatedObjectUndo(go, "Create Content");
        return rect;
    }

    private void CreateSettingsPlaceholder(RectTransform parent)
    {
        var go = new GameObject("PlaceholderText", typeof(RectTransform), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0.5f);
        rect.anchorMax = new Vector2(1, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(0, 100);
        
        var tmp = go.GetComponent<TextMeshProUGUI>();
        tmp.text = "Settings will be available\nin a future update";
        tmp.fontSize = 32;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = textSecondaryColor;

        Undo.RegisterCreatedObjectUndo(go, "Create Placeholder");
    }

    private void SetPrivateField(object target, string fieldName, object value)
    {
        var field = target.GetType().GetField(fieldName, 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance);
        
        if (field != null)
        {
            field.SetValue(target, value);
            EditorUtility.SetDirty(target as Object);
        }
    }
}
