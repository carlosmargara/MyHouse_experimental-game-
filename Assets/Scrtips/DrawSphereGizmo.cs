using UnityEngine;

public class DrawSphereGizmo : MonoBehaviour
{
    [Header("Sphere Settings")]
    public float radius = 1f;
    public Color color = Color.green;

    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
