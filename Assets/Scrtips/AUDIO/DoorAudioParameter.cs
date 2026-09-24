using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class DoorAudioParameter : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private EventReference audioEvent; //Tengo una referencia al evento de FMOD que elegí desde Unity.

    private EventInstance eventInstance; //Declaro dónde voy a guardar una instancia de ese evento.

    private void Start()
    {
        eventInstance = RuntimeManager.CreateInstance(audioEvent); //Creo la instancia.

        eventInstance.set3DAttributes(
            RuntimeUtils.To3DAttributes(transform)
        ); //Le digo dónde está ubicada esa instancia en el mundo 3D.

        eventInstance.start(); //La reproduzco.
    }

    private void Update()
    {
        // Actualizar posición 3D del sonido
        eventInstance.set3DAttributes(
            RuntimeUtils.To3DAttributes(transform)
        );

        // Calcular distancia
        float distance = Vector3.Distance(
            player.position,
            transform.position
        );

        // Convertir distancia a 0-1
        float parameterValue =
            1f - Mathf.Clamp01(distance / maxDistance);

        // Enviar parámetro a FMOD
        eventInstance.setParameterByName(
            "Distancia",
            parameterValue
        );

        Debug.Log("Distancia: " + distance + " | Parametro: " + parameterValue);
    }

    private void OnDestroy()
    {
        eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        eventInstance.release();
    }
}