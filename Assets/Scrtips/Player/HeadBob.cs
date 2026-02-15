using UnityEngine;

public class HeadBob : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private FPSController fpsController;
    [SerializeField] private Transform cameraPivot;

    [Header("Head Bob")]
    public float bobSpeed = 12f;
    public float bobAmount = 0.04f;
    public float returnSpeed = 6f;

    private Vector3 startLocalPos;
    private float timer;

    void Start()
    {
        startLocalPos = cameraPivot.localPosition;
    }

    void Update()
    {
        Vector2 move = fpsController.MoveInput; // ahora lo exponemos bien

        if (move.magnitude > 0.1f && fpsController.IsGrounded)
        {
            timer += Time.deltaTime * bobSpeed;

            float bobY = Mathf.Sin(timer) * bobAmount;

            cameraPivot.localPosition = startLocalPos + new Vector3(0f, bobY, 0f);
        }
        else
        {
            timer = 0f;
            cameraPivot.localPosition = Vector3.Lerp(
                cameraPivot.localPosition,
                startLocalPos,
                Time.deltaTime * returnSpeed
            );
        }
    }
}



