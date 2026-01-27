using UnityEngine;
using System;
using DG.Tweening;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private ShapeController playerShape;
    [SerializeField] private ShapeController targetShape;
    [SerializeField] private RotationController rotationController;
    [SerializeField] private LockMechanic lockMechanic;
    [SerializeField] private ShapeData shapeData;

    [Header("Target Settings")]
    [SerializeField] private float targetAlpha = 0.3f;
    [SerializeField] private int targetSortingOrder = 5;

    [Header("Round Transition")]
    [SerializeField] private float delayAfterLock = 0.5f;
    [SerializeField] private float transitionDuration = 0.3f;

    [Header("Debug (Read Only)")]
    [SerializeField] private int currentRound;

    public event Action<int> OnRoundStart;
    public event Action<LockResult> OnRoundEnd;

    public int CurrentRound => currentRound;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (lockMechanic == null)
            lockMechanic = LockMechanic.Instance;

        if (lockMechanic != null)
        {
            lockMechanic.OnLock += HandleLock;
        }

        SetupTargetShape();
        StartNewRound();
    }

    private void OnDestroy()
    {
        if (lockMechanic != null)
        {
            lockMechanic.OnLock -= HandleLock;
        }
    }

    private void SetupTargetShape()
    {
        if (targetShape == null)
            return;

        targetShape.SetAlpha(targetAlpha);
        
        if (targetShape.SpriteRenderer != null)
        {
            targetShape.SpriteRenderer.sortingOrder = targetSortingOrder;
        }
    }

    private void HandleLock(LockResult result)
    {
        OnRoundEnd?.Invoke(result);

        if (result == LockResult.Miss)
        {
            DOVirtual.DelayedCall(delayAfterLock, () =>
            {
                RestartCurrentRound();
            });
        }
        else
        {
            DOVirtual.DelayedCall(delayAfterLock, () =>
            {
                StartNewRound();
            });
        }
    }

    public void StartNewRound()
    {
        currentRound++;

        if (shapeData == null || shapeData.ShapeCount == 0)
        {
            Debug.LogWarning("RoundManager: No ShapeData assigned or empty!");
            return;
        }

        int shapeIndex = UnityEngine.Random.Range(0, shapeData.ShapeCount);
        Sprite selectedShape = shapeData.GetShape(shapeIndex);
        
        int bladeCount = ParseBladeCount(selectedShape.name);

        Color[] colors = GenerateRandomColors(bladeCount);

        float targetAngle = UnityEngine.Random.Range(0f, 360f);

        SetupTarget(selectedShape, colors, targetAngle, bladeCount);
        SetupPlayer(selectedShape, colors, bladeCount);

        if (lockMechanic != null)
        {
            lockMechanic.SetCanLock(true);
        }

        OnRoundStart?.Invoke(currentRound);
    }

    private void RestartCurrentRound()
    {
        if (rotationController != null)
        {
            rotationController.ResetRotation();
            rotationController.ResumeRotation();
        }

        if (lockMechanic != null)
        {
            lockMechanic.SetCanLock(true);
        }
    }

    private void SetupTarget(Sprite shape, Color[] colors, float angle, int bladeCount)
    {
        if (targetShape == null)
            return;

        targetShape.SetShape(shape);
        targetShape.SetRotation(angle);
        targetShape.SetBladeColors(colors);
        targetShape.CreateBladeMarkers();
        targetShape.SetAlpha(targetAlpha);
    }

    private void SetupPlayer(Sprite shape, Color[] colors, int bladeCount)
    {
        if (playerShape == null)
            return;

        playerShape.SetShape(shape);
        playerShape.SetBladeColors(colors);
        playerShape.CreateBladeMarkers();

        if (rotationController != null)
        {
            rotationController.ResetRotation();
            rotationController.ResumeRotation();
        }
    }

    private Color[] GenerateRandomColors(int count)
    {
        Color[] possibleColors = new Color[]
        {
            new Color(1f, 0.3f, 0.3f),
            new Color(0.3f, 1f, 0.3f),
            new Color(0.3f, 0.5f, 1f),
            new Color(1f, 1f, 0.3f),
            new Color(1f, 0.5f, 0f),
            new Color(0.8f, 0.3f, 1f),
            new Color(0f, 1f, 1f)
        };

        Color[] colors = new Color[count];
        for (int i = 0; i < count; i++)
        {
            colors[i] = possibleColors[UnityEngine.Random.Range(0, possibleColors.Length)];
        }

        return colors;
    }

    private int ParseBladeCount(string spriteName)
    {
        for (int i = 3; i <= 7; i++)
        {
            if (spriteName.Contains(i.ToString() + "blade") ||
                spriteName.Contains(i.ToString() + "_blade") ||
                spriteName.Contains(i.ToString() + "Blade"))
            {
                return i;
            }
        }
        return 4;
    }

    public void SetShapeData(ShapeData data)
    {
        shapeData = data;
    }

    public void SetReferences(ShapeController player, ShapeController target, RotationController rotation, LockMechanic lockMech)
    {
        playerShape = player;
        targetShape = target;
        rotationController = rotation;
        lockMechanic = lockMech;
    }
}
