using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_ChallengePopup : UI_Popup
{

    enum Buttons
    {
        B_Back,
        StartButton,
        Close
    }

    enum GameObejcts
    {
        Content,
    }

    Transform _grid;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));
        BindObject(typeof(GameObejcts));

        GetButton((int)Buttons.B_Back).gameObject.BindEvent(B_BackClick);
        _grid = GetObject((int)GameObejcts.Content).transform;

        Clear();
        MakeItme();
        return true;
    }

    private void B_BackClick()
    {
        Managers.UI.ClosePopupUI(this);
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

}
