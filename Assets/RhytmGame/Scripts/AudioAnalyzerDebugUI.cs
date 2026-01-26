using UnityEngine;
using UnityEngine.UI;

public class AudioAnalyzerDebugUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioAnalyzer audioAnalyzer;

    [Header("Bars")]
    [SerializeField] private Image lowBar;
    [SerializeField] private Image midBar;
    [SerializeField] private Image highBar;

    [Header("Settings")]
    [SerializeField] private bool showRawValues = false;

    [Header("Debug")]
    [SerializeField] private float debugLow;
    [SerializeField] private float debugMid;
    [SerializeField] private float debugHigh;

    private void Start()
    {
        if (lowBar == null || midBar == null || highBar == null)
        {
            TryFindBars();
        }
    }

    private void TryFindBars()
    {
        var allImages = FindObjectsOfType<Image>(true);
        foreach (var img in allImages)
        {
            if (img.gameObject.name == "LowBar" && lowBar == null)
                lowBar = img;
            else if (img.gameObject.name == "MidBar" && midBar == null)
                midBar = img;
            else if (img.gameObject.name == "HighBar" && highBar == null)
                highBar = img;
        }
    }

    private void Update()
    {
        if (audioAnalyzer == null)
        {
            if (AudioAnalyzer.Instance != null)
            {
                Debug.Log("Work");
                audioAnalyzer = AudioAnalyzer.Instance;
            }
            else
            {
                Debug.Log("Return");
                return;
            }
        }

        float low = showRawValues ? audioAnalyzer.LowValueRaw : audioAnalyzer.LowValue;
        float mid = showRawValues ? audioAnalyzer.MidValueRaw : audioAnalyzer.MidValue;
        float high = showRawValues ? audioAnalyzer.HighValueRaw : audioAnalyzer.HighValue;

        debugLow = low;
        debugMid = mid;
        debugHigh = high;
        
        Debug.Log("Change");

        if (lowBar != null)
        {
            Debug.Log("Fill low");
            lowBar.fillAmount = low;
        }

        if (midBar != null)
        {
            Debug.Log("Fill mid");
            midBar.fillAmount = mid;
        }

        if (highBar != null)
        {
            Debug.Log("Fill high");
            highBar.fillAmount = high;
        }
    }
}
