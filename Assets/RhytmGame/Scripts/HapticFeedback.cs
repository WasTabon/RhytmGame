using UnityEngine;

public class HapticFeedback : MonoBehaviour
{
    public static HapticFeedback Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private bool enableHaptics = true;

    [Header("References")]
    [SerializeField] private LockMechanic lockMechanic;

#if UNITY_IOS
    [Header("iOS Settings")]
    [SerializeField] private bool useiOSHaptics = true;
#endif

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
    }

    private void OnDestroy()
    {
        if (lockMechanic != null)
        {
            lockMechanic.OnLock -= HandleLock;
        }
    }

    private void HandleLock(LockResult result)
    {
        if (!enableHaptics)
            return;

        switch (result)
        {
            case LockResult.Perfect:
                VibrateLight();
                break;
            case LockResult.Good:
                VibrateLight();
                break;
            case LockResult.Miss:
                VibrateMedium();
                break;
        }
    }

    public void VibrateLight()
    {
        if (!enableHaptics)
            return;

#if UNITY_IOS && !UNITY_EDITOR
        iOSHapticFeedback.ImpactLight();
#elif UNITY_ANDROID && !UNITY_EDITOR
        Handheld.Vibrate();
#endif
    }

    public void VibrateMedium()
    {
        if (!enableHaptics)
            return;

#if UNITY_IOS && !UNITY_EDITOR
        iOSHapticFeedback.ImpactMedium();
#elif UNITY_ANDROID && !UNITY_EDITOR
        Handheld.Vibrate();
#endif
    }

    public void VibrateHeavy()
    {
        if (!enableHaptics)
            return;

#if UNITY_IOS && !UNITY_EDITOR
        iOSHapticFeedback.ImpactHeavy();
#elif UNITY_ANDROID && !UNITY_EDITOR
        Handheld.Vibrate();
#endif
    }

    public void SetEnabled(bool enabled)
    {
        enableHaptics = enabled;
    }

    public bool IsEnabled()
    {
        return enableHaptics;
    }
}

#if UNITY_IOS
public static class iOSHapticFeedback
{
    public static void ImpactLight()
    {
        _impactOccurred(0);
    }

    public static void ImpactMedium()
    {
        _impactOccurred(1);
    }

    public static void ImpactHeavy()
    {
        _impactOccurred(2);
    }

    public static void SelectionChanged()
    {
        _selectionChanged();
    }

    public static void NotificationSuccess()
    {
        _notificationOccurred(0);
    }

    public static void NotificationWarning()
    {
        _notificationOccurred(1);
    }

    public static void NotificationError()
    {
        _notificationOccurred(2);
    }

#if UNITY_IOS && !UNITY_EDITOR
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void _impactOccurred(int style);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void _selectionChanged();

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void _notificationOccurred(int type);
#else
    private static void _impactOccurred(int style) { }
    private static void _selectionChanged() { }
    private static void _notificationOccurred(int type) { }
#endif
}
#endif
