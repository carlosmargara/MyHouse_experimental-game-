using UnityEngine;

public class SurfaceIdentifier : MonoBehaviour
{
    [SerializeField] private SurfaceType surfaceType;
    [SerializeField] private bool isOutdoor;

    public SurfaceType SurfaceType => surfaceType;
    public bool IsOutdoor => isOutdoor;
}
