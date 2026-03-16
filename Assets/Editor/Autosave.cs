#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class AutoSceneSaveEditor
{
    private const string EnabledKey = "AutoSave_Enabled";
    private const string IntervalKey = "AutoSave_Interval";

    private static double nextSaveTime;

    static AutoSceneSaveEditor()
    {
        if (!EditorPrefs.HasKey(EnabledKey))
            EditorPrefs.SetBool(EnabledKey, true);

        if (!EditorPrefs.HasKey(IntervalKey))
            EditorPrefs.SetFloat(IntervalKey, 300f); // default 5 min

        ScheduleNextSave();
        EditorApplication.update += Update;
    }

    // =====================================================
    private static void Update()
    {
        if (!IsEnabled())
            return;

        if (EditorApplication.isPlaying)
            return;

        if (EditorApplication.timeSinceStartup >= nextSaveTime)
        {
            SaveScene();
            ScheduleNextSave();
        }
    }

    // =====================================================
    private static void SaveScene()
    {
        if (!EditorSceneManager.GetActiveScene().isDirty)
            return;

        EditorSceneManager.SaveOpenScenes();

        // 🔔 Show notification popup
        ShowNotification("Scene Auto-Saved");
        Debug.Log("[AutoSave] Scene saved automatically.");
    }

    private static void ScheduleNextSave()
    {
        float interval = EditorPrefs.GetFloat(IntervalKey, 300f);
        nextSaveTime = EditorApplication.timeSinceStartup + interval;
    }

    private static bool IsEnabled()
    {
        return EditorPrefs.GetBool(EnabledKey, true);
    }

    // =====================================================
    // MENU
    // =====================================================

    [MenuItem("Tools/Auto Save/Enable Auto Save")]
    private static void ToggleAutoSave()
    {
        bool current = IsEnabled();
        EditorPrefs.SetBool(EnabledKey, !current);

        ShowNotification(!current ? "Auto Save Enabled" : "Auto Save Disabled");
    }

    [MenuItem("Tools/Auto Save/Enable Auto Save", true)]
    private static bool ToggleAutoSaveValidate()
    {
        Menu.SetChecked("Tools/Auto Save/Enable Auto Save", IsEnabled());
        return true;
    }

    [MenuItem("Tools/Auto Save/Set Interval (Minutes)")]
    private static void SetInterval()
    {
        float currentMinutes = EditorPrefs.GetFloat(IntervalKey, 300f) / 60f;
        string input = EditorUtility.DisplayDialogComplex(
            "Auto Save Interval",
            $"Current Interval: {currentMinutes} minutes\n\nSet to:\n1 min\n5 min\n10 min",
            "1 Min",
            "5 Min",
            "10 Min"
        ).ToString();

        float newInterval = 300f;

        switch (input)
        {
            case "0": newInterval = 60f; break;
            case "1": newInterval = 300f; break;
            case "2": newInterval = 600f; break;
        }

        EditorPrefs.SetFloat(IntervalKey, newInterval);
        ScheduleNextSave();
        ShowNotification("Auto Save Interval Updated");
    }

    // =====================================================
    private static void ShowNotification(string message)
    {
        EditorWindow window = EditorWindow.focusedWindow;
        if (window != null)
        {
            window.ShowNotification(new GUIContent(message));
        }
    }
}

#endif
