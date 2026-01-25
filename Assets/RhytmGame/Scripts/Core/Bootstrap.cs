using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [Header("Manager Prefabs")]
    [SerializeField] private GameObject gameManagerPrefab;
    [SerializeField] private GameObject audioManagerPrefab;
    [SerializeField] private GameObject vibrationManagerPrefab;
    [SerializeField] private GameObject sceneTransitionPrefab;

    private void Awake()
    {
        InitializeManagers();
    }

    private void InitializeManagers()
    {
        if (GameManager.Instance == null && gameManagerPrefab != null)
        {
            Instantiate(gameManagerPrefab);
        }

        if (AudioManager.Instance == null && audioManagerPrefab != null)
        {
            Instantiate(audioManagerPrefab);
        }

        if (VibrationManager.Instance == null && vibrationManagerPrefab != null)
        {
            Instantiate(vibrationManagerPrefab);
        }

        if (SceneTransition.Instance == null && sceneTransitionPrefab != null)
        {
            Instantiate(sceneTransitionPrefab);
        }
    }
}
