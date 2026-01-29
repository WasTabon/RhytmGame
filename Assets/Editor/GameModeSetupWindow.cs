using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class GameModeSetupWindow : EditorWindow
{
    private Canvas targetCanvas;

    private Color panelColor = new Color(0f, 0f, 0f, 0.8f);
    private Color buttonColor = new Color(0.2f, 0.2f, 0.3f, 1f);
    private Color accentColor = new Color(0.4f, 0.6f, 1f, 1f);

    private string[] levelNames = new string[]
    {
        "First Steps",
        "Warm Up",
        "Getting Started",
        "Easy Breeze",
        "Smooth Sailing",
        "Rising Sun",
        "Quick Feet",
        "Steady Rhythm",
        "Building Momentum",
        "Finding Flow",
        "Sharp Focus",
        "Swift Motion",
        "Growing Strong",
        "Gaining Speed",
        "Half Way",
        "Breaking Through",
        "Rising Challenge",
        "Pushing Limits",
        "High Gear",
        "Full Throttle",
        "No Mercy",
        "Razor Edge",
        "Lightning Fast",
        "Storm Chaser",
        "Fire Dance",
        "Shadow Strike",
        "Final Push",
        "Ultimate Test",
        "Grand Finale",
        "Perfect Master"
    };

    [MenuItem("RhythmGame/Setup Game Mode UI")]
    public static void ShowWindow()
    {
        GetWindow<GameModeSetupWindow>("Game Mode Setup");
    }

    private void OnGUI()
    {
        GUILayout.Label("Game Mode UI Setup", EditorStyles.boldLabel);
        GUILayout.Space(10);

        targetCanvas = (Canvas)EditorGUILayout.ObjectField("Target Canvas", targetCanvas, typeof(Canvas), true);

        GUILayout.Space(20);

        EditorGUI.BeginDisabledGroup(targetCanvas == null);
        if (GUILayout.Button("Create All Game Mode UI", GUILayout.Height(40)))
        {
            CreateAllUI();
        }
        EditorGUI.EndDisabledGroup();

        GUILayout.Space(10);

        if (GUILayout.Button("Create Level Data (30 Levels)", GUILayout.Height(30)))
        {
            CreateLevelData();
        }

        GUILayout.Space(10);

        if (targetCanvas == null)
        {
            EditorGUILayout.HelpBox("Drag a Canvas here to create Game Mode UI", MessageType.Info);
        }
    }

    private void CreateAllUI()
    {
        var timerText = CreateTimerText();
        var progressText = CreateProgressText();
        var missesText = CreateMissesText();
        var gameOverPanel = CreateGameOverPanel();

        CreateTimeAttackTimer(timerText);
        CreateLevelProgressUI(progressText);
        CreateMissesUI(missesText);
        CreateGameOverUI(gameOverPanel);

        Debug.Log("Game Mode UI created successfully!");
    }

    private TextMeshProUGUI CreateTimerText()
    {
        GameObject go = new GameObject("TimerText", typeof(RectTransform), typeof(TextMeshProUGUI));
        go.transform.SetParent(targetCanvas.transform, false);

        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(1, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(1, 1);
        rect.anchoredPosition = new Vector2(-50, -50);
        rect.sizeDelta = new Vector2(200, 60);

        TextMeshProUGUI tmp = go.GetComponent<TextMeshProUGUI>();
        tmp.text = "1:00";
        tmp.fontSize = 48;
        tmp.alignment = TextAlignmentOptions.Right;
        tmp.color = Color.white;

        Undo.RegisterCreatedObjectUndo(go, "Create TimerText");
        return tmp;
    }

    private TextMeshProUGUI CreateProgressText()
    {
        GameObject go = new GameObject("ProgressText", typeof(RectTransform), typeof(TextMeshProUGUI));
        go.transform.SetParent(targetCanvas.transform, false);

        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(1, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(1, 1);
        rect.anchoredPosition = new Vector2(-50, -50);
        rect.sizeDelta = new Vector2(200, 60);

        TextMeshProUGUI tmp = go.GetComponent<TextMeshProUGUI>();
        tmp.text = "0 / 5";
        tmp.fontSize = 48;
        tmp.alignment = TextAlignmentOptions.Right;
        tmp.color = Color.white;

        Undo.RegisterCreatedObjectUndo(go, "Create ProgressText");
        return tmp;
    }

    private TextMeshProUGUI CreateMissesText()
    {
        GameObject go = new GameObject("MissesText", typeof(RectTransform), typeof(TextMeshProUGUI));
        go.transform.SetParent(targetCanvas.transform, false);

        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(1, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(1, 1);
        rect.anchoredPosition = new Vector2(-50, -120);
        rect.sizeDelta = new Vector2(200, 60);

        TextMeshProUGUI tmp = go.GetComponent<TextMeshProUGUI>();
        tmp.text = "♥ 5 / 5";
        tmp.fontSize = 36;
        tmp.alignment = TextAlignmentOptions.Right;
        tmp.color = new Color(1f, 0.3f, 0.3f);

        Undo.RegisterCreatedObjectUndo(go, "Create MissesText");
        return tmp;
    }

    private GameObject CreateGameOverPanel()
    {
        GameObject panel = new GameObject("GameOverPanel", typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
        panel.transform.SetParent(targetCanvas.transform, false);

        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image panelImage = panel.GetComponent<Image>();
        panelImage.color = panelColor;

        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;

        CreatePanelText(panel.transform, "TitleText", "GAME OVER", 72, 300);
        CreatePanelText(panel.transform, "GOScoreText", "Score: 0", 48, 100);
        CreatePanelText(panel.transform, "MaxComboText", "Max Combo: 0", 36, 30);
        CreatePanelText(panel.transform, "AccuracyText", "Accuracy: 0%", 36, -30);
        CreatePanelText(panel.transform, "ShapesText", "Shapes: 0", 36, -90);

        CreatePanelButton(panel.transform, "RetryButton", "RETRY", -200);
        CreatePanelButton(panel.transform, "MenuButton", "MENU", -280);

        Undo.RegisterCreatedObjectUndo(panel, "Create GameOverPanel");
        return panel;
    }

    private void CreatePanelText(Transform parent, string name, string text, int fontSize, float posY)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);

        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0, posY);
        rect.sizeDelta = new Vector2(600, fontSize + 20);

        TextMeshProUGUI tmp = go.GetComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
    }

    private void CreatePanelButton(Transform parent, string name, string text, float posY)
    {
        GameObject buttonGO = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        buttonGO.transform.SetParent(parent, false);

        RectTransform buttonRect = buttonGO.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.pivot = new Vector2(0.5f, 0.5f);
        buttonRect.anchoredPosition = new Vector2(0, posY);
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
        tmp.fontSize = 36;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
    }

    private void CreateTimeAttackTimer(TextMeshProUGUI timerText)
    {
        GameObject go = new GameObject("TimeAttackTimer");
        TimeAttackTimer timer = go.AddComponent<TimeAttackTimer>();
        SetPrivateField(timer, "timerText", timerText);
        Undo.RegisterCreatedObjectUndo(go, "Create TimeAttackTimer");
    }

    private void CreateLevelProgressUI(TextMeshProUGUI progressText)
    {
        GameObject go = new GameObject("LevelProgressUI");
        LevelProgressUI progress = go.AddComponent<LevelProgressUI>();
        SetPrivateField(progress, "progressText", progressText);
        Undo.RegisterCreatedObjectUndo(go, "Create LevelProgressUI");
    }

    private void CreateMissesUI(TextMeshProUGUI missesText)
    {
        GameObject go = new GameObject("MissesUI");
        MissesUI misses = go.AddComponent<MissesUI>();
        SetPrivateField(misses, "missesText", missesText);
        Undo.RegisterCreatedObjectUndo(go, "Create MissesUI");
    }

    private void CreateGameOverUI(GameObject panel)
    {
        GameObject go = new GameObject("GameOverUI");
        GameOverUI gameOverUI = go.AddComponent<GameOverUI>();

        SetPrivateField(gameOverUI, "panel", panel);
        SetPrivateField(gameOverUI, "canvasGroup", panel.GetComponent<CanvasGroup>());

        Transform panelTransform = panel.transform;
        SetPrivateField(gameOverUI, "titleText", panelTransform.Find("TitleText")?.GetComponent<TextMeshProUGUI>());
        SetPrivateField(gameOverUI, "scoreText", panelTransform.Find("GOScoreText")?.GetComponent<TextMeshProUGUI>());
        SetPrivateField(gameOverUI, "maxComboText", panelTransform.Find("MaxComboText")?.GetComponent<TextMeshProUGUI>());
        SetPrivateField(gameOverUI, "accuracyText", panelTransform.Find("AccuracyText")?.GetComponent<TextMeshProUGUI>());
        SetPrivateField(gameOverUI, "shapesText", panelTransform.Find("ShapesText")?.GetComponent<TextMeshProUGUI>());
        SetPrivateField(gameOverUI, "retryButton", panelTransform.Find("RetryButton")?.GetComponent<Button>());
        SetPrivateField(gameOverUI, "menuButton", panelTransform.Find("MenuButton")?.GetComponent<Button>());

        Undo.RegisterCreatedObjectUndo(go, "Create GameOverUI");
    }

    private void CreateLevelData()
    {
        LevelData levelData = ScriptableObject.CreateInstance<LevelData>();

        LevelInfo[] levels = new LevelInfo[30];
        for (int i = 0; i < 30; i++)
        {
            levels[i] = new LevelInfo
            {
                levelName = levelNames[i],
                shapesToComplete = Random.Range(5, 16)
            };
        }

        SetPrivateField(levelData, "levels", levels);

        string path = "Assets/RhythmGame/Data";
        if (!AssetDatabase.IsValidFolder(path))
        {
            AssetDatabase.CreateFolder("Assets/RhythmGame", "Data");
        }

        string assetPath = AssetDatabase.GenerateUniqueAssetPath($"{path}/GameLevels.asset");
        AssetDatabase.CreateAsset(levelData, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeObject = levelData;
        Debug.Log($"Level Data created at {assetPath} with 30 levels!");
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
