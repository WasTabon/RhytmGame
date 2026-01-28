using UnityEngine;
using UnityEditor;
using TMPro;

public class HUDSetupWindow : EditorWindow
{
    private Canvas targetCanvas;

    [MenuItem("RhythmGame/Setup HUD Texts")]
    public static void ShowWindow()
    {
        GetWindow<HUDSetupWindow>("HUD Setup");
    }

    private void OnGUI()
    {
        GUILayout.Label("HUD Setup", EditorStyles.boldLabel);
        GUILayout.Space(10);

        targetCanvas = (Canvas)EditorGUILayout.ObjectField("Target Canvas", targetCanvas, typeof(Canvas), true);

        GUILayout.Space(20);

        EditorGUI.BeginDisabledGroup(targetCanvas == null);
        if (GUILayout.Button("Create HUD Texts", GUILayout.Height(40)))
        {
            CreateHUDTexts();
        }
        EditorGUI.EndDisabledGroup();

        if (targetCanvas == null)
        {
            EditorGUILayout.HelpBox("Drag a Canvas here to create HUD texts", MessageType.Info);
        }
    }

    private void CreateHUDTexts()
    {
        CreateScoreText();
        CreateComboText();
        CreateRoundText();

        Selection.activeGameObject = targetCanvas.gameObject;
        Debug.Log("HUD Texts created! Assign them to GameHUD component.");
    }

    private void CreateScoreText()
    {
        GameObject go = new GameObject("ScoreText", typeof(RectTransform), typeof(TextMeshProUGUI));
        go.transform.SetParent(targetCanvas.transform, false);

        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = new Vector2(150, -50);
        rect.sizeDelta = new Vector2(400, 60);

        TextMeshProUGUI tmp = go.GetComponent<TextMeshProUGUI>();
        tmp.text = "SCORE: 0";
        tmp.fontSize = 48;
        tmp.alignment = TextAlignmentOptions.Left;
        tmp.color = Color.white;

        Undo.RegisterCreatedObjectUndo(go, "Create ScoreText");
    }

    private void CreateComboText()
    {
        GameObject go = new GameObject("ComboText", typeof(RectTransform), typeof(TextMeshProUGUI));
        go.transform.SetParent(targetCanvas.transform, false);

        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = new Vector2(150, -110);
        rect.sizeDelta = new Vector2(300, 50);

        TextMeshProUGUI tmp = go.GetComponent<TextMeshProUGUI>();
        tmp.text = "COMBO: x0";
        tmp.fontSize = 36;
        tmp.alignment = TextAlignmentOptions.Left;
        tmp.color = Color.white;

        Undo.RegisterCreatedObjectUndo(go, "Create ComboText");
    }

    private void CreateRoundText()
    {
        GameObject go = new GameObject("RoundText", typeof(RectTransform), typeof(TextMeshProUGUI));
        go.transform.SetParent(targetCanvas.transform, false);

        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(0, 0);
        rect.pivot = new Vector2(0, 0);
        rect.anchoredPosition = new Vector2(150, 250);
        rect.sizeDelta = new Vector2(300, 50);

        TextMeshProUGUI tmp = go.GetComponent<TextMeshProUGUI>();
        tmp.text = "ROUND: 1";
        tmp.fontSize = 36;
        tmp.alignment = TextAlignmentOptions.Left;
        tmp.color = Color.white;

        Undo.RegisterCreatedObjectUndo(go, "Create RoundText");
    }
}
