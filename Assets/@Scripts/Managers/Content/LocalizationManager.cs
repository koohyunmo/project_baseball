using Newtonsoft.Json;
using System;
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
                //currentLanguage = Language.Korean;
                break;
            case SystemLanguage.English:
                currentLanguage = Language.English;
                break;
                // ... ��Ÿ �� ���� ó�� �߰�
        }

        //LoadLocalizedText(currentLanguage);
    }

    public void LoadLocalizedText(Language language = Language.English)
    {
        Debug.Log("��� ����");

        localizedText = new Dictionary<string, string>();
        string filePath = $"LocalizedText_{language.ToString()}";
        filePath = "LocalizedText";
        TextAsset targetFile = Managers.Resource.Load<TextAsset>(filePath);

        //Debug.Log(targetFile.text);

        if (targetFile == null)
        {
            Debug.LogError($"key : LocalizedText_{language.ToString()}�� ��� ������ ������ �����ϴ�");
            return;
        }

        LocalizationData localizationData = JsonConvert.DeserializeObject<LocalizationData>(targetFile.text);
        foreach (var item in localizationData.items)
        {
            localizedText[item.Key] = GetTranslation(item.Value);
        }

        foreach (var skill in localizationData.skills)
        {
            localizedText[skill.Key] = GetTranslation(skill.Value);
        }

        // bats ��ųʸ��� localizedText�� ��ġ��
        foreach (var bat in localizationData.bats)
        {
            localizedText[bat.Key] = GetTranslation(bat.Value);
        }

        foreach (var bat in localizationData.types)
        {
            localizedText[bat.Key] = GetTranslation(bat.Value);
        }
    }

    private string GetTranslation(LocalizedItem item)
    {
        switch (currentLanguage)
        {
            case Language.English:
                return item.en;
            case Language.Korean:
                return item.ko;
            // ... ��Ÿ �� ���� ó�� �߰�
            default:
                return item.en; // �⺻������ ���� ��ȯ
        }
    }


    public string GetLocalizedValue(string key)
    {
        key = key.ToLower();
        string result = localizedText.ContainsKey(key) ? localizedText[key] : $"{key} Find Failed";
        return result;
    }

    public void ChangeLanguage(Language lang)
    {
        if (currentLanguage == lang)
            return;
        currentLanguage = lang;
        //localizedText.Clear();

        LoadLocalizedText(lang);
        Debug.Log(lang);
    }
}
