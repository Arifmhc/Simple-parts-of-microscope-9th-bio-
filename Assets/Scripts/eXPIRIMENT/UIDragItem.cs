using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(RectTransform))]
public class UIDragItem : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    [SerializeField] private string itemID;

    [Header("Drag")]
    [SerializeField] private float dragScale = 1.2f;
    [SerializeField] private float scaleLerpSpeed = 12f;

    [Header("Return")]
    [SerializeField] private float returnSpeed = 12f;

    [Tooltip("Distance from original position where return event fires")]
    [SerializeField] private float returnEventThreshold = 50f;

    [Header("Events")]
    public UnityEvent<string> OnReturnedWithID;
    public UnityEvent<string> OnCorrectSnapped;
    public UnityEvent OnEnabled;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvas;
    private Camera mainCamera;

    private Vector2 originalPosition;
    private Vector3 originalScale;

    private Coroutine moveRoutine;
    private Coroutine scaleRoutine;

    private bool returnEventTriggered;

    public string ItemID => itemID;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        mainCamera = Camera.main;

        originalScale = rectTransform.localScale;
    }

    private void OnEnable()
    {
        OnEnabled?.Invoke();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition;
        canvasGroup.blocksRaycasts = false;

        returnEventTriggered = false;

        StartScale(originalScale * dragScale);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition +=
            eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        if (mainCamera != null)
        {
            Ray ray = mainCamera.ScreenPointToRay(eventData.position);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GhostDropTarget target =
                    hit.collider.GetComponent<GhostDropTarget>();

                if (target != null && target.TryDrop(this))
                {
                    OnCorrectSnapped?.Invoke(itemID);
                    return;
                }
            }
        }

        StartReturn();
        StartScale(originalScale);
    }

    private void StartReturn()
    {
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(ReturnRoutine());
    }

    private IEnumerator ReturnRoutine()
    {
        while (Vector2.Distance(rectTransform.anchoredPosition, originalPosition) > 0.01f)
        {
            float distance = Vector2.Distance(rectTransform.anchoredPosition, originalPosition);

            if (!returnEventTriggered && distance <= returnEventThreshold)
            {
                returnEventTriggered = true;
                OnReturnedWithID?.Invoke(itemID);
            }

            rectTransform.anchoredPosition =
                Vector2.Lerp(rectTransform.anchoredPosition,
                             originalPosition,
                             Time.deltaTime * returnSpeed);

            yield return null;
        }

        rectTransform.anchoredPosition = originalPosition;
    }

    private void StartScale(Vector3 target)
    {
        if (scaleRoutine != null)
            StopCoroutine(scaleRoutine);

        scaleRoutine = StartCoroutine(ScaleRoutine(target));
    }

    private IEnumerator ScaleRoutine(Vector3 target)
    {
        while (Vector3.Distance(rectTransform.localScale, target) > 0.001f)
        {
            rectTransform.localScale =
                Vector3.Lerp(rectTransform.localScale,
                             target,
                             Time.deltaTime * scaleLerpSpeed);

            yield return null;
        }

        rectTransform.localScale = target;
    }
}