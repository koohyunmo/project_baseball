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


        Clear();
        MakeItme();
        UpdateUI();

        return true;
    }

    private void B_BackClick()
    {
        Managers.UI.ClosePopupUI(this);
    }

    private void UpdateUI()
    {
        Get<Slider>((int)Sliders.Slider).value = Managers.Game.GameDB.challengeClearCount / (float)Managers.Game.ChallengeCount;
        Get<TextMeshProUGUI>((int)TMPs.SliderTMP).text = $"Challenges {Managers.Game.GameDB.challengeClearCount} / {Managers.Game.ChallengeCount}";

    }

    private void Clear()
    {
        // Clear
        foreach (Transform child in _grid.transform)
            Managers.Resource.Destroy(child.gameObject);
    }

    private void MakeItme()
    {

        foreach (var itemID in Managers.Resource.Resources.Keys)
        {
            // "BAT_" 문자열을 포함하지 않는 경우, 다음 반복으로 건너뜁니다.
            if (!itemID.Contains("CSO_"))
                continue;

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
