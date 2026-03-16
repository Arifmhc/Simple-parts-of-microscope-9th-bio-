using UnityEngine;
using UnityEngine.Events;

public class UIDropCompletionController : MonoBehaviour
{
    [SerializeField] private int totalItems = 6;

    public UnityEvent OnAllItemsSnapped;

    private int snappedCount;

    private void OnEnable()
    {
        DraggableScreenSpace.OnItemSnapped += HandleSnap;
    }

    private void OnDisable()
    {
        DraggableScreenSpace.OnItemSnapped -= HandleSnap;
    }

    private void HandleSnap()
    {
        snappedCount++;

        if (snappedCount >= totalItems)
        {
            OnAllItemsSnapped?.Invoke();
        }
    }
}