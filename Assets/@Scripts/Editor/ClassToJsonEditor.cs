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
    List<PlayerItem> itemsList = new List<PlayerItem>(); // �ӽ÷� �������� ������ ����Ʈ

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

        jsonFileName = EditorGUILayout.TextField("[���� �� �̸�]jsonFileName:", jsonFileName );

        // ������ ����Ʈ�� ǥ���ϰ� ����
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
            EditorGUILayout.Space(); // ���� �߰�
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
