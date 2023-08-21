using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ChallengePopup : UI_Popup
{

    enum Buttons
    {
        B_Back,
        StartButton,
        Close
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

      BindButton(typeof(Buttons));


      GetButton((int)Buttons.B_Back).gameObject.BindEvent(B_BackClick);

        return true;
    }

    private void B_BackClick()
    {
        Managers.UI.ClosePopupUI(this);
    }


}
