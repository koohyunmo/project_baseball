using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_EndPopup : UI_Popup
{
    enum Buttons
    {
        retryButton,
        replayButton,
        homeButton

    }

    enum TMPs
    {
        StrikeTMP,
        GameScoreTMP
    }

    public TextMeshProUGUI strikeTMP;
    public TextMeshProUGUI gameScorTMP;
    public Button homeButton;
    public Button replayButton;
    public Button retryButton;
    void Start()
    {
        BindUI();
    }

    void BindUI()
    {
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(TMPs));

        homeButton = GetButton((int)Buttons.homeButton);
        replayButton = GetButton((int)Buttons.replayButton);
        retryButton = GetButton((int)Buttons.retryButton);
        strikeTMP = Get<TextMeshProUGUI>((int)TMPs.StrikeTMP);
        gameScorTMP = Get<TextMeshProUGUI>((int)TMPs.GameScoreTMP);

        gameScorTMP.gameObject.SetActive(false);

        StartCoroutine(c_Delay());
        homeButton.gameObject.BindEvent(HomeButtonClick);
        replayButton.gameObject.BindEvent(ReplayButtonClick);
        retryButton.gameObject.BindEvent(RetryButtonClick);
        retryButton.interactable = Managers.Ad.CanShowIntAd();

        Managers.Obj.DespawnBall();
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

        if (Managers.Ad.CanShowIntAd())
        {
            Managers.UI.ClosePopupUI(this);
            Managers.Ad.ShowInterstitialAd();
        }
        else
            return;

        //Managers.Game.GameRetry();
    }

    IEnumerator c_Delay()
    {
        Managers.Sound.Play(Define.Sound.Effect, "strike");
        strikeTMP.text = Managers.Localization.GetLocalizedValue(LanguageKey.strike.ToString());
        string hightScore = Managers.Localization.GetLocalizedValue(LanguageKey.highscore.ToString());
        string currentScore = Managers.Localization.GetLocalizedValue(LanguageKey.currentscore.ToString());
        gameScorTMP.text = $" {Managers.Localization.GetLocalizedValue(Managers.Game.League.ToString())}\n\n {hightScore} : {Managers.Game.GetBestScore()}\n {currentScore} : {Managers.Game.GameScore}";
        yield return new WaitForSeconds(0.7f);
        strikeTMP.gameObject.SetActive(false);
        gameScorTMP.gameObject.SetActive(true);
        homeButton.gameObject.SetActive(true);
    }

}
