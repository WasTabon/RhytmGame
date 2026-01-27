using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class GameSceneSetupWindow : EditorWindow
{
    private Canvas targetCanvas;

    private Color backgroundColor = new Color(0.08f, 0.08f, 0.12f, 1f);
    private Color panelColor = new Color(0.12f, 0.12f, 0.18f, 0.9f);
    private Color lowColor = new Color(1f, 0.3f, 0.3f, 1f);
    private Color midColor = new Color(0.3f, 1f, 0.3f, 1f);
    private Color highColor = new Color(0.3f, 0.5f, 1f, 1f);
    private Color textColor = new Color(0.9f, 0.9f, 0.9f, 1f);

    [MenuItem("RhythmGame/Setup Game Scene")]
    public static void ShowWindow()
    {
        GetWindow<GameSceneSetupWindow>("Game Scene Setup");
    }

    private void OnGUI()
    {
        GUILayout.Label("Game Scene Setup", EditorStyles.boldLabel);
        GUILayout.Space(10);

        targetCanvas = (Canvas)EditorGUILayout.ObjectField("Target Canvas", targetCanvas, typeof(Canvas), true);

        GUILayout.Space(20);

        EditorGUI.BeginDisabledGroup(targetCanvas == null);
        if (GUILayout.Button("Create Game Scene UI", GUILayout.Height(40)))
        {
            CreateGameSceneUI();
        }
        EditorGUI.EndDisabledGroup();

        if (targetCanvas == null)
        {
            EditorGUILayout.HelpBox("Drag a Canvas here to setup Game Scene UI", MessageType.Info);
        }
    }

    private void CreateGameSceneUI()
    {
        Undo.RegisterCompleteObjectUndo(targetCanvas.gameObject, "Create Game Scene UI");

        CreateBackground(targetCanvas.transform);
        var fadePanel = CreateFadePanel(targetCanvas.transform);
        var debugPanel = CreateDebugPanel(targetCanvas.transform);

        var audioManagerGO = new GameObject("MusicManager");
        var audioSource = audioManagerGO.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = true;
        var musicManager = audioManagerGO.AddComponent<MusicManager>();
        SetPrivateField(musicManager, "audioSource", audioSource);
        var audioAnalyzer = audioManagerGO.AddComponent<AudioAnalyzer>();
        SetPrivateField(audioAnalyzer, "audioSource", audioSource);
        Undo.RegisterCreatedObjectUndo(audioManagerGO, "Create MusicManager");

        var initGO = new GameObject("GameSceneInit");
        var init = initGO.AddComponent<GameSceneInit>();
        SetPrivateField(init, "fadeImage", fadePanel.GetComponent<Image>());
        Undo.RegisterCreatedObjectUndo(initGO, "Create GameSceneInit");

        var debugUIGO = new GameObject("AudioAnalyzerDebugUI");
        var debugUI = debugUIGO.AddComponent<AudioAnalyzerDebugUI>();
        SetPrivateField(debugUI, "audioAnalyzer", audioAnalyzer);
        SetPrivateField(debugUI, "lowBar", debugPanel.Find("LowBarContainer/LowBar").GetComponent<Image>());
        SetPrivateField(debugUI, "midBar", debugPanel.Find("MidBarContainer/MidBar").GetComponent<Image>());
        SetPrivateField(debugUI, "highBar", debugPanel.Find("HighBarContainer/HighBar").GetComponent<Image>());
        Undo.RegisterCreatedObjectUndo(debugUIGO, "Create AudioAnalyzerDebugUI");

        var gameAreaGO = new GameObject("GameArea");
        gameAreaGO.transform.position = Vector3.zero;
        Undo.RegisterCreatedObjectUndo(gameAreaGO, "Create GameArea");

        var playerShapeGO = new GameObject("PlayerShape");
        playerShapeGO.transform.SetParent(gameAreaGO.transform);
        playerShapeGO.transform.localPosition = Vector3.zero;
        var playerSpriteRenderer = playerShapeGO.AddComponent<SpriteRenderer>();
        playerSpriteRenderer.sortingOrder = 10;
        var playerShapeController = playerShapeGO.AddComponent<ShapeController>();
        SetPrivateField(playerShapeController, "spriteRenderer", playerSpriteRenderer);
        Undo.RegisterCreatedObjectUndo(playerShapeGO, "Create PlayerShape");

        Selection.activeGameObject = targetCanvas.gameObject;
        EditorUtility.SetDirty(targetCanvas);

        Debug.Log("Game Scene UI created! Assign a MusicPlaylist to MusicManager, or an AudioClip directly to AudioSource.");
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

    private RectTransform CreateDebugPanel(Transform parent)
    {
        var go = new GameObject("DebugPanel", typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);

        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(1, 0);
        rect.pivot = new Vector2(0.5f, 0);
        rect.anchoredPosition = new Vector2(0, 40);
        rect.sizeDelta = new Vector2(-80, 200);

        var image = go.GetComponent<Image>();
        image.color = panelColor;

        CreateBarWithLabel("LowBarContainer", "LowBar", "LOW", rect, 0, lowColor);
        CreateBarWithLabel("MidBarContainer", "MidBar", "MID", rect, 1, midColor);
        CreateBarWithLabel("HighBarContainer", "HighBar", "HIGH", rect, 2, highColor);

        Undo.RegisterCreatedObjectUndo(go, "Create DebugPanel");
        return rect;
    }

    private void CreateBarWithLabel(string containerName, string barName, string labelText, RectTransform parent, int index, Color barColor)
    {
        float xPos = -250 + index * 250;

        var container = new GameObject(containerName, typeof(RectTransform));
        container.transform.SetParent(parent, false);

        var containerRect = container.GetComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.5f, 0.5f);
        containerRect.anchorMax = new Vector2(0.5f, 0.5f);
        containerRect.pivot = new Vector2(0.5f, 0.5f);
        containerRect.anchoredPosition = new Vector2(xPos, 0);
        containerRect.sizeDelta = new Vector2(200, 150);

        var labelGO = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
        labelGO.transform.SetParent(container.transform, false);

        var labelRect = labelGO.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0, 1);
        labelRect.anchorMax = new Vector2(1, 1);
        labelRect.pivot = new Vector2(0.5f, 1);
        labelRect.anchoredPosition = new Vector2(0, 0);
        labelRect.sizeDelta = new Vector2(0, 30);

        var labelTmp = labelGO.GetComponent<TextMeshProUGUI>();
        labelTmp.text = labelText;
        labelTmp.fontSize = 24;
        labelTmp.alignment = TextAlignmentOptions.Center;
        labelTmp.color = barColor;

        var barBgGO = new GameObject("BarBackground", typeof(RectTransform), typeof(Image));
        barBgGO.transform.SetParent(container.transform, false);

        var barBgRect = barBgGO.GetComponent<RectTransform>();
        barBgRect.anchorMin = new Vector2(0, 0);
        barBgRect.anchorMax = new Vector2(1, 1);
        barBgRect.offsetMin = new Vector2(10, 10);
        barBgRect.offsetMax = new Vector2(-10, -35);

        var barBgImage = barBgGO.GetComponent<Image>();
        barBgImage.color = new Color(0.1f, 0.1f, 0.1f, 1f);

        var barGO = new GameObject(barName, typeof(RectTransform), typeof(Image));
        barGO.transform.SetParent(container.transform, false);

        var barRect = barGO.GetComponent<RectTransform>();
        barRect.anchorMin = new Vector2(0, 0);
        barRect.anchorMax = new Vector2(1, 1);
        barRect.offsetMin = new Vector2(10, 10);
        barRect.offsetMax = new Vector2(-10, -35);

        var barImage = barGO.GetComponent<Image>();
        barImage.color = barColor;
        barImage.type = Image.Type.Filled;
        barImage.fillMethod = Image.FillMethod.Vertical;
        barImage.fillOrigin = 0;
        barImage.fillAmount = 0.5f;

        Undo.RegisterCreatedObjectUndo(container, "Create Bar Container");
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
