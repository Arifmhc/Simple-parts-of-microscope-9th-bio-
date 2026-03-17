using UnityEngine;
using UnityEngine.Events;


namespace PartsOfSimpleMicroscope
{
    public class EnableEventBroadcasterignorefirst : MonoBehaviour
    {
        public UnityEvent OnEnabled;
        public UnityEvent OnDisabled;
        public UnityEvent OnStart;
    
        private bool hasStarted;   // true after Start()
        private bool firstEnable = true;
    
        private void Start()
        {
            hasStarted = true;
    
            OnStart?.Invoke();     // fires once
            // ❌ Do NOT call OnEnabled here
        }
    
        private void OnEnable()
        {
            // ❌ Skip the very first enable (scene load)
            if (firstEnable)
            {
                firstEnable = false;
                return;
            }
    
            // ✅ Only fires on re-enable
            if (hasStarted)
                OnEnabled?.Invoke();
        }
    
        private void OnDisable()
        {
            if (hasStarted)
                OnDisabled?.Invoke();
        }
    }
    
    
}