using UnityEngine;

using UnityEngine.EventSystems;




namespace Partofsimplemicroscpoe
{
    [RequireComponent(typeof(RectTransform))]

    public class BringToFrontOnMove : MonoBehaviour, IBeginDragHandler, IDragHandler

    {

        private RectTransform rectTransform;

        private Transform grandParentTransform;

    

        // =====================================================

        void Awake()

        {

            rectTransform = GetComponent<RectTransform>();

    

            if (transform.parent != null && transform.parent.parent != null)

            {

                grandParentTransform = transform.parent.parent;

            }

            else

            {

                Debug.LogWarning(

                    $"{nameof(BringToFrontOnMove)} requires a grandparent to function.",

                    this

                );

            }

        }

    

        // =====================================================

        public void OnBeginDrag(PointerEventData eventData)

        {

            BringToFront();

        }

    

        // =====================================================

        public void OnDrag(PointerEventData eventData)

        {

            BringToFront();

        }

    

        // =====================================================

        private void BringToFront()

        {

            if (grandParentTransform == null)

                return;

    

            // Move THIS object under grandparent and bring to front

            if (transform.parent != grandParentTransform)

                transform.SetParent(grandParentTransform, worldPositionStays: true);

    

            if (transform.GetSiblingIndex() != grandParentTransform.childCount - 1)

                transform.SetAsLastSibling();

        }

    }
    
}