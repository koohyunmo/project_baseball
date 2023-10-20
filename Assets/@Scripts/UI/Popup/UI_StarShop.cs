using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using Color = UnityEngine.Color;

public class UI_StarShop : UI_Popup
{


    enum Images
    {
        Buy1DollarButton,
        Buy2DollarButton,
        Buy3DollarButton,
        AdPlayButton,
        BG,
        Close
    }

    enum TMPs
    {
        ADTitle
    }

    public override bool Init()
    {
        if(base.Init() == false)
        {
            return false;
        }

        BindImage(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(TMPs));

        GetImage((int)Images.AdPlayButton).gameObject.BindEvent(ClickAdPlayButton);
        GetImage((int)Images.Buy1DollarButton).gameObject.BindEvent(Click_1_Dollar);
        GetImage((int)Images.Buy2DollarButton).gameObject.BindEvent(Click_2_Dollar);
        GetImage((int)Images.Buy3DollarButton).gameObject.BindEvent(Click_3_Dollar);
        GetImage((int)Images.BG).gameObject.BindEvent(() => ClosePopupUI());
        GetImage((int)Images.Close).gameObject.BindEvent(() => ClosePopupUI());


        StartCoroutine(co_UpdateUI());

        return true;
    }


    IEnumerator co_UpdateUI()
    {
        while (true)
        {
            Get<TextMeshProUGUI>((int)TMPs.ADTitle).text = Managers.Game.ADBounusTimeDisplay();

            //252 170 75
            GetImage((int)Images.AdPlayButton).color = Managers.Game.CanCllickAdButton() && Managers.Ad.CanShowRewardAd() ? new Color(252/255.0f,170/255.0f,75/255.0f,255/255.0f) : Color.gray;
            yield return new WaitForSeconds(1f);
        }
    }

    private void ClickAdPlayButton()
    {
        if (Managers.Game.CanCllickAdButton() && Managers.Ad.CanShowRewardAd())
        {
            Managers.Game.ClickAdButton();
            Debug.Log("±¤°í Å¬¸¯");
        }
        else
            return;
    }

    private void Click_1_Dollar()
    {

    }
    private void Click_2_Dollar()
    {

    }
    private void Click_3_Dollar()
    {

    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
