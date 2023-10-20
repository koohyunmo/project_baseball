using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    public GameObject starAndAd;
    public TextMeshProUGUI starTMP;


    public Sprite starIcon;

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
                        popupInfoText.text += '\n' + Managers.Localization.GetLocalizedValue(childItem.batType.ToString()) + '\n' + Managers.Localization.GetLocalizedValue(LanguageKey.power.ToString()) + " : " + childItem.power.ToString() + '\n';
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
            popupButtonIcon.sprite = _itemSO.icon;
            popupButtonText.text = Managers.Localization.GetLocalizedValue(LanguageKey.equip.ToString());
            popupButton.interactable = true;
            popupButton.gameObject.BindEvent(EquipItem);
            OnButton();
            return;
        }
        else
        {
            popupButtonText.text = Managers.Localization.GetLocalizedValue(LanguageKey.get.ToString());
            popupButton.interactable = Managers.Ad.CanShowRewardAd();
            popupButton.gameObject.BindEvent(GetItem);
        }

        if (Managers.Game.EquipBallId.Equals(_itemSO.id) || Managers.Game.EquipBatId.Equals(_itemSO.id) || Managers.Game.EquipSkillId.Equals(_itemSO.id))
        {
            popupButtonText.text = Managers.Localization.GetLocalizedValue(LanguageKey.equipping.ToString());
            popupButton.interactable = false;
        }


        if ((int)_itemSO.grade >= (int)Define.Grade.Epic)
        {
            popupButton.interactable = Managers.Game.CanPay(Managers.Game.GetPrice(_itemSO.grade));

            OffButton();
            popupButton.gameObject.RemoveBindEvent(GetItem);
            //popupButton.gameObject.BindEvent(GetItemAndPaid);
            popupButton.gameObject.BindEvent(Paid);
            starTMP.text = Managers.Game.GetPrice(_itemSO.grade).ToString();
            popupButtonIcon.sprite = starIcon;
        }
        else
        {
            OnButton();
        }
    }

    private void OffButton()
    {
        popupButtonText.gameObject.SetActive(false);
        popupButtonIcon.gameObject.SetActive(false);

        starAndAd.SetActive(true);
    }

    private void OnButton()
    {
        popupButtonText.gameObject.SetActive(true);
        popupButtonIcon.gameObject.SetActive(true);

        starAndAd.SetActive(false);
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


                //ClosePopupUI();
                Managers.Ad.ShowRewardedAd(GetRewardAdsItemRemoveActions);

                Debug.Log("TODO ±¤°í or µ·");
            }

    }

    private void GetItemAndPaid()
    {


        if (Managers.Ad.CanShowRewardAd() == false && Managers.Game.CanPay(Managers.Game.GetPrice(_itemSO.grade)) == false)
        {
            return;
        }
        else
        {
            popupButton.interactable = Managers.Ad.CanShowRewardAd();


            //ClosePopupUI();
            //Managers.Ad.ShowRewardedAd(GetRewardAdAndPaysItemRemoveActions);

            Debug.Log("TODO ±¤°í or µ·");
        }

    }

    private void Paid()
    {
        if (Managers.Game.CanPay(Managers.Game.GetPrice(_itemSO.grade)) == false)
        {
            return;
        }
        else
        {
            popupButton.interactable = Managers.Ad.CanShowRewardAd();

            if (Managers.Game.PlayerInfo.star > Managers.Game.GetPrice(_itemSO.grade))
            {
                Managers.Game.GetItem(_itemSO.id);
                Managers.Game.MinusStar(Managers.Game.GetPrice(_itemSO.grade));
                //popupButton.gameObject.RemoveBindEvent(GetItemAndPaid);
                popupButton.gameObject.RemoveBindEvent(Paid);
                ButtonUpdate();
                _lockUIAction?.Invoke();
            }
        }
    }
    private void GetRewardAdsItemRemoveActions()
    {
        Managers.Game.GetItem(_itemSO.id);
        popupButton.gameObject.RemoveBindEvent(GetItem);
        ButtonUpdate();
        _lockUIAction?.Invoke();
    }

    private void GetRewardAdAndPaysItemRemoveActions()
    {
        if (Managers.Game.PlayerInfo.star > Managers.Game.GetPrice(_itemSO.grade))
        {
            Managers.Game.GetItem(_itemSO.id);
            Managers.Game.MinusStar(Managers.Game.GetPrice(_itemSO.grade));
            popupButton.gameObject.RemoveBindEvent(GetItemAndPaid);
            ButtonUpdate();
            _lockUIAction?.Invoke();
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
