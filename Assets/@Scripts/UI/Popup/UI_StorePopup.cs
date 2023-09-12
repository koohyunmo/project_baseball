using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_StorePopup : UI_PopupHasBButton
{

    enum Buttons
    {
        B_Back,
        Button_Ad
    }

    enum TMPs
    {
        TimeTMP
    }

    enum GameObjects
    {
        RouletteController
    }

    RouletteController rc;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<TextMeshProUGUI>(typeof(TMPs));
        BindObject((typeof(GameObjects)));
        BindButton(typeof(Buttons));
        GetButton((int)Buttons.B_Back).gameObject.BindEvent(()=>Managers.UI.ClosePopupUI(this));
        

        rc = GetObject((int)GameObjects.RouletteController).GetComponent<RouletteController>();
        GetButton((int)Buttons.Button_Ad).gameObject.BindEvent(rc.StartSpin);
        Get<TextMeshProUGUI>((int)TMPs.TimeTMP).text = System.DateTime.Now.ToString();

        return true;
    }
}
