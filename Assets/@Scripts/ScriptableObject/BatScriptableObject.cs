using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BatScriptableObject : ItemScriptableObject
{
    public float power;

    public override void Settings()
    {
        power = ((int)grade+1) * 5.0f;
        type = Define.ItemType.Bat;

    }
}
