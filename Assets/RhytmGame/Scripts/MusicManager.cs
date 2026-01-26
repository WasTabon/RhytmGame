using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private MusicPlaylist playlist;

    [Header("Settings")]
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private bool loop = true;

    [Header("Current Track (Read Only)")]
    [SerializeField] private string currentTrackName;

    public AudioSource AudioSource => audioSource;
    public AudioClip CurrentTrack => audioSource != null ? audioSource.clip : null;

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
        if (playOnStart)
        {
            PlayRandomTrack();
        }
    }

    public void PlayRandomTrack()
    {
        if (playlist == null || playlist.TrackCount == 0)
        {
            if (audioSource != null && audioSource.clip != null)
            {
                audioSource.loop = loop;
                audioSource.Play();
                currentTrackName = audioSource.clip.name;
            }
            return;
        }

        var track = playlist.GetRandomTrack();
        if (track != null && audioSource != null)
        {
            audioSource.clip = track;
            audioSource.loop = loop;
            audioSource.Play();
            currentTrackName = track.name;
        }
    }

    public void Stop()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    public void SetPlaylist(MusicPlaylist newPlaylist)
    {
        playlist = newPlaylist;
    }
}
