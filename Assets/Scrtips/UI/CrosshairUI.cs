using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CrosshairUI : MonoBehaviour
{
    [SerializeField] private Image crosshairImage;
    [SerializeField] private float fadeDuration = 0.15f;

    private Tween fadeTween;

    private void Awake()
    {
        if (crosshairImage == null)
            crosshairImage = GetComponent<Image>();
    }

    public void Show()
    {
        gameObject.SetActive(true);

        fadeTween?.Kill();

        Color color = crosshairImage.color;
        color.a = 0f;
        crosshairImage.color = color;

        fadeTween = crosshairImage
            .DOFade(1f, fadeDuration)
            .SetUpdate(true);
    }

    public void Hide()
    {
        fadeTween?.Kill();

        fadeTween = crosshairImage
            .DOFade(0f, fadeDuration)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
    }
}
