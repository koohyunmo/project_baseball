using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(ItemScriptableObject), true)]
[CanEditMultipleObjects] // 여러 오브젝트 편집 지원
public class ItemScriptableObejctEditor : Editor
{
#if UNITY_EDITOR

    float power = 0f;
    public override void OnInspectorGUI()
    {
        ItemScriptableObject item = (ItemScriptableObject)target;

        // Draw default inspector
        DrawDefaultInspector();

        // Add a button to generate the description
        if (GUILayout.Button("ItemSetting"))
        {
            item.Settings();
            EditorUtility.SetDirty(item); // Mark the object as changed
            AssetDatabase.SaveAssets();
        }

        // Display the icon if it exists
        if (item.icon != null)
        {
            GUILayout.Label("Icon Preview:");
            GUILayout.Label(item.icon.texture);
        }

        if (GUILayout.Button("Change FileName AND Key"))
        {
            item.Settings();
            string newName = $"{item.type}_";
            item.id = newName;
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(item), newName);
            AssetDatabase.SaveAssets();
        }

    }
#endif
}
