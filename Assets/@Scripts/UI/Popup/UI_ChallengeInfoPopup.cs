using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ChallengeInfoPopup : UI_InfoPopup
{
    ChallengeScriptableObject _cso;

    private void Start()
    {
        Init();
    }

    public virtual void InitData(ChallengeScriptableObject challengeInfo)
    {
        _cso = challengeInfo;
    }

    public override bool Init()
    {
        if(base.Init() == false)
        {
            return false;
        }

        if (_cso != null)
        {
            popupInfoText.text = _cso.desc;
            popupButtonText.text = "PLAY";
            popupButton.gameObject.BindEvent(ChallengeButtonClick);
        }


        return true;
    }

    private void ChallengeButtonClick()
    {
        Managers.UI.CloseAllPopupUI();
        Managers.Game.GameReady(Define.GameMode.Challenge);
        Managers.Game.SetChallengeMode(_cso.score, _cso.mode,_cso.key);
    }

}
