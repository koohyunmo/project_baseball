using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ChallengeItem : UI_Base
{

    enum Images
    {
        UI_ChallengeItem,
    }

    enum TMPs
    {
        IDTMP,
    }

    ChallengeScriptableObject _cso;
    Action _parentClose;
    string _itemId;

    public string desc;

    private void Start()
    {
        itemInit();
    }

    private void itemInit()
    {
        Bind<TextMeshProUGUI>(typeof(TMPs));
        Bind<Image>(typeof(Images));
        gameObject.BindEvent(ShowPopup);
        GetImage((int)Images.UI_ChallengeItem).color = Utils.GetColor(_cso.league);
        UpdateUI();
    }

    private void ShowPopup()
    {
        var challengePopup = Managers.UI.ShowPopupUI<UI_ChallengeInfoPopup>();
        challengePopup.InitData(_cso);
        Managers.Game.SaveChallengGridPos();
    }

    public void InitData(string itemId, Action parentClose)
    {
        _itemId = itemId;
        _parentClose = parentClose;

        if (Managers.Resource.Resources.TryGetValue(_itemId, out UnityEngine.Object obj) && obj is ChallengeScriptableObject cso)
        {
            _cso = cso;
        }
        else
        {
            _cso = null;
            Debug.LogError("CSO is not a GameObject or the key does not exist.");
        }


    }

    private void UpdateUI()
    {
        if(_cso == null)
        {
            
            Debug.LogError($"{_itemId} : CSO is NULL");
            return;
        }

        if(Managers.Game.GameDB.challengeData[_cso.key] == false)
            Get<TextMeshProUGUI>((int)(TMPs.IDTMP)).text = _cso.orderID.ToString();
        else
            Get<TextMeshProUGUI>((int)(TMPs.IDTMP)).text = Managers.Localization.GetLocalizedValue(LanguageKey.clear.ToString());
        desc = _cso.desc;
    }


}
