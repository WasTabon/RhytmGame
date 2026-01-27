using UnityEngine;
using System;

public enum LockResult
{
    Perfect,
    Good,
    Miss
}

public class LockMechanic : MonoBehaviour
{
    public static LockMechanic Instance { get; private set; }

    [Header("References")]
    [SerializeField] private ShapeController playerShape;
    [SerializeField] private ShapeController targetShape;
    [SerializeField] private RotationController rotationController;

    [Header("Timing Windows")]
    [SerializeField] private float perfectAngleTolerance = 5f;
    [SerializeField] private float goodAngleTolerance = 15f;

    [Header("Color Matching")]
    [SerializeField] private bool requireColorMatch = true;

    [Header("Debug (Read Only)")]
    [SerializeField] private float lastAngleDifference;
    [SerializeField] private bool lastColorsMatched;
    [SerializeField] private LockResult lastResult;

    public event Action<LockResult> OnLock;
    public event Action OnMiss;

    private bool canLock = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        if (!canLock)
            return;

        if (GetInputDown())
        {
            TryLock();
        }
    }

    private bool GetInputDown()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
            return true;
#endif

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            return true;

        if (Input.GetMouseButtonDown(0))
            return true;

        return false;
    }

    private void TryLock()
    {
        if (playerShape == null || targetShape == null)
            return;

        float playerAngle = NormalizeAngle(playerShape.GetRotation());
        float targetAngle = NormalizeAngle(targetShape.GetRotation());

        float angleDifference = Mathf.Abs(Mathf.DeltaAngle(playerAngle, targetAngle));
        lastAngleDifference = angleDifference;

        bool colorsMatch = CheckColorsMatch();
        lastColorsMatched = colorsMatch;

        LockResult result = EvaluateResult(angleDifference, colorsMatch);
        lastResult = result;

        if (rotationController != null)
        {
            rotationController.StopRotation();
        }

        OnLock?.Invoke(result);

        if (result == LockResult.Miss)
        {
            OnMiss?.Invoke();
        }
    }

    private float NormalizeAngle(float angle)
    {
        angle = angle % 360f;
        if (angle < 0) angle += 360f;
        return angle;
    }

    private bool CheckColorsMatch()
    {
        if (!requireColorMatch)
            return true;

        Color[] playerColors = playerShape.BladeColors;
        Color[] targetColors = targetShape.BladeColors;

        if (playerColors == null || targetColors == null)
            return true;

        if (playerColors.Length != targetColors.Length)
            return false;

        float playerAngle = NormalizeAngle(playerShape.GetRotation());
        float targetAngle = NormalizeAngle(targetShape.GetRotation());
        float angleDiff = Mathf.DeltaAngle(playerAngle, targetAngle);

        int bladeCount = playerColors.Length;
        float anglePerBlade = 360f / bladeCount;

        int offset = Mathf.RoundToInt(angleDiff / anglePerBlade);
        offset = ((offset % bladeCount) + bladeCount) % bladeCount;

        for (int i = 0; i < bladeCount; i++)
        {
            int targetIndex = (i + offset) % bladeCount;
            if (!ColorsApproximatelyEqual(playerColors[i], targetColors[targetIndex]))
            {
                return false;
            }
        }

        return true;
    }

    private bool ColorsApproximatelyEqual(Color a, Color b)
    {
        float threshold = 0.1f;
        return Mathf.Abs(a.r - b.r) < threshold &&
               Mathf.Abs(a.g - b.g) < threshold &&
               Mathf.Abs(a.b - b.b) < threshold;
    }

    private LockResult EvaluateResult(float angleDifference, bool colorsMatch)
    {
        if (angleDifference <= perfectAngleTolerance && colorsMatch)
        {
            return LockResult.Perfect;
        }
        else if (angleDifference <= goodAngleTolerance)
        {
            return colorsMatch ? LockResult.Perfect : LockResult.Good;
        }
        else
        {
            return LockResult.Miss;
        }
    }

    public void SetCanLock(bool value)
    {
        canLock = value;
    }

    public void SetReferences(ShapeController player, ShapeController target, RotationController rotation)
    {
        playerShape = player;
        targetShape = target;
        rotationController = rotation;
    }

    public float GetLastAngleDifference()
    {
        return lastAngleDifference;
    }

    public bool GetLastColorsMatched()
    {
        return lastColorsMatched;
    }

    public LockResult GetLastResult()
    {
        return lastResult;
    }
}
