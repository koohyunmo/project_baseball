using System.Collections.Generic;

[System.Serializable]
public class LocalizationData
{
    public Dictionary<string, LocalizedItem> items;
    public Dictionary<string, LocalizedItem> skills;
    public Dictionary<string, LocalizedItem> bats; // bats ��ųʸ� �߰�
    public Dictionary<string, LocalizedItem> types; // bats ��ųʸ� �߰�
}


[System.Serializable]
public class LocalizedItem
{
    public string en;
    public string ko;
    // ... ��Ÿ ��� �߰�
}

