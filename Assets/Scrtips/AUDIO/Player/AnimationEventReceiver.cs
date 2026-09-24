using UnityEngine;

public class AnimationEventReceiver : MonoBehaviour
{
    [SerializeField] private Footsteps footsteps;

    private void Awake()
    {
        //footsteps = GetComponentInParent<Footsteps>();
    }

    public void HandleFootstep()
    {
        if (footsteps != null)
        {
            footsteps.HandleFootstep();
        }
        else
        {
            Debug.LogWarning("AnimationEventReceiver: No se encontró Footsteps en el Player.");
        }
    }
}
