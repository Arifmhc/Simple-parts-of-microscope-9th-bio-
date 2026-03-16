using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
public class UIDraggItem: MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    [SerializeField] private string itemKey;

    private RectTransform rect;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    private Vector2 startPos;

    public string Key => itemKey;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPos = rect.anchoredPosition;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rect.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        UIDragDropEvents.OnDropAttempt?.Invoke(this, itemKey);
    }

    public void Return()
    {
        rect.anchoredPosition = startPos;
    }
}