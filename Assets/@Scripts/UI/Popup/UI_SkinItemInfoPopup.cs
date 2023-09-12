using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SkinItemInfoPopup : UI_InfoPopup
{

    private void Start()
    {
        Init();
    }

    protected ItemScriptableObject _itemSO;
    private Action _lockUIAction = null;
    private Action _equipUIAction = null;

    public void InitData<T>(T itemInfo, Action updateLockUI, Action updateEquipUI) where T : ItemScriptableObject
    {
        var type = typeof(T);


        if (type == typeof(BallScriptableObject))
        {

        }
        else if (type == typeof(BatScriptableObject))
        {

        }
        else if (type == typeof(SkillScriptableObject))
        {

        }
        else if (type == typeof(ItemScriptableObject))
        {

        }

        _lockUIAction = updateLockUI;
        _equipUIAction = updateEquipUI;
        _itemSO = itemInfo;
    }

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        if (_itemSO != null)
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
        if (Managers.Game.GameDB.playerInventory.Contains(_itemSO.id))
        {
            //popupButtonText.text = "EQUIP";
            popupButtonText.text = Managers.Localization.GetLocalizedValue("equip");
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
        Managers.Game.GetItem(_itemSO.id);
        _lockUIAction?.Invoke();
        ClosePopupUI();
        Debug.Log("TODO ±¤°í or µ·");
    }
    private void EquipItem()
    {
        Managers.Game.ChangeItem(_itemSO.id);
        _equipUIAction?.Invoke();
        Managers.Game.EquipItemAction?.Invoke(); // µî·ÏµÈ Item
        ClosePopupUI();
    }
}
