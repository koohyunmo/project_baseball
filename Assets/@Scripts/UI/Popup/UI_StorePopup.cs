using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_StorePopup : UI_PopupHasBButton
{

    enum Buttons
    {
        B_Back,
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

        GetButton((int)Buttons.Button_Free).gameObject.BindEvent(rc.RollFreeRoullet);

        Get<TextMeshProUGUI>((int)TMPs.TimeTMP).text = Managers.Game.FreeRTimeDisplay();
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

            yield return new WaitForSeconds(1f);

        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

}
