using UnityEngine;
using System.Collections;


namespace PartsOfSimpleMicroscope
{
    public class UIImageFade : MonoBehaviour
    {
        [Header("Target Canvas Group")]
        public CanvasGroup targetCanvasGroup;
    
        [Header("Fade Settings")]
        public float fadeDuration = 1f;
    
        public bool fadeInOnStart = false;
    
        private Coroutine fadeCoroutine;
    
    
        void Update()
        {
            if (fadeInOnStart == true)
            {
                FadeToAlpha(1f);
               // Only fade in once on start
            }
        }
    
        // 🔥 Call this from EventTrigger / Button
        public void FadeToAlpha(float targetAlpha)
        {
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);
    
            float currentAlpha = targetCanvasGroup.alpha;
            fadeCoroutine = StartCoroutine(FadeImage(currentAlpha, targetAlpha));
        }
    
        IEnumerator FadeImage(float startAlpha, float endAlpha)
        {
            float time = 0f;
    
            while (time < fadeDuration)
            {
                float alpha = Mathf.Lerp(startAlpha, endAlpha, time / fadeDuration);
                targetCanvasGroup.alpha = alpha;
    
                time += Time.deltaTime;
                yield return null;
            }
    
            targetCanvasGroup.alpha = endAlpha;
        }
    }
    
}