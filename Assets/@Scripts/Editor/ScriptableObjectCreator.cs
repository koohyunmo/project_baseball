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

    ItemType soType = ItemType.NONE; // 기본값을 Twenty로 설정

    [MenuItem("Tools/SO Creator")]
    public static void ShowWindow()
    {
        GetWindow<ScriptableObjectCreator>("SO Creator");
    }
    void OnGUI()
    {
        GUILayout.Label("SO Creator", EditorStyles.boldLabel);


        // 추가: 저장 경로와 폴더 경로를 수정할 수 있는 필드
        savePath = EditorGUILayout.TextField("[저장 폴더]Save Path:", savePath);
        modelsPath = EditorGUILayout.TextField("[모델 폴더]Folder Path:", modelsPath);
        iconPath = EditorGUILayout.TextField("[아이콘 폴더]Folder Path:", iconPath);

        GUILayout.Label("[ 아이템 타입 ]:", EditorStyles.boldLabel);
        soType = (ItemType)EditorGUILayout.EnumPopup("ItemType", soType);

        if (GUILayout.Button("[스크립타블 오브젝트 만들기]CreateScriptableObjects"))
        {
            CreateScriptableObjects(soType);
        }
    }

    static void CreateScriptableObjects(ItemType itemType = ItemType.NONE)
    {
        // If the user didn't select a folder, don't proceed
        if (string.IsNullOrEmpty(modelsPath) || string.IsNullOrEmpty(iconPath))
            return;

        // 폴더 생성
        string directoryPath = Path.GetDirectoryName(savePath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // 폴더 비우기
        string[] existingFiles = Directory.GetFiles(directoryPath);
        foreach (string file in existingFiles)
        {
            File.Delete(file);
        }



        // 모델파일 읽기
        string[] modelPaths = Directory.GetFiles(modelsPath, "*.prefab");
        int count = 1;


        // 스크립오브젝트로 조립후 저장
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