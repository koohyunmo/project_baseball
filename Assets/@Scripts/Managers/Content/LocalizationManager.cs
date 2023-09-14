using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum Language
{
    English,
    Korean,
    Spanish,
    // ... 기타 언어 추가
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

        // 기본 언어 설정
        switch (userLanguage)
        {
            case SystemLanguage.Korean:
                //currentLanguage = Language.Korean;
                break;
            case SystemLanguage.English:
                currentLanguage = Language.English;
                break;
                // ... 기타 언어에 대한 처리 추가
        }

        //LoadLocalizedText(currentLanguage);
    }

    public void LoadLocalizedText(Language language = Language.English)
    {
        Debug.Log("언어 변경");

        localizedText = new Dictionary<string, string>();
        string filePath = $"LocalizedText_{language.ToString()}";
        filePath = "LocalizedText";
        TextAsset targetFile = Managers.Resource.Load<TextAsset>(filePath);

        //Debug.Log(targetFile.text);

        if (targetFile == null)
        {
            Debug.LogError($"key : LocalizedText_{language.ToString()}의 언어 파일을 읽을수 없습니다");
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

        // bats 딕셔너리도 localizedText에 합치기
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
            // ... 기타 언어에 대한 처리 추가
            default:
                return item.en; // 기본값으로 영어 반환
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
