using UnityEngine;

public class BathroomCameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 3f;
    [SerializeField] private float maxAngle = 15f;

    private float initialYRotation;

    private void Start()
    {
        initialYRotation = transform.eulerAngles.y;
    }

    private void Update()
    {
        if (player == null)
            return;

        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
            return;

        float targetY = Quaternion.LookRotation(direction).eulerAngles.y;

        float relativeAngle = Mathf.DeltaAngle(initialYRotation, targetY);
        relativeAngle = Mathf.Clamp(relativeAngle, -maxAngle, maxAngle);

        float finalY = initialYRotation + relativeAngle;

        Quaternion targetRotation = Quaternion.Euler(
            transform.eulerAngles.x,
            finalY,
            transform.eulerAngles.z
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}
