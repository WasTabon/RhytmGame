using UnityEngine;

public class RotationController : MonoBehaviour
{
    [Header("Base Rotation")]
    [SerializeField] private float baseSpeed = 90f;
    [SerializeField] private bool clockwise = true;

    [Header("Music Reaction - Speed (Mid)")]
    [SerializeField] private bool enableSpeedReaction = true;
    [SerializeField] private float speedMultiplier = 2f;

    [Header("Music Reaction - Scale (Low)")]
    [SerializeField] private bool enableScaleReaction = true;
    [SerializeField] private float baseScale = 1f;
    [SerializeField] private float scaleAmount = 0.2f;

    [Header("Music Reaction - Direction (High)")]
    [SerializeField] private bool enableDirectionChange = true;
    [SerializeField] [Range(0f, 1f)] private float highThreshold = 0.7f;
    [SerializeField] private float directionCooldown = 1f;

    [Header("Debug (Read Only)")]
    [SerializeField] private float currentSpeed;
    [SerializeField] private float currentScale;
    [SerializeField] private int currentDirection;

    private AudioAnalyzer audioAnalyzer;
    private float lastDirectionChangeTime;
    private float currentRotation;

    private void Start()
    {
        audioAnalyzer = AudioAnalyzer.Instance;
        currentDirection = clockwise ? -1 : 1;
        lastDirectionChangeTime = -directionCooldown;
        currentRotation = transform.rotation.eulerAngles.z;
    }

    private void Update()
    {
        if (audioAnalyzer == null)
        {
            audioAnalyzer = AudioAnalyzer.Instance;
            if (audioAnalyzer == null)
            {
                ApplyBaseRotation();
                return;
            }
        }

        HandleSpeedReaction();
        HandleScaleReaction();
        HandleDirectionChange();
        ApplyRotation();
    }

    private void HandleSpeedReaction()
    {
        if (enableSpeedReaction && audioAnalyzer != null)
        {
            float midValue = audioAnalyzer.MidValue;
            currentSpeed = baseSpeed + (baseSpeed * midValue * speedMultiplier);
        }
        else
        {
            currentSpeed = baseSpeed;
        }
    }

    private void HandleScaleReaction()
    {
        if (enableScaleReaction && audioAnalyzer != null)
        {
            float lowValue = audioAnalyzer.LowValue;
            currentScale = baseScale + (lowValue * scaleAmount);
            transform.localScale = Vector3.one * currentScale;
        }
        else
        {
            currentScale = baseScale;
            transform.localScale = Vector3.one * baseScale;
        }
    }

    private void HandleDirectionChange()
    {
        if (!enableDirectionChange || audioAnalyzer == null)
            return;

        float highValue = audioAnalyzer.HighValue;
        
        if (highValue >= highThreshold && Time.time - lastDirectionChangeTime >= directionCooldown)
        {
            currentDirection *= -1;
            lastDirectionChangeTime = Time.time;
        }
    }

    private void ApplyRotation()
    {
        currentRotation += currentSpeed * currentDirection * Time.deltaTime;
        currentRotation = currentRotation % 360f;
        transform.rotation = Quaternion.Euler(0, 0, currentRotation);
    }

    private void ApplyBaseRotation()
    {
        currentSpeed = baseSpeed;
        currentDirection = clockwise ? -1 : 1;
        currentRotation += currentSpeed * currentDirection * Time.deltaTime;
        currentRotation = currentRotation % 360f;
        transform.rotation = Quaternion.Euler(0, 0, currentRotation);
    }

    public void SetBaseSpeed(float speed)
    {
        baseSpeed = speed;
    }

    public void SetClockwise(bool isClockwise)
    {
        clockwise = isClockwise;
        currentDirection = clockwise ? -1 : 1;
    }

    public void SetBaseScale(float scale)
    {
        baseScale = scale;
    }

    public void StopRotation()
    {
        enabled = false;
    }

    public void ResumeRotation()
    {
        enabled = true;
    }

    public void ResetRotation()
    {
        currentRotation = 0f;
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    public float GetCurrentRotation()
    {
        return currentRotation;
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    public int GetCurrentDirection()
    {
        return currentDirection;
    }
}
