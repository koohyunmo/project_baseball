using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

public class DeleteSaveFileEditor
{

#if UNITY_EDITOR
    public static string SaveFilePath = Application.persistentDataPath + "/SaveData.json";


    [MenuItem("Tools/Save/Read Save File")]
    public static void ReadSaveFile()
    {
        string filePath = SaveFilePath;

        if (File.Exists(filePath))
        {
            string fileContent = File.ReadAllText(filePath);
            UnityEngine.Debug.Log(fileContent);
        }
        else
        {
            UnityEngine.Debug.LogWarning("Save file not found!");
        }
    }

    [MenuItem("Tools/Save/Delete Save File")]
    public static void DeleteSaveFile()
    {
        string filePath = SaveFilePath;
        

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            UnityEngine.Debug.Log("Save file deleted successfully!");
        }
        else
        {
            UnityEngine.Debug.LogWarning("Save file not found!");
        }
    }


    [MenuItem("Tools/Save/Open Save Folder")]
    public static void OpenSaveFolder()
    {
        string folderPath = Application.persistentDataPath;
        OpenFolder(folderPath);
    }

    public static void OpenFolder(string path)
    {
#if UNITY_EDITOR_WIN
        Process.Start("explorer.exe", path.Replace('/', '\\'));
#elif UNITY_EDITOR_OSX
        Process.Start("open", path);
#else
        Debug.LogError("Folder opening not supported on this platform.");
#endif
    }
#endif
}
