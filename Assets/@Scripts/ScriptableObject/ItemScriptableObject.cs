using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

[CreateAssetMenu(fileName = "ItemScriptableObject", menuName = "SO/ItemSO")]
public class ItemScriptableObject : ScriptableObject
{

    public string id;
    public GameObject model;
    public Sprite icon;
    public string name;
    public ItemType type = ItemType.None;
    public Grade grade = Grade.Common;

    public virtual void Settings()
    {

    }

    public virtual List<string> GetData() { return new List<string>(); }

}
