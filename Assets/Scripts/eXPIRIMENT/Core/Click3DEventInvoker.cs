using UnityEngine;
using UnityEngine.Events;


namespace PartsOfSimpleMicroscope
{
    [DisallowMultipleComponent]
    public class Click3DEventInvoker : MonoBehaviour
    {
        [Header("Camera")]
        [SerializeField] private Camera mainCamera;
    
        [Header("Interaction")]
        [SerializeField] private bool isInteractable = true;
    
        [Header("Event")]
        [SerializeField] private UnityEvent onClicked;
    
        [Header("Debug")]
        [SerializeField] private bool enableDebug = false;
        [SerializeField] private bool drawDebugRay = false;
    
        private Collider cachedCollider;
    
        // =====================================================
        void Awake()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;
    
            if (mainCamera == null)
            {
                Debug.LogError("Camera not assigned", this);
                enabled = false;
                return;
            }
    
            cachedCollider = GetComponent<Collider>();
            if (cachedCollider == null)
            {
                cachedCollider = gameObject.AddComponent<BoxCollider>();
                Log("No collider found. BoxCollider added automatically.");
            }
        }
    
        // =====================================================
        void Update()
        {
    #if UNITY_EDITOR
            HandleMouse();
    #else
            HandleTouch();
    #endif
        }
    
        // ===================== PUBLIC API =====================
    
        public void SetInteractable(bool value)
        {
            isInteractable = value;
            Log($"Interactable set to {value}");
        }
    
        public void EnableInteraction()
        {
            isInteractable = true;
            Log("Interaction enabled");
        }
    
        public void DisableInteraction()
        {
            isInteractable = false;
            Log("Interaction disabled");
        }
    
        // =====================================================
        void HandleMouse()
        {
            if (Input.GetMouseButtonDown(0))
            {
                ProcessInput(Input.mousePosition);
            }
        }
    
        // =====================================================
        void HandleTouch()
        {
            if (Input.touchCount == 0)
                return;
    
            Touch touch = Input.GetTouch(0);
    
            if (touch.phase == TouchPhase.Began)
            {
                ProcessInput(touch.position);
            }
        }
    
        // =====================================================
        void ProcessInput(Vector2 screenPosition)
        {
            if (!isInteractable)
            {
                Log("Click ignored (not interactable)");
                return;
            }
    
            Ray ray = mainCamera.ScreenPointToRay(screenPosition);
    
            if (drawDebugRay)
                Debug.DrawRay(ray.origin, ray.direction * 100f, Color.green, 1f);
    
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                if (hit.collider == cachedCollider)
                {
                    Log("Object clicked → invoking event");
                    onClicked?.Invoke();
                }
            }
        }
    
        // =====================================================
        void Log(string msg)
        {
            if (enableDebug)
                Debug.Log($"[Click3DEventInvoker] {msg}", this);
        }
    }
    
}