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
    public Image SpeedSlider;

    public Transform starGroup;
    public GameObject starBG;

    public List<Image> starFills = new List<Image>();

    private void Start()
    {
        Bind();

        //thorwBallTypeTMP.gameObject.SetActive(false);
    }

    private void Bind()
    {
        Managers.Game.SetThrowBallEvent(UpdateUI);
        Managers.Game.SetGameUiEvent(UpdateGameUI);
        Managers.Game.SetHitCallBack(StarUpdate);

        UpdateGameUI();


        var color = Utils.GetColor(Managers.Game.League);

        ballSpeedTMP.text = "";
        thorwBallTypeTMP.text = "";


        starFills.Clear();

        starFills.Add(starBG.transform.GetChild(0).GetComponent<Image>());

        for (int i = 0; i < (int)Managers.Game.League; i++)
        {
            var star = Managers.Resource.Instantiate(starBG, starGroup);

            starFills.Add(star.transform.GetChild(0).GetComponent<Image>());
        }

        if(Managers.Game.League == Define.League.Master)
        {
            var star = Managers.Resource.Instantiate(starBG, starGroup);
            starFills.Add(star.transform.GetChild(0).GetComponent<Image>());
        }

        if(Managers.Game.GameMode == Define.GameMode.Challenge)
        {
            starGroup.gameObject.SetActive(false);
        }

        StarUpdate();
    }

    private void UpdateUI()
    {
        StartCoroutine(co_Update());
    }

    IEnumerator co_Update()
    {
        if (Managers.Game.GameState != Define.GameState.InGround)
            yield break;


        thorwBallTypeTMP.text = Managers.Game.ThrowType.ToString();

        string colorCode = Managers.Game.GetSpeedColorString();
        yield return null;
        ballSpeedTMP.text = $"<color=#{colorCode}> {Managers.Localization.GetLocalizedValue(Managers.Game.ThrowType.ToString())} : {Managers.Game.Speed.ToString("F2")}Km/s </color>";

        // HTML 색상 문자열을 Color 객체로 변환합니다.
        Color color;
        if (ColorUtility.TryParseHtmlString("#" + colorCode, out color))
        {
            SpeedSlider.fillAmount = Managers.Game.Speed / 300.0f;
            SpeedSlider.color = color;  // Color 객체를 사용하여 SpeedSlider.color 속성을 설정합니다.
        }
        else
        {
            Debug.LogWarning("Invalid color code: " + colorCode);
        }


    }



    private void StarUpdate()
    {
        if (starFills == null || starFills.Count == 0)
            return;

        int maxLeague = (int)Managers.Game.League;
        long gameScore = Managers.Game.GameScore;


        int maxCount = starFills.Count;


        for (int i = 0; i < maxCount; i++)
        {
            if (starFills[i] != null && starFills[i].fillAmount < 1)
            {
                starFills[i].fillAmount = (gameScore - Define.POINT * i) / (float)Define.POINT;
            }
        }
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
