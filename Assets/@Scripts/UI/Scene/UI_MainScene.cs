using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_MainScene : UI_Scene
{

    enum Buttons
    {
        B_Option,
        B_Skin,
        B_Chanllenge,
        B_Stroe,
    }

    enum Images
    {
        Notification
    }


    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));
        BindImage(typeof(Images));

        GetButton((int)Buttons.B_Skin).gameObject.BindEvent(B_SkinClick);

        return true;

    }

    private void B_SkinClick()
    {
        Managers.UI.ShowPopupUI<UI_SkinPopup>();
    }
}
