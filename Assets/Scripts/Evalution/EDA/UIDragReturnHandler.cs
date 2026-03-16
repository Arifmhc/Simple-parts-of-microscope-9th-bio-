using UnityEngine;

public class UIDragReturnHandler : MonoBehaviour
{
    private void OnEnable()
    {
        UIDragDropEvents.OnSnapAccepted += HandleAccepted;
        UIDragDropEvents.OnDropAttempt += HandleRejected;
    }

    private void OnDisable()
    {
        UIDragDropEvents.OnSnapAccepted -= HandleAccepted;
        UIDragDropEvents.OnDropAttempt -= HandleRejected;
    }

    void HandleAccepted(UIDraggItem item)
    {
        // do nothing
    }

    void HandleRejected(UIDraggItem item, string key)
    {
        item.Return();
    }
}