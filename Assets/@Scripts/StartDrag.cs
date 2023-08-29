using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
public class StartDrag : MonoBehaviour, IDragHandler
{

    private bool _isDrag = false;

    UI_Main ui_Main;

    private void Start()
    {
        ui_Main = GetComponentInParent<UI_Main>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Managers.Game.GameState == Define.GameState.Home && _isDrag == false)
        {
            _isDrag = true;

            ui_Main.DoStart();
        }
    }

}
