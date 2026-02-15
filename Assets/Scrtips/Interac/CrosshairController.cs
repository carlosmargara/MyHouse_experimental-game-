using UnityEngine;
using UnityEngine.UI;

public class CrosshairController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Image crosshairImage;
    [SerializeField] private Camera cam;

    [Header("Raycast")]
    [SerializeField] private float rayDistance = 3f;
    [SerializeField] private LayerMask interactableLayer;

    [Header("Colores")]
    [SerializeField] private Color defaultColor = Color.gray;
    [SerializeField] private Color interactColor = new Color32(163, 3, 3, 255);

    private Interactable currentInteractable;

    void Start()
    {
        if (cam == null)
            cam = Camera.main;

        if (crosshairImage != null)
            crosshairImage.color = defaultColor;
    }

    void Update()
    {
        CheckForInteractable();
    }

    void CheckForInteractable()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.blue);

        if (Physics.Raycast(ray, out hit, rayDistance, interactableLayer))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();

            if (interactable != null)
            {
                currentInteractable = interactable;
                SetCrosshairColor(interactColor);
                return;
            }
        }

        currentInteractable = null;
        SetCrosshairColor(defaultColor);
    }

    public void TryInteract()
    {
        if (currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }

    void SetCrosshairColor(Color color)
    {
        if (crosshairImage != null)
            crosshairImage.color = color;
    }
}

