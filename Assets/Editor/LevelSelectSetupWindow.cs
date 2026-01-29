using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class LevelSelectSetupWindow : EditorWindow
{
    private Canvas targetCanvas;
    private LevelData levelData;

    private Color panelColor = new Color(0.08f, 0.08f, 0.12f, 0.95f);
    private Color cardUnlockedColor = new Color(0.2f, 0.25f, 0.35f, 1f);
    private Color cardLockedColor = new Color(0.15f, 0.15f, 0.2f, 0.7f);
    private Color accentColor = new Color(0.4f, 0.7f, 1f, 1f);

    [MenuItem("RhythmGame/Setup Level Select UI")]
    public static void ShowWindow()
    {
        GetWindow<LevelSelectSetupWindow>("Level Select Setup");
    }

    private void OnGUI()
    {
        GUILayout.Label("Level Select UI Setup", EditorStyles.boldLabel);
        GUILayout.Space(10);

        targetCanvas = (Canvas)EditorGUILayout.ObjectField("Target Canvas", targetCanvas, typeof(Canvas), true);
        levelData = (LevelData)EditorGUILayout.ObjectField("Level Data", levelData, typeof(LevelData), false);

        GUILayout.Space(20);

        EditorGUI.BeginDisabledGroup(targetCanvas == null);
        if (GUILayout.Button("Create Level Select UI", GUILayout.Height(40)))
        {
            CreateLevelSelectUI();
        }
        EditorGUI.EndDisabledGroup();

        GUILayout.Space(10);

        if (GUILayout.Button("Create Card Prefab Only", GUILayout.Height(30)))
        {
            CreateCardPrefab();
        }

        GUILayout.Space(10);

        if (targetCanvas == null)
        {
            EditorGUILayout.HelpBox("Drag a Canvas to create Level Select UI", MessageType.Info);
        }

        if (levelData == null)
        {
            EditorGUILayout.HelpBox("Assign Level Data for the list to work properly", MessageType.Warning);
        }
    }

    private void CreateLevelSelectUI()
    {
        GameObject panel = CreatePanel();
        GameObject header = CreateHeader(panel.transform);
        GameObject scrollView = CreateScrollView(panel.transform);
        Button backButton = CreateBackButton(panel.transform);
        LevelCard cardPrefab = CreateAndSaveCardPrefab();

        GameObject levelSelectObj = new GameObject("LevelSelectUI");
        LevelSelectUI levelSelectUI = levelSelectObj.AddComponent<LevelSelectUI>();

        SetPrivateField(levelSelectUI, "panelRect", panel.GetComponent<RectTransform>());
        SetPrivateField(levelSelectUI, "panelCanvasGroup", panel.GetComponent<CanvasGroup>());
        SetPrivateField(levelSelectUI, "scrollRect", scrollView.GetComponent<ScrollRect>());
        SetPrivateField(levelSelectUI, "content", scrollView.transform.Find("Viewport/Content").GetComponent<RectTransform>());
        SetPrivateField(levelSelectUI, "viewport", scrollView.transform.Find("Viewport").GetComponent<RectTransform>());
        SetPrivateField(levelSelectUI, "cardPrefab", cardPrefab);
        SetPrivateField(levelSelectUI, "backButton", backButton);

        if (levelData != null)
        {
            SetPrivateField(levelSelectUI, "levelData", levelData);
        }

        Undo.RegisterCreatedObjectUndo(levelSelectObj, "Create LevelSelectUI");
        Undo.RegisterCreatedObjectUndo(panel, "Create LevelSelectPanel");

        UpdateMainMenuController(levelSelectUI);

        Selection.activeGameObject = panel;
        Debug.Log("Level Select UI created! Don't forget to assign Level Data and connect to MainMenuController.");
    }

    private GameObject CreatePanel()
    {
        GameObject panel = new GameObject("LevelSelectPanel", typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
        panel.transform.SetParent(targetCanvas.transform, false);

        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image panelImage = panel.GetComponent<Image>();
        panelImage.color = panelColor;

        return panel;
    }

    private GameObject CreateHeader(Transform parent)
    {
        GameObject header = new GameObject("Header", typeof(RectTransform));
        header.transform.SetParent(parent, false);

        RectTransform headerRect = header.GetComponent<RectTransform>();
        headerRect.anchorMin = new Vector2(0, 1);
        headerRect.anchorMax = new Vector2(1, 1);
        headerRect.pivot = new Vector2(0.5f, 1);
        headerRect.anchoredPosition = new Vector2(0, 0);
        headerRect.sizeDelta = new Vector2(0, 120);

        GameObject titleGO = new GameObject("TitleText", typeof(RectTransform), typeof(TextMeshProUGUI));
        titleGO.transform.SetParent(header.transform, false);

        RectTransform titleRect = titleGO.GetComponent<RectTransform>();
        titleRect.anchorMin = Vector2.zero;
        titleRect.anchorMax = Vector2.one;
        titleRect.offsetMin = new Vector2(20, 20);
        titleRect.offsetMax = new Vector2(-20, -20);

        TextMeshProUGUI titleTMP = titleGO.GetComponent<TextMeshProUGUI>();
        titleTMP.text = "SELECT LEVEL";
        titleTMP.fontSize = 56;
        titleTMP.alignment = TextAlignmentOptions.Center;
        titleTMP.color = Color.white;
        titleTMP.fontStyle = FontStyles.Bold;

        return header;
    }

    private GameObject CreateScrollView(Transform parent)
    {
        GameObject scrollView = new GameObject("ScrollView", typeof(RectTransform), typeof(ScrollRect));
        scrollView.transform.SetParent(parent, false);

        RectTransform scrollRect = scrollView.GetComponent<RectTransform>();
        scrollRect.anchorMin = Vector2.zero;
        scrollRect.anchorMax = Vector2.one;
        scrollRect.offsetMin = new Vector2(0, 100);
        scrollRect.offsetMax = new Vector2(0, -120);

        GameObject viewport = new GameObject("Viewport", typeof(RectTransform), typeof(Image), typeof(Mask));
        viewport.transform.SetParent(scrollView.transform, false);

        RectTransform viewportRect = viewport.GetComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = Vector2.zero;
        viewportRect.offsetMax = Vector2.zero;

        Image viewportImage = viewport.GetComponent<Image>();
        viewportImage.color = new Color(1, 1, 1, 0.01f);

        Mask viewportMask = viewport.GetComponent<Mask>();
        viewportMask.showMaskGraphic = false;

        GameObject content = new GameObject("Content", typeof(RectTransform));
        content.transform.SetParent(viewport.transform, false);

        RectTransform contentRect = content.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.pivot = new Vector2(0.5f, 1);
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.sizeDelta = new Vector2(0, 2000);

        ScrollRect scroll = scrollView.GetComponent<ScrollRect>();
        scroll.content = contentRect;
        scroll.viewport = viewportRect;
        scroll.horizontal = false;
        scroll.vertical = true;
        scroll.movementType = ScrollRect.MovementType.Elastic;
        scroll.elasticity = 0.1f;
        scroll.inertia = true;
        scroll.decelerationRate = 0.135f;
        scroll.scrollSensitivity = 20f;

        return scrollView;
    }

    private Button CreateBackButton(Transform parent)
    {
        GameObject buttonGO = new GameObject("BackButton", typeof(RectTransform), typeof(Image), typeof(Button));
        buttonGO.transform.SetParent(parent, false);

        RectTransform buttonRect = buttonGO.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0);
        buttonRect.anchorMax = new Vector2(0.5f, 0);
        buttonRect.pivot = new Vector2(0.5f, 0);
        buttonRect.anchoredPosition = new Vector2(0, 20);
        buttonRect.sizeDelta = new Vector2(200, 60);

        Image buttonImage = buttonGO.GetComponent<Image>();
        buttonImage.color = new Color(0.3f, 0.3f, 0.4f, 1f);

        GameObject textGO = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        textGO.transform.SetParent(buttonGO.transform, false);

        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        TextMeshProUGUI buttonTMP = textGO.GetComponent<TextMeshProUGUI>();
        buttonTMP.text = "BACK";
        buttonTMP.fontSize = 32;
        buttonTMP.alignment = TextAlignmentOptions.Center;
        buttonTMP.color = Color.white;

        return buttonGO.GetComponent<Button>();
    }

    private LevelCard CreateAndSaveCardPrefab()
    {
        GameObject cardGO = CreateCardGameObject();

        string prefabPath = "Assets/RhythmGame/Prefabs";
        if (!AssetDatabase.IsValidFolder(prefabPath))
        {
            AssetDatabase.CreateFolder("Assets/RhythmGame", "Prefabs");
        }

        string fullPath = $"{prefabPath}/LevelCard.prefab";
        
        GameObject existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(fullPath);
        if (existingPrefab != null)
        {
            AssetDatabase.DeleteAsset(fullPath);
        }

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(cardGO, fullPath);
        DestroyImmediate(cardGO);

        Debug.Log($"Card prefab saved to {fullPath}");

        return prefab.GetComponent<LevelCard>();
    }

    private void CreateCardPrefab()
    {
        CreateAndSaveCardPrefab();
        Debug.Log("Card prefab created!");
    }

    private GameObject CreateCardGameObject()
    {
        GameObject card = new GameObject("LevelCard", typeof(RectTransform), typeof(CanvasGroup), typeof(Image), typeof(Button), typeof(LevelCard));

        RectTransform cardRect = card.GetComponent<RectTransform>();
        cardRect.sizeDelta = new Vector2(400, 180);

        Image cardImage = card.GetComponent<Image>();
        cardImage.color = cardUnlockedColor;

        GameObject numberBG = new GameObject("NumberBG", typeof(RectTransform), typeof(Image));
        numberBG.transform.SetParent(card.transform, false);

        RectTransform numberBGRect = numberBG.GetComponent<RectTransform>();
        numberBGRect.anchorMin = new Vector2(0, 0.5f);
        numberBGRect.anchorMax = new Vector2(0, 0.5f);
        numberBGRect.pivot = new Vector2(0, 0.5f);
        numberBGRect.anchoredPosition = new Vector2(15, 0);
        numberBGRect.sizeDelta = new Vector2(70, 70);

        Image numberBGImage = numberBG.GetComponent<Image>();
        numberBGImage.color = accentColor;

        GameObject numberText = CreateCardText(numberBG.transform, "NumberText", "1", 42, TextAlignmentOptions.Center);
        RectTransform numberTextRect = numberText.GetComponent<RectTransform>();
        numberTextRect.anchorMin = Vector2.zero;
        numberTextRect.anchorMax = Vector2.one;
        numberTextRect.offsetMin = Vector2.zero;
        numberTextRect.offsetMax = Vector2.zero;
        numberText.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        GameObject nameText = CreateCardText(card.transform, "NameText", "Level Name", 36, TextAlignmentOptions.Left);
        RectTransform nameRect = nameText.GetComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0, 1);
        nameRect.anchorMax = new Vector2(1, 1);
        nameRect.pivot = new Vector2(0, 1);
        nameRect.anchoredPosition = new Vector2(100, -20);
        nameRect.sizeDelta = new Vector2(-150, 45);

        GameObject shapesText = CreateCardText(card.transform, "ShapesText", "5 shapes", 24, TextAlignmentOptions.Left);
        RectTransform shapesRect = shapesText.GetComponent<RectTransform>();
        shapesRect.anchorMin = new Vector2(0, 1);
        shapesRect.anchorMax = new Vector2(1, 1);
        shapesRect.pivot = new Vector2(0, 1);
        shapesRect.anchoredPosition = new Vector2(100, -65);
        shapesRect.sizeDelta = new Vector2(-150, 30);
        shapesText.GetComponent<TextMeshProUGUI>().color = new Color(0.7f, 0.7f, 0.8f, 1f);

        GameObject bestScoreText = CreateCardText(card.transform, "BestScoreText", "Best: ---", 22, TextAlignmentOptions.Left);
        RectTransform bestScoreRect = bestScoreText.GetComponent<RectTransform>();
        bestScoreRect.anchorMin = new Vector2(0, 0);
        bestScoreRect.anchorMax = new Vector2(0.5f, 0);
        bestScoreRect.pivot = new Vector2(0, 0);
        bestScoreRect.anchoredPosition = new Vector2(100, 20);
        bestScoreRect.sizeDelta = new Vector2(-110, 30);
        bestScoreText.GetComponent<TextMeshProUGUI>().color = new Color(0.6f, 0.8f, 0.6f, 1f);

        GameObject bestComboText = CreateCardText(card.transform, "BestComboText", "Combo: ---", 22, TextAlignmentOptions.Left);
        RectTransform bestComboRect = bestComboText.GetComponent<RectTransform>();
        bestComboRect.anchorMin = new Vector2(0.5f, 0);
        bestComboRect.anchorMax = new Vector2(1, 0);
        bestComboRect.pivot = new Vector2(0, 0);
        bestComboRect.anchoredPosition = new Vector2(10, 20);
        bestComboRect.sizeDelta = new Vector2(-60, 30);
        bestComboText.GetComponent<TextMeshProUGUI>().color = new Color(0.6f, 0.7f, 0.9f, 1f);

        GameObject lockIcon = new GameObject("LockIcon", typeof(RectTransform), typeof(Image));
        lockIcon.transform.SetParent(card.transform, false);

        RectTransform lockRect = lockIcon.GetComponent<RectTransform>();
        lockRect.anchorMin = new Vector2(1, 0.5f);
        lockRect.anchorMax = new Vector2(1, 0.5f);
        lockRect.pivot = new Vector2(1, 0.5f);
        lockRect.anchoredPosition = new Vector2(-20, 0);
        lockRect.sizeDelta = new Vector2(50, 50);

        Image lockImage = lockIcon.GetComponent<Image>();
        lockImage.color = new Color(0.5f, 0.5f, 0.5f, 0.8f);

        GameObject completedIcon = new GameObject("CompletedIcon", typeof(RectTransform), typeof(Image));
        completedIcon.transform.SetParent(card.transform, false);

        RectTransform completedRect = completedIcon.GetComponent<RectTransform>();
        completedRect.anchorMin = new Vector2(1, 0.5f);
        completedRect.anchorMax = new Vector2(1, 0.5f);
        completedRect.pivot = new Vector2(1, 0.5f);
        completedRect.anchoredPosition = new Vector2(-20, 0);
        completedRect.sizeDelta = new Vector2(50, 50);

        Image completedImage = completedIcon.GetComponent<Image>();
        completedImage.color = new Color(0.3f, 0.9f, 0.4f, 1f);
        completedIcon.SetActive(false);

        LevelCard levelCard = card.GetComponent<LevelCard>();
        SetPrivateField(levelCard, "cardRect", cardRect);
        SetPrivateField(levelCard, "canvasGroup", card.GetComponent<CanvasGroup>());
        SetPrivateField(levelCard, "backgroundImage", cardImage);
        SetPrivateField(levelCard, "lockIcon", lockImage);
        SetPrivateField(levelCard, "completedIcon", completedImage);
        SetPrivateField(levelCard, "numberText", numberText.GetComponent<TextMeshProUGUI>());
        SetPrivateField(levelCard, "nameText", nameText.GetComponent<TextMeshProUGUI>());
        SetPrivateField(levelCard, "shapesText", shapesText.GetComponent<TextMeshProUGUI>());
        SetPrivateField(levelCard, "bestScoreText", bestScoreText.GetComponent<TextMeshProUGUI>());
        SetPrivateField(levelCard, "bestComboText", bestComboText.GetComponent<TextMeshProUGUI>());
        SetPrivateField(levelCard, "cardButton", card.GetComponent<Button>());

        return card;
    }

    private GameObject CreateCardText(Transform parent, string name, string text, int fontSize, TextAlignmentOptions alignment)
    {
        GameObject textGO = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        textGO.transform.SetParent(parent, false);

        TextMeshProUGUI tmp = textGO.GetComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = alignment;
        tmp.color = Color.white;

        return textGO;
    }

    private void UpdateMainMenuController(LevelSelectUI levelSelectUI)
    {
        MainMenuController mainMenu = FindObjectOfType<MainMenuController>();
        if (mainMenu != null)
        {
            SetPrivateField(mainMenu, "levelSelectUI", levelSelectUI);
            EditorUtility.SetDirty(mainMenu);
            Debug.Log("MainMenuController updated with LevelSelectUI reference!");
        }
        else
        {
            Debug.LogWarning("MainMenuController not found in scene. Please assign LevelSelectUI manually.");
        }
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
