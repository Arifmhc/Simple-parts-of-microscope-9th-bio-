using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(RectTransform))]
public class DropSlotUI2 : MonoBehaviour, IDropHandler
{
    [Header("Correct Draggable ID")]
    [SerializeField] private string correctItemID;

    [Header("Snap Scale")]
    [SerializeField] private bool applySnapScale = true;
    [SerializeField] private Vector3 snapScale = Vector3.one;

    [Header("Font Settings")]
    [SerializeField] private float targetFontSize = 36f;

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

        // Force Z = 0
        Vector3 localPos = draggableRect.localPosition;
        localPos.z = 0f;
        draggableRect.localPosition = localPos;

        // Apply Snap Scale
        if (applySnapScale)
        {
            draggableRect.localScale = snapScale;
        }

        TMP_Text tmp = draggable.GetComponentInChildren<TMP_Text>();
        if (tmp != null)
        {
            tmp.enableAutoSizing = false;
            tmp.fontSize = targetFontSize;
        }
    }

    public void ResetSlot()
    {
        IsOccupied = false;
    }
}