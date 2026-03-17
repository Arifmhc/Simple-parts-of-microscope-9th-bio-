using UnityEngine;
using System.Collections.Generic;


namespace PartsOfSimpleMicroscope
{
    [DisallowMultipleComponent]
    public class CanvasPageObjectActivator : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CanvasGroupStepper stepper;
    
        [Header("ALL Objects (Master List)")]
        [SerializeField] private List<GameObject> commonObjectList = new List<GameObject>();
    
        [Header("Page-wise Object Mapping")]
        [SerializeField] private List<PageObjects> pageObjects = new List<PageObjects>();
    
        [Header("Debug")]
        [SerializeField] private bool enableDebug = false;
    
        private int lastIndex = -1;
    
        // =====================================================
        void OnEnable()
        {
            ForceRefresh();
        }
    
        // =====================================================
        void Update()
        {
            if (stepper == null) return;
    
            int currentIndex = stepper.CurrentIndex;
            if (currentIndex == lastIndex) return;
    
            lastIndex = currentIndex;
            ApplyPage(currentIndex);
        }
    
        // =====================================================
        void ForceRefresh()
        {
            if (stepper == null) return;
    
            lastIndex = -1;
            ApplyPage(stepper.CurrentIndex);
        }
    
        // =====================================================
        void ApplyPage(int index)
        {
            if (index < 0 || index >= pageObjects.Count)
                return;
    
            // 🔑 Objects required for this page
            HashSet<GameObject> required =
                pageObjects[index].GetObjectSet();
    
            // 🔁 Only change what is needed
            for (int i = 0; i < commonObjectList.Count; i++)
            {
                GameObject obj = commonObjectList[i];
                if (obj == null) continue;
    
                bool shouldBeActive = required.Contains(obj);
    
                if (obj.activeSelf != shouldBeActive)
                    obj.SetActive(shouldBeActive);
            }
    
            Log($"Applied page {index}");
        }
    
        // =====================================================
        void Log(string msg)
        {
            if (enableDebug)
                Debug.Log($"[CanvasPageObjectActivator] {msg}", this);
        }
    }
    
    // =====================================================
    [System.Serializable]
    public class PageObjects
    {
        [SerializeField] private List<GameObject> objects = new List<GameObject>();
    
        public HashSet<GameObject> GetObjectSet()
        {
            HashSet<GameObject> set = new HashSet<GameObject>();
    
            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i] != null)
                    set.Add(objects[i]);
            }
    
            return set;
        }
    }
    
}