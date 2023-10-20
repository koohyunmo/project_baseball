using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ChallengeClearPopup : UI_InfoPopup
{

    ChallengeScriptableObject _cso;
    bool _isFailed = false;
    private void Start()
    {
        Init();
    }

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        if (_isFailed == false)
        {
            Managers.Sound.Play(Define.Sound.Effect, "Win05");

            popupInfoText.text = "<color=green>COMPLETE</color>";
            popupButtonText.text = "OK";
            popupButton.gameObject.BindEvent(ClearButton);
        }
        else
        {
            Managers.Sound.Play(Define.Sound.Effect, "Lose05");

            popupIcon.color = Color.red;
            popupInfoText.text = "<color=red>FAIL</color>";
            popupButtonText.text = "OK";
            popupButton.gameObject.BindEvent(ClearButton);
        }


        return true;
    }

    private void ClearButton()
    {
        Managers.UI.ClosePopupUI(this);
        Managers.Game.GoHome();
    }

    public void Failed()
    {
        _isFailed = true;

        if(Init())
        {
            return;
        }
    }

    protected override void ClickCloseButton()
    {
        Managers.UI.ClosePopupUI(this);
        Managers.Game.GoHome();
    }

    private void OnEnable()
    {
        _isFailed = false;
    }
}
