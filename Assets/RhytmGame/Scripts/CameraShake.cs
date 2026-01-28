using UnityEngine;
using DG.Tweening;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private AudioAnalyzer audioAnalyzer;
    [SerializeField] private LockMechanic lockMechanic;

    [Header("Miss Shake")]
    [SerializeField] private float missShakeDuration = 0.3f;
    [SerializeField] private float missShakeStrength = 0.3f;
    [SerializeField] private int missShakeVibrato = 20;

    [Header("Bass Shake")]
    [SerializeField] private bool enableBassShake = true;
    [SerializeField] private float bassShakeStrength = 0.05f;
    [SerializeField] [Range(0f, 1f)] private float bassThreshold = 0.7f;

    private Vector3 originalPosition;
    private bool isShaking;

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
        if (targetCamera == null)
            targetCamera = Camera.main;

        if (targetCamera != null)
            originalPosition = targetCamera.transform.localPosition;

        if (audioAnalyzer == null)
            audioAnalyzer = AudioAnalyzer.Instance;

        if (lockMechanic == null)
            lockMechanic = LockMechanic.Instance;

        if (lockMechanic != null)
        {
            lockMechanic.OnLock += HandleLock;
        }
    }

    private void OnDestroy()
    {
        if (lockMechanic != null)
        {
            lockMechanic.OnLock -= HandleLock;
        }
    }

    private void Update()
    {
        if (!enableBassShake || isShaking || audioAnalyzer == null || targetCamera == null)
            return;

        float lowValue = audioAnalyzer.LowValue;

        if (lowValue >= bassThreshold)
        {
            float strength = (lowValue - bassThreshold) / (1f - bassThreshold) * bassShakeStrength;
            Vector3 offset = Random.insideUnitSphere * strength;
            offset.z = 0;
            targetCamera.transform.localPosition = originalPosition + offset;
        }
        else
        {
            targetCamera.transform.localPosition = Vector3.Lerp(
                targetCamera.transform.localPosition,
                originalPosition,
                Time.deltaTime * 10f
            );
        }
    }

    private void HandleLock(LockResult result)
    {
        if (result == LockResult.Miss)
        {
            ShakeMiss();
        }
    }

    public void ShakeMiss()
    {
        if (targetCamera == null)
            return;

        isShaking = true;
        targetCamera.transform.DOKill();
        targetCamera.transform.localPosition = originalPosition;

        targetCamera.transform.DOShakePosition(
            missShakeDuration,
            missShakeStrength,
            missShakeVibrato,
            90f,
            false,
            true
        ).OnComplete(() =>
        {
            targetCamera.transform.localPosition = originalPosition;
            isShaking = false;
        });
    }

    public void ShakeCustom(float duration, float strength)
    {
        if (targetCamera == null)
            return;

        isShaking = true;
        targetCamera.transform.DOKill();
        targetCamera.transform.localPosition = originalPosition;

        targetCamera.transform.DOShakePosition(
            duration,
            strength,
            20,
            90f,
            false,
            true
        ).OnComplete(() =>
        {
            targetCamera.transform.localPosition = originalPosition;
            isShaking = false;
        });
    }
}
