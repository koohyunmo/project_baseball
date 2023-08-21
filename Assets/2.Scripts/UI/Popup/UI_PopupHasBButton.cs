using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PopupHasBButton : UI_Popup
{

    protected Button B_Back { get; private set; }

    protected enum B_Button
    {
        B_Back,
    }

    protected virtual void  BindBButton()
    {
        Bind<Button>(typeof(B_Button));
        B_Back = GetButton((int)B_Button.B_Back);
        B_Back.gameObject.BindEvent(OnCliCkBack);
    }

    private void OnCliCkBack()
    {
        Managers.UI.ClosePopupUI(this);
    }
}
