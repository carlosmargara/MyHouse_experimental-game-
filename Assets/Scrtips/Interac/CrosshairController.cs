using UnityEngine;
using UnityEngine.UI;

public class CrosshairController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Image crosshairImage;
    [SerializeField] private Camera cam;
    [SerializeField] private Transform playerTransform;

    [Header("Raycast")]
    [SerializeField] private float rayDistance = 3f;
    [SerializeField] private float fixedCameraRayHeight = 1f;
    [SerializeField] private float sphereRadius = 0.35f;
    [Header("CapsuleCast")]
    [SerializeField] private float capsuleBottomHeight = 0.3f;
    [SerializeField] private float capsuleTopHeight = 1.7f;
    [SerializeField] private float capsuleRadius = 0.35f;
    [SerializeField] private LayerMask interactableLayer;

    [Header("Colores")]
    [SerializeField] private Color defaultColor = Color.gray;
    [SerializeField] private Color interactColor = new Color32(163, 3, 3, 255);

    private Interactable currentInteractable;
    private Interactable inventoryTarget;

    public Interactable CurrentInteractable => currentInteractable;
    public Interactable InventoryTarget => inventoryTarget;

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
        currentInteractable = null;

        if (CameraModeManager.Instance.currentMode == CameraMode.FPS)
        {
            CheckFPSInteraction();
        }
        else
        {
            CheckFixedCameraInteraction();
        }

        SetCrosshairColor(currentInteractable != null ? interactColor : defaultColor);
    }

    void CheckFPSInteraction()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.blue);

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, interactableLayer))
        {
            currentInteractable = hit.collider.GetComponent<Interactable>();
        }
    }

    void CheckFixedCameraInteraction()
    {
        if (playerTransform == null)
            return;

        Vector3 bottom = playerTransform.position + Vector3.up * capsuleBottomHeight;
        Vector3 top = playerTransform.position + Vector3.up * capsuleTopHeight;
        Vector3 direction = playerTransform.forward;

        if (Physics.CapsuleCast(
            bottom,
            top,
            capsuleRadius,
            direction,
            out RaycastHit hit,
            rayDistance,
            interactableLayer))
        {
            currentInteractable = hit.collider.GetComponent<Interactable>();

            Debug.Log("Pegó contra: " + hit.collider.name);

            Debug.Log("GetComponent: " + hit.collider.GetComponent<Interactable>());
            Debug.Log("GetComponentInParent: " + hit.collider.GetComponentInParent<Interactable>());
        }
    }

    public void TryInteract()
    {
        if (MessageUIManager.Instance.IsDialogueActive)
            return;

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

    public void ShowCrosshair(bool show)
    {
        if (crosshairImage != null)
            crosshairImage.enabled = show;
    }

    public void SaveInventoryTarget() //Fucion que guarda el ultimo objeto interactuable que miro, toco el jugador 
    {
        inventoryTarget = currentInteractable;
        Debug.Log("Inventory Target: " + inventoryTarget);
    }

    void OnDrawGizmos()
    {
        if (playerTransform == null)
            return;

        Vector3 bottom = playerTransform.position + Vector3.up * capsuleBottomHeight;
        Vector3 top = playerTransform.position + Vector3.up * capsuleTopHeight;

        Vector3 bottomEnd = bottom + playerTransform.forward * rayDistance;
        Vector3 topEnd = top + playerTransform.forward * rayDistance;

        Gizmos.color = Color.cyan;

        Gizmos.DrawWireSphere(bottom, capsuleRadius);
        Gizmos.DrawWireSphere(top, capsuleRadius);
        Gizmos.DrawLine(bottom, top);

        Gizmos.DrawWireSphere(bottomEnd, capsuleRadius);
        Gizmos.DrawWireSphere(topEnd, capsuleRadius);
        Gizmos.DrawLine(bottomEnd, topEnd);

        Gizmos.DrawLine(bottom, bottomEnd);
        Gizmos.DrawLine(top, topEnd);
    }
}

