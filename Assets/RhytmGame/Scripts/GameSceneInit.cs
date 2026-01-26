using UnityEngine;
using UnityEngine.UI;

public class GameSceneInit : MonoBehaviour
{
    [SerializeField] private Image fadeImage;

    private void Start()
    {
        if (FadeController.Instance != null && fadeImage != null)
        {
            FadeController.Instance.SetFadeImage(fadeImage);
            FadeController.Instance.FadeIn(null);
        }
    }
}
