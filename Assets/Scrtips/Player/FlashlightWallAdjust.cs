using UnityEngine;

public class FlashlightWallAdjust : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Light flashlight;
    [SerializeField] private FlashlightToggle flashlightToggle;
    [SerializeField] private Transform cameraTransform;

    [Header("Raycast")]
    [SerializeField] private float rayDistance = 1.3f;
    [SerializeField] private float minWallDistance = 0.35f;

    [Header("Posición Z local (encendedor)")]
    [SerializeField] private float normalZ = 0.9f;
    [SerializeField] private float closeZ = 0.45f;
    [SerializeField] private float positionSmooth = 10f;

    [Header("Atenuación")]
    [SerializeField] private float closeIntensityMultiplier = 0.4f;
    [SerializeField] private float intensitySmooth = 10f;

    private float currentMultiplier = 1f;
    private bool isHittingWall;
    private float lastHitDistance;

    private void Update()
    {
        if (flashlight == null || flashlightToggle == null || cameraTransform == null)
            return;

        if (!flashlight.enabled)
            return;

        Vector3 forwardFlat = cameraTransform.forward;
        forwardFlat.y = 0f;
        forwardFlat.Normalize();

        Ray ray = new Ray(cameraTransform.position, forwardFlat); // de esta forma con el forwardFlat estoy midiendo la distancia de posicion no de vision de la camara
                                                                  // como pasaba con las lineas que estan abajo 

        // Ray SIEMPRE desde la cámara (mirada)
        //Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

        float targetZ = normalZ;
        float targetMultiplier = 1f;

        isHittingWall = false;

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            isHittingWall = true;
            lastHitDistance = hit.distance;

            if (hit.distance <= minWallDistance)
            {
                float t = Mathf.InverseLerp(0f, minWallDistance, hit.distance);

                targetZ = Mathf.Lerp(closeZ, normalZ, t);
                targetMultiplier = Mathf.Lerp(closeIntensityMultiplier, 1f, t);
            }
        }

        // Movimiento local de la luz (hija del player)
        Vector3 localPos = transform.localPosition;
        localPos.z = Mathf.Lerp(localPos.z, targetZ, Time.deltaTime * positionSmooth);
        transform.localPosition = localPos;

        // Intensidad multiplicativa (respeta flicker)
        currentMultiplier = Mathf.Lerp(
            currentMultiplier,
            targetMultiplier,
            Time.deltaTime * intensitySmooth
        );

        flashlight.intensity = flashlightToggle.CurrentIntensity * currentMultiplier;
    }

    private void OnDrawGizmos()
    {
        if (cameraTransform == null)
            return;

        Gizmos.color = isHittingWall ? Color.red : Color.green;
        Gizmos.DrawRay(cameraTransform.position, cameraTransform.forward * rayDistance);

        if (isHittingWall)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(
                cameraTransform.position + cameraTransform.forward * lastHitDistance,
                0.05f
            );
        }
    }
}


