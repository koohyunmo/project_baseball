using Newtonsoft.Json;
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
                currentLanguage = Language.Korean;
                break;
            case SystemLanguage.English:
                currentLanguage = Language.English;
                break;
                // ... 기타 언어에 대한 처리 추가
        }

        //LoadLocalizedText(currentLanguage);
    }

    public void LoadLocalizedText(Language language)
    {
        localizedText = new Dictionary<string, string>();
        string filePath = $"LocalizedText_{language.ToString()}"; // 예: "LocalizedText_English"
        //TextAsset targetFile = Resources.Load<TextAsset>(filePath);
        TextAsset targetFile = Managers.Resource.Load<TextAsset>(filePath);

        Debug.Log(targetFile);

        if(targetFile == null)
        {
            Debug.LogError($"key : LocalizedText_{language.ToString()}의 언어 파일을 읽을수 없습니다");
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
            Debug.LogError($"key : LocalizedText_{currentLanguage.ToString()}의 언어 파일을 읽을수 없습니다");
            return;
        }

        LocalizationData localizationData = JsonConvert.DeserializeObject<LocalizationData>(targetFile.text);
        localizedText = localizationData.items;

        // skills 딕셔너리도 localizedText에 합치기
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
