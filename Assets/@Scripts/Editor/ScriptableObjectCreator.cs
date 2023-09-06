using DG.DemiEditor;
using System.Drawing;
using System.IO;
using UnityEditor;
using UnityEngine;
using static BatchCaptureWindow;
using static Define;

public class ScriptableObjectCreator : EditorWindow
{
#if UNITY_EDITOR
    static string iconPath = "Assets/iconPath/";
    static string modelsPath = "Assets/modelsPath/";
    static string savePath = "Assets/savePath/";

     enum ItemType
    {
        NONE,
        Bat,
        Ball,
    }

    ItemType soType = ItemType.NONE; // �⺻���� Twenty�� ����

    [MenuItem("Tools/SO Creator")]
    public static void ShowWindow()
    {
        GetWindow<ScriptableObjectCreator>("SO Creator");
    }
    void OnGUI()
    {
        GUILayout.Label("SO Creator", EditorStyles.boldLabel);


        // �߰�: ���� ��ο� ���� ��θ� ������ �� �ִ� �ʵ�
        savePath = EditorGUILayout.TextField("[���� ����]Save Path:", savePath);
        modelsPath = EditorGUILayout.TextField("[�� ����]Folder Path:", modelsPath);
        iconPath = EditorGUILayout.TextField("[������ ����]Folder Path:", iconPath);

        GUILayout.Label("[ ������ Ÿ�� ]:", EditorStyles.boldLabel);
        soType = (ItemType)EditorGUILayout.EnumPopup("ItemType", soType);

        if (GUILayout.Button("[��ũ��Ÿ�� ������Ʈ �����]CreateScriptableObjects"))
        {
            CreateScriptableObjects(soType);
        }
    }

    static void CreateScriptableObjects(ItemType itemType = ItemType.NONE)
    {
        // If the user didn't select a folder, don't proceed
        if (string.IsNullOrEmpty(modelsPath) || string.IsNullOrEmpty(iconPath))
            return;

        // ���� ����
        string directoryPath = Path.GetDirectoryName(savePath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // ���� ����
        string[] existingFiles = Directory.GetFiles(directoryPath);
        foreach (string file in existingFiles)
        {
            File.Delete(file);
        }



        // ������ �б�
        string[] modelPaths = Directory.GetFiles(modelsPath, "*.prefab");
        int count = 1;


        // ��ũ��������Ʈ�� ������ ����
        foreach (string modelPath in modelPaths)
        {
            string spriteName = Path.GetFileNameWithoutExtension(modelPath) + ".png";
            string spriteFullPath = Path.Combine(iconPath, spriteName);

            // Load the model
            GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
            Sprite icon = AssetDatabase.LoadAssetAtPath<Sprite>(spriteFullPath);
            if (icon == null)
            {
                Debug.LogWarning($"{spriteFullPath} is wrong");
            }

            // Create a new scriptable object


            string firstWord;
            switch (itemType)
            {
                case ItemType.Bat:
                    firstWord = "BAT";
                    CreateFolder(savePath + "/Bat");
                    {
                        BatScriptableObject item = ScriptableObject.CreateInstance<BatScriptableObject>();

                        item.name = Path.GetFileNameWithoutExtension(modelPath).Split(' ')[0];
                        item.id = firstWord + "_" + count++;
                        item.model = model;
                        item.icon = icon;

                        // Save the scriptable object
                        string newSavePath = Path.Combine(savePath + "/" + firstWord, item.id + ".asset");
                        AssetDatabase.CreateAsset(item, newSavePath);
                    }
                    break;
                case ItemType.Ball:
                    firstWord = "BALL";
                    CreateFolder(savePath + "/Ball");
                    {
                        BallScriptableObject item = ScriptableObject.CreateInstance<BallScriptableObject>();

                        item.name = Path.GetFileNameWithoutExtension(modelPath).Split(' ')[0];
                        item.id = firstWord + "_" + count++;
                        item.model = model;
                        item.icon = icon;

                        // Save the scriptable object
                        string newSavePath = Path.Combine(savePath + "/" + firstWord, item.id + ".asset");
                        AssetDatabase.CreateAsset(item, newSavePath);
                    }
                    break;
                default:
                    firstWord = Path.GetFileNameWithoutExtension(modelPath).Split(' ')[0];
                    {
                        ItemScriptableObject item = ScriptableObject.CreateInstance<ItemScriptableObject>();

                        item.name = Path.GetFileNameWithoutExtension(modelPath).Split(' ')[0];
                        item.id = firstWord + "_" + count++;
                        item.model = model;
                        item.icon = icon;

                        // Save the scriptable object
                        string newSavePath = Path.Combine(savePath + "/" + firstWord, item.id + ".asset");
                        AssetDatabase.CreateAsset(item, newSavePath);
                    }
                    break;
            }



        }
        AssetDatabase.SaveAssets();
    }

    private void CreateScriptableObject<T>() where T : ItemScriptableObject
    {


    }
    private static void CreateFolder(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
#endif
}