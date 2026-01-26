using UnityEngine;

public class AudioAnalyzer : MonoBehaviour
{
    public static AudioAnalyzer Instance { get; private set; }

    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource;

    [Header("Spectrum Settings")]
    [SerializeField] private int spectrumSize = 64;
    [SerializeField] private FFTWindow fftWindow = FFTWindow.BlackmanHarris;

    [Header("Frequency Ranges")]
    [SerializeField] private int lowRangeStart = 0;
    [SerializeField] private int lowRangeEnd = 8;
    [SerializeField] private int midRangeStart = 8;
    [SerializeField] private int midRangeEnd = 32;
    [SerializeField] private int highRangeStart = 32;
    [SerializeField] private int highRangeEnd = 64;

    [Header("Sensitivity")]
    [SerializeField] [Range(1f, 500f)] private float lowMultiplier = 100f;
    [SerializeField] [Range(1f, 500f)] private float midMultiplier = 50f;
    [SerializeField] [Range(1f, 500f)] private float highMultiplier = 30f;

    [Header("Smoothing")]
    [SerializeField] private bool useSmoothing = true;
    [SerializeField] [Range(1f, 30f)] private float smoothingSpeed = 10f;

    [Header("Output (Read Only)")]
    [SerializeField] [Range(0f, 1f)] private float lowValue;
    [SerializeField] [Range(0f, 1f)] private float midValue;
    [SerializeField] [Range(0f, 1f)] private float highValue;

    public float LowValue => lowValue;
    public float MidValue => midValue;
    public float HighValue => highValue;

    public float LowValueRaw { get; private set; }
    public float MidValueRaw { get; private set; }
    public float HighValueRaw { get; private set; }

    private float[] spectrumData;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        spectrumData = new float[spectrumSize];
    }

    private void Update()
    {
        if (audioSource == null || !audioSource.isPlaying)
            return;

        if (spectrumData.Length != spectrumSize)
            spectrumData = new float[spectrumSize];

        audioSource.GetSpectrumData(spectrumData, 0, fftWindow);

        LowValueRaw = GetFrequencyBandAverage(lowRangeStart, lowRangeEnd) * lowMultiplier;
        MidValueRaw = GetFrequencyBandAverage(midRangeStart, midRangeEnd) * midMultiplier;
        HighValueRaw = GetFrequencyBandAverage(highRangeStart, highRangeEnd) * highMultiplier;

        LowValueRaw = Mathf.Clamp01(LowValueRaw);
        MidValueRaw = Mathf.Clamp01(MidValueRaw);
        HighValueRaw = Mathf.Clamp01(HighValueRaw);

        if (useSmoothing)
        {
            lowValue = Mathf.Lerp(lowValue, LowValueRaw, Time.deltaTime * smoothingSpeed);
            midValue = Mathf.Lerp(midValue, MidValueRaw, Time.deltaTime * smoothingSpeed);
            highValue = Mathf.Lerp(highValue, HighValueRaw, Time.deltaTime * smoothingSpeed);
        }
        else
        {
            lowValue = LowValueRaw;
            midValue = MidValueRaw;
            highValue = HighValueRaw;
        }
    }

    private float GetFrequencyBandAverage(int start, int end)
    {
        start = Mathf.Clamp(start, 0, spectrumSize - 1);
        end = Mathf.Clamp(end, start + 1, spectrumSize);

        float sum = 0f;
        for (int i = start; i < end; i++)
        {
            sum += spectrumData[i];
        }

        return sum / (end - start);
    }

    public void SetAudioSource(AudioSource source)
    {
        audioSource = source;
    }
}
