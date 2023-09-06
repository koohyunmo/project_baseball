using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ChallengeItem : UI_Base
{

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
        gameObject.BindEvent(ShowPopup);
        UpdateUI();
    }

    private void ShowPopup()
    {

        var challengePopup = Managers.UI.ShowPopupUI<UI_ChallengeInfoPopup>();
        challengePopup.InitData(_cso);

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

        Get<TextMeshProUGUI>((int)(TMPs.IDTMP)).text = _cso.orderID.ToString();
        desc = _cso.desc;
    }


}
