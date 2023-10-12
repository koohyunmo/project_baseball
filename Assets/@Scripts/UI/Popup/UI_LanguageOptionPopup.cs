using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_LanguageOptionPopup : UI_OptionPopup
{
    // Start is called before the first frame update




    public Image US;
    public Image KR;
    public Image JP;

    public Image UsOK;
    public Image KrOK;
    public Image JpOK;



    private void Start()
    {
        Init();
    }


    public override bool Init()
    {
         if(base.Init() == false)
            return false;


        

        

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
        Managers.Localization.ChangeLanguage(Language.EN);
        UIUpdate();
    }

    private void ChangeKR()
    {
        Managers.Localization.ChangeLanguage(Language.KR);
        UIUpdate();
    }

    private void ChangeJP()
    {
        Managers.Localization.ChangeLanguage(Language.JP);
        UIUpdate();
    }

    private void UIUpdate()
    {
        UsOK.gameObject.SetActive(false);
        KrOK.gameObject.SetActive(false);
        JpOK.gameObject.SetActive(false);

        switch (Managers.Localization.currentLanguage)
        {
            case Language.EN:
                UsOK.gameObject.SetActive(true);
                break;
            case Language.KR:
                KrOK.gameObject.SetActive(true);
                break;
            case Language.JP:
                JpOK.gameObject.SetActive(true);
                break;

        }

        titleTMP.text = Managers.Localization.GetLocalizedValue(LanguageKey.language.ToString());
    }

}
