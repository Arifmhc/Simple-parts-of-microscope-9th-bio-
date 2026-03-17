using UnityEngine;

using System.Collections;

using System.Collections.Generic;




namespace Partsofsimplemicroscpoe
{
    [DisallowMultipleComponent]

    public class UIReturnCanvasGroupController : MonoBehaviour

    {

        [Header("Canvas Groups (Index Driven)")]

        [SerializeField] private List<CanvasGroup> canvasGroups = new List<CanvasGroup>();

    

        [Header("ItemID → CanvasGroup Index")]

        [SerializeField] private List<ItemCanvasMap> mappings = new List<ItemCanvasMap>();

    

        [Header("Timing")]

        [SerializeField] private float visibleDuration = 2f;

        [SerializeField] private float fadeSpeed = 8f;

    

        [Header("Objects To Toggle")]

        [Tooltip("Disabled when any canvas alpha = 1, enabled when alpha = 0")]

        [SerializeField] private List<GameObject> objectsToToggle = new List<GameObject>();

    

        private Coroutine activeRoutine;

        private CanvasGroup activeGroup;

    

        // =====================================================

        void Awake()

        {

            // Ensure all canvases start hidden

            for (int i = 0; i < canvasGroups.Count; i++)

            {

                if (canvasGroups[i] != null)

                    canvasGroups[i].alpha = 0f;

            }

    

            EnableToggleObjects();

        }

    

        // =====================================================

        /// CALL THIS FROM UIDragItem EVENT

        public void OnItemReturned(string itemID)

        {

            int index = GetIndexForID(itemID);

            if (index < 0 || index >= canvasGroups.Count)

                return;

    

            CanvasGroup targetGroup = canvasGroups[index];

    

            // Stop previous animation

            if (activeRoutine != null)

                StopCoroutine(activeRoutine);

    

            // Force previous canvas OFF

            if (activeGroup != null && activeGroup != targetGroup)

                activeGroup.alpha = 0f;

    

            activeGroup = targetGroup;

            activeRoutine = StartCoroutine(ShowRoutine(targetGroup));

        }

    

        // =====================================================

        IEnumerator ShowRoutine(CanvasGroup group)

        {

            // 🔒 Disable objects when UI becomes visible

            DisableToggleObjects();

    

            yield return Fade(group, 1f);

            yield return new WaitForSeconds(visibleDuration);

            yield return Fade(group, 0f);

    

            // 🔓 Enable objects after UI fully hidden

            EnableToggleObjects();

    

            activeRoutine = null;

        }

    

        // =====================================================

        IEnumerator Fade(CanvasGroup group, float target)

        {

            while (!Mathf.Approximately(group.alpha, target))

            {

                group.alpha = Mathf.MoveTowards(

                    group.alpha,

                    target,

                    Time.deltaTime * fadeSpeed

                );

                yield return null;

            }

    

            group.alpha = target;

        }

    

        // =====================================================

        void DisableToggleObjects()

        {

            for (int i = 0; i < objectsToToggle.Count; i++)

            {

                if (objectsToToggle[i] != null)

                    objectsToToggle[i].SetActive(false);

            }

        }

    

        // =====================================================

        void EnableToggleObjects()

        {

            for (int i = 0; i < objectsToToggle.Count; i++)

            {

                if (objectsToToggle[i] != null)

                    objectsToToggle[i].SetActive(true);

            }

        }

    

        // =====================================================

        int GetIndexForID(string id)

        {

            for (int i = 0; i < mappings.Count; i++)

            {

                if (mappings[i].itemID == id)

                    return mappings[i].canvasIndex;

            }

            return -1;

        }

    }

    

    // =====================================================

    [System.Serializable]

    public class ItemCanvasMap

    {

        public string itemID;

        public int canvasIndex;

    }
    
}