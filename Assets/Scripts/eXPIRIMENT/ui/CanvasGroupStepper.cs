using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace PartsOfSimpleMicroscope
{
    [DisallowMultipleComponent]
    public class CanvasGroupStepper : MonoBehaviour
    {
        [Header("Canvas Groups (Order Matters)")]
        [SerializeField] private List<CanvasGroup> canvasGroups = new List<CanvasGroup>();
    
        [Header("Fade")]
        [SerializeField] private bool smoothFade = true;
        [SerializeField] private float fadeSpeed = 8f;
    
        [Header("Debug")]
        [SerializeField] private bool enableDebug = false;
    
        private int currentIndex = 0;
        public int CurrentIndex => currentIndex;
    
        private Coroutine fadeOutRoutine;
        private Coroutine fadeInRoutine;
    
        // =====================================================
        void Awake()
        {
            Initialize();
        }
    
        // =====================================================
        void Initialize()
        {
            if (canvasGroups.Count == 0)
                return;
    
            for (int i = 0; i < canvasGroups.Count; i++)
            {
                SetInstant(canvasGroups[i], i == currentIndex);
            }
    
            Log($"Initialized at index {currentIndex}");
        }
    
        // =====================================================
        public void Next()
        {
            if (currentIndex >= canvasGroups.Count - 1)
                return;
    
            ChangeIndex(currentIndex + 1);
        }
    
        // =====================================================
        public void Back()
        {
            if (currentIndex <= 0)
                return;
    
            ChangeIndex(currentIndex - 1);
        }
    
        // =====================================================
        void ChangeIndex(int newIndex)
        {
            if (newIndex < 0 || newIndex >= canvasGroups.Count)
                return;
    
            CanvasGroup current = canvasGroups[currentIndex];
            CanvasGroup next = canvasGroups[newIndex];
    
            if (smoothFade)
            {
                if (fadeOutRoutine != null)
                    StopCoroutine(fadeOutRoutine);
    
                if (fadeInRoutine != null)
                    StopCoroutine(fadeInRoutine);
    
                fadeOutRoutine = StartCoroutine(FadeOut(current));
                fadeInRoutine = StartCoroutine(FadeIn(next));
            }
            else
            {
                SetInstant(current, false);
                SetInstant(next, true);
            }
    
            currentIndex = newIndex;
            Log($"Switched to index {currentIndex}");
        }
    
        // =====================================================
        void SetInstant(CanvasGroup cg, bool visible)
        {
            if (!cg) return;
    
            cg.alpha = visible ? 1f : 0f;
            cg.interactable = visible;
            cg.blocksRaycasts = visible;
        }
    
        // =====================================================
        IEnumerator FadeOut(CanvasGroup cg)
        {
            if (!cg) yield break;
    
            cg.interactable = false;
            cg.blocksRaycasts = false;
    
            while (cg.alpha > 0f)
            {
                cg.alpha = Mathf.MoveTowards(
                    cg.alpha,
                    0f,
                    Time.deltaTime * fadeSpeed
                );
    
                yield return null;
            }
    
            cg.alpha = 0f;
        }
    
        // =====================================================
        IEnumerator FadeIn(CanvasGroup cg)
        {
            if (!cg) yield break;
    
            while (cg.alpha < 1f)
            {
                cg.alpha = Mathf.MoveTowards(
                    cg.alpha,
                    1f,
                    Time.deltaTime * fadeSpeed
                );
    
                yield return null;
            }
    
            cg.alpha = 1f;
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }
    
        // =====================================================
        void Log(string msg)
        {
            if (enableDebug)
                Debug.Log($"[CanvasGroupStepper] {msg}", this);
        }
    }
    
}