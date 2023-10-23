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
    public Define.GetType _getType = Define.GetType.Failed;
    public long _gold;
    public Sprite startIcon;

    private void Bind()
    {
        if (_itemSO)
            _title.text = Managers.Localization.GetLocalizedValue(_itemSO.type.ToString());
        else
            _title.text = "TODO";

        popupButton.gameObject.BindEvent(ChallengeButtonClick);
    }

    public void InitData(Define.GetType getType, string id)
    {
        _itemSO = Managers.Resource.GetItemScriptableObjet<ItemScriptableObject>(id);
        _getType = getType;
    }

    public void InitData(Define.GetType getType, Define.Grade grade, long gold)
    {
        _grade = grade;
        _gold = gold;
        _itemSO = null;
        _getType = getType;
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
            popupInfoText.text = $"{Managers.Localization.GetLocalizedValue(_getType.ToString())} !\n {Managers.Localization.GetLocalizedValue(_grade.ToString())} : {_gold} ";
            popupButtonText.text = Managers.Localization.GetLocalizedValue(LanguageKey.confirm.ToString());
        }

        
        return true;
    }

    private void ChallengeButtonClick()
    {
        Managers.UI.ClosePopupUI();
    }
}
