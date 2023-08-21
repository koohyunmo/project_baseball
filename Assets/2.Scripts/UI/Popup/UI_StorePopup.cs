using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_StorePopup : UI_PopupHasBButton
{

    enum Buttons
    {
        B_Back,
    }



    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));
        GetButton((int)Buttons.B_Back).gameObject.BindEvent(()=>Managers.UI.ClosePopupUI(this));

        return true;
    }
}
