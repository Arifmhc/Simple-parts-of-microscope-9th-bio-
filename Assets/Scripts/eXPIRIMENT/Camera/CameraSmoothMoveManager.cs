using UnityEngine;
using System.Collections;


namespace PartsOfSimpleMicroscope
{
    public class CameraSmoothMoveManager : MonoBehaviour
    {
        [Header("Camera")]
        public Transform cameraTransform;
    
        [Header("Initial Pose")]
        public Transform initialPose;
    
        [Header("Movement Settings")]
        public float moveDuration = 1.2f;
        public AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
        [Header("Thresholds")]
        public float positionThreshold = 0.01f;
        public float rotationThreshold = 0.5f;
    
        [Header("Camera Points (Index Based)")]
        public Transform[] cameraPoints;
    
        // =====================================================
        // 🔊 SFX
        // =====================================================
        [Header("Move SFX")]
        public AudioSource audioSource;          // assign in inspector
        public AudioClip moveLoopClip;           // looping travel sound
        public bool loopSFX = true;
    
        private bool cameraBusy = false;
        private Coroutine currentMoveRoutine;
    
        void Awake()
        {
            if (cameraTransform == null && Camera.main != null)
                cameraTransform = Camera.main.transform;
        }
    
        void Start()
        {
            // Apply initial pose once
            if (initialPose != null)
            {
                cameraTransform.position = initialPose.position;
                cameraTransform.rotation = initialPose.rotation;
            }
        }
    
        // 🔘 SAFE BUTTON CALL (NO TRANSFORM PARAM)
        public void MoveToPointByIndex(int index)
        {
            if (index < 0 || index >= cameraPoints.Length)
                return;
    
            MoveToPoint(cameraPoints[index]);
        }
    
        // 🔹 INTERNAL MOVE
        public void MoveToPoint(Transform targetPoint)
        {
            if (targetPoint == null)
                return;
    
            // Interrupt previous movement safely
            if (currentMoveRoutine != null)
            {
                StopCoroutine(currentMoveRoutine);
                currentMoveRoutine = null;
                cameraBusy = false;
    
                StopMoveSFX();   // 🔥 stop sound if interrupted
            }
    
            currentMoveRoutine = StartCoroutine(MoveRoutine(targetPoint));
        }
    
        IEnumerator MoveRoutine(Transform target)
        {
            cameraBusy = true;
    
            PlayMoveSFX();   // 🔥 START SOUND
    
            Vector3 startPos = cameraTransform.position;
            Quaternion startRot = cameraTransform.rotation;
    
            float time = 0f;
    
            while (time < moveDuration)
            {
                time += Time.deltaTime;
                float t = movementCurve.Evaluate(time / moveDuration);
    
                cameraTransform.position = Vector3.Lerp(startPos, target.position, t);
                cameraTransform.rotation = Quaternion.Slerp(startRot, target.rotation, t);
    
                // Early exit if close enough
                if (Vector3.Distance(cameraTransform.position, target.position) <= positionThreshold &&
                    Quaternion.Angle(cameraTransform.rotation, target.rotation) <= rotationThreshold)
                {
                    break;
                }
    
                yield return null;
            }
    
            cameraTransform.position = target.position;
            cameraTransform.rotation = target.rotation;
    
            cameraBusy = false;
            currentMoveRoutine = null;
    
            StopMoveSFX();   // 🔥 STOP SOUND
        }
    
        // =====================================================
        // 🔊 SFX CONTROL
        // =====================================================
    
        void PlayMoveSFX()
        {
            if (audioSource == null || moveLoopClip == null)
                return;
    
            audioSource.clip = moveLoopClip;
            audioSource.loop = loopSFX;
            audioSource.Play();
        }
    
        void StopMoveSFX()
        {
            if (audioSource == null)
                return;
    
            if (audioSource.isPlaying)
                audioSource.Stop();
        }
    
        // Optional
        public bool IsCameraBusy()
        {
            return cameraBusy;
        }
    }
    
    
}