using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_Skin_Item : UI_Base
{
    public Image _icon;
    public Material _mat;
    public ItemScriptableObject _item;
    public GameObject _bat;
    public string _key;


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

    public void InitData(string key)
    {
        _key = key;

        if (Managers.Resource.Bats[_key] is ItemScriptableObject so)
        {

            _item = so;

           
            MeshRenderer renderer = _item.model.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                _mat = renderer.sharedMaterial;
            }
            else
            {
                Debug.LogError("No MeshRenderer component found on the GameObject.");
            }
        }
        else
        {
            Debug.LogError("Bats[key] is not a GameObject.");
        }
    }

    private void UpdateUI()
    {
        _bat = _item.model;
        _icon.sprite = _item.icon;
    }

    private void OnClick()
    {
        if(_mat != null)
        {
            Managers.Game.Bat.ChangeBatMat(_mat);
            Debug.Log($"ChangeBat ID :  {_key}");
        }

    }


}
