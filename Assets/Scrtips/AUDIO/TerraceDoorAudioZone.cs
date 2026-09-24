using UnityEngine;

public class TerraceDoorAudioZone : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform audioPoint;
    [SerializeField] private AmbientController ambientController;

    [Header("Distance")]
    [SerializeField] private float maxDistance = 3f;

    [Header("Exposure")]
    [SerializeField] private float maxExposure = 0.5f;

    private void Update()
    {
        Debug.DrawLine(player.position, audioPoint.position, Color.red);

        if (ambientController.IsOutdoor)
            return;

        float distance = Vector3.Distance(
            player.position,
            audioPoint.position
        );

        //Debug.Log("DISTANCIA PUERTA: " + distance);

        if (distance <= maxDistance)
        {
            //Debug.Log("CERCA DE LA PUERTA → ROOFTOP ON");

            ambientController.SetTerraceDoorActive(true);
            ambientController.SetRooftop(true);
            ambientController.SetExteriorExposure(maxExposure);
        }
        else
        {
            //Debug.Log("LEJOS DE LA PUERTA → ROOFTOP OFF");

            ambientController.SetTerraceDoorActive(false);
            ambientController.SetRooftop(false);
        }
    }
}