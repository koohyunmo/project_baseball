using Codice.Client.BaseCommands.BranchExplorer;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using static Define;

public class ScriptableObjectCreator : EditorWindow
{
#if UNITY_EDITOR

    readonly static string iconPathRead = "Assets/@SOData/@SOIcons/";
    readonly static string modelsPathRead = "Assets/@SOData/@SOModels/";
    readonly static string savePathRead = "Assets/@SOData/@SOSavePath";
    readonly static string csvFilePathRead = "Assets/@SOData/@SOData/";

    static string iconPath = "Assets/";
    static string modelsPath = "Assets/";
    static string savePath = "Assets/";
    static string csvFilePath = "Assets/";

    enum ItemType
    {
        NONE,
        Item,
        Bat,
        Ball,
        Skill,
    }


    static ItemType soType = ItemType.NONE; // 기본값을 Twenty로 설정

    [MenuItem("Tools/SO Creator")]
    public static void ShowWindow()
    {
        GetWindow<ScriptableObjectCreator>("SO Creator");
    }
    void OnGUI()
    {
        GUILayout.Label("SO Creator", EditorStyles.boldLabel);

        // CSV 파일을 선택할 수 있는 필드 추가
        csvFilePath = EditorGUILayout.TextField("[CSV 파일]CSV File Path:", csvFilePath);
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

        if (GUILayout.Button("LOAD 기본설정"))
        {
            LoadData();
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

        if (itemType == ItemType.NONE)
        {
            throw new Exception($"아이템 타입을 설정해주세요");
        }

        // 폴더 비우기
        string[] existingFiles = Directory.GetFiles(directoryPath);
        foreach (string file in existingFiles)
        {
            File.Delete(file);
        }


        CSVBaseCreate(soType);

        // 스크립오브젝트로 조립후 저장
       
        AssetDatabase.SaveAssets();


    }

    private static void ModelBaseCreate(ItemType itemType = ItemType.NONE)
    {
        // 모델파일 읽기
        string[] modelPaths = Directory.GetFiles(modelsPath, "*.prefab");
        string[] data = LoadCSVFileSplit().ToArray();
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

            int index_1 = 0 + (count - 1) * 3;
            int index_2 = count + (count - 1) * 2;
            int index_3 = (count + 1) + (count - 1) * 2;

            string firstWord;
            switch (itemType)
            {
                case ItemType.Bat:
                    firstWord = "BAT";
                    savePath = savePathRead;



                    CreateFolder(savePath + "/Bat");
                    {
                        BatScriptableObject item = ScriptableObject.CreateInstance<BatScriptableObject>();

                        item.name = Path.GetFileNameWithoutExtension(modelPath).Split(' ')[0];

                        Debug.Log(data[index_1] + " : " + data[index_2] + " : " + data[index_3]);

                        item.grade = ParseGrade(data[index_2]);
                        item.batType = ParseBatType(data[index_3]);

                        item.id = firstWord + "_" + count++;
                        item.model = model;
                        item.icon = icon;

                        // Save the scriptable object
                        item.Settings();
                        string newSavePath = Path.Combine(savePath + "/" + firstWord, item.id + ".asset");
                        Debug.Log(newSavePath);
                        AssetDatabase.CreateAsset(item, newSavePath);

                    }
                    break;
                case ItemType.Ball:
                    firstWord = "BALL";
                    savePath = savePathRead;
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
            }


        }
        AssetDatabase.SaveAssets();
    }

    private static List<string> LoadCSVFileSplit()
    {
        csvFilePath = Path.Combine(csvFilePathRead, soType.ToString() + "CSV.csv");


        if (string.IsNullOrEmpty(csvFilePath))
        {
            Debug.LogError("CSV 파일 경로를 입력하세요.");
            return null;
        }

        if (!File.Exists(csvFilePath))
        {
            Debug.LogError("지정한 경로에 CSV 파일이 존재하지 않습니다.");
            return null;
        }

        // CSV 파일 읽기 로직을 추가하십시오.
        string[] csvLines = File.ReadAllLines(csvFilePath);

        // 읽어온 CSV 데이터를 처리하는 로직을 여기에 추가하세요.
        List<string> data = new List<string>();

        // 첫 번째 라인은 헤더이므로 건너뜁니다.
        bool isFirstLine = true;

        foreach (string line in csvLines)
        {
            if (isFirstLine)
            {
                isFirstLine = false;
                continue;
            }

            string[] lineData = line.Split(',');
            data.AddRange(lineData);
        }

        for (int i = 0; i < data.Count; i++)
        {
            data[i] = data[i].ToLower();
        }

        return data;
    }

