using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class AchievementsSetupWindow : EditorWindow
{
    private Canvas targetCanvas;

    private Color panelColor = new Color(0.08f, 0.08f, 0.12f, 0.95f);
    private Color cardUnlockedColor = new Color(0.2f, 0.25f, 0.35f, 1f);
    private Color cardLockedColor = new Color(0.12f, 0.12f, 0.15f, 0.9f);

    [MenuItem("RhythmGame/Setup Achievements")]
    public static void ShowWindow()
    {
        GetWindow<AchievementsSetupWindow>("Achievements Setup");
    }

    private void OnGUI()
    {
        GUILayout.Label("Achievements Setup", EditorStyles.boldLabel);
        GUILayout.Space(10);

        targetCanvas = (Canvas)EditorGUILayout.ObjectField("Target Canvas", targetCanvas, typeof(Canvas), true);

        GUILayout.Space(20);

        if (GUILayout.Button("Create Achievement Data (20 Achievements)", GUILayout.Height(30)))
        {
            CreateAchievementData();
        }

        GUILayout.Space(10);

        EditorGUI.BeginDisabledGroup(targetCanvas == null);
        if (GUILayout.Button("Create Achievements UI", GUILayout.Height(40)))
        {
            CreateAchievementsUI();
        }

        if (GUILayout.Button("Create Achievement Popup", GUILayout.Height(30)))
        {
            CreateAchievementPopup();
        }
        EditorGUI.EndDisabledGroup();

        GUILayout.Space(10);

        if (GUILayout.Button("Create Managers (Stats + Achievements)", GUILayout.Height(30)))
        {
            CreateManagers();
        }

        GUILayout.Space(10);

        if (targetCanvas == null)
        {
            EditorGUILayout.HelpBox("Drag a Canvas to create UI elements", MessageType.Info);
        }
    }

    private void CreateAchievementData()
    {
        AchievementData data = ScriptableObject.CreateInstance<AchievementData>();

        AchievementInfo[] achievements = new AchievementInfo[]
        {
            new AchievementInfo { id = "first_perfect", title = "First Blood", description = "Get your first Perfect hit", category = AchievementCategory.Beginner, condition = AchievementCondition.FirstPerfect, targetValue = 1, starReward = 1 },
            new AchievementInfo { id = "first_level", title = "Baby Steps", description = "Complete your first level", category = AchievementCategory.Beginner, condition = AchievementCondition.FirstLevelComplete, targetValue = 1, starReward = 1 },
            new AchievementInfo { id = "shapes_10", title = "Getting Started", description = "Complete 10 shapes total", category = AchievementCategory.Beginner, condition = AchievementCondition.TotalShapesCompleted, targetValue = 10, starReward = 1 },
            new AchievementInfo { id = "combo_10", title = "Combo Starter", description = "Reach a combo of 10", category = AchievementCategory.Beginner, condition = AchievementCondition.ComboReached, targetValue = 10, starReward = 1 },

            new AchievementInfo { id = "perfect_5_row", title = "On Fire", description = "Get 5 Perfects in a row", category = AchievementCategory.Skill, condition = AchievementCondition.PerfectsInRow, targetValue = 5, starReward = 2 },
            new AchievementInfo { id = "perfect_10_row", title = "Unstoppable", description = "Get 10 Perfects in a row", category = AchievementCategory.Skill, condition = AchievementCondition.PerfectsInRow, targetValue = 10, starReward = 3 },
            new AchievementInfo { id = "combo_25", title = "Combo Master", description = "Reach a combo of 25", category = AchievementCategory.Skill, condition = AchievementCondition.ComboReached, targetValue = 25, starReward = 2 },
            new AchievementInfo { id = "combo_50", title = "Combo Legend", description = "Reach a combo of 50", category = AchievementCategory.Skill, condition = AchievementCondition.ComboReached, targetValue = 50, starReward = 3 },
            new AchievementInfo { id = "no_miss_level", title = "Flawless Run", description = "Complete a level without missing", category = AchievementCategory.Skill, condition = AchievementCondition.NoMissesOnLevel, targetValue = 1, starReward = 3 },
            new AchievementInfo { id = "perfect_accuracy", title = "Perfectionist", description = "Get 100% Perfect accuracy on a level", category = AchievementCategory.Skill, condition = AchievementCondition.PerfectAccuracyOnLevel, targetValue = 1, starReward = 5 },

            new AchievementInfo { id = "shapes_50", title = "Shape Shifter", description = "Complete 50 shapes total", category = AchievementCategory.Endurance, condition = AchievementCondition.TotalShapesCompleted, targetValue = 50, starReward = 2 },
            new AchievementInfo { id = "shapes_100", title = "Century", description = "Complete 100 shapes total", category = AchievementCategory.Endurance, condition = AchievementCondition.TotalShapesCompleted, targetValue = 100, starReward = 3 },
            new AchievementInfo { id = "shapes_500", title = "Shape Veteran", description = "Complete 500 shapes total", category = AchievementCategory.Endurance, condition = AchievementCondition.TotalShapesCompleted, targetValue = 500, starReward = 5 },
            new AchievementInfo { id = "score_1000", title = "Point Collector", description = "Earn 1,000 total points", category = AchievementCategory.Endurance, condition = AchievementCondition.TotalScoreReached, targetValue = 1000, starReward = 1 },
            new AchievementInfo { id = "score_10000", title = "Score Hunter", description = "Earn 10,000 total points", category = AchievementCategory.Endurance, condition = AchievementCondition.TotalScoreReached, targetValue = 10000, starReward = 3 },
            new AchievementInfo { id = "playtime_30", title = "Dedicated", description = "Play for 30 minutes total", category = AchievementCategory.Endurance, condition = AchievementCondition.PlayTimeMinutes, targetValue = 30, starReward = 2 },

            new AchievementInfo { id = "levels_5", title = "Rising Star", description = "Complete 5 levels", category = AchievementCategory.Mastery, condition = AchievementCondition.LevelsCompleted, targetValue = 5, starReward = 2 },
            new AchievementInfo { id = "levels_15", title = "Halfway There", description = "Complete 15 levels", category = AchievementCategory.Mastery, condition = AchievementCondition.LevelsCompleted, targetValue = 15, starReward = 3 },
            new AchievementInfo { id = "levels_all", title = "Champion", description = "Complete all levels", category = AchievementCategory.Mastery, condition = AchievementCondition.AllLevelsCompleted, targetValue = 1, starReward = 10 },
            new AchievementInfo { id = "combo_100", title = "Rhythm God", description = "Reach a combo of 100", category = AchievementCategory.Mastery, condition = AchievementCondition.ComboReached, targetValue = 100, starReward = 10 },
        };

        SetPrivateField(data, "achievements", achievements);

        string path = "Assets/RhythmGame/Data";
        if (!AssetDatabase.IsValidFolder(path))
        {
            AssetDatabase.CreateFolder("Assets/RhythmGame", "Data");
        }

        string assetPath = AssetDatabase.GenerateUniqueAssetPath($"{path}/AchievementData.asset");
        AssetDatabase.CreateAsset(data, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeObject = data;
        Debug.Log($"Achievement Data created at {assetPath} with 20 achievements!");
    }

    private void CreateAchievementsUI()
    {
        GameObject panel = CreatePanel();
        GameObject header = CreateHeader(panel.transform);
        GameObject scrollView = CreateScrollView(panel.transform);
        Button backButton = CreateBackButton(panel.transform);
        AchievementCard cardPrefab = CreateAndSaveCardPrefab();

        GameObject achievementsUIObj = new GameObject("AchievementsUI");
        AchievementsUI achievementsUI = achievementsUIObj.AddComponent<AchievementsUI>();

        SetPrivateField(achievementsUI, "panelRect", panel.GetComponent<RectTransform>());
        SetPrivateField(achievementsUI, "panelCanvasGroup", panel.GetComponent<CanvasGroup>());
        SetPrivateField(achievementsUI, "titleText", header.transform.Find("TitleText")?.GetComponent<TextMeshProUGUI>());
        SetPrivateField(achievementsUI, "progressText", header.transform.Find("ProgressText")?.GetComponent<TextMeshProUGUI>());
        SetPrivateField(achievementsUI, "starsText", header.transform.Find("StarsText")?.GetComponent<TextMeshProUGUI>());
        SetPrivateField(achievementsUI, "scrollRect", scrollView.GetComponent<ScrollRect>());
        SetPrivateField(achievementsUI, "content", scrollView.transform.Find("Viewport/Content").GetComponent<RectTransform>());
        SetPrivateField(achievementsUI, "viewport", scrollView.transform.Find("Viewport").GetComponent<RectTransform>());
        SetPrivateField(achievementsUI, "cardPrefab", cardPrefab);
        SetPrivateField(achievementsUI, "backButton", backButton);

        Undo.RegisterCreatedObjectUndo(achievementsUIObj, "Create AchievementsUI");
        Undo.RegisterCreatedObjectUndo(panel, "Create AchievementsPanel");

        Debug.Log("Achievements UI created!");
    }

    private GameObject CreatePanel()
    {
        GameObject panel = new GameObject("AchievementsPanel", typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
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
        headerRect.anchoredPosition = Vector2.zero;
        headerRect.sizeDelta = new Vector2(0, 140);

        GameObject titleGO = new GameObject("TitleText", typeof(RectTransform), typeof(TextMeshProUGUI));
        titleGO.transform.SetParent(header.transform, false);

        RectTransform titleRect = titleGO.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.5f);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.offsetMin = new Vector2(20, 0);
        titleRect.offsetMax = new Vector2(-20, -15);

        TextMeshProUGUI titleTMP = titleGO.GetComponent<TextMeshProUGUI>();
        titleTMP.text = "ACHIEVEMENTS";
        titleTMP.fontSize = 48;
        titleTMP.alignment = TextAlignmentOptions.Center;
        titleTMP.color = Color.white;
        titleTMP.fontStyle = FontStyles.Bold;

        GameObject progressGO = new GameObject("ProgressText", typeof(RectTransform), typeof(TextMeshProUGUI));
        progressGO.transform.SetParent(header.transform, false);

        RectTransform progressRect = progressGO.GetComponent<RectTransform>();
        progressRect.anchorMin = new Vector2(0, 0);
        progressRect.anchorMax = new Vector2(0.5f, 0.5f);
        progressRect.offsetMin = new Vector2(30, 10);
        progressRect.offsetMax = new Vector2(0, 0);

        TextMeshProUGUI progressTMP = progressGO.GetComponent<TextMeshProUGUI>();
        progressTMP.text = "0 / 20";
        progressTMP.fontSize = 28;
        progressTMP.alignment = TextAlignmentOptions.Left;
        progressTMP.color = new Color(0.7f, 0.7f, 0.8f, 1f);

        GameObject starsGO = new GameObject("StarsText", typeof(RectTransform), typeof(TextMeshProUGUI));
        starsGO.transform.SetParent(header.transform, false);

        RectTransform starsRect = starsGO.GetComponent<RectTransform>();
        starsRect.anchorMin = new Vector2(0.5f, 0);
        starsRect.anchorMax = new Vector2(1, 0.5f);
        starsRect.offsetMin = new Vector2(0, 10);
        starsRect.offsetMax = new Vector2(-30, 0);

        TextMeshProUGUI starsTMP = starsGO.GetComponent<TextMeshProUGUI>();
        starsTMP.text = "0 ★";
        starsTMP.fontSize = 28;
        starsTMP.alignment = TextAlignmentOptions.Right;
        starsTMP.color = new Color(1f, 0.85f, 0.3f, 1f);

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
        scrollRect.offsetMax = new Vector2(0, -140);

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

    private AchievementCard CreateAndSaveCardPrefab()
    {
        GameObject cardGO = CreateCardGameObject();

        string prefabPath = "Assets/RhythmGame/Prefabs";
        if (!AssetDatabase.IsValidFolder(prefabPath))
        {
            AssetDatabase.CreateFolder("Assets/RhythmGame", "Prefabs");
        }

        string fullPath = $"{prefabPath}/AchievementCard.prefab";

        GameObject existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(fullPath);
        if (existingPrefab != null)
        {
            AssetDatabase.DeleteAsset(fullPath);
        }

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(cardGO, fullPath);
        DestroyImmediate(cardGO);

        Debug.Log($"Achievement card prefab saved to {fullPath}");

        return prefab.GetComponent<AchievementCard>();
    }

    private GameObject CreateCardGameObject()
    {
        GameObject card = new GameObject("AchievementCard", typeof(RectTransform), typeof(CanvasGroup), typeof(Image), typeof(AchievementCard));

        RectTransform cardRect = card.GetComponent<RectTransform>();
        cardRect.sizeDelta = new Vector2(400, 140);

        Image cardImage = card.GetComponent<Image>();
        cardImage.color = cardUnlockedColor;

        GameObject iconBG = new GameObject("IconBG", typeof(RectTransform), typeof(Image));
        iconBG.transform.SetParent(card.transform, false);

        RectTransform iconBGRect = iconBG.GetComponent<RectTransform>();
        iconBGRect.anchorMin = new Vector2(0, 0.5f);
        iconBGRect.anchorMax = new Vector2(0, 0.5f);
        iconBGRect.pivot = new Vector2(0, 0.5f);
        iconBGRect.anchoredPosition = new Vector2(15, 0);
        iconBGRect.sizeDelta = new Vector2(80, 80);

        Image iconBGImage = iconBG.GetComponent<Image>();
        iconBGImage.color = new Color(0.3f, 0.3f, 0.4f, 1f);

        GameObject iconImage = new GameObject("IconImage", typeof(RectTransform), typeof(Image));
        iconImage.transform.SetParent(iconBG.transform, false);

        RectTransform iconRect = iconImage.GetComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0.15f, 0.15f);
        iconRect.anchorMax = new Vector2(0.85f, 0.85f);
        iconRect.offsetMin = Vector2.zero;
        iconRect.offsetMax = Vector2.zero;

        Image iconImg = iconImage.GetComponent<Image>();
        iconImg.color = new Color(0.5f, 0.6f, 0.9f, 1f);

        GameObject categoryBadge = new GameObject("CategoryBadge", typeof(RectTransform), typeof(Image));
        categoryBadge.transform.SetParent(card.transform, false);

        RectTransform badgeRect = categoryBadge.GetComponent<RectTransform>();
        badgeRect.anchorMin = new Vector2(0, 1);
        badgeRect.anchorMax = new Vector2(0, 1);
        badgeRect.pivot = new Vector2(0, 1);
        badgeRect.anchoredPosition = new Vector2(110, -10);
        badgeRect.sizeDelta = new Vector2(8, 40);

        Image badgeImage = categoryBadge.GetComponent<Image>();
        badgeImage.color = new Color(0.4f, 0.7f, 0.4f, 1f);

        GameObject titleText = CreateCardText(card.transform, "TitleText", "Achievement Title", 28, TextAlignmentOptions.Left);
        RectTransform titleRect = titleText.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 1);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.pivot = new Vector2(0, 1);
        titleRect.anchoredPosition = new Vector2(125, -15);
        titleRect.sizeDelta = new Vector2(-180, 35);

        GameObject descText = CreateCardText(card.transform, "DescriptionText", "Achievement description goes here", 20, TextAlignmentOptions.Left);
        RectTransform descRect = descText.GetComponent<RectTransform>();
        descRect.anchorMin = new Vector2(0, 1);
        descRect.anchorMax = new Vector2(1, 1);
        descRect.pivot = new Vector2(0, 1);
        descRect.anchoredPosition = new Vector2(125, -55);
        descRect.sizeDelta = new Vector2(-180, 50);
        descText.GetComponent<TextMeshProUGUI>().color = new Color(0.7f, 0.7f, 0.8f, 1f);

        GameObject starsText = CreateCardText(card.transform, "StarsText", "3 ★", 24, TextAlignmentOptions.Right);
        RectTransform starsRect = starsText.GetComponent<RectTransform>();
        starsRect.anchorMin = new Vector2(1, 0);
        starsRect.anchorMax = new Vector2(1, 0);
        starsRect.pivot = new Vector2(1, 0);
        starsRect.anchoredPosition = new Vector2(-15, 15);
        starsRect.sizeDelta = new Vector2(80, 30);
        starsText.GetComponent<TextMeshProUGUI>().color = new Color(1f, 0.85f, 0.3f, 1f);

        GameObject lockedOverlay = new GameObject("LockedOverlay", typeof(RectTransform), typeof(Image));
        lockedOverlay.transform.SetParent(card.transform, false);

        RectTransform overlayRect = lockedOverlay.GetComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.offsetMin = Vector2.zero;
        overlayRect.offsetMax = Vector2.zero;

        Image overlayImage = lockedOverlay.GetComponent<Image>();
        overlayImage.color = new Color(0, 0, 0, 0.5f);
        lockedOverlay.SetActive(false);

        AchievementCard achievementCard = card.GetComponent<AchievementCard>();
        SetPrivateField(achievementCard, "cardRect", cardRect);
        SetPrivateField(achievementCard, "canvasGroup", card.GetComponent<CanvasGroup>());
        SetPrivateField(achievementCard, "backgroundImage", cardImage);
        SetPrivateField(achievementCard, "iconImage", iconImg);
        SetPrivateField(achievementCard, "categoryBadge", badgeImage);
        SetPrivateField(achievementCard, "titleText", titleText.GetComponent<TextMeshProUGUI>());
        SetPrivateField(achievementCard, "descriptionText", descText.GetComponent<TextMeshProUGUI>());
        SetPrivateField(achievementCard, "starsText", starsText.GetComponent<TextMeshProUGUI>());
        SetPrivateField(achievementCard, "lockedOverlay", lockedOverlay);

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

    private void CreateAchievementPopup()
    {
        GameObject popupGO = new GameObject("AchievementPopup", typeof(RectTransform), typeof(CanvasGroup), typeof(AchievementPopup));
        popupGO.transform.SetParent(targetCanvas.transform, false);

        RectTransform popupRect = popupGO.GetComponent<RectTransform>();
        popupRect.anchorMin = new Vector2(0.5f, 1);
        popupRect.anchorMax = new Vector2(0.5f, 1);
        popupRect.pivot = new Vector2(0.5f, 1);
        popupRect.anchoredPosition = new Vector2(0, -100);
        popupRect.sizeDelta = new Vector2(500, 100);

        GameObject bg = new GameObject("Background", typeof(RectTransform), typeof(Image));
        bg.transform.SetParent(popupGO.transform, false);

        RectTransform bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        Image bgImage = bg.GetComponent<Image>();
        bgImage.color = new Color(0.2f, 0.5f, 0.3f, 0.95f);

        GameObject iconBG = new GameObject("IconBG", typeof(RectTransform), typeof(Image));
        iconBG.transform.SetParent(popupGO.transform, false);

        RectTransform iconBGRect = iconBG.GetComponent<RectTransform>();
        iconBGRect.anchorMin = new Vector2(0, 0.5f);
        iconBGRect.anchorMax = new Vector2(0, 0.5f);
        iconBGRect.pivot = new Vector2(0, 0.5f);
        iconBGRect.anchoredPosition = new Vector2(15, 0);
        iconBGRect.sizeDelta = new Vector2(70, 70);

        Image iconBGImage = iconBG.GetComponent<Image>();
        iconBGImage.color = new Color(1, 1, 1, 0.2f);

        GameObject iconImage = new GameObject("IconImage", typeof(RectTransform), typeof(Image));
        iconImage.transform.SetParent(iconBG.transform, false);

        RectTransform iconRect = iconImage.GetComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0.15f, 0.15f);
        iconRect.anchorMax = new Vector2(0.85f, 0.85f);
        iconRect.offsetMin = Vector2.zero;
        iconRect.offsetMax = Vector2.zero;

        Image iconImg = iconImage.GetComponent<Image>();
        iconImg.color = Color.white;

        GameObject titleText = CreateCardText(popupGO.transform, "TitleText", "Achievement Unlocked!", 26, TextAlignmentOptions.Left);
        RectTransform titleRect = titleText.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.5f);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.pivot = new Vector2(0, 0.5f);
        titleRect.anchoredPosition = new Vector2(100, 0);
        titleRect.sizeDelta = new Vector2(-160, 0);
        titleText.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        GameObject descText = CreateCardText(popupGO.transform, "DescriptionText", "You did something awesome!", 18, TextAlignmentOptions.Left);
        RectTransform descRect = descText.GetComponent<RectTransform>();
        descRect.anchorMin = new Vector2(0, 0);
        descRect.anchorMax = new Vector2(1, 0.5f);
        descRect.pivot = new Vector2(0, 0.5f);
        descRect.anchoredPosition = new Vector2(100, 0);
        descRect.sizeDelta = new Vector2(-160, 0);
        descText.GetComponent<TextMeshProUGUI>().color = new Color(0.9f, 0.9f, 0.9f, 1f);

        GameObject starsText = CreateCardText(popupGO.transform, "StarsText", "+1 ★", 22, TextAlignmentOptions.Right);
        RectTransform starsRect = starsText.GetComponent<RectTransform>();
        starsRect.anchorMin = new Vector2(1, 0.5f);
        starsRect.anchorMax = new Vector2(1, 0.5f);
        starsRect.pivot = new Vector2(1, 0.5f);
        starsRect.anchoredPosition = new Vector2(-15, 0);
        starsRect.sizeDelta = new Vector2(80, 30);
        starsText.GetComponent<TextMeshProUGUI>().color = new Color(1f, 0.9f, 0.3f, 1f);

        AchievementPopup popup = popupGO.GetComponent<AchievementPopup>();
        SetPrivateField(popup, "popupRect", popupRect);
        SetPrivateField(popup, "canvasGroup", popupGO.GetComponent<CanvasGroup>());
        SetPrivateField(popup, "backgroundImage", bgImage);
        SetPrivateField(popup, "iconImage", iconImg);
        SetPrivateField(popup, "titleText", titleText.GetComponent<TextMeshProUGUI>());
        SetPrivateField(popup, "descriptionText", descText.GetComponent<TextMeshProUGUI>());
        SetPrivateField(popup, "starsText", starsText.GetComponent<TextMeshProUGUI>());

        Undo.RegisterCreatedObjectUndo(popupGO, "Create AchievementPopup");

        Debug.Log("Achievement Popup created!");
    }

    private void CreateManagers()
    {
        if (FindObjectOfType<StatsManager>() == null)
        {
            GameObject statsGO = new GameObject("StatsManager");
            statsGO.AddComponent<StatsManager>();
            Undo.RegisterCreatedObjectUndo(statsGO, "Create StatsManager");
            Debug.Log("StatsManager created!");
        }
        else
        {
            Debug.Log("StatsManager already exists.");
        }

        if (FindObjectOfType<AchievementManager>() == null)
        {
            GameObject achievementsGO = new GameObject("AchievementManager");
            achievementsGO.AddComponent<AchievementManager>();
            Undo.RegisterCreatedObjectUndo(achievementsGO, "Create AchievementManager");
            Debug.Log("AchievementManager created! Don't forget to assign AchievementData.");
        }
        else
        {
            Debug.Log("AchievementManager already exists.");
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
