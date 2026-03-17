using UnityEngine;
using UnityEngine.Playables;
using System.Collections.Generic;


namespace PartsOfSimpleMicroscope
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayableDirector))]
    public class TimelineIndexRangePlayer : MonoBehaviour
    {
        [Header("Timeline")]
        [SerializeField] private PlayableDirector director;
    
        [Header("Ranges (Index Driven)")]
        [SerializeField] private List<TimelineRange> ranges = new List<TimelineRange>();
    
        [Header("Reverse Settings")]
        [SerializeField] private float reverseSpeed = 1f;
    
        [Header("Debug")]
        [SerializeField] private bool enableDebug = false;
    
        private double targetEndTime;
        private bool isPlaying;
        private bool isReverse;
    
        // =====================================================
        void Awake()
        {
            if (director == null)
                director = GetComponent<PlayableDirector>();
    
            director.Pause();
        }
    
        // =====================================================
        /// CALL FROM UNITY EVENT / BUTTON / SIGNAL
        public void PlayRangeByIndex(int index)
        {
            if (index < 0 || index >= ranges.Count)
            {
                Log($"Invalid index {index}");
                return;
            }
    
            TimelineRange range = ranges[index];
    
            if (Mathf.Approximately(range.startTime, range.endTime))
            {
                Log($"Zero-length range at index {index}");
                return;
            }
    
            StartRange(range.startTime, range.endTime);
        }
    
        // =====================================================
        void StartRange(double startTime, double endTime)
        {
            isReverse = endTime < startTime;
            targetEndTime = endTime;
            isPlaying = true;
    
            director.time = startTime;
    
            if (!isReverse)
            {
                director.Play();
            }
            else
            {
                director.Pause();
                director.Evaluate();
            }
    
            Log(isReverse
                ? $"Playing REVERSE {startTime} → {endTime}"
                : $"Playing FORWARD {startTime} → {endTime}");
        }
    
        // =====================================================
        void Update()
        {
            if (!isPlaying)
                return;
    
            if (!isReverse)
            {
                if (director.time >= targetEndTime)
                {
                    director.Pause();
                    isPlaying = false;
                    Log("Forward range completed");
                }
            }
            else
            {
                director.time -= Time.deltaTime * reverseSpeed;
                director.Evaluate();
    
                if (director.time <= targetEndTime)
                {
                    director.time = targetEndTime;
                    director.Evaluate();
                    director.Pause();
                    isPlaying = false;
                    Log("Reverse range completed");
                }
            }
        }
    
        // =====================================================
        // ⭐ NEW FUNCTION
        /// <summary>
        /// Instantly jumps timeline to a target time and updates scene.
        /// Callable from UnityEvent / Button / Signal.
        /// </summary>
        public void JumpToTime(float targetTime)
        {
            isPlaying = false;
            director.Pause();
            director.time = targetTime;
            director.Evaluate();
    
            Log($"Jumped to time {targetTime}s");
        }
    
        // =====================================================
        public void PauseTimeline()
        {
            director.Pause();
            isPlaying = false;
            Log("Timeline paused manually");
        }
    
        // =====================================================
        void Log(string msg)
        {
            if (enableDebug)
                Debug.Log($"[TimelineIndexRangePlayer] {msg}", this);
        }
    }
    
    // =====================================================
    [System.Serializable]
    public class TimelineRange
    {
        public float startTime;
        public float endTime;
    }
    
}