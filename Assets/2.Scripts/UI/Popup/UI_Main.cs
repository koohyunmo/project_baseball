using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Main : UI_Popup, IDragHandler
{
    enum Buttons
    {
        B_Options,
        B_Skin,
        B_Challenge,
        B_Store,
    }

    enum Images
    {
        Notification
    }

    private List<Image> images = new List<Image>();
    public TextMeshProUGUI leagueTMP;
    public Button nextLeague;
    public Button prevLeague;

    private bool _isDrag = false;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        _isDrag = true;
        BindButton(typeof(Buttons));
        BindImage(typeof(Images));

        GetButton((int)Buttons.B_Skin).gameObject.BindEvent(B_SkinClick);
        GetButton((int)Buttons.B_Challenge).gameObject.BindEvent(B_ChanllengeClick);
        GetButton((int)Buttons.B_Store).gameObject.BindEvent(B_StoreClick);
        leagueTMP.text = Managers.Game.League.ToString();

        nextLeague.gameObject.BindEvent(OnClickNextLeague);
        prevLeague.gameObject.BindEvent(OnClickPrevLeague);

        StartCoroutine(co_GetAllImages());

        return true;

    }

    private void OnClickNextLeague()
    {


        float league = (int)Managers.Game.League +1;
        league = Mathf.Clamp(league, 0, (int)Define.League.COUNT-1);

        Managers.Game.SetLeague((Define.League)league);
        leagueTMP.text = Managers.Game.League.ToString();
    }

    private void OnClickPrevLeague()
    {


        float league = (int)Managers.Game.League - 1;
        league = Mathf.Clamp(league, 0, (int)Define.League.COUNT - 1);

        Managers.Game.SetLeague((Define.League)league);
        leagueTMP.text = Managers.Game.League.ToString();
    }

    private void B_SkinClick()
    {
        Managers.UI.ShowPopupUI<UI_SkinPopup>();
    }
    private void B_ChanllengeClick()
    {
        Managers.UI.ShowPopupUI<UI_ChallengePopup>();
    }
    private void B_StoreClick()
    {
        Managers.UI.ShowPopupUI<UI_StorePopup>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(Managers.Game.GameState == Define.GameState.Home && _isDrag == false)
        {
            _isDrag = true;
            StartCoroutine(co_DoFade());
        }
    }


    IEnumerator co_GetAllImages()
    {
        

        for (int i = 0; i < transform.childCount; i++)
        {
            Image img = transform.GetChild(i).GetComponent<Image>();
            if (img != null) // null 체크 추가
            {
                images.Add(img);
            }
            yield return null;

        }

        Debug.Log(images.Count);
        _isDrag = false;
        yield break;
    }

    IEnumerator co_DoFade()
    {

        yield return new WaitForEndOfFrame();

        foreach (var item in images)
        {
            item.DOFade(0f, 0.5f);
            yield return null;
        }

        yield return new WaitForSeconds(0.51f);
        Managers.UI.ClosePopupUI(this);
        Managers.Game.GameReady();
        yield break;
    }
}
