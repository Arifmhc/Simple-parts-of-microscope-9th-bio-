using System;
using UnityEngine;

public static class UIDragDropEvents
{
    public static Action<UIDraggItem, string> OnDropAttempt;
    public static Action<UIDraggItem> OnSnapAccepted;
    public static Action<UIDraggItem> OnSnapRejected;
}