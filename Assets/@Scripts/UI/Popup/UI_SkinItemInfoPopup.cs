using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class UI_SkinItemInfoPopup : UI_InfoPopup
{

    private void Start()
    {
        Init();
    }

    protected ItemScriptableObject _itemSO;
    private Action _updateUI = null;

    public virtual void InitData<T>(T itemInfo,Action updateItem) where T : ItemScriptableObject
    {
        var type = typeof(T);

        if (type == typeof(ItemScriptableObject))
        {

        }
        else if (type == typeof(BallScriptableObject))
        {

        }
        else if (type == typeof(BatScriptableObject))
        {

        }

        _updateUI = updateItem;
        _itemSO = itemInfo;
    }

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        if(_itemSO != null)
        {
            popupIcon.sprite = _itemSO.icon;
            popupInfoText.text = _itemSO.name;
            popupButtonIcon.sprite = _itemSO.icon;

            ButtonUpdate();
        }


        return true;
    }

    private void ButtonUpdate()
    {
        if(Managers.Game.GameDB.playerInventory.Contains(_itemSO.id))
        {
            popupButtonText.text = "EQUIP";
            popupButton.gameObject.BindEvent(EquipItem);
        }
        else
        {
            popupButtonText.text = "GET";
            popupButton.gameObject.BindEvent(GetItem);
        }
    }

    private void GetItem()
    {
        Managers.Game.Getitme(_itemSO.id);
        _updateUI?.Invoke();
        ClosePopupUI();
        Debug.Log("TODO ±¤°í or µ·");
    }
    private void EquipItem()
    {
        Managers.Game.ChangeItem(_itemSO.id);
        ClosePopupUI();
    }
}
