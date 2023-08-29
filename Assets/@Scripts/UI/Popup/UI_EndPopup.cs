using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_EndPopup : UI_Popup
{
    public TextMeshProUGUI strikeTMP;
    public Button homeButton;
    public Button replayButton;
    public Button retryButton;
    void Start()
    {
        StartCoroutine(c_Delay());
        homeButton.gameObject.BindEvent(HomeButtonClick);
        replayButton.gameObject.BindEvent(ReplayButtonClick);
        retryButton.gameObject.BindEvent(RetryButtonClick);

        Managers.Object.DespawnBall();
    }

    private void HomeButtonClick()
    {
        Managers.UI.ClosePopupUI(this);
        Managers.Game.GoHome();
        
    }

    private void ReplayButtonClick()
    {
        Managers.Game.ReplayReview();
    }

    private void RetryButtonClick()
    {
        Managers.UI.ClosePopupUI(this);
        Managers.Game.GameRetry();
    }

    IEnumerator c_Delay()
    {
        yield return new WaitForSeconds(2f);
        strikeTMP.gameObject.SetActive(false);
        homeButton.gameObject.SetActive(true);
    }

}
