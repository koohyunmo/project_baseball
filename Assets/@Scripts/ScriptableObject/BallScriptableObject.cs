using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScriptableObject : ItemScriptableObject
{

    public override void Settings()
    {
        type = Define.ItemType.Ball;
    }
}
