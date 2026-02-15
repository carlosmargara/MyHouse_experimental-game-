using UnityEngine;
using DG.Tweening;

public class CeilingFan : MonoBehaviour
{
    [Header("Fan Settings")]
    [SerializeField] private float rotationSpeed = 360f; // grados por segundo
    [SerializeField] private bool startOn = true;

    private Tween rotateTween;

    private void Start()
    {
        if (startOn)
            StartFan();
    }

    public void StartFan()
    {
        if (rotateTween != null && rotateTween.IsActive())
            return;

        rotateTween = transform
            .DORotate(
                new Vector3(0f, 360f, 0f),
                360f / rotationSpeed,
                RotateMode.FastBeyond360
            )
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }

    public void StopFan()
    {
        if (rotateTween != null)
        {
            rotateTween.Kill();
            rotateTween = null;
        }
    }
}


