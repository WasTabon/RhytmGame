using UnityEngine;
using UnityEngine.UI;

public class MainMenuInit : MonoBehaviour
{
    [SerializeField] private Image fadeImage;

    private void Awake()
    {
        if (GameManager.Instance == null)
        {
            var go = new GameObject("GameManager");
            go.AddComponent<GameManager>();
        }

        if (SceneLoader.Instance == null)
        {
            var go = new GameObject("SceneLoader");
            go.AddComponent<SceneLoader>();
        }

        if (FadeController.Instance == null)
        {
            var go = new GameObject("FadeController");
            var fc = go.AddComponent<FadeController>();
            fc.SetFadeImage(fadeImage);
            DontDestroyOnLoad(go);
        }
        else if (fadeImage != null)
        {
            FadeController.Instance.SetFadeImage(fadeImage);
        }
    }

    private void Start()
    {
        if (FadeController.Instance != null)
        {
            FadeController.Instance.FadeIn(null);
        }
    }
}
