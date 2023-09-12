using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public enum Language
{
    English,
    Korean,
    Spanish,
    // ... ��Ÿ ��� �߰�
}

public class LocalizationManager
{
    public static LocalizationManager Instance;

    private Dictionary<string, string> localizedText;
    public Language currentLanguage = Language.English;

    public void Init()
    {
        SystemLanguage userLanguage = Application.systemLanguage;
        Debug.Log("User's system language is: " + userLanguage.ToString());

        // �⺻ ��� ����
        switch (userLanguage)
        {
            case SystemLanguage.Korean:
                currentLanguage = Language.Korean;
                break;
            case SystemLanguage.English:
                currentLanguage = Language.English;
                break;
                // ... ��Ÿ �� ���� ó�� �߰�
        }

        //LoadLocalizedText(currentLanguage);
    }

    public void LoadLocalizedText(Language language)
    {
        localizedText = new Dictionary<string, string>();
        string filePath = $"LocalizedText_{language.ToString()}"; // ��: "LocalizedText_English"
        //TextAsset targetFile = Resources.Load<TextAsset>(filePath);
        TextAsset targetFile = Managers.Resource.Load<TextAsset>(filePath);

        Debug.Log(targetFile);

        if(targetFile == null)
        {
            Debug.LogError($"key : LocalizedText_{language.ToString()}�� ��� ������ ������ �����ϴ�");
            return;
            
        }

        LocalizationData localizationData = JsonUtility.FromJson<LocalizationData>(targetFile.text);
        localizedText = localizationData.items;
    }
    public void LoadLocalizedText()
    {
        localizedText = new Dictionary<string, string>();
        string filePath = $"LocalizedText_{currentLanguage.ToString()}";
        TextAsset targetFile = Managers.Resource.Load<TextAsset>(filePath);

        if (targetFile == null)
        {
            Debug.LogError($"key : LocalizedText_{currentLanguage.ToString()}�� ��� ������ ������ �����ϴ�");
            return;
        }

        LocalizationData localizationData = JsonConvert.DeserializeObject<LocalizationData>(targetFile.text);
        localizedText = localizationData.items;

        // skills ��ųʸ��� localizedText�� ��ġ��
        foreach (var skill in localizationData.skills)
        {
            localizedText[skill.Key] = skill.Value;
        }

        if (localizedText != null)
        {
            foreach (var item in localizedText.Keys)
            {
                Debug.Log($"{item} : {localizedText[item]}");
            }
        }
        else
        {
            Debug.LogError("localizedText is null!");
        }
    }


    public string GetLocalizedValue(string key)
    {
        string result = localizedText.ContainsKey(key) ? localizedText[key] : key;
        return result;
    }
}
