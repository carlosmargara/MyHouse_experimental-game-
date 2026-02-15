using UnityEngine;
using System.Collections;

public class SimpleOpenClose : Interactable
{
    public enum OpenType
    {
        Translate,
        Rotate
    }

    [Header("Tipo de apertura")]
    public OpenType openType = OpenType.Translate;

    [Header("Movimiento (Translate)")]
    public float moveDistance = 0.4f;   // cuánto se mueve en Z
    public float speed = 3f;

    [Header("Rotación (Rotate)")]
    public float rotateAngle = 90f;     // grados en Y

    Vector3 closedPosition;
    Vector3 openPosition;

    Quaternion closedRotation;
    Quaternion openRotation;

    bool isOpen = false;
    bool isMoving = false;

    void Start()
    {
        // Guardamos estados cerrados
        closedPosition = transform.localPosition;
        closedRotation = transform.localRotation;

        // Calculamos estados abiertos
        openPosition = closedPosition + Vector3.forward * moveDistance;
        openRotation = Quaternion.Euler(0, rotateAngle, 0) * closedRotation;
    }

    public override void Interact()
    {
        if (isMoving) return;

        isOpen = !isOpen;
        StopAllCoroutines();

        if (openType == OpenType.Translate)
        {
            StartCoroutine(MovePosition(isOpen ? openPosition : closedPosition));
        }
        else
        {
            StartCoroutine(MoveRotation(isOpen ? openRotation : closedRotation));
        }
    }

    IEnumerator MovePosition(Vector3 target)
    {
        isMoving = true;

        while (Vector3.Distance(transform.localPosition, target) > 0.001f)
        {
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                target,
                Time.deltaTime * speed
            );
            yield return null;
        }

        transform.localPosition = target;
        isMoving = false;
    }

    IEnumerator MoveRotation(Quaternion target)
    {
        isMoving = true;

        while (Quaternion.Angle(transform.localRotation, target) > 0.5f)
        {
            transform.localRotation = Quaternion.Lerp(
                transform.localRotation,
                target,
                Time.deltaTime * speed
            );
            yield return null;
        }

        transform.localRotation = target;
        isMoving = false;
    }
}

