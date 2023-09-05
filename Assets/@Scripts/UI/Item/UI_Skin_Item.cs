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
        LockImage
    }

    public void Start()
    {
        Init();
    }

    private void Init()
    {
        Bind<Image>(typeof(Images));
        _icon = Get<Image>((int)Images.Icon);
        gameObject.gameObject.BindEvent(OnClick);

        GetImage((int)Images.Background).gameObject.SetActive(false);
        UpdateUI();
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
            case ScollViewType.Background:
                break;
        }

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

        _icon.sprite = _item.icon;
    }

    private void OnClick()
    {

        switch (_type)
        {
            case ScollViewType.Ball:
                BallClick();
              
                break;
            case ScollViewType.Bat:
                BatClick();
                break;
            case ScollViewType.Background:
                break;
        }



    }


    private void BallClick()
    {
        Managers.Game.ChangeBall(_key);
        Managers.Game.Getitme(_key);
    }

    private void BatClick()
    {
        if (_mats != null && _mesh != null)
        {
            Managers.Game.Bat.ChangeBatMat(_mats);
            Managers.Game.Bat.ChangeBatMesh(_mesh);
            //Managers.Game.Bat.SetBetHandle();
            Debug.Log($"ChangeBat ID :  {_key}");
            Managers.Game.Getitme(_key);
            Managers.Game.ChangeBat(_key);
        }
        else
        {
            Debug.LogWarning("æ∆¿Ã≈€ ∏µ®");
        }

    }

}
