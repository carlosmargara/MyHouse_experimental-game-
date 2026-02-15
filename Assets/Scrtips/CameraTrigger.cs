using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(BoxCollider))]
public class CameraTrigger : MonoBehaviour
{
    public CinemachineVirtualCamera fixedCamera;

    [Header("Gizmo")]
    public Color gizmoColor = new Color(0f, 1f, 1f, 0.25f);

    private BoxCollider boxCollider;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        FixedCameraManager.Instance.SetFixedCamera(fixedCamera);
    }

    void OnDrawGizmos()
    {
        if (boxCollider == null)
            boxCollider = GetComponent<BoxCollider>();

        Gizmos.color = gizmoColor;

        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.DrawCube(boxCollider.center, boxCollider.size);
        Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);

        Gizmos.matrix = oldMatrix;
    }
}


