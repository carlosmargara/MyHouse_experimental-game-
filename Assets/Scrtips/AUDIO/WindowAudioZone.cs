using UnityEngine;

public class WindowAudioZone : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private AmbientController ambientController;

    [Header("Distance")]
    [SerializeField] private float maxDistance = 3f;

    [Header("Floor")]
    [SerializeField] private float floorHeight = 0f;
    [SerializeField] private float floorTolerance = 1f;

    private void Update()
    {
        if (ambientController.IsOutdoor || ambientController.TerraceDoorActive)
            return;

        float floorDifference = Mathf.Abs(player.position.y - floorHeight);

        if (floorDifference > floorTolerance)
        {
            ambientController.SetExteriorExposure(0f);
            return;
        }

        float distance = Vector3.Distance(player.position, transform.position);

        float exposure = 1f - Mathf.Clamp01(distance / maxDistance);

        //Debug.Log($"Distance: {distance:F2} | Floor Difference: {floorDifference:F2} | Exposure: {exposure:F2}");

        ambientController.SetExteriorExposure(exposure);
    }
}
