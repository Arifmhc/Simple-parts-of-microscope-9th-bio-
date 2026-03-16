using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

[DisallowMultipleComponent]
[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(AudioSource))]
public class OverlayBlinker : MonoBehaviour
{
    [Header("Renderer")]
    [SerializeField] private Renderer targetRenderer;

    [Header("Overlay Material")]
    [SerializeField] private Material overlayMaterial;

    [Header("Blink Settings")]
    [SerializeField] private float blinkInterval = 0.4f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip blinkLoopClip;

    [Header("Test")]
    [SerializeField] private bool testBlinking = false;

    [Header("Debug")]
    [SerializeField] private bool enableDebug = true;

    private Coroutine blinkRoutine;
    private bool isBlinking;

    private List<Material> baseMaterials = new List<Material>();

    // =====================================================
    void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        CacheBaseMaterials();
    }

    // =====================================================
    void CacheBaseMaterials()
    {
        baseMaterials.Clear();
        baseMaterials.AddRange(targetRenderer.materials);
    }

    // =====================================================
    public void StartBlinking()
    {
        LogCaller("StartBlinking");

        if (isBlinking || overlayMaterial == null)
            return;

        blinkRoutine = StartCoroutine(BlinkRoutine());
        isBlinking = true;

        StartAudio();

        Log("Blinking started");
    }

    // =====================================================
    public void StopBlinking()
    {
        LogCaller("StopBlinking");

        if (!isBlinking)
            return;

        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);

        RemoveOverlay();
        StopAudio();

        blinkRoutine = null;
        isBlinking = false;

        Log("Blinking stopped");
    }

    // =====================================================
    IEnumerator BlinkRoutine()
    {
        bool overlayActive = false;

        while (true)
        {
            overlayActive = !overlayActive;

            if (overlayActive)
                AddOverlay();
            else
                RemoveOverlay();

            yield return new WaitForSeconds(blinkInterval);
        }
    }

    // =====================================================
    void AddOverlay()
    {
        Material[] mats = targetRenderer.materials;

        foreach (var mat in mats)
            if (mat == overlayMaterial)
                return;

        List<Material> list = new List<Material>(mats);
        list.Add(overlayMaterial);

        targetRenderer.materials = list.ToArray();
    }

    // =====================================================
    void RemoveOverlay()
    {
        targetRenderer.materials = baseMaterials.ToArray();
    }

    // =====================================================
    void StartAudio()
    {
        if (audioSource == null || blinkLoopClip == null)
            return;

        audioSource.clip = blinkLoopClip;
        audioSource.loop = true;
        audioSource.Play();
    }

    // =====================================================
    void StopAudio()
    {
        if (audioSource == null)
            return;

        audioSource.Stop();
    }

    // =====================================================
    void Log(string msg)
    {
        if (enableDebug)
            UnityEngine.Debug.Log($"[OverlayBlinker] {msg}", this);
    }

    // =====================================================
    void LogCaller(string functionName)
    {
        if (!enableDebug) return;

        StackTrace stack = new StackTrace();
        StackFrame frame = stack.GetFrame(2);

        string caller = frame.GetMethod().Name;

        UnityEngine.Debug.Log(
            $"[OverlayBlinker] {functionName} called by → {caller}",
            this
        );
    }
}