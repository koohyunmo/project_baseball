using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_EndPopup : UI_Popup
{
    public TextMeshProUGUI strikeTMP;
    public Button homeButton;
    void Start()
    {
        StartCoroutine(c_Delay());
        homeButton.gameObject.BindEvent(HomeButtonClick);
    }

    private void HomeButtonClick()
    {
        Managers.UI.ClosePopupUI(this);
        Managers.Game.GoHome();
        
    }

    IEnumerator c_Delay()
    {
        yield return new WaitForSeconds(2f);
        strikeTMP.gameObject.SetActive(false);
        homeButton.gameObject.SetActive(true);
    }

}
