using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(RectTransform))]
public class DropSlotUI : MonoBehaviour, IDropHandler
{
    [Header("Correct Draggable ID")]
    [SerializeField] private string correctItemID;

    [Header("Scale Settings")]
    [SerializeField] private bool scaleOnDrop = true;
    [SerializeField] private Vector3 targetScale = Vector3.one;
    [SerializeField] private float targetFontSize = 36f;

    [Header("Deactivate Target")]
    [SerializeField] private GameObject deactivateTarget; // object to disable after snap

    public bool IsOccupied { get; private set; }

    public void OnDrop(PointerEventData eventData)
    {
        if (IsOccupied || eventData.pointerDrag == null)
            return;

        DraggableScreenSpace draggable =
            eventData.pointerDrag.GetComponent<DraggableScreenSpace>();

        if (draggable == null)
            return;

        if (!string.Equals(draggable.ItemID, correctItemID))
            return;

        IsOccupied = true;

        RectTransform draggableRect =
            draggable.GetComponent<RectTransform>();

        draggable.MarkDroppedCorrectly(transform as RectTransform);

        // Anchor middle center
        draggableRect.anchorMin = new Vector2(0.5f, 0.5f);
        draggableRect.anchorMax = new Vector2(0.5f, 0.5f);
        draggableRect.pivot = new Vector2(0.5f, 0.5f);

        // Center position
        draggableRect.anchoredPosition = Vector2.zero;

        // Ensure Z = 0
        Vector3 localPos = draggableRect.localPosition;
        localPos.z = 0f;
        draggableRect.localPosition = localPos;

        // Apply scale
        if (scaleOnDrop)
        {
            draggableRect.localScale = targetScale;
        }

        // Font settings
        TMP_Text tmp = draggable.GetComponentInChildren<TMP_Text>();
        if (tmp != null)
        {
            tmp.enableAutoSizing = false;
            tmp.fontSize = targetFontSize;
        }

        // Disable assigned object instead of this slot
        if (deactivateTarget != null)
        {
            deactivateTarget.SetActive(false);
        }
    }

    public void ResetSlot()
    {
        IsOccupied = false;

        if (deactivateTarget != null)
        {
            deactivateTarget.SetActive(true);
        }
    }
}