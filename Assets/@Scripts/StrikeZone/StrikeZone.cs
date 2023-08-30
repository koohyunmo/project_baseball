using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class StrikeZone : MonoBehaviour
{
    void Start()
    {
        Managers.Game.SetStrikeZone(this);
    }


}
