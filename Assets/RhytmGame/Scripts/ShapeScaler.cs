using UnityEngine;

public class ShapeScaler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerShape;
    [SerializeField] private Transform targetShape;
    [SerializeField] private Camera mainCamera;

    [Header("Settings")]
    [SerializeField] [Range(0f, 0.4f)] private float screenPadding = 0.15f;
    [SerializeField] private float spriteWorldSize = 5.12f;

    [Header("Debug (Read Only)")]
    [SerializeField] private float calculatedScale;
    [SerializeField] private float screenWorldHeight;
    [SerializeField] private float screenWorldWidth;

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        CalculateAndApplyScale();
    }

    private void CalculateAndApplyScale()
    {
        if (mainCamera == null)
            return;

        screenWorldHeight = mainCamera.orthographicSize * 2f;
        screenWorldWidth = screenWorldHeight * mainCamera.aspect;

        float availableHeight = screenWorldHeight * (1f - screenPadding * 2f);
        float availableWidth = screenWorldWidth * (1f - screenPadding * 2f);

        float availableSize = Mathf.Min(availableWidth, availableHeight);

        calculatedScale = availableSize / spriteWorldSize;

        ApplyScale();
    }

    private void ApplyScale()
    {
        if (playerShape != null)
        {
            playerShape.localScale = Vector3.one * calculatedScale;
        }

        if (targetShape != null)
        {
            targetShape.localScale = Vector3.one * calculatedScale;
        }
    }

    public void SetReferences(Transform player, Transform target)
    {
        playerShape = player;
        targetShape = target;
        CalculateAndApplyScale();
    }

    public void Recalculate()
    {
        CalculateAndApplyScale();
    }

    public float GetCalculatedScale()
    {
        return calculatedScale;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying && mainCamera != null)
        {
            CalculateAndApplyScale();
        }
    }
#endif
}
