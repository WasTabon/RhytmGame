using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeArea : MonoBehaviour
{
    private RectTransform rectTransform;
    private Rect lastSafeArea = Rect.zero;
    private Vector2Int lastScreenSize = Vector2Int.zero;
    private ScreenOrientation lastOrientation = ScreenOrientation.AutoRotation;

    [SerializeField] private bool conformX = true;
    [SerializeField] private bool conformY = true;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        Refresh();
    }

    private void Update()
    {
        Refresh();
    }

    private void Refresh()
    {
        Rect safeArea = Screen.safeArea;

        if (safeArea != lastSafeArea
            || Screen.width != lastScreenSize.x
            || Screen.height != lastScreenSize.y
            || Screen.orientation != lastOrientation)
        {
            lastScreenSize.x = Screen.width;
            lastScreenSize.y = Screen.height;
            lastOrientation = Screen.orientation;
            lastSafeArea = safeArea;

            ApplySafeArea(safeArea);
        }
    }

    private void ApplySafeArea(Rect safeArea)
    {
        if (rectTransform == null) return;

        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        if (!conformX)
        {
            anchorMin.x = 0;
            anchorMax.x = 1;
        }

        if (!conformY)
        {
            anchorMin.y = 0;
            anchorMax.y = 1;
        }

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
    }
}
