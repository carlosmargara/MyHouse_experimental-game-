using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
using TMPro;

public class InventoryCarousel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Transform itemParent;
    [SerializeField] private CrosshairController crosshairController;

    [Header("Settings")]
    [SerializeField] private float radius = 2f;
    [SerializeField] private float rotationSpeed = 180f;

    [Header("Inventory UI")]
    [SerializeField] private TextMeshProUGUI titleItemTMP;
    [SerializeField] private TextMeshProUGUI descriptionItemTMP;
    [SerializeField] private TextMeshProUGUI actionItemTMP; //spaceBar - Text la Jerarquia

    [Space]
    [SerializeField] private float buildDuration = 0.4f;
    [SerializeField] private float buildDelayBetweenItems = 0.05f;
    [SerializeField] private float closeDuration = 0.25f;

    private readonly List<Transform> itemModels = new List<Transform>();
    private readonly List<int> slotIndexes = new List<int>();

    private InputAction navigateAction;
    private InputAction submitAction;

    private int currentIndex;
    private float currentRotation;
    private float targetRotation;

    private bool isMoving;
    private bool isBuildingCarousel;

    private void Awake()
    {
        if (playerInput == null)
            playerInput = FindObjectOfType<PlayerInput>();

        navigateAction = playerInput.actions["Navigate"];
        submitAction = playerInput.actions["Submit"];
    }

    private void OnEnable()
    {
        if (navigateAction != null)
            navigateAction.performed += OnNavigate;

        if (submitAction != null)
            submitAction.performed += OnSubmit;

        BuildCarouselFromInventory();
        BuildCarouselAnimation();
    }

    private void OnDisable()
    {
        if (navigateAction != null)
            navigateAction.performed -= OnNavigate;

        if (submitAction != null)
            submitAction.performed -= OnSubmit;

        ClearCarousel();
    }

    private void Update()
    {
        if (itemModels.Count == 0)
            return;

        currentRotation = Mathf.MoveTowardsAngle(
            currentRotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );

        if (!isBuildingCarousel)
            ArrangeItems();

        RotateSelectedItem();

        if (Mathf.Abs(Mathf.DeltaAngle(currentRotation, targetRotation)) < 0.1f)
        {
            currentRotation = targetRotation;
            isMoving = false;
        }
    }

    private void BuildCarouselFromInventory()
    {
        ClearCarousel();

        if (Inventory.Instance == null)
            return;

        Inventory_Item[] inventoryItems = Inventory.Instance.Items;

        for (int i = 0; i < inventoryItems.Length; i++)
        {
            Inventory_Item item = inventoryItems[i];

            if (item == null) continue;
            if (item.prefabModel == null) continue;

            GameObject model = Instantiate(item.prefabModel, itemParent);

            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.identity;
            model.transform.localScale = Vector3.one;

            itemModels.Add(model.transform);
            slotIndexes.Add(i);
        }

        currentIndex = 0;
        currentRotation = 0f;
        targetRotation = 0f;
        isMoving = false;

        if (itemModels.Count > 0)
        {
            Inventory.Instance.SetCurrentIndex(slotIndexes[currentIndex]);
            UpdateItemInfoUI();
        }
        else
        {
            titleItemTMP.text = "";
            descriptionItemTMP.text = "";
        }
    }

    private void ClearCarousel()
    {
        for (int i = 0; i < itemModels.Count; i++)
        {
            if (itemModels[i] != null)
            {
                itemModels[i].DOKill();
                Destroy(itemModels[i].gameObject);
            }
        }

        itemModels.Clear();
        slotIndexes.Clear();
    }

    private void OnNavigate(InputAction.CallbackContext ctx)
    {
        if (isBuildingCarousel) return;
        if (isMoving) return;
        if (itemModels.Count == 0) return;

        Vector2 input = ctx.ReadValue<Vector2>();

        if (input.x > 0.5f)
        {
            MoveNext();
        }
        else if (input.x < -0.5f)
        {
            MovePrevious();
        }
    }

    private void OnSubmit(InputAction.CallbackContext ctx)
    {
        if (isBuildingCarousel) return;
        if (isMoving) return;

        UseCurrentItem();
    }

    private void UseCurrentItem()
    {
        if (Inventory.Instance == null)
            return;

        Inventory_Item currentItem = Inventory.Instance.GetCurrentItem();

        if (currentItem == null)
            return;

        bool success = false;

        switch (currentItem.typeItem)
        {
            case TypeItem.Use:

                Interactable target = crosshairController.InventoryTarget;

                if (target is DoorTransitionInteractable door)
                {
                    if (door.CanUseItem(currentItem))
                    {
                        door.UseItem(currentItem);

                        Inventory.Instance.RemoveItem(currentItem);

                        InventoryManager.Instance.CloseInventory(() =>
                        {
                            ShowItemUsedMessage();
                        });
                    }
                    else
                    {
                        InventoryManager.Instance.CloseInventory(() =>
                        {
                            ShowCannotUseMessage();
                        });
                    }
                }
                else
                {
                    InventoryManager.Instance.CloseInventory(() =>
                    {
                        ShowCannotUseMessage();
                    });
                }

                break;

            case TypeItem.Equip:
                success = currentItem.EquipItem();
                break;
        }

        if (success)
        {
            InventoryManager.Instance.CloseInventory();
        }
    }

    private void ShowCannotUseMessage() //Mjs que se muestra cuando no estas en el lugar correcto para usar ese objeto
    {
        MessageUIManager.Instance.StartObjectDescription(" ", new string[] { "No puedes usar esto aquí." });
    }

    private void ShowItemUsedMessage() //Mjs que se muestra cuando lo usaste correctamente
    {
        MessageUIManager.Instance.StartObjectDescription(" ", new string[] { "Has usado el WD-40 en la puerta de la terraza." });
    }

    private void MoveNext()
    {
        currentIndex++;

        if (currentIndex >= itemModels.Count)
            currentIndex = 0;

        UpdateTargetRotation();
    }

    private void MovePrevious()
    {
        currentIndex--;

        if (currentIndex < 0)
            currentIndex = itemModels.Count - 1;

        UpdateTargetRotation();
    }

    private void UpdateTargetRotation()
    {
        float angleStep = 360f / itemModels.Count;

        targetRotation = -currentIndex * angleStep;

        Inventory.Instance.SetCurrentIndex(slotIndexes[currentIndex]);
        UpdateItemInfoUI();

        isMoving = true;
    }

    private void BuildCarouselAnimation()
    {
        if (itemModels.Count == 0)
            return;

        isBuildingCarousel = true;

        float angleStep = 360f / itemModels.Count;
        int completedTweens = 0;

        for (int i = 0; i < itemModels.Count; i++)
        {
            float angle = (i * angleStep + currentRotation) * Mathf.Deg2Rad;

            float x = Mathf.Sin(angle) * radius;
            float z = Mathf.Cos(angle) * radius;

            Vector3 targetPosition = new Vector3(x, 0f, z);

            itemModels[i].localPosition = Vector3.zero;

            itemModels[i]
                .DOLocalMove(targetPosition, buildDuration)
                .SetDelay(i * buildDelayBetweenItems)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    completedTweens++;

                    if (completedTweens >= itemModels.Count)
                        isBuildingCarousel = false;
                });
        }
    }

    private void ArrangeItems()
    {
        if (itemModels.Count == 0)
            return;

        float angleStep = 360f / itemModels.Count;

        for (int i = 0; i < itemModels.Count; i++)
        {
            float angle = (i * angleStep + currentRotation) * Mathf.Deg2Rad;

            float x = Mathf.Sin(angle) * radius;
            float z = Mathf.Cos(angle) * radius;

            itemModels[i].localPosition = new Vector3(x, 0f, z);
        }
    }

    private void RotateSelectedItem()
    {
        if (itemModels.Count == 0)
            return;

        itemModels[currentIndex].Rotate(
            0f,
            50f * Time.deltaTime,
            0f,
            Space.Self
        );
    }

    public void CloseCarouselAnimation(System.Action onComplete)
    {
        if (itemModels.Count == 0)
        {
            onComplete?.Invoke();
            return;
        }

        isBuildingCarousel = true;

        int completedTweens = 0;

        for (int i = 0; i < itemModels.Count; i++)
        {
            itemModels[i].DOKill();

            itemModels[i]
                .DOLocalMove(Vector3.zero, closeDuration)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    completedTweens++;

                    if (completedTweens >= itemModels.Count)
                    {
                        isBuildingCarousel = false;
                        onComplete?.Invoke();
                    }
                });
        }
    }

    private void UpdateItemInfoUI()
    {
        if (Inventory.Instance == null)
            return;

        Inventory_Item currentItem = Inventory.Instance.GetCurrentItem();

        if (currentItem == null)
        {
            titleItemTMP.text = "";
            descriptionItemTMP.text = "";
            actionItemTMP.text = "";
            return;
        }

        titleItemTMP.text = currentItem.itemName;
        descriptionItemTMP.text = currentItem.description;

        switch (currentItem.typeItem)
        {
            case TypeItem.Use:
                actionItemTMP.text = "USAR: SPACE BAR";
                break;

            case TypeItem.Equip:
                actionItemTMP.text = "EQUIPAR: SPACE BAR";
                break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        const int segments = 64;

        Vector3 previousPoint = transform.position + Vector3.forward * radius;

        for (int i = 1; i <= segments; i++)
        {
            float angle = (i / (float)segments) * Mathf.PI * 2f;

            Vector3 newPoint = transform.position +
                               new Vector3(
                                   Mathf.Sin(angle) * radius,
                                   0f,
                                   Mathf.Cos(angle) * radius);

            Gizmos.DrawLine(previousPoint, newPoint);

            previousPoint = newPoint;
        }
    }
}