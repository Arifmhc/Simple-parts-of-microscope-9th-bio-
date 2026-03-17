using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


namespace PartsOfSimpleMicroscope
{
    [DisallowMultipleComponent]
    public class ObjectStepper : MonoBehaviour
    {
        [Header("Objects (Order Matters)")]
        [SerializeField] private List<GameObject> objects = new List<GameObject>();
    
        [Header("Navigation Buttons")]
        [SerializeField] private Button nextButton;
        [SerializeField] private Button backButton;
    
        [Header("Lock Settings")]
        [Tooltip("Enable or disable the lock system completely")]
        [SerializeField] private bool enableLockSystem = true;   // ⭐ NEW
    
        [Header("Debug")]
        [SerializeField] private bool enableDebug = false;
    
        private int currentIndex = 0;
        private bool isLocked = false;
    
        // remembers which forward steps already locked once
        private HashSet<int> lockedForwardIndices = new HashSet<int>();
    
        // =====================================================
        void Awake()
        {
            Initialize();
        }
    
        void Start()
        {
            LockButtons();   // Ensure buttons are in correct state based on lock system
        }
    
        // =====================================================
        void Initialize()
        {
            if (objects == null || objects.Count == 0)
                return;
    
            SetActiveIndex(0, allowLock: false);
            Log("Initialized at index 0");
        }
    
        // =====================================================
        // CALL FROM NEXT BUTTON
        public void Next()
        {
            if (enableLockSystem && isLocked)
                return;
    
            if (currentIndex >= objects.Count - 1)
                return;
    
            int nextIndex = currentIndex + 1;
    
            bool shouldLock =
                enableLockSystem &&
                !lockedForwardIndices.Contains(nextIndex);
    
            SetActiveIndex(nextIndex, shouldLock);
    
            if (shouldLock)
                lockedForwardIndices.Add(nextIndex);
        }
    
        // =====================================================
        // CALL FROM BACK BUTTON
        public void Back()
        {
            if (enableLockSystem && isLocked)
                return;
    
            if (currentIndex <= 0)
                return;
    
            SetActiveIndex(currentIndex - 1, allowLock: false);
        }
    
        // =====================================================
        void SetActiveIndex(int newIndex, bool allowLock)
        {
            if (newIndex < 0 || newIndex >= objects.Count)
                return;
    
            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i] != null)
                    objects[i].SetActive(i == newIndex);
            }
    
            currentIndex = newIndex;
    
            if (enableLockSystem && allowLock)
                LockButtons();
    
            Log($"Switched to index {currentIndex}");
        }
    
        // =====================================================
        void LockButtons()
        {
            isLocked = true;
    
            if (nextButton != null)
                nextButton.interactable = false;
    
            if (backButton != null)
                backButton.interactable = false;
    
            Log("Buttons locked");
        }
    
        // =====================================================
        /// CALL FROM EVENT (animation / timeline / etc.)
        public void Unlock()
        {
            isLocked = false;
    
            if (nextButton != null)
                nextButton.interactable = true;
    
            if (backButton != null)
                backButton.interactable = true;
    
            Log("Buttons unlocked");
        }
    
        // =====================================================
        /// Optional: turn lock system ON at runtime
        public void EnableLockSystem()
        {
            enableLockSystem = true;
            Log("Lock system enabled");
        }
    
        // =====================================================
        /// Optional: turn lock system OFF at runtime
        public void DisableLockSystem()
        {
            enableLockSystem = false;
            isLocked = false;
    
            if (nextButton != null)
                nextButton.interactable = true;
    
            if (backButton != null)
                backButton.interactable = true;
    
            Log("Lock system disabled");
        }
    
        // =====================================================
        void Log(string msg)
        {
            if (enableDebug)
                Debug.Log($"[ObjectStepper] {msg}", this);
        }
    }
    
}