using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_StorePopup : UI_PopupHasBButton
{

    enum Buttons
    {
        B_Back,
        Button_Ad,
        Button_Free
    }

    enum TMPs
    {
        TimeTMP,
        T_AD,
        T_Free
    }

    enum GameObjects
    {
        RouletteController
    }

    RouletteController rc;

    public TextMeshProUGUI roullettitle;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<TextMeshProUGUI>(typeof(TMPs));
        BindObject((typeof(GameObjects)));
        BindButton(typeof(Buttons));
        GetButton((int)Buttons.B_Back).gameObject.BindEvent(ClosePopup);
        

        rc = GetObject((int)GameObjects.RouletteController).GetComponent<RouletteController>();

        GetButton((int)Buttons.Button_Ad).gameObject.BindEvent(rc.RollADRoullet);
        GetButton((int)Buttons.Button_Free).gameObject.BindEvent(rc.RollFreeRoullet);

        Get<TextMeshProUGUI>((int)TMPs.TimeTMP).text = Managers.Game.FreeRTimeDisplay();
        Get<TextMeshProUGUI>((int)TMPs.T_AD).text = Managers.Localization.GetLocalizedValue(LanguageKey.spinwithad.ToString());
        Get<TextMeshProUGUI>((int)TMPs.T_Free).text = Managers.Localization.GetLocalizedValue(LanguageKey.spinforfree.ToString());
        StartCoroutine(co_UpdateUI());

        roullettitle.text = Managers.Localization.GetLocalizedValue(LanguageKey.freespin.ToString());

        return true;
    }

    private void ClosePopup()
    {
        if(rc.isSpinning)
        {
            return;
        }

        ClosePopupUI();

    }

    IEnumerator co_UpdateUI()
    {
        while (true)
        {
           
            Get<TextMeshProUGUI>((int)TMPs.TimeTMP).text = Managers.Game.FreeRTimeDisplay();

           
            GetButton((int)Buttons.Button_Free).interactable = Managers.Game.GetFreeRoullet();


            if(Managers.Ad.CanShowRewardAd() == false)
            {
                GetButton((int)Buttons.Button_Ad).interactable = false;
            }

            if(Managers.Game.GetADRoullet() == false)
            {
                GetButton((int)Buttons.Button_Ad).interactable = false;
                Get<TextMeshProUGUI>((int)TMPs.T_AD).text = Managers.Game.ADRTimeDisplay();
            }

            yield return new WaitForSeconds(1f);

        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

}
