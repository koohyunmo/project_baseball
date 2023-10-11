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

    private Action mainTMPUpdate;
    string _settingPath = "";

    public void Init()
    {
        SystemLanguage userLanguage = Application.systemLanguage;
        Debug.Log("User's system language is: " + userLanguage.ToString());
        _settingPath = Application.persistentDataPath + "/SettingData.json";

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

        try
        {
            currentLanguage = ES3.Load<Language>("Lang", _settingPath);
        }catch
        {
            currentLanguage = Language.English;
            ES3.Save<Language>("Lang", currentLanguage,_settingPath);
        }

        //LoadLocalizedText(currentLanguage);
    }

    public void ChangeLocalizedText(Language language)
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

    public void LoadLocalizedText()
    {
        Debug.Log("����� ��� �б�");


        localizedText = new Dictionary<string, string>();
        string filePath = $"LocalizedText_{currentLanguage.ToString()}";
        filePath = "LocalizedText";
        TextAsset targetFile = Managers.Resource.Load<TextAsset>(filePath);

        //Debug.Log(targetFile.text);

        if (targetFile == null)
        {
            Debug.LogError($"key : LocalizedText_{currentLanguage.ToString()}�� ��� ������ ������ �����ϴ�");
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

        ChangeLocalizedText(lang);
        ES3.Save<Language>("Lang", currentLanguage, _settingPath);
        mainTMPUpdate?.Invoke();
    }

    public void SetLocalChangeUpdateTMP(Action mainTMPUpdate)
    {
        this.mainTMPUpdate = mainTMPUpdate;
    }
}
