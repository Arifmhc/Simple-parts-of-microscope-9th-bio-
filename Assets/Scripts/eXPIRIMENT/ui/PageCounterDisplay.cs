using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace PartsOfSimpleMicroscope
{
    [DisallowMultipleComponent]
    public class SimplePageCounter : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TMP_Text pageText;
    
        [Header("Buttons")]
        [SerializeField] private Button nextButton;
        [SerializeField] private Button backButton;
    
        [Header("Settings")]
        [SerializeField] private int totalSlides = 5;
        [SerializeField] private int indexOffset = 1; // 1 = show 1-based index
    
        [Header("Debug")]
        [SerializeField] private bool enableDebug = false;
    
        private int currentIndex = 0;
    
        // =====================================================
        void Start()
        {
            if (nextButton)
                nextButton.onClick.AddListener(Next);
    
            if (backButton)
                backButton.onClick.AddListener(Back);
    
            UpdateDisplay();
            UpdateButtonState();
        }
    
        // =====================================================
        public void Next()
        {
            if (currentIndex >= totalSlides - 1)
                return;
    
            currentIndex++;
            UpdateDisplay();
            UpdateButtonState();
    
            Log("Next clicked");
        }
    
        // =====================================================
        public void Back()
        {
            if (currentIndex <= 0)
                return;
    
            currentIndex--;
            UpdateDisplay();
            UpdateButtonState();
    
            Log("Back clicked");
        }
    
        // =====================================================
        private void UpdateDisplay()
        {
            if (pageText == null)
                return;
    
            pageText.text = $"{currentIndex + indexOffset} / {totalSlides}";
        }
    
        // =====================================================
        private void UpdateButtonState()
        {
            if (nextButton)
                nextButton.interactable = currentIndex < totalSlides - 1;
    
            if (backButton)
                backButton.interactable = currentIndex > 0;
        }
    
        // =====================================================
        private void Log(string msg)
        {
            if (enableDebug)
                Debug.Log($"[SimplePageCounter] {msg}", this);
        }
    }
    
}