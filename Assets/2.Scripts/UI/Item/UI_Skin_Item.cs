using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_Skin_Item : UI_Base
{
    public Image _icon;
    public Material _mat;
    public GameObject _bat;
    public string _key;


    public void Start()
    {
        gameObject.gameObject.BindEvent(OnClick);
    }

    public void InitData(string key)
    {
        _key = key;


        if (Managers.Resource.Bats[_key] is GameObject go)
        {
            _bat = go;
            MeshRenderer renderer = _bat.GetComponent<MeshRenderer>();
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

    private void OnClick()
    {
        if(_mat != null)
        {
            Managers.Game.Bat.ChangeBat(_mat);
            Debug.Log($"ChangeBat {_key}");
        }

    }


}
