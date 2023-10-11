using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Burst.Intrinsics.X86.Avx;

public class UI_GameInfoPopup : UI_Popup
{

    public TextMeshProUGUI ballSpeedTMP;
    public TextMeshProUGUI thorwBallTypeTMP;
    public TextMeshProUGUI gameScoreTMP;

    public Image Bilboard;

    private void Start()
    {
        Managers.Game.SetThrowBallEvent(UpdateUI);
        Managers.Game.SetGameUiEvent(UpdateGameUI);

        UpdateGameUI();


        var color = Utils.GetColor(Managers.Game.League);

        ballSpeedTMP.text = "";
        thorwBallTypeTMP.text = "";

        //thorwBallTypeTMP.gameObject.SetActive(false);
    }

    private void UpdateUI()
    {
        if (Managers.Game.GameState != Define.GameState.InGround)
            return;


        thorwBallTypeTMP.text = Managers.Game.ThrowType.ToString();

        string colorCode = Managers.Game.GetSpeedColorString();
        ballSpeedTMP.text = $"<color=#{colorCode}> {Managers.Game.Speed.ToString("F2")}Km/s </color>";

    }

    private void UpdateGameUI()
    {

        if(gameScoreTMP == null)
        {
            Debug.LogWarning("Score TMP is NULL");
            return;
        }

        if (Managers.Game.GameMode != Define.GameMode.Challenge)
            gameScoreTMP.text = Managers.Game.GameScore.ToString();
        else
        {
            switch (Managers.Game.ChallengeMode)
            {
                case Define.ChallengeType.ScoreMode:
                    gameScoreTMP.text = $"{Managers.Game.GameScore} / {Managers.Game.ChallengeScore}";
                    break;
                case Define.ChallengeType.HomeRunMode:
                    gameScoreTMP.text = $"{Managers.Game.HomeRunCount} / {Managers.Game.ChallengeScore}";
                    break;
                case Define.ChallengeType.RealMode:
                    gameScoreTMP.text = $"{Managers.Game.SwingCount} / {Managers.Game.ChallengeScore}";
                    break;
 
            }
        }

        gameScoreTMP.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.4f);
        gameScoreTMP.DOFade(0, 0.4f).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            gameScoreTMP.DOFade(1, 0.2f);
        });
    }

    private void OnDestroy()
    {
        gameScoreTMP.transform.DOKill();
        Managers.Game.RemoveThrowBallEvent(UpdateUI);
        Managers.Game.RemoveGameUiEvent(UpdateGameUI);
    }

}