    private static  void CSVBaseCreate(ItemType itemType = ItemType.NONE)
    {
        // CSV 파일 읽기 로직을 추가하십시오.
        string[] csvLines = File.ReadAllLines(csvFilePath);
        Debug.Log("csvLines Count : " + csvLines.Length);
        //string[] modelPaths = Directory.GetFiles(modelsPath, "*.prefab");
        int count = 0;

        bool isFirstLine = false;

        foreach (var line in csvLines)
        {
            if(isFirstLine == false)
            {
                Debug.Log(line);
                isFirstLine = true;
                continue;
            }

            string[] lineData = line.Split(',');

            Debug.Log(count + " : "+ lineData[0] + " : " + lineData[1] + " : " + lineData[2]);

            string spriteName = Path.GetFileNameWithoutExtension(lineData[0].ToLower())  + ".png";
            string modelName = Path.GetFileNameWithoutExtension(lineData[0].ToLower())  + ".prefab";

            string spriteFullPath = Path.Combine(iconPath, spriteName);
            string modelFullPath = Path.Combine(modelsPath, modelName);

            // Load the model
            GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(modelFullPath);
            if(model == null)
            {
                throw new ArgumentException($"{modelFullPath} is null");
            }
            Sprite icon = AssetDatabase.LoadAssetAtPath<Sprite>(spriteFullPath);
            if (icon == null)
            {
                throw new ArgumentException($"{spriteFullPath} is null");
            }

            if (icon == null)
            {
                Debug.LogWarning($"{spriteFullPath} is wrong");
                //continue;
            }

            string firstWord;
            switch (itemType)
            {
                case ItemType.Bat:
                    firstWord = "BAT";
                    savePath = savePathRead;
                    CreateFolder(savePath + "/Bat");
                    {
                        BatScriptableObject item = ScriptableObject.CreateInstance<BatScriptableObject>();

                        item.name = Path.GetFileNameWithoutExtension(lineData[0]).Split(' ')[0];
                        item.name = item.name.ToLower();
                        item.grade = ParseGrade(lineData[1]);
                        item.batType = ParseBatType(lineData[2]);

                        item.id = firstWord + "_" + count++;

                        if(model == null)
                        {
                            throw new Exception($"{modelFullPath} : {modelName} : {item.name} ");
                        }
                        item.model = model;
                        item.icon = icon;

                        // Save the scriptable object
                        item.Settings();
                        string newSavePath = Path.Combine(savePath + "/" + firstWord, item.id + ".asset");
                        Debug.Log(newSavePath);
                        AssetDatabase.CreateAsset(item, newSavePath);

                    }
                    break;
                case ItemType.Ball:
                    firstWord = "BALL";
                    savePath = savePathRead;
                    CreateFolder(savePath + "/Ball");
                    {
                        BallScriptableObject item = ScriptableObject.CreateInstance<BallScriptableObject>();
                        item.name = Path.GetFileNameWithoutExtension(lineData[0]).Split(' ')[0];
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
    private bool LoadData()
    {
        csvFilePath = Path.Combine(csvFilePathRead, soType.ToString() + "CSV.csv");
        iconPath = Path.Combine(iconPathRead, soType.ToString() + "Icon/");
        modelsPath = Path.Combine(modelsPathRead, soType.ToString() + "Model/");
        savePath = savePathRead;


        if (string.IsNullOrEmpty(csvFilePath))
        {
            Debug.LogError("CSV 파일 경로를 입력하세요.");
            return false;
        }

        if (!File.Exists(csvFilePath))
        {
            Debug.LogError("지정한 경로에 CSV 파일이 존재하지 않습니다.");
            return false;
        }

        // CSV 파일 읽기 로직을 추가하십시오.
        string[] csvLines = File.ReadAllLines(csvFilePath);

        if (csvLines.Length < 0)
            return false;

        Debug.Log("csv file data lines : " + csvLines.Length);
        return true;
    }

    private static void CreateFolder(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }


    }

    public static Define.Grade ParseGrade(string gradeText)
    {
        gradeText = gradeText.ToLower();

        switch (gradeText)
        {
            case "common":
                return Grade.Common;
            case "uncommon":
                return Grade.Uncommon;
            case "rare":
                return Grade.Rare;
            case "epic":
                return Grade.Epic;
            case "legendary":
                return Grade.Legendary;

        }

        throw new ArgumentException("유효하지 않은 등급입니다: " + gradeText);
    }

    public static Define.BatType ParseBatType(string type)
    {
        type = type .ToLower();

        switch (type)
        {
            case "tree":
                return BatType.Tree;
            case "alu":
                return BatType.Alu;
            case "sp1":
                return BatType.Sp1;
            case "sp2":
                return BatType.Sp2;
            case "sp3":
                return BatType.Sp3;
            case "sp4":
                return BatType.Sp4;

        }

        throw new ArgumentException("유효하지 않은 타입입니다: " + type);
    }
#endif
}