using System.Collections.Generic;

[System.Serializable]
public class LocalizationData
{
    public Dictionary<string, LocalizedItem> items;
    public Dictionary<string, LocalizedItem> skills;
    public Dictionary<string, LocalizedItem> bats; // bats µñ¼Å³Ê¸® Ãß°¡
    public Dictionary<string, LocalizedItem> types; // bats µñ¼Å³Ê¸® Ãß°¡
}


[System.Serializable]
public class LocalizedItem
{
    public string en;
    public string ko;
    // ... ±âÅ¸ ¾ð¾î Ãß°¡
}

