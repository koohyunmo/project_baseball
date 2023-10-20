using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_RoulletItemInfoPopup : UI_InfoPopup
{
    enum TMPs
    {
        Title
    }

    ItemScriptableObject _itemSO;
    public TextMeshProUGUI _title;
    Define.Grade _grade;
    public bool _isduplicate = false;
    public long _gold;
    public Sprite startIcon;

    private void Bind()
    {
        if (_itemSO)
            _title.text = Managers.Localization.GetLocalizedValue(_itemSO.name);
        else
            _title.text = "TODO";

        popupButton.gameObject.BindEvent(ChallengeButtonClick);
    }

    public void InitData(string id)
    {
        _itemSO = Managers.Resource.GetItemScriptableObjet<ItemScriptableObject>(id);
        _isduplicate = false;
    }

    public void InitData(Define.Grade grade, long gold)
    {
        _grade = grade;
        _gold = gold;
        _itemSO = null;
        _isduplicate = true;
    }


    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        if (_itemSO != null)
        {
            if (popupInfoText == null)
            {
                Debug.LogWarning($"Info Popup is Null ID {_itemSO.id}");
                return false;
            }

            popupInfoText.text = Managers.Localization.GetLocalizedValue(_itemSO.name);
            popupButtonText.text = Managers.Localization.GetLocalizedValue(LanguageKey.confirm.ToString());
            popupIcon.sprite = _itemSO.icon;
            Bind();
        }
        else
        {
            Bind();
            popupIcon.sprite = startIcon;
            _title.text = Managers.Localization.GetLocalizedValue(LanguageKey.star.ToString());
            popupInfoText.text = $"{Managers.Localization.GetLocalizedValue(LanguageKey.duplicate.ToString())} !\n {Managers.Localization.GetLocalizedValue(_grade.ToString())} : {_gold} ";
            popupButtonText.text = Managers.Localization.GetLocalizedValue(LanguageKey.confirm.ToString());
        }

        
        return true;
    }

    private void ChallengeButtonClick()
    {
        Managers.UI.ClosePopupUI();
    }
}
