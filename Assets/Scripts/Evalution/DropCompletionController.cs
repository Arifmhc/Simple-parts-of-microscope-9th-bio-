using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class DropCompletionController : MonoBehaviour
{
    [SerializeField] private List<DropSlotUI> dropSlots = new();
    public UnityEvent onAllDropsCompleted;

    private int completedCount;
    private bool fired;

    public void NotifySlotCompleted()
    {
        if (fired)
            return;

        completedCount++;

        if (completedCount >= dropSlots.Count)
        {
            fired = true;
            onAllDropsCompleted?.Invoke();
        }
    }

    public void ResetCompletion()
    {
        completedCount = 0;
        fired = false;

        foreach (var slot in dropSlots)
            slot.ResetSlot();
    }
}