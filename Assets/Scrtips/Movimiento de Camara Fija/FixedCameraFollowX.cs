using UnityEngine;

public class FixedCameraFollowX : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float rotationSpeed = 3f;

    [Header("Limits")]
    [SerializeField] private float minX = -20f;
    [SerializeField] private float maxX = 30f;

    private Quaternion initialRotation;

    private void Start()
    {
        initialRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 dir = target.position - transform.position;

        float angleX = Mathf.Atan2(dir.y,
            new Vector2(dir.x, dir.z).magnitude) * Mathf.Rad2Deg;

        angleX = Mathf.Clamp(angleX, minX, maxX);

        Quaternion targetRotation =
            Quaternion.Euler(angleX,
                             initialRotation.eulerAngles.y,
                             initialRotation.eulerAngles.z);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime);
    }
}
