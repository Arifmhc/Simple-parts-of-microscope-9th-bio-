using UnityEngine;

using System.Collections;




namespace Partsofsimplemicroscpoe
{
    [DisallowMultipleComponent]

    public class TargetMoveToPosition : MonoBehaviour

    {

        [Header("Target Settings")]

        [SerializeField] private Vector3 targetPosition;

    

        [SerializeField] private bool useLocalPosition = false;

    

        [SerializeField] private float duration = 1f;

    

        [Header("Ease Curve")]

        [SerializeField] private AnimationCurve movementCurve =

            AnimationCurve.EaseInOut(0, 0, 1, 1);

    

        [Header("Debug")]

        [SerializeField] private bool enableDebug = false;

    

        private Coroutine moveRoutine;

    

        // =====================================================

        /// CALL THIS FUNCTION FROM BUTTON / EVENT / CODE

        public void MoveToTarget()

        {

            if (moveRoutine != null)

                StopCoroutine(moveRoutine);

    

            moveRoutine = StartCoroutine(MoveRoutine());

        }

    

        // =====================================================

        private IEnumerator MoveRoutine()

        {

            Vector3 startPosition =

                useLocalPosition ? transform.localPosition : transform.position;

    

            float time = 0f;

    

            while (time < duration)

            {

                time += Time.deltaTime;

    

                float normalizedTime = Mathf.Clamp01(time / duration);

    

                float curveValue = movementCurve.Evaluate(normalizedTime);

    

                Vector3 next = Vector3.Lerp(

                    startPosition,

                    targetPosition,

                    curveValue

                );

    

                if (useLocalPosition)

                    transform.localPosition = next;

                else

                    transform.position = next;

    

                yield return null;

            }

    

            if (useLocalPosition)

                transform.localPosition = targetPosition;

            else

                transform.position = targetPosition;

    

            if (enableDebug)

                Debug.Log("Reached target position", this);

    

            moveRoutine = null;

        }

    }
    
}