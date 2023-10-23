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

    public TextMeshProUGUI buy1Dollar;
    public TextMeshProUGUI buy2Dollar;
    public TextMeshProUGUI buy3Dollar;

    public TextMeshProUGUI price1;
    public TextMeshProUGUI price2;
    public TextMeshProUGUI price3;

    [SerializeField]
    public TextMeshProUGUI adTmp;
    [SerializeField]
    public TextMeshProUGUI title;


    public override bool Init()
    {
        if(base.Init() == false)
        {
            return false;
        }

        BindImage(typeof(Images));

        GetImage((int)Images.AdPlayButton).gameObject.BindEvent(ClickAdPlayButton);
        GetImage((int)Images.Buy1DollarButton).gameObject.BindEvent(Click_1_Dollar);
        GetImage((int)Images.Buy2DollarButton).gameObject.BindEvent(Click_2_Dollar);
        GetImage((int)Images.Buy3DollarButton).gameObject.BindEvent(Click_3_Dollar);
        GetImage((int)Images.BG).gameObject.BindEvent(() => ClosePopupUI());
        GetImage((int)Images.Close).gameObject.BindEvent(() => ClosePopupUI());


        var key = LanguageKey.buy.ToString();
        buy1Dollar.text = Managers.Localization.GetLocalizedValue(key);
        buy2Dollar.text = Managers.Localization.GetLocalizedValue(key);
        buy3Dollar.text = Managers.Localization.GetLocalizedValue(key);

        price1.text = Managers.Localization.GetLocalizedValue(LanguageKey.onedollar.ToString());
        price2.text = Managers.Localization.GetLocalizedValue(LanguageKey.twodollar.ToString());
        price3.text = Managers.Localization.GetLocalizedValue(LanguageKey.threedollar.ToString());

        adTmp.text = Managers.Localization.GetLocalizedValue(LanguageKey.ad.ToString());
        title.text = Managers.Localization.GetLocalizedValue(LanguageKey.starshop.ToString());

        StartCoroutine(co_UpdateUI());

        return true;
    }


    IEnumerator co_UpdateUI()
    {
        while (true)
        {
            adTmp.text = Managers.Game.ADBounusTimeDisplay();

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
        Managers.IAP.Purchase(Managers.IAP.productId_1_id);
    }
    private void Click_2_Dollar()
    {
        Managers.IAP.Purchase(Managers.IAP.productId_2_id);
    }
    private void Click_3_Dollar()
    {
        Managers.IAP.Purchase(Managers.IAP.productId_3_id);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
