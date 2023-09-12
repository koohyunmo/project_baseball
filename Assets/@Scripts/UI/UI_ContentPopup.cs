using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ContentPopup : UI_Popup
{
    protected void OnEnable()
    {
        Managers.Game.MainBatOff();
    }

    protected virtual void OnDestroy()
    {
       Managers.Game.MainBatOn();
    }
}
