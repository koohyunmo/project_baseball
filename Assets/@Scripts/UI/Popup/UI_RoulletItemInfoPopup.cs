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

    private void Bind()
    {
        _title.text = Managers.Localization.GetLocalizedValue(_itemSO.name);
    }

    public void InitData(string id)
    {
        _itemSO = Managers.Resource.GetItemScriptableObjet<ItemScriptableObject>(id);
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
            popupButton.gameObject.BindEvent(ChallengeButtonClick);
        }

        Bind();
        return true;
    }

    private void ChallengeButtonClick()
    {
        Managers.UI.ClosePopupUI();
    }
}
