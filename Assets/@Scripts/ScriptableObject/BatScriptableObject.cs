using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BatScriptableObject : ItemScriptableObject
{
    public float power;
    public Define.BatType batType = Define.BatType.NONE;




#if UNITY_EDITOR

    private Dictionary<string, Define.BatType> batTypeDict = new Dictionary<string, Define.BatType>();

    public override void Settings()
    {
        power = ((int)grade + 1) * 5.0f;
        type = Define.ItemType.BAT;

        InitializeBatTypeDict(TreeType, Define.BatType.Tree);
        InitializeBatTypeDict(AluType, Define.BatType.Alu);
        InitializeBatTypeDict(Sp1Type, Define.BatType.Sp1);
        InitializeBatTypeDict(Sp2Type, Define.BatType.Sp2);
        InitializeBatTypeDict(Sp3Type, Define.BatType.Sp3);
        InitializeBatTypeDict(Sp4Type, Define.BatType.Sp4);

        if (batTypeDict.ContainsKey(name))
            batType = batTypeDict[name];
    }

    private void InitializeBatTypeDict(string[] names, Define.BatType type)
    {
        foreach (string batName in names)
        {
            batTypeDict[batName.ToLower()] = type;
        }
    }

    private string[] TreeType =
    {
        "baseball_Bat_regular",
        "baseball_Bat_nails",
        "baseball_Bat",
        "black_Bat",
        "black_stripe",
        "blue_Bat",
        "blue_stripe",
        "decal01",
        "decal02",
        "decal03",
        "decal04",
        "industrial_Bat",
        "industrial_Batworn",
        "orange_Bat",
        "orange_stripe",
        "purple_Bat",
        "purple_stripe",
        "red_Bat",
        "red_stripe",
        "white_stripe",
        "yellow_Bat",
        "yellow_stripe",
        "white_bat",
    };

    private string[] AluType =
    {
        "decal05",
        "decal06",
        "decal07",
        "decal08",
        "decal09",
        "decal10",
        "decal11"
    };

    private string[] Sp1Type =
    {
        "flaming_Bat"
    };

    private string[] Sp2Type =
    {
        "future_blue",
        "future_full_blue",
        "future_full_purple",
        "future_full_red",
        "future_full_white",
        "future_purple",
        "future_red",
        "future_white"
    };

    private string[] Sp3Type =
    {
        "Hammer_03"
    };

    private string[] Sp4Type =
    {
        "purple_flaming_Bat"
    };
#endif
}
