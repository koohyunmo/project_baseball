using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class ClassToJsonEditor : EditorWindow
{
#if UNITY_EDITOR
    GameDB gameDB = new GameDB();
    List<PlayerItem> itemsList = new List<PlayerItem>(); // 임시로 아이템을 저장할 리스트

    string savePath = "Assets/@Resources/Data/";
    string jsonFileName = "SaveJson";

    [MenuItem("Tools/GameDB Editor")]
    public static void ShowWindow()
    {
        GetWindow<ClassToJsonEditor>("GameDB Editor");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Edit GameDB Data", EditorStyles.boldLabel);

        jsonFileName = EditorGUILayout.TextField("[저장 할 이름]jsonFileName:", jsonFileName );

        // 아이템 리스트를 표시하고 편집
        EditorGUILayout.LabelField("Items:", EditorStyles.boldLabel);
        int deleteIndex = -1;
        for (int i = 0; i < itemsList.Count; i++)
        {
            EditorGUILayout.BeginVertical("box");

            itemsList[i].itemId = EditorGUILayout.TextField("Item ID:", itemsList[i].itemId);
            itemsList[i].itemType = (Define.ItemType)EditorGUILayout.EnumPopup("Type:", itemsList[i].itemType);
            itemsList[i].itemName = EditorGUILayout.TextField("Name:", itemsList[i].itemName);
            itemsList[i].description = EditorGUILayout.TextField("Description:", itemsList[i].description);

            if (GUILayout.Button("Delete"))
            {
                deleteIndex = i;
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(); // 공간 추가
        }


        if (deleteIndex != -1)
        {
            itemsList.RemoveAt(deleteIndex);
        }

        if (GUILayout.Button("Add New Item"))
        {
            itemsList.Add(new PlayerItem());
        }

        savePath = EditorGUILayout.TextField("Save Path:", savePath);

        if (GUILayout.Button("Save to JSON"))
        {
            SaveToJson();
        }
    }

    void SaveToJson()
    {

        string directoryPath = Path.GetDirectoryName(savePath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        foreach (var item in itemsList)
        {
            if (!gameDB.playerInventory.Contains(item.itemId))
            {
                gameDB.playerInventory.Add(item.itemId);
            }
        }

        string jsonData = JsonConvert.SerializeObject(gameDB, Formatting.Indented);
        File.WriteAllText(savePath+ $"/{jsonFileName}.json", jsonData);
        AssetDatabase.Refresh();
        Debug.Log("Saved GameDB to JSON!");
    }

#endif
}
