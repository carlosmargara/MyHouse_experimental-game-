using UnityEngine;

public class FlashlightWallAdjust : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Light flashlight;
    [SerializeField] private LighterSystem lighterSystem;
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
        if (flashlight == null || lighterSystem == null || cameraTransform == null)
            return;

        if (!flashlight.enabled)
            return;

        Vector3 forwardFlat = cameraTransform.forward;
        forwardFlat.y = 0f;
        forwardFlat.Normalize();

        Ray ray = new Ray(cameraTransform.position, forwardFlat);

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

        Vector3 localPos = transform.localPosition;
        localPos.z = Mathf.Lerp(localPos.z, targetZ, Time.deltaTime * positionSmooth);
        transform.localPosition = localPos;

        currentMultiplier = Mathf.Lerp(
            currentMultiplier,
            targetMultiplier,
            Time.deltaTime * intensitySmooth
        );

        flashlight.intensity = lighterSystem.CurrentIntensity * currentMultiplier;
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


