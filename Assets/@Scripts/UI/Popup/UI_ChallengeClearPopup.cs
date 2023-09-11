using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ChallengeClearPopup : UI_InfoPopup
{

    ChallengeScriptableObject _cso;

    private void Start()
    {
        Init();
    }

    public void InitData(ChallengeScriptableObject challengeInfo)
    {
        _cso = challengeInfo;
    }
    public void InitData(string key)
    {
        _cso = Managers.Resource.GetChallengeScriptableObjet(key);
    }

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        popupInfoText.text = "CLEAR CHALLENGE";
        popupButtonText.text = "OK";
        popupButton.gameObject.BindEvent(ClearButton);


        return true;
    }

    private void ClearButton()
    {
        Managers.UI.ClosePopupUI(this);
        Managers.Game.GoHome();
    }

}
