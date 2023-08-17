using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_MainTest : UI_Popup
{
    public Button startButton;
    public Button majorButton;
    public Button minorButton;
    public Button proButton;
    public Button semiProButton;
    public TextMeshProUGUI stateTMP;
    public TextMeshProUGUI leagueTMP;
    void Start()
    {
        StartCoroutine(co_UiUpdate());

        startButton.gameObject.BindEvent(ClickStartButton);

        majorButton.gameObject.BindEvent(OnClickMajorLeague);
        minorButton.gameObject.BindEvent(OnClickMinorLeague);
        proButton.gameObject.BindEvent(OnClickProLeague);
        semiProButton.gameObject.BindEvent(OnClickSemiProLeague);

        leagueTMP.text = Managers.Game.League.ToString();
    }

    private void ClickStartButton()
    {
        Managers.UI.ClosePopupUI(this);
        Managers.Game.GameReady();
    }

    private void OnClickMajorLeague()
    {
        Managers.Game.SetLeague(Define.League.Major);
        leagueTMP.text = Managers.Game.League.ToString();
    }

    private void OnClickMinorLeague()
    {
        Managers.Game.SetLeague(Define.League.Mainor);
        leagueTMP.text = Managers.Game.League.ToString();
    }

    private void OnClickProLeague()
    {
        Managers.Game.SetLeague(Define.League.SemiPro);
        leagueTMP.text = Managers.Game.League.ToString();
    }

    private void OnClickSemiProLeague()
    {
        Managers.Game.SetLeague(Define.League.Amateur);
        leagueTMP.text = Managers.Game.League.ToString();
    }

    IEnumerator co_UiUpdate()
    {
        while (true)
        {
            yield return null;
            stateTMP.text = $" State : {Managers.Game.GameState}";
        }
    }


}
