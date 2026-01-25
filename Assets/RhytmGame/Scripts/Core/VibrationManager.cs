using UnityEngine;
#if UNITY_IOS
using System.Runtime.InteropServices;
#endif

public class VibrationManager : MonoBehaviour
{
    public static VibrationManager Instance { get; private set; }

    private const string VIBRATION_KEY = "VibrationEnabled";

    private bool vibrationEnabled = true;
    public bool VibrationEnabled => vibrationEnabled;

#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void _impactOccurred(int style);
    
    [DllImport("__Internal")]
    private static extern void _notificationOccurred(int type);
    
    [DllImport("__Internal")]
    private static extern void _selectionChanged();
#endif

    public enum ImpactStyle
    {
        Light = 0,
        Medium = 1,
        Heavy = 2,
        Soft = 3,
        Rigid = 4
    }

    public enum NotificationType
    {
        Success = 0,
        Warning = 1,
        Error = 2
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadSettings();
    }

    private void LoadSettings()
    {
        vibrationEnabled = PlayerPrefs.GetInt(VIBRATION_KEY, 1) == 1;
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetInt(VIBRATION_KEY, vibrationEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetVibrationEnabled(bool enabled)
    {
        vibrationEnabled = enabled;
        SaveSettings();
    }

    public void Impact(ImpactStyle style = ImpactStyle.Medium)
    {
        if (!vibrationEnabled) return;

#if UNITY_IOS && !UNITY_EDITOR
        _impactOccurred((int)style);
#elif UNITY_ANDROID && !UNITY_EDITOR
        Handheld.Vibrate();
#endif
    }

    public void Notification(NotificationType type)
    {
        if (!vibrationEnabled) return;

#if UNITY_IOS && !UNITY_EDITOR
        _notificationOccurred((int)type);
#elif UNITY_ANDROID && !UNITY_EDITOR
        Handheld.Vibrate();
#endif
    }

    public void Selection()
    {
        if (!vibrationEnabled) return;

#if UNITY_IOS && !UNITY_EDITOR
        _selectionChanged();
#elif UNITY_ANDROID && !UNITY_EDITOR
        // Android doesn't have selection feedback
#endif
    }

    public void VibrateLight() => Impact(ImpactStyle.Light);
    public void VibrateMedium() => Impact(ImpactStyle.Medium);
    public void VibrateHeavy() => Impact(ImpactStyle.Heavy);
    public void VibrateSuccess() => Notification(NotificationType.Success);
    public void VibrateError() => Notification(NotificationType.Error);
}
