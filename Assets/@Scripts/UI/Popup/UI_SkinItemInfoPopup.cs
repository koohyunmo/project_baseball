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
    private string _itemInfo;

    enum Type
    {
        Item,
        Ball,
        Bat,
        Skill
    }

    Type _type = Type.Item;

    public void InitData<T>(T itemInfo, Action updateLockUI, Action updateEquipUI) where T : ItemScriptableObject
    {
        _lockUIAction = updateLockUI;
        _equipUIAction = updateEquipUI;
        _itemSO = itemInfo;

        _type = Type.Item;
    }

    public void InitData<T>(string itemID, Action updateLockUI, Action updateEquipUI) where T : ItemScriptableObject
    {
        var type = typeof(T);


        if (type == typeof(BallScriptableObject))
        {
            _type = Type.Ball;
            _itemSO = Managers.Resource.GetItemScriptableObjet<BallScriptableObject>(itemID);
        }
        else if (type == typeof(BatScriptableObject))
        {
            _type = Type.Bat;
            _itemSO = Managers.Resource.GetItemScriptableObjet<BatScriptableObject>(itemID);
        }
        else if (type == typeof(SkillScriptableObject))
        {
            _type = Type.Skill;
            _itemSO = Managers.Resource.GetItemScriptableObjet<SkillScriptableObject>(itemID);
        }
        else if (type == typeof(ItemScriptableObject))
        {
            _type = Type.Item;
            _itemSO = Managers.Resource.GetItemScriptableObjet<ItemScriptableObject>(itemID);
        }

        _lockUIAction = updateLockUI;
        _equipUIAction = updateEquipUI;
    }


    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        if (_itemSO != null)
        {
            SetUIData();
            ButtonUpdate();
        }


        return true;
    }

    private void SetUIData()
    {

        popupIcon.sprite = _itemSO.icon;
        popupInfoText.text = Managers.Localization.GetLocalizedValue(_itemSO.name);
        //popupButtonIcon.sprite = _itemSO.icon;

        switch (_type)
        {
            case Type.Item:
                return;
            case Type.Ball:
                {
                    if (_itemSO is BallScriptableObject childItem)
                    {
                        //TODO
                    }
                }
                return;
            case Type.Bat:
                {
                    if (_itemSO is BatScriptableObject childItem)
                    {
                        popupInfoText.text += '\n' + Managers.Localization.GetLocalizedValue(childItem.batType.ToString()) + '\n' + "Power : " + childItem.power.ToString() + '\n';
                    }
                }
                return;
            case Type.Skill:
                {
                    if (_itemSO is SkillScriptableObject childItem)
                    {
                        //TODO
                    }
                }
                return;
        }
    }


    private void ButtonUpdate()
    {
        if (Managers.Game.GameDB.playerInventory.Contains(_itemSO.id))
        {
            //popupButtonText.text = "EQUIP";
            popupButtonText.text = Managers.Localization.GetLocalizedValue(LanguageKey.equip.ToString());
            popupButton.interactable = true;
            popupButton.gameObject.BindEvent(EquipItem);
        }
        else
        {
            popupButtonText.text = Managers.Localization.GetLocalizedValue(LanguageKey.get.ToString());
            popupButton.interactable = Managers.Ad.CanShowRewardAd();
            popupButton.gameObject.BindEvent(GetItem);
        }

        if(Managers.Game.EquipBallId.Equals(_itemSO.id) || Managers.Game.EquipBatId.Equals(_itemSO.id) || Managers.Game.EquipSkillId.Equals(_itemSO.id))
        {
            popupButtonText.text = Managers.Localization.GetLocalizedValue(LanguageKey.equipping.ToString());
            popupButton.interactable = false;
        }
    }

    private void GetItem()
    {
        if (Managers.Ad.CanShowRewardAd() == false)
        {
            return;
        }
        else
        {
            popupButton.interactable = Managers.Ad.CanShowRewardAd();
            Managers.Game.GetItem(_itemSO.id);
            _lockUIAction?.Invoke();
            ClosePopupUI();
            Managers.Ad.ShowRewardedAd();
            Debug.Log("TODO ±¤°í or µ·");
        }



    }
    private void EquipItem()
    {
        Managers.Game.ChangeItem(_itemSO.id);
        _equipUIAction?.Invoke();
        Managers.Game.EquipItemAction?.Invoke(); // µî·ÏµÈ Item
        ClosePopupUI();
    }
}
