using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Settings")]
    [SerializeField] private float musicFadeDuration = 0.5f;

    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";

    private float musicVolume = 1f;
    private float sfxVolume = 1f;

    private Dictionary<string, AudioClip> sfxCache = new Dictionary<string, AudioClip>();

    public float MusicVolume => musicVolume;
    public float SFXVolume => sfxVolume;
    public AudioSource MusicSource => musicSource;

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
        ApplyVolumes();
    }

    private void LoadSettings()
    {
        musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1f);
        sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, musicVolume);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, sfxVolume);
        PlayerPrefs.Save();
    }

    private void ApplyVolumes()
    {
        if (musicSource != null)
            musicSource.volume = musicVolume;
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
            musicSource.volume = musicVolume;
        SaveSettings();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        SaveSettings();
    }

    public void PlayMusic(AudioClip clip, bool loop = true, bool fade = true)
    {
        if (musicSource == null || clip == null) return;

        if (fade && musicSource.isPlaying)
        {
            musicSource.DOFade(0f, musicFadeDuration).OnComplete(() =>
            {
                musicSource.clip = clip;
                musicSource.loop = loop;
                musicSource.Play();
                musicSource.DOFade(musicVolume, musicFadeDuration);
            });
        }
        else
        {
            musicSource.clip = clip;
            musicSource.loop = loop;
            musicSource.volume = musicVolume;
            musicSource.Play();
        }
    }

    public void StopMusic(bool fade = true)
    {
        if (musicSource == null) return;

        if (fade)
        {
            musicSource.DOFade(0f, musicFadeDuration).OnComplete(() =>
            {
                musicSource.Stop();
                musicSource.volume = musicVolume;
            });
        }
        else
        {
            musicSource.Stop();
        }
    }

    public void PauseMusic()
    {
        if (musicSource != null)
            musicSource.Pause();
    }

    public void ResumeMusic()
    {
        if (musicSource != null)
            musicSource.UnPause();
    }

    public void PlaySFX(AudioClip clip, float volumeMultiplier = 1f)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume * volumeMultiplier);
    }

    public void PlaySFX(string clipName, float volumeMultiplier = 1f)
    {
        if (!sfxCache.TryGetValue(clipName, out AudioClip clip))
        {
            clip = Resources.Load<AudioClip>($"Audio/SFX/{clipName}");
            if (clip != null)
                sfxCache[clipName] = clip;
        }

        if (clip != null)
            PlaySFX(clip, volumeMultiplier);
    }

    public void PreloadSFX(params string[] clipNames)
    {
        foreach (var name in clipNames)
        {
            if (!sfxCache.ContainsKey(name))
            {
                var clip = Resources.Load<AudioClip>($"Audio/SFX/{name}");
                if (clip != null)
                    sfxCache[name] = clip;
            }
        }
    }
}
