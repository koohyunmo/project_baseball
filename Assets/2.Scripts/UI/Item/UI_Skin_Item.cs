using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_Skin_Item : UI_Base
{
    public Image _icon;
    public List<Material> _mats;
    public Mesh _mesh;
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
            var meshfilter = _item.model.GetComponent<MeshFilter>();
            if (renderer != null && meshfilter != null)
            {
                _mats = new List<Material>();

                var modelMats = renderer.sharedMaterials;
                _mats.AddRange(modelMats);
                _mesh = meshfilter.sharedMesh;
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
        if(_mats != null && _mesh != null)
        {
            Managers.Game.Bat.ChangeBatMat(_mats);
            Managers.Game.Bat.ChangeBatMesh(_mesh);
            Managers.Game.Bat.SetBetHandle();
            Debug.Log($"ChangeBat ID :  {_key}");
        }
        else
        {
            Debug.LogWarning("æ∆¿Ã≈€ ∏µ®");
        }

    }


}
