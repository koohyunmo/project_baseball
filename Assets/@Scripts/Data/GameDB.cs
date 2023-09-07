using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class GameDB 
{
    //public Dictionary<string,PlayerItem> playerItem = new Dictionary<string, PlayerItem>();
    public HashSet<string> playerInventory = new HashSet<string>();
    public PlayerInfo playerInfo = new PlayerInfo();
    public Dictionary<string, bool> challengeData = new Dictionary<string, bool>();
}

[Serializable]
public class PlayerItem
{
    public PlayerItem()
    {

    }

    public PlayerItem(string id,string name, string desc,Define.ItemType itemType = Define.ItemType.BAT)
    {
        itemId = id;
        this.itemType = itemType;
        itemName = name;
        description = desc;
       
    }

    public string itemId; // 고유한 아이템 ID
    public Define.ItemType itemType; // "Ball" or "Bat"
    public string itemName;
    public string description;
}

[Serializable]
public class PlayerInfo
{
    public string playerId;
    public long money;
    public int level;
    public int exp;
    public string equipBatId;
    public string equipBallId;
    public string equipBackgroundID;
    public int playerPower;
    public string equipSkillId;
}



