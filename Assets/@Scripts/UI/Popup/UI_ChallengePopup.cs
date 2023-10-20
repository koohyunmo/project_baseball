using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_ChallengePopup : UI_ContentPopup
{

    enum Buttons
    {
        B_Back,
    }

    enum GameObejcts
    {
        Content,
    }

    enum Sliders
    {
        Slider
    }

    enum TMPs
    {
        SliderTMP
    }
    Transform _grid;

    private ChallengeProc _challengeProc = ChallengeProc.None;

    private void Start()
    {
        Init();
    }


    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));
        BindObject(typeof(GameObejcts));
        Bind<Slider>(typeof(Sliders));
        Bind<TextMeshProUGUI>(typeof(TMPs));

        GetButton((int)Buttons.B_Back).gameObject.BindEvent(B_BackClick);
        _grid = GetObject((int)GameObejcts.Content).transform;

        Managers.Game.SetLobbyUIUpdate(UpdateUI);
        // 챌린지 보상
        Get<Slider>((int)Sliders.Slider).gameObject.BindEvent(Managers.Game.GetCSOReward);

        Clear();
        MakeItme();
        UpdateUI();

        return true;
    }

    private void B_BackClick()
    {
        ClosePopupUI();
    }

    private void UpdateUI()
    {
        var clearCount = Mathf.Clamp(Managers.Game.GameDB.challengeClearCount, 0, Managers.Game.GameDB.challengeData.Count);

        Get<Slider>((int)Sliders.Slider).value = clearCount / (float)Managers.Game.GameDB.challengeData.Count;
        Get<TextMeshProUGUI>((int)TMPs.SliderTMP).text = $"{Managers.Localization.GetLocalizedValue(LanguageKey.challenges.ToString())} {clearCount} / {Managers.Game.GameDB.challengeData.Count}";


        if(Get<Slider>((int)Sliders.Slider).value >= 1 && Managers.Game.PlayerInfo.csoAllRewawrd == false)
        {
            Get<TextMeshProUGUI>((int)TMPs.SliderTMP).text = " 보상을 받으세요 ";          
        }

    }

    private void Clear()
    {
        // Clear
        foreach (Transform child in _grid.transform)
            Managers.Resource.Destroy(child.gameObject);
    }

    private void MakeItme()
    {

        foreach (var itemID in Managers.Resource.challengeOrderList)
        {
            var item = Managers.Resource.Instantiate("UI_ChallengeItem", _grid.transform);
            if (item == null)
            {
                Debug.Log("item is null");
                continue;
            }
            UI_ChallengeItem skinItem = item.GetOrAddComponent<UI_ChallengeItem>();
            skinItem.InitData(itemID,null);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Managers.Game.RemoveLobbyUIUpdate(UpdateUI);
    }


    internal void InitData(ChallengeProc fail)
    {
        _challengeProc = fail;
    }
}
