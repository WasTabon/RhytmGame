using UnityEngine;

[CreateAssetMenu(fileName = "MusicPlaylist", menuName = "RhythmGame/Music Playlist")]
public class MusicPlaylist : ScriptableObject
{
    [SerializeField] private AudioClip[] tracks;

    public AudioClip GetRandomTrack()
    {
        if (tracks == null || tracks.Length == 0)
            return null;

        return tracks[Random.Range(0, tracks.Length)];
    }

    public AudioClip[] GetAllTracks()
    {
        return tracks;
    }

    public int TrackCount => tracks != null ? tracks.Length : 0;
}
