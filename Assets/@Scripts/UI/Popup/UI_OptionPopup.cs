using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_OptionPopup : UI_Popup
{
    private enum Images
    {
        BG,
        Close
    }

    private enum TMPS
    {
        Title,
        //ButtonText
    }

    private enum Buttons
    {
        //OkButton
    }

    protected TextMeshProUGUI titleTMP;
    protected Button applyButton;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;


        BindData();



        return true;
    }

    private void BindData()
    {
        BindImage(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(TMPS));
        BindButton(typeof(Buttons));

        titleTMP = Get<TextMeshProUGUI>((int)TMPS.Title);
        //applyButton = GetButton((int)Buttons.OkButton);


        GetImage((int)Images.BG).gameObject.BindEvent(ClosePopopButton);
        GetImage((int)Images.Close).gameObject.BindEvent(ClosePopopButton);
        //applyButton.gameObject.BindEvent(ClickOkButton);

        //Get<TextMeshProUGUI>((int)(TMPS.ButtonText)).text = Managers.Localization.GetLocalizedValue(LanguageKey.apply.ToString());
    }

    protected virtual void ClickOkButton()
    {
       
    }

    protected virtual void ClosePopopButton()
    {
        ClosePopupUI();
    }

}
