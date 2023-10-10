using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_ChallengeInfoPopup : UI_InfoPopup
{

    enum TMPs
    {
        Title
    }

    ChallengeScriptableObject _cso;
    public TextMeshProUGUI _title;

    private void Bind()
    {
        if(_title)
        _title.text = Managers.Localization.GetLocalizedValue(LanguageKey.challenges.ToString());
        else
        {
            Debug.LogError("TMP is Null");
        }
    }

    public void InitData(ChallengeScriptableObject challengeInfo)
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
            if(popupInfoText == null)
            {
                Debug.LogWarning($"Info Popup is Null ID {_cso.key}");
                return false;
            }

            //popupInfoText.text = _cso.desc;

            int challengeScore = _cso.score; // 동적인 값
            string translation = string.Format(Managers.Localization.GetLocalizedValue(_cso.mode.ToString().ToLower()), challengeScore); // "Hit consecutively 3 times."와 같은 문자열을 생성

            popupInfoText.text = translation;
            popupButtonText.text = Managers.Localization.GetLocalizedValue(LanguageKey.play.ToString());
            popupIcon.color = Utils.GetColor(_cso.league);
            popupButton.gameObject.BindEvent(ChallengeButtonClick);
        }

        Bind();
        return true;
    }

    private void ChallengeButtonClick()
    {
        Managers.UI.CloseAllPopupUI();
        Managers.Game.GameReady(Define.GameMode.Challenge);
        Managers.Game.SetChallengeMode(_cso);
    }

}
