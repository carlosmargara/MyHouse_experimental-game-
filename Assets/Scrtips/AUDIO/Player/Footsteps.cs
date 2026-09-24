using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class Footsteps : MonoBehaviour
{
    [SerializeField] private EventReference footstepEvent;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float rayDistance = 1.8f;

    [Header("Ambiente")]
    [SerializeField] private AmbientController ambientController;

    private bool currentOutdoorState;

    private void Update()
    {
        DetectOutdoorState();
    }

    public void HandleFootstep()
    {
        SurfaceType surface = DetectSurfaceType();

        EventInstance instance = RuntimeManager.CreateInstance(footstepEvent);

        instance.setParameterByNameWithLabel("Surface", surface.ToString());
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(transform));

        instance.start();
        instance.release();
    }

    private SurfaceType DetectSurfaceType()
    {
        Ray ray = new Ray(
            transform.position + Vector3.up * 0.1f,
            Vector3.down
        );

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, groundLayer))
        {
            SurfaceIdentifier surfaceIdentifier =
                hit.collider.GetComponent<SurfaceIdentifier>();

            if (surfaceIdentifier != null)
            {
                return surfaceIdentifier.SurfaceType;
            }
        }

        return SurfaceType.Default;
    }

    private void DetectOutdoorState()
    {
        Ray ray = new Ray(
            transform.position + Vector3.up * 0.1f,
            Vector3.down
        );

        bool isOutdoor = false;

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, groundLayer))
        {
            SurfaceIdentifier surfaceIdentifier =
                hit.collider.GetComponent<SurfaceIdentifier>();

            if (surfaceIdentifier != null)
            {
                isOutdoor = surfaceIdentifier.IsOutdoor;
            }
        }

        if (isOutdoor != currentOutdoorState)
        {
            currentOutdoorState = isOutdoor;

            ambientController.SetOutdoor(isOutdoor);
        }
    }
}
