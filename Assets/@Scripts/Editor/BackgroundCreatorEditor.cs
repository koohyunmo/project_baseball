using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(BackgroundCreator))]
public class BackgroundCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BackgroundCreator creator = (BackgroundCreator)target;

        if (GUILayout.Button("Create Background"))
        {
            creator.CreateBackground();
        }

        if (GUILayout.Button("Save Background"))
        {
            creator.SaveBackground();
        }

        if (GUILayout.Button("Clear Background"))
        {
            creator.ClearBackground();
        }
    }
}
