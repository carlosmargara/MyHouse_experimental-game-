using System.Collections;
using UnityEngine;
using Cinemachine;

public class CameraSequence : MonoBehaviour
{
    [System.Serializable]
    public class Shot
    {
        public CinemachineVirtualCamera cam;
        public float duration = 3f;
        public float delayBefore = 0f;
    }

    public Shot[] shots;

    public int activePriority = 20;
    public int inactivePriority = 0;

    private Coroutine _routine;

    public float delayBetweenLoops = 1f;

    private void Start()
    {
        _routine = StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        // apaga todas al inicio
        SetAllInactive();

        while (true) // 👈 loop infinito
        {
            for (int i = 0; i < shots.Length; i++)
            {
                var shot = shots[i];

                if (shot.delayBefore > 0f)
                    yield return new WaitForSeconds(shot.delayBefore);

                SetAllInactive();

                if (shot.cam != null)
                    shot.cam.Priority = activePriority;

                yield return new WaitForSeconds(shot.duration);
            }

                // pausa entre repeticiones
            if (delayBetweenLoops > 0f)
            yield return new WaitForSeconds(delayBetweenLoops);
        
        }

        // opcional: al terminar, dejar la última fija
        // o volver a una cámara default
    }

    void SetAllInactive()
    {
        for (int i = 0; i < shots.Length; i++)
        {
            if (shots[i].cam != null)
                shots[i].cam.Priority = inactivePriority;
        }
    }

    // opcional: si querés reiniciar desde afuera
    public void Restart()
    {
        if (_routine != null) StopCoroutine(_routine);
        _routine = StartCoroutine(PlaySequence());
    }
}

