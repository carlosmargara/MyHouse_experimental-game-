using UnityEngine;
using UnityEngine.UI;

public class RetroScaler : MonoBehaviour
{
    public RawImage rawImage;
    public float targetAspect = 4f / 3f;

    Canvas rootCanvas;
    RectTransform canvasRect;

    void Start()
    {
        rootCanvas = GetComponentInParent<Canvas>();
        canvasRect = rootCanvas.GetComponent<RectTransform>();
        Resize();
    }

    void Resize()
    {
        float canvasWidth = canvasRect.rect.width;
        float canvasHeight = canvasRect.rect.height;

        float canvasAspect = canvasWidth / canvasHeight;

        RectTransform rt = rawImage.rectTransform;

        if (canvasAspect > targetAspect)
        {
            // pantalla más ancha → barras laterales
            float height = canvasHeight;
            float width = height * targetAspect;
            rt.sizeDelta = new Vector2(width, height);
        }
        else
        {
            // pantalla más alta (poco común)
            float width = canvasWidth;
            float height = width / targetAspect;
            rt.sizeDelta = new Vector2(width, height);
        }
    }
}
