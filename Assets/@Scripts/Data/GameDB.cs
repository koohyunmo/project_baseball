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
    public HashSet<string> playerBalls = new HashSet<string>();
    public HashSet<string> playerBats = new HashSet<string>();
    public HashSet<string> playerSkills = new HashSet<string>();
    public PlayerInfo playerInfo = new PlayerInfo();
    public Dictionary<string, bool> challengeData = new Dictionary<string, bool>();
    public int challengeClearCount = 0;
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
    public long gold;
    public long star;
    public int level;
    public int exp;
    public string equipBatId;
    public string equipBallId;
    public string equipBackgroundID;
    public int playerPower;
    public string equipSkillId;
    public Dictionary<Define.League, long> playerBestScore = new Dictionary<Define.League, long>();
    public Dictionary<Define.League, bool> playerClearLeague = new Dictionary<Define.League, bool>();
}



