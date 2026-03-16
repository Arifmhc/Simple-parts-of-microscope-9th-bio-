#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[DisallowMultipleComponent]
public class TouchDragSnap3D : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera mainCamera;

    [Header("Drag Behaviour")]
    [SerializeField] private float cameraPullDistance = 1.5f;

    [Header("Snap Settings")]
    [SerializeField] private Transform snapPoint;
    [SerializeField] private float snapRange = 0.5f;
    [SerializeField] private float snapSpeed = 6f;
    [SerializeField] private float returnSpeed = 6f;
    [SerializeField] private Vector3 snapOffset = Vector3.zero;

    [Header("Snap Rotation")]
    [SerializeField] private bool includeSnapRotation = false;   // ⭐ NEW
    [SerializeField] private Vector3 snapRotation = Vector3.zero; // ⭐ NEW

    [Header("Snap Control")]
    [SerializeField] private bool allowSnap = true;

    [Header("Events")]
    [SerializeField] private UnityEvent onSnapped;
    [SerializeField] private UnityEvent onReturned;

    private Vector3 startPosition;
    private Quaternion startRotation;

    private bool isDragging;
    private float originalDepth;
    private float dragDepth;
    private Coroutine moveRoutine;

    private bool isReturning;
    private float returnProgress;
    private Vector3 returnStartPos;
    private Quaternion returnStartRot;

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

        if (GetComponent<Collider>() == null)
            gameObject.AddComponent<BoxCollider>();

        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    // =====================================================
    void OnEnable()
    {
        if (isReturning)
            moveRoutine = StartCoroutine(ReturnToStart_Resume());
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

    // ===================== PUBLIC =========================
    public void SetSnapEnabled(bool enabled)
    {
        allowSnap = enabled;
    }

    public void ForceReturnToInitial()
    {
        StopMoveRoutine();

        isDragging = false;
        isReturning = true;
        returnProgress = 0f;

        returnStartPos = transform.position;
        returnStartRot = transform.rotation;

        moveRoutine = StartCoroutine(ReturnToStart_Resume());
    }

    // ===================== INPUT =========================
    void HandleMouse()
    {
        if (Input.GetMouseButtonDown(0))
            TrySelect(Input.mousePosition);

        if (isDragging && Input.GetMouseButton(0))
            Drag(Input.mousePosition);

        if (isDragging && Input.GetMouseButtonUp(0))
            Release();
    }

    void HandleTouch()
    {
        if (Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
            TrySelect(touch.position);

        if (isDragging &&
            (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary))
            Drag(touch.position);

        if (isDragging &&
            (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
            Release();
    }

    // =====================================================
    void TrySelect(Vector2 screenPos)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            if (hit.transform == transform)
            {
                StopMoveRoutine();

                originalDepth =
                    Vector3.Distance(mainCamera.transform.position, transform.position);

                dragDepth = Mathf.Max(0.1f, originalDepth - cameraPullDistance);
                isDragging = true;
            }
        }
    }

    void Drag(Vector2 screenPos)
    {
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(
            new Vector3(screenPos.x, screenPos.y, dragDepth));

        transform.position = worldPos;
    }

    void Release()
    {
        isDragging = false;
        CheckSnap();
    }

    void CheckSnap()
    {
        StopMoveRoutine();

        if (!allowSnap || snapPoint == null)
        {
            ForceReturnToInitial();
            return;
        }

        float dist = Vector3.Distance(transform.position, snapPoint.position);

        if (dist <= snapRange)
            moveRoutine = StartCoroutine(SnapToPoint());
        else
            ForceReturnToInitial();
    }

    // =====================================================
    IEnumerator SnapToPoint()
    {
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        Vector3 targetPos =
            snapPoint.position + snapPoint.TransformDirection(snapOffset);

        Quaternion targetRot = startRot;

        if (includeSnapRotation)
            targetRot = Quaternion.Euler(snapRotation);

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * snapSpeed;

            transform.position =
                Vector3.Lerp(startPos, targetPos, t);

            transform.rotation =
                Quaternion.Lerp(startRot, targetRot, t);

            yield return null;
        }

        transform.position = targetPos;
        transform.rotation = targetRot;

        onSnapped?.Invoke();
    }

    // =====================================================
    IEnumerator ReturnToStart_Resume()
    {
        while (returnProgress < 1f)
        {
            returnProgress += Time.deltaTime * returnSpeed;

            transform.position =
                Vector3.Lerp(returnStartPos, startPosition, returnProgress);

            transform.rotation =
                Quaternion.Lerp(returnStartRot, startRotation, returnProgress);

            yield return null;
        }

        transform.position = startPosition;
        transform.rotation = startRotation;

        isReturning = false;
        returnProgress = 0f;

        onReturned?.Invoke();
    }

    // =====================================================
    void StopMoveRoutine()
    {
        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
            moveRoutine = null;
        }
    }

    // ===================== GIZMOS ========================
    void OnDrawGizmosSelected()
    {
        if (snapPoint == null)
            return;

        Gizmos.color = new Color(0f, 1f, 1f, 0.25f);
        Gizmos.DrawSphere(snapPoint.position, snapRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(snapPoint.position, snapRange);

#if UNITY_EDITOR
        EditorGUI.BeginChangeCheck();

        Vector3 worldOffset =
            snapPoint.position + snapPoint.TransformDirection(snapOffset);

        Vector3 newWorld =
            Handles.PositionHandle(worldOffset, snapPoint.rotation);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(this, "Move Snap Offset");

            snapOffset =
                snapPoint.InverseTransformDirection(newWorld - snapPoint.position);

            EditorUtility.SetDirty(this);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(snapPoint.position, worldOffset);
        Gizmos.DrawSphere(worldOffset, 0.05f);
#endif
    }
}