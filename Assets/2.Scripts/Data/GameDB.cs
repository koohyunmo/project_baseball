using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class GameDB 
{
    public Dictionary<string,GameItem> playerItem = new Dictionary<string, GameItem>();
    public PlayerInfo playerInfo = new PlayerInfo();
}

[Serializable]
public class GameItem
{
    public GameItem()
    {

    }

    public GameItem(string id,string name, string desc,Define.ItemType itemType = Define.ItemType.Bat)
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
}



