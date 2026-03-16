using UnityEngine;
using UnityEditor;

public class GameObjectReferenceReplacer : EditorWindow
{
    private GameObject existingObject;
    private GameObject replacementObject;

    [MenuItem("Tools/Scene/GameObject Reference Replacer")]
    static void Init()
    {
        GetWindow<GameObjectReferenceReplacer>("GO Replacer");
    }

    void OnGUI()
    {
        GUILayout.Space(10);

        EditorGUILayout.LabelField("Replace GameObjects In Scene", EditorStyles.boldLabel);

        existingObject = (GameObject)EditorGUILayout.ObjectField(
            "Existing GameObject",
            existingObject,
            typeof(GameObject),
            true);

        replacementObject = (GameObject)EditorGUILayout.ObjectField(
            "Replacement GameObject",
            replacementObject,
            typeof(GameObject),
            true);

        GUILayout.Space(10);

        if (GUILayout.Button("Replace In Scene"))
        {
            ReplaceObjects();
        }
    }

    void ReplaceObjects()
    {
        if (existingObject == null || replacementObject == null)
        {
            Debug.LogError("Assign both objects.");
            return;
        }

        GameObject[] allObjects = FindObjectsOfType<GameObject>(true);

        int replaced = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name == existingObject.name)
            {
                Transform parent = obj.transform.parent;

                Vector3 pos = obj.transform.position;
                Quaternion rot = obj.transform.rotation;
                Vector3 scale = obj.transform.localScale;

                Undo.DestroyObjectImmediate(obj);

                GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(replacementObject);

                newObj.transform.SetParent(parent);
                newObj.transform.position = pos;
                newObj.transform.rotation = rot;
                newObj.transform.localScale = scale;

                Undo.RegisterCreatedObjectUndo(newObj, "Replace GameObject");

                replaced++;
            }
        }

        Debug.Log($"Replaced {replaced} GameObjects in scene.");
    }
}