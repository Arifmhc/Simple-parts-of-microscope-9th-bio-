using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class SpriteReferenceReplacer : EditorWindow
{
    private Sprite existingSprite;
    private Sprite replacementSprite;

    [MenuItem("Tools/UI/Sprite Reference Replacer")]
    static void Init()
    {
        GetWindow<SpriteReferenceReplacer>("Sprite Replacer");
    }

    void OnGUI()
    {
        GUILayout.Space(10);

        EditorGUILayout.LabelField("Replace Sprite References", EditorStyles.boldLabel);

        existingSprite = (Sprite)EditorGUILayout.ObjectField(
            "Existing Sprite",
            existingSprite,
            typeof(Sprite),
            false);

        replacementSprite = (Sprite)EditorGUILayout.ObjectField(
            "Replacement Sprite",
            replacementSprite,
            typeof(Sprite),
            false);

        GUILayout.Space(10);

        if (GUILayout.Button("Replace In Scene"))
        {
            ReplaceSprites();
        }
    }

    void ReplaceSprites()
    {
        if (existingSprite == null || replacementSprite == null)
        {
            Debug.LogError("Assign both sprites.");
            return;
        }

        Image[] images = FindObjectsOfType<Image>(true);

        int replaced = 0;

        foreach (var img in images)
        {
            if (img.sprite == existingSprite)
            {
                Undo.RecordObject(img, "Replace Sprite");
                img.sprite = replacementSprite;
                EditorUtility.SetDirty(img);
                replaced++;
            }
        }

        Debug.Log($"Sprite replaced in {replaced} UI Images.");
    }
}