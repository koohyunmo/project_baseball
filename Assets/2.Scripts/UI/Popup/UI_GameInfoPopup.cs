using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_GameInfoPopup : UI_Popup
{

    public TextMeshProUGUI ballSpeedTMP;
    public TextMeshProUGUI thorwBallTypeTMP;


    private void Start()
    {
        Managers.Game.SetThrowBallEvent(UpdateUI);
    }

    private void UpdateUI()
    {
        if (Managers.Game.GameState != Define.GameState.InGround)
            return;

        ballSpeedTMP.text = Managers.Game.Speed.ToString("F2");
        thorwBallTypeTMP.text = Managers.Game.ThrowType.ToString();
    }

}
