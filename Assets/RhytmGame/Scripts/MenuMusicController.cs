using UnityEngine;
using DG.Tweening;

public class MenuMusicController : MonoBehaviour
{
    public static MenuMusicController Instance { get; private set; }

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip menuMusic;

    [Header("Settings")]
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private bool loop = true;
    [SerializeField] [Range(0f, 1f)] private float baseVolume = 1f;

    [Header("Fade")]
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float fadeOutDuration = 0.5f;

    private float currentVolumeMultiplier = 1f;

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
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.loop = loop;
        audioSource.playOnAwake = false;

        if (menuMusic != null)
        {
            audioSource.clip = menuMusic;
        }

        ApplyVolume();

        if (playOnStart && menuMusic != null)
        {
            PlayWithFadeIn();
        }

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.RegisterMusicSource(audioSource);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void Play()
    {
        if (audioSource == null)
            return;

        audioSource.volume = baseVolume * currentVolumeMultiplier;
        audioSource.Play();
    }

    public void PlayWithFadeIn()
    {
        if (audioSource == null)
            return;

        audioSource.volume = 0f;
        audioSource.Play();

        audioSource.DOKill();
        audioSource.DOFade(baseVolume * currentVolumeMultiplier, fadeInDuration).SetEase(Ease.OutQuad);
    }

    public void Stop()
    {
        if (audioSource == null)
            return;

        audioSource.Stop();
    }

    public void StopWithFadeOut()
    {
        if (audioSource == null)
            return;

        audioSource.DOKill();
        audioSource.DOFade(0f, fadeOutDuration).SetEase(Ease.InQuad).OnComplete(() =>
        {
            audioSource.Stop();
        });
    }

    public void SetVolume(float multiplier)
    {
        currentVolumeMultiplier = Mathf.Clamp01(multiplier);
        ApplyVolume();
    }

    private void ApplyVolume()
    {
        if (audioSource != null)
        {
            audioSource.volume = baseVolume * currentVolumeMultiplier;
        }
    }

    public AudioSource GetAudioSource()
    {
        return audioSource;
    }

    public bool IsPlaying()
    {
        return audioSource != null && audioSource.isPlaying;
    }
}
