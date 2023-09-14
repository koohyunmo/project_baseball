using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_LanguageOptionPopup : UI_OptionPopup
{
    // Start is called before the first frame update



    private void Start()
    {
        Init();
    }

    public Image US;
    public Image KR;
    public Image JP;

    public Image UsOK;
    public Image KrOK;
    public Image JpOK;

    public override bool Init()
    {
         if(base.Init() == false)
            return false;


        titleTMP.text = "Language";

        

        US.gameObject.BindEvent(ChangeEN);
        KR.gameObject.BindEvent(ChangeKR);
        JP.gameObject.BindEvent(ChangeJP);

        UsOK.gameObject.SetActive(false);
        KrOK.gameObject.SetActive(false);
        JpOK.gameObject.SetActive(false);

        UIUpdate();

        return true;
    }

    private void ChangeEN()
    {
        Managers.Localization.ChangeLanguage(Language.English);
        UIUpdate();
    }

    private void ChangeKR()
    {
        Managers.Localization.ChangeLanguage(Language.Korean);
        UIUpdate();
    }

    private void ChangeJP()
    {
        Managers.Localization.ChangeLanguage(Language.Spanish);
        UIUpdate();
    }

    private void UIUpdate()
    {
        UsOK.gameObject.SetActive(false);
        KrOK.gameObject.SetActive(false);
        JpOK.gameObject.SetActive(false);

        switch (Managers.Localization.currentLanguage)
        {
            case Language.English:
                UsOK.gameObject.SetActive(true);
                break;
            case Language.Korean:
                KrOK.gameObject.SetActive(true);
                break;
            case Language.Spanish:
                JpOK.gameObject.SetActive(true);
                break;

        }
    }

}
