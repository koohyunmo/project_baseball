using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ChallengeScriptableObject))]
public class ChallengeScriptableObjectEditor : Editor
{
#if UNITY_EDITOR
    public override void OnInspectorGUI()
    {
        ChallengeScriptableObject challenge = (ChallengeScriptableObject)target;

        // Draw default inspector
        DrawDefaultInspector();

        // Add a button to generate the description
        if (GUILayout.Button("Generate Description"))
        {
            challenge.GenerateDescription();
            EditorUtility.SetDirty(challenge); // Mark the object as changed
        }

        // Add a button to change the name of the ScriptableObject
        if (GUILayout.Button("Change Name"))
        {
            string newName = $"CSO_{challenge.id}";
            challenge.name = newName;
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(challenge), newName);
            AssetDatabase.SaveAssets();
        }
    }
#endif
}
