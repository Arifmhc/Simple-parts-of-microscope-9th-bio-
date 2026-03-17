using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


namespace PartsOfSimpleMicroscope
{
    [DisallowMultipleComponent]
    public class RotationController : MonoBehaviour
    {
        [Header("Targets")]
        [SerializeField] private Transform horizontalTarget;
        [SerializeField] private Transform verticalTarget;
    
        [Header("Sliders")]
        [SerializeField] private Slider horizontalSlider;
        [SerializeField] private Slider verticalSlider;
    
        [Header("Rotation Count")]
        [Tooltip("How many full rotations the slider represents")]
        [SerializeField] private float horizontalRotations = 1f;
    
        [SerializeField] private float verticalRotations = 1f;
    
        [Header("Hard stop values (0–1)")]
        [SerializeField] private float horizontalStop = 0.6f;
        [SerializeField] private float verticalStop = 0.4f;
    
        [Header("UI")]
        [SerializeField] private GameObject successPanel;
    
        [Header("Events")]
        public UnityEvent onCompleted;   // ⭐ NEW EVENT
    
        private bool horizontalReached;
        private bool verticalReached;
        private bool completed;
    
        // =====================================================
        void OnEnable()
        {
            horizontalReached = false;
            verticalReached = false;
            completed = false;
    
            if (successPanel != null)
                successPanel.SetActive(false);
    
            if (horizontalSlider != null)
                horizontalSlider.interactable = true;
    
            if (verticalSlider != null)
                verticalSlider.interactable = true;
    
            UpdateHorizontalRotation();
            UpdateVerticalRotation();
        }
    
        // =====================================================
        public void SetHorizontal(float value)
        {
            if (horizontalSlider == null || horizontalTarget == null)
                return;
    
            if (!horizontalReached && value >= horizontalStop)
            {
                horizontalReached = true;
    
                horizontalSlider.SetValueWithoutNotify(horizontalStop);
                horizontalSlider.interactable = false;
            }
    
            UpdateHorizontalRotation();
            TryComplete();
        }
    
        // =====================================================
        public void SetVertical(float value)
        {
            if (verticalSlider == null || verticalTarget == null)
                return;
    
            if (!verticalReached && value >= verticalStop)
            {
                verticalReached = true;
    
                verticalSlider.SetValueWithoutNotify(verticalStop);
                verticalSlider.interactable = false;
            }
    
            UpdateVerticalRotation();
            TryComplete();
        }
    
        // =====================================================
        void UpdateHorizontalRotation()
        {
            float angle = horizontalSlider.value * 360f * horizontalRotations;
    
            horizontalTarget.localRotation =
                Quaternion.Euler(0f, 0f, angle);
        }
    
        // =====================================================
        void UpdateVerticalRotation()
        {
            float angle = verticalSlider.value * 360f * verticalRotations;
    
            verticalTarget.localRotation =
                Quaternion.Euler(angle, 0f, 0f);
        }
    
        // =====================================================
        void TryComplete()
        {
            if (completed)
                return;
    
            if (horizontalReached && verticalReached)
            {
                completed = true;
    
                if (successPanel != null)
                    successPanel.SetActive(true);
    
                onCompleted?.Invoke();   // ⭐ EVENT FIRED
            }
        }
    }
    
}