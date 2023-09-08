using System.Collections;
using System.Collections.Generic;
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

    enum Images
    {
        Icon,
        Background,
        Choice,
        LockImage
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

        _icon = Get<Image>((int)Images.Icon);
        gameObject.gameObject.BindEvent(OnClick);

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
        var infoPopup = Managers.UI.ShowPopupUI<UI_SkinItemInfoPopup>();
        infoPopup.InitData<ItemScriptableObject>(_item, UpdateLockUI, ChoiceUIUpdate);
    }
    private void BatSetting()
    {
        MeshRenderer renderer = _item.model.GetComponent<MeshRenderer>();
        var meshfilter = _item.model.GetComponent<MeshFilter>();
        if (renderer != null && meshfilter != null)
        {
            _mats = new List<Material>();

            var modelMats = renderer.sharedMaterials;
            _mats.AddRange(modelMats);
            _mesh = meshfilter.sharedMesh;
        }
    }

    private void UpdateUI()
    {
        if(_item == null)
        {
            Debug.LogError("Item is Null");
            return;
        }

        if(Managers.Game.GameDB.playerInventory.Contains(_item.id) == true)
        {
            GetImage((int)Images.LockImage).gameObject.SetActive(false);
        }

        _icon.sprite = _item.icon;
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
            GetImage((int)Images.Choice).gameObject.SetActive(true);
            Managers.Game.SetEquipUIItemAction(ChoiceUIUpdate);
            return;
        }
        //else if (Managers.Game.EquipBatId.Equals(_item.id))
        //{
        //    GetImage((int)Images.Choice).gameObject.SetActive(true);
        //    Managers.Game.SetEquipUIItemAction(ChoiceUIUpdate);
        //    return;
        //}
        //else if(Managers.Game.EquipSkillId.Equals(_item.id))
        //{
        //    GetImage((int)Images.Choice).gameObject.SetActive(true);
        //    Managers.Game.SetEquipUIItemAction(ChoiceUIUpdate);
        //    return;
        //}
        else
        {
            GetImage((int)Images.Choice).gameObject.SetActive(false);
            Managers.Game.RemoveEqupUIItemAction(ChoiceUIUpdate);
        }

    }

    private void OnClick()
    {

        if(Managers.Game.GameDB.playerInventory.Contains(_key) == false)
        {
            var infoPopup = Managers.UI.ShowPopupUI<UI_SkinItemInfoPopup>();
            infoPopup.InitData<ItemScriptableObject>(_item, UpdateLockUI, ChoiceUIUpdate);
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
        Managers.Game.Getitme(_item.id);
    }

    private void BatClick()
    {
        if (_mats != null && _mesh != null)
        {
            Managers.Game.Bat.ChangeBatMat(_mats);
            Managers.Game.Bat.ChangeBatMesh(_mesh);
            Managers.Game.Getitme(_item.id);
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
    }

}
