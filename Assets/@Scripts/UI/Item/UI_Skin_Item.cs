using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_Skin_Item : UI_Base
{
    public Image _icon;
    public List<Material> _mats;
    public Mesh _mesh;
    public ItemScriptableObject _item;
    public string _key;
    public ScollViewType _type;

    private Image _skinItem;

    enum Images
    {
        Icon,
        Background,
        LockImage
    }

    enum TMPs
    {
        Choice
    }

    enum Buttons
    {
        B_Information
    }

    public void Start()
    {
        Init();
    }

    private void Init()
    {
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(TMPs));

        _icon = Get<Image>((int)Images.Icon);
        gameObject.gameObject.BindEvent(OnClick);
        _skinItem = GetComponent<Image>();

        GetImage((int)Images.Background).gameObject.SetActive(false);
        GetButton((int)Buttons.B_Information).gameObject.BindEvent(ShowInfoPopup);


        UpdateUI();
        ChoiceUIUpdate();

    }

    public void InitData(string key, ScollViewType type)
    {
        _key = key;
        _type = type;

        if (Managers.Resource.Resources.TryGetValue(_key, out Object obj) && obj is ItemScriptableObject so)
            _item = so;
        else
            _item = null;


        switch (_type)
        {
            case ScollViewType.Ball:
                break;
            case ScollViewType.Bat:
                BatSetting();
                break;
            case ScollViewType.Skill:
                break;
        }

    }


    private void ShowInfoPopup()
    {
        switch (_type)
        {
            case ScollViewType.Ball:
                {
                    var infoPopup = Managers.UI.ShowPopupUI_Generic<UI_SkinItemInfoPopup>();
                    infoPopup.InitData<BallScriptableObject>(_item.id, UpdateLockUI, ChoiceUIUpdate);
                }
                return;
            case ScollViewType.Bat:
                {
                    var infoPopup = Managers.UI.ShowPopupUI_Generic<UI_SkinItemInfoPopup>();
                    infoPopup.InitData<BatScriptableObject>(_item.id, UpdateLockUI, ChoiceUIUpdate);
                }
                return;
            case ScollViewType.Skill:
                {
                    var infoPopup = Managers.UI.ShowPopupUI_Generic<UI_SkinItemInfoPopup>();
                    infoPopup.InitData<SkillScriptableObject>(_item.id, UpdateLockUI, ChoiceUIUpdate);
                }
                return;
        }

        var infoPopup2 = Managers.UI.ShowPopupUI_Generic<UI_SkinItemInfoPopup>();
        infoPopup2.InitData<ItemScriptableObject>(_item.id, UpdateLockUI, ChoiceUIUpdate);

    }
    private void BatSetting()
    {
        if (_item.model)
        {
            MeshRenderer renderer = _item.model.GetComponent<MeshRenderer>();

            if (renderer)
            {
                var meshfilter = _item.model.GetComponent<MeshFilter>();

                if (meshfilter)
                {
                    _mats = new List<Material>();

                    var modelMats = renderer.sharedMaterials;
                    _mats.AddRange(modelMats);
                    _mesh = meshfilter.sharedMesh;
                }
            }
        }

    }

    private void UpdateUI()
    {
        if (_item == null)
        {
            Debug.LogError("Item is Null");
            return;
        }

        if (Managers.Game.GameDB.playerInventory.Contains(_item.id) == true)
        {
            GetImage((int)Images.LockImage).gameObject.SetActive(false);
        }

        _icon.sprite = _item.icon;

        switch (_item.grade)
        {
            case Grade.Common:
                _skinItem.color = Color.green;
                break;
            case Grade.Uncommon:
                _skinItem.color = Color.gray;
                break;
            case Grade.Rare:
                _skinItem.color = Color.blue;
                break;
            case Grade.Epic:
                _skinItem.color = Color.magenta;
                break;
            case Grade.Legendary:
                _skinItem.color = Color.red;
                break;
        }

        _skinItem.color = new Color(_skinItem.color.r, _skinItem.color.g, _skinItem.color.b, 0.5f);
    }

    private void UpdateLockUI()
    {

        if (Managers.Game.GameDB.playerInventory.Contains(_item.id) == true)
        {
            GetImage((int)Images.LockImage).gameObject.SetActive(false);
        }

    }

    private void ChoiceUIUpdate()
    {
        if (Managers.Game.EquipBallId.Equals(_item.id) || Managers.Game.EquipBatId.Equals(_item.id) || Managers.Game.EquipSkillId.Equals(_item.id))
        {
            Get<TextMeshProUGUI>((int)TMPs.Choice).text = Managers.Localization.GetLocalizedValue(LanguageKey.equipping.ToString());
            Get<TextMeshProUGUI>((int)TMPs.Choice).gameObject.SetActive(true);
            Managers.Game.SetEquipUIItemAction(ChoiceUIUpdate);
            return;
        }
        else
        {
            Get<TextMeshProUGUI>((int)TMPs.Choice).gameObject.SetActive(false);
            Managers.Game.RemoveEqupUIItemAction(ChoiceUIUpdate);
        }

    }

    private void OnClick()
    {

        if (Managers.Game.GameDB.playerInventory.Contains(_key) == false)
        {
            var infoPopup = Managers.UI.ShowPopupUI_Generic<UI_SkinItemInfoPopup>();

            switch (_type)
            {
                case ScollViewType.Ball:
                    {
                        infoPopup.InitData<BallScriptableObject>(_item.id, UpdateLockUI, ChoiceUIUpdate);
                    }
                    return;
                case ScollViewType.Bat:
                    {
                        infoPopup.InitData<BatScriptableObject>(_item.id, UpdateLockUI, ChoiceUIUpdate);
                    }
                    return;
                case ScollViewType.Skill:
                    {
                        infoPopup.InitData<SkillScriptableObject>(_item.id, UpdateLockUI, ChoiceUIUpdate);
                    }
                    return;
            }


            infoPopup.InitData(_item, UpdateLockUI, ChoiceUIUpdate);
            return;
        }
        else
        {
            switch (_type)
            {
                case ScollViewType.Ball:
                    BallClick();
                    break;
                case ScollViewType.Bat:
                    BatClick();
                    break;
                case ScollViewType.Skill:
                    SkillClick();
                    break;
            }


            Managers.Game.EquipItemAction?.Invoke(); // 등록된 Item
            ChoiceUIUpdate(); // 현재 Item
        }

    }


    private void BallClick()
    {
        Managers.Game.ChangeBall(_item.id);
        Managers.Game.GetItem(_item.id);
    }

    private void BatClick()
    {
        if (_mats != null && _mesh != null)
        {
            //Managers.Game.Bat.ChangeBatMat(_mats);
            //Managers.Game.Bat.ChangeBatMesh(_mesh);
            Managers.Game.GetItem(_item.id);
            Managers.Game.ChangeBat(_item.id);
            //Managers.Game.Bat.SetBetHandle();
            Debug.Log($"ChangeBat ID :  {_item.id}");
        }
        else
        {
            Debug.LogWarning("아이템 모델");
        }

    }

    private void SkillClick()
    {
        Managers.Game.ChangeSkill(_item.id);
    }

    private void OnDestroy()
    {
        Managers.Game.RemoveEqupUIItemAction(ChoiceUIUpdate);
        GetButton((int)Buttons.B_Information).transform.DOKill();
    }

}
