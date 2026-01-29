using UnityEngine;
using TMPro;
using DG.Tweening;
using System;

public class TimeAttackTimer : MonoBehaviour
{
    public static TimeAttackTimer Instance { get; private set; }

    [Header("References")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Warning Settings")]
    [SerializeField] private float warningTime = 10f;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color warningColor = new Color(1f, 0.3f, 0.3f);
    [SerializeField] private bool pulseOnWarning = true;
    [SerializeField] private float pulseScale = 1.2f;

    [Header("Format")]
    [SerializeField] private string timerFormat = "{0}:{1:00}";

    [Header("Current State (Read Only)")]
    [SerializeField] private float timeRemaining;
    [SerializeField] private bool isRunning;

    public event Action OnTimerEnd;
    public event Action<float> OnTimerTick;

    public float TimeRemaining => timeRemaining;
    public bool IsRunning => isRunning;

    private bool isWarning;
    private Tweener pulseTween;

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
        if (!isRunning)
            return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            isRunning = false;
            UpdateTimerDisplay();
            StopPulse();
            OnTimerEnd?.Invoke();
            return;
        }

        UpdateTimerDisplay();
        CheckWarning();
        OnTimerTick?.Invoke(timeRemaining);
    }

    private void UpdateTimerDisplay()
    {
        if (timerText == null)
            return;

        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);

        timerText.text = string.Format(timerFormat, minutes, seconds);
    }

    private void CheckWarning()
    {
        if (timeRemaining <= warningTime && !isWarning)
        {
            isWarning = true;
            timerText.color = warningColor;

            if (pulseOnWarning)
            {
                StartPulse();
            }
        }
    }

    private void StartPulse()
    {
        if (timerText == null)
            return;

        StopPulse();

        pulseTween = timerText.transform
            .DOScale(pulseScale, 0.5f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private void StopPulse()
    {
        if (pulseTween != null)
        {
            pulseTween.Kill();
            pulseTween = null;
        }

        if (timerText != null)
        {
            timerText.transform.localScale = Vector3.one;
        }
    }

    public void StartTimer(float duration)
    {
        timeRemaining = duration;
        isRunning = true;
        isWarning = false;

        if (timerText != null)
        {
            timerText.color = normalColor;
            timerText.gameObject.SetActive(true);
        }

        UpdateTimerDisplay();
    }

    public void StopTimer()
    {
        isRunning = false;
        StopPulse();
    }

    public void PauseTimer()
    {
        isRunning = false;
    }

    public void ResumeTimer()
    {
        if (timeRemaining > 0)
        {
            isRunning = true;
        }
    }

    public void AddTime(float seconds)
    {
        timeRemaining += seconds;

        if (timeRemaining > warningTime && isWarning)
        {
            isWarning = false;
            timerText.color = normalColor;
            StopPulse();
        }
    }

    public void Hide()
    {
        isRunning = false;
        StopPulse();

        if (timerText != null)
        {
            timerText.gameObject.SetActive(false);
        }
    }

    public void Show()
    {
        if (timerText != null)
        {
            timerText.gameObject.SetActive(true);
        }
    }

    public void SetTimerText(TextMeshProUGUI text)
    {
        timerText = text;
    }
}
