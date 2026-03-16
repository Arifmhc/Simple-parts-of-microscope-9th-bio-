using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[DisallowMultipleComponent]
public class DelayedEventInvoker : MonoBehaviour
{
    [Header("Delay Settings")]
    [Tooltip("Delay in seconds before invoking the event")]
    [SerializeField] private float delaySeconds = 1f;

    [Header("Event")]
    [SerializeField] private UnityEvent onDelayedInvoke;

    [Header("Debug")]
    [SerializeField] private bool enableDebug = false;
    [Tooltip("Log remaining time every frame (use only for debugging)")]
    [SerializeField] private bool logCountdown = false;

    private Coroutine delayRoutine;

    // =====================================================
    /// CALL FROM BUTTON / UNITY EVENT / CODE
    public void InvokeAfterDelay()
    {
        if (delayRoutine != null)
        {
            StopCoroutine(delayRoutine);
            Log("Previous timer stopped");
        }

        delayRoutine = StartCoroutine(DelayRoutine());
        Log($"Timer started for {delaySeconds} seconds");
    }

    // =====================================================
    IEnumerator DelayRoutine()
    {
        float remaining = delaySeconds;

        while (remaining > 0f)
        {
            if (logCountdown)
                Log($"Time remaining: {remaining:F2}s");

            remaining -= Time.deltaTime;
            yield return null;
        }

        Log("Timer completed → invoking event");
        onDelayedInvoke?.Invoke();
        delayRoutine = null;
    }

    // =====================================================
    /// Optional: cancel pending invoke
    public void Cancel()
    {
        if (delayRoutine != null)
        {
            StopCoroutine(delayRoutine);
            delayRoutine = null;
            Log("Timer cancelled");
        }
    }

    // =====================================================
    void Log(string msg)
    {
        if (enableDebug)
            Debug.Log($"[DelayedEventInvoker] {msg}", this);
    }
}