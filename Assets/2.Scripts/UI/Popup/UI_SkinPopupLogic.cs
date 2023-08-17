using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_SkinPopupLogic : MonoBehaviour
{
    UI_SkinPopup m_popup;

    private void Start()
    {
        m_popup = GetComponent<UI_SkinPopup>();

        m_popup.B_Back.gameObject.BindEvent(B_BackClick);
    }


    private void B_BackClick()
    {
        Managers.UI.ClosePopupUI(m_popup);
    }

}
