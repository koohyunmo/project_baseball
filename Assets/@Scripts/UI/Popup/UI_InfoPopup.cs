using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_InfoPopup : UI_Popup
{
    protected enum SharedText
    {
        ButtonText,
        InfoText
    }

    protected enum SharedImage
    {
        Icon,
        ButtonIcon,
        Close,
        BG
    }

    protected enum SharedIButton
    {
        InfoButton
    }


    protected Image popupIcon;
    protected Image popupButtonIcon;
    protected Text popupInfoText;
    protected Text popupButtonText;
    protected Button popupButton;



    public override bool Init()
    {
        if(base.Init() == false)
        {
            return false ;
        }

        Bind<Text>(typeof(SharedText));
        Bind<Image>(typeof(SharedImage));
        Bind<Button>(typeof(SharedIButton));


        Get<Image>((int)SharedImage.Close).gameObject.BindEvent(ClickCloseButton);
        Get<Image>((int)SharedImage.BG).gameObject.BindEvent(ClickCloseButton);

        popupIcon = Get<Image>((int)SharedImage.Icon);
        popupButtonIcon = Get<Image>((int)SharedImage.ButtonIcon);
        popupInfoText = Get<Text>((int)SharedText.InfoText);
        popupButton = Get<Button>((int)SharedIButton.InfoButton);
        popupButtonText = Get<Text>((int)SharedText.ButtonText);


        return true;
    }

    private void ClickCloseButton()
    {
        Managers.UI.ClosePopupUI(this);
    }

}
