using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class GameSceneSetupWindow : EditorWindow
{
    private Canvas targetCanvas;

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

        var fadePanel = CreateFadePanel(targetCanvas.transform);

        var initGO = new GameObject("GameSceneInit");
        var init = initGO.AddComponent<GameSceneInit>();
        SetPrivateField(init, "fadeImage", fadePanel.GetComponent<Image>());
        Undo.RegisterCreatedObjectUndo(initGO, "Create GameSceneInit");

        Selection.activeGameObject = targetCanvas.gameObject;
        EditorUtility.SetDirty(targetCanvas);

        Debug.Log("Game Scene UI created successfully!");
    }

    private GameObject CreateFadePanel(Transform parent)
    {
        var go = new GameObject("FadePanel", typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        
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
