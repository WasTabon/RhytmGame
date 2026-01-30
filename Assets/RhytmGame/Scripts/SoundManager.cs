using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip perfectSound;
    [SerializeField] private AudioClip goodSound;
    [SerializeField] private AudioClip missSound;
    [SerializeField] private AudioClip comboBreakSound;
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip achievementSound;
    [SerializeField] private AudioClip levelCompleteSound;
    [SerializeField] private AudioClip gameOverSound;

    [Header("Volume")]
    [SerializeField] [Range(0f, 1f)] private float musicVolume = 1f;
    [SerializeField] [Range(0f, 1f)] private float sfxVolume = 1f;

    private Dictionary<string, AudioClip> soundClips = new Dictionary<string, AudioClip>();
    private AudioSource currentMusicSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadVolumes();
        RegisterSounds();
        CreateSFXSourceIfNeeded();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        FindAndApplyMusicSource();
        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindAndApplyMusicSource();
        LateSubscribe();
    }

    private void FindAndApplyMusicSource()
    {
        currentMusicSource = null;

        if (MusicManager.Instance != null)
        {
            currentMusicSource = MusicManager.Instance.GetComponent<AudioSource>();
        }

        if (currentMusicSource == null)
        {
            MenuMusicController menuMusic = FindObjectOfType<MenuMusicController>();
            if (menuMusic != null)
            {
                currentMusicSource = menuMusic.GetComponent<AudioSource>();
            }
        }

        if (currentMusicSource == null)
        {
            AudioSource[] sources = FindObjectsOfType<AudioSource>();
            foreach (var source in sources)
            {
                if (source.clip != null && source.loop)
                {
                    currentMusicSource = source;
                    break;
                }
            }
        }

        ApplyMusicVolume();
    }

    private void CreateSFXSourceIfNeeded()
    {
        if (sfxSource == null)
        {
            GameObject sfxGO = new GameObject("SFXSource");
            sfxGO.transform.SetParent(transform);
            sfxSource = sfxGO.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }
    }

    private void RegisterSounds()
    {
        if (perfectSound != null) soundClips["perfect"] = perfectSound;
        if (goodSound != null) soundClips["good"] = goodSound;
        if (missSound != null) soundClips["miss"] = missSound;
        if (comboBreakSound != null) soundClips["combo_break"] = comboBreakSound;
        if (buttonClickSound != null) soundClips["button"] = buttonClickSound;
        if (achievementSound != null) soundClips["achievement"] = achievementSound;
        if (levelCompleteSound != null) soundClips["level_complete"] = levelCompleteSound;
        if (gameOverSound != null) soundClips["game_over"] = gameOverSound;
    }

    private void SubscribeToEvents()
    {
        if (LockMechanic.Instance != null)
        {
            LockMechanic.Instance.OnLock += HandleLock;
        }

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnComboReset += HandleComboReset;
        }

        if (GameModeController.Instance != null)
        {
            GameModeController.Instance.OnLevelComplete += HandleLevelComplete;
            GameModeController.Instance.OnGameOver += HandleGameOver;
        }

        if (AchievementManager.Instance != null)
        {
            AchievementManager.Instance.OnAchievementUnlocked += HandleAchievementUnlocked;
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (LockMechanic.Instance != null)
        {
            LockMechanic.Instance.OnLock -= HandleLock;
        }

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnComboReset -= HandleComboReset;
        }

        if (GameModeController.Instance != null)
        {
            GameModeController.Instance.OnLevelComplete -= HandleLevelComplete;
            GameModeController.Instance.OnGameOver -= HandleGameOver;
        }

        if (AchievementManager.Instance != null)
        {
            AchievementManager.Instance.OnAchievementUnlocked -= HandleAchievementUnlocked;
        }
    }

    public void LateSubscribe()
    {
        UnsubscribeFromEvents();
        SubscribeToEvents();
    }

    private void HandleLock(LockResult result)
    {
        switch (result)
        {
            case LockResult.Perfect:
                PlaySFX("perfect");
                break;
            case LockResult.Good:
                PlaySFX("good");
                break;
            case LockResult.Miss:
                PlaySFX("miss");
                break;
        }
    }

    private void HandleComboReset()
    {
        PlaySFX("combo_break");
    }

    private void HandleLevelComplete()
    {
        PlaySFX("level_complete");
    }

    private void HandleGameOver()
    {
        PlaySFX("game_over");
    }

    private void HandleAchievementUnlocked(AchievementInfo achievement)
    {
        PlaySFX("achievement");
    }

    public void PlaySFX(string soundName)
    {
        if (sfxSource == null || sfxVolume <= 0f)
            return;

        if (soundClips.TryGetValue(soundName, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip, sfxVolume);
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource == null || clip == null || sfxVolume <= 0f)
            return;

        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void PlayButtonClick()
    {
        PlaySFX("button");
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        ApplyMusicVolume();
    }

    private void ApplyMusicVolume()
    {
        if (currentMusicSource != null)
        {
            currentMusicSource.volume = musicVolume;
        }
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
    }

    public float GetMusicVolume()
    {
        return musicVolume;
    }

    public float GetSFXVolume()
    {
        return sfxVolume;
    }

    private void LoadVolumes()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
    }

    public void RegisterSound(string name, AudioClip clip)
    {
        if (!string.IsNullOrEmpty(name) && clip != null)
        {
            soundClips[name] = clip;
        }
    }

    public void RefreshMusicSource()
    {
        FindAndApplyMusicSource();
    }

    public void RegisterMusicSource(AudioSource source)
    {
        if (source != null)
        {
            currentMusicSource = source;
            ApplyMusicVolume();
        }
    }
}
