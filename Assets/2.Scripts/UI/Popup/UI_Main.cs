using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Main : UI_Popup
{
    enum Buttons
    {
        B_Options,
        B_Sound,
        B_Vibration,
        B_Skin,
        B_Challenge,
        B_Store,
    }

    enum Images
    {
        Notification,
        StartDrag,
    }

    private List<Image> images = new List<Image>();
    public TextMeshProUGUI leagueTMP;
    public Button nextLeague;
    public Button prevLeague;

    private Button vibrationButton;
    private Button soundButton;

    private bool _isDrag = true;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));
        BindImage(typeof(Images));

        GetButton((int)Buttons.B_Skin).gameObject.BindEvent(B_SkinClick);
        GetButton((int)Buttons.B_Challenge).gameObject.BindEvent(B_ChanllengeClick);
        GetButton((int)Buttons.B_Store).gameObject.BindEvent(B_StoreClick);
        leagueTMP.text = Managers.Game.League.ToString();

        nextLeague.gameObject.BindEvent(OnClickNextLeague);
        prevLeague.gameObject.BindEvent(OnClickPrevLeague);

        vibrationButton = GetButton((int)Buttons.B_Vibration);
        soundButton = GetButton((int)Buttons.B_Sound);

        vibrationButton.gameObject.SetActive(false);
        soundButton.gameObject.SetActive(false);

        vibrationButton.transform.DOScale(0, 0);
        soundButton.transform.DOScale(0, 0);

        GetButton((int)Buttons.B_Options).gameObject.BindEvent(B_OptionsClick);

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

    private void B_OptionsClick()
    {
        if(vibrationButton.gameObject.activeInHierarchy == false)
        {

            // 이미 실행 중인 애니메이션 중지
            DOTween.Kill(vibrationButton.transform);
            DOTween.Kill(soundButton.transform);

            vibrationButton.gameObject.SetActive(true);
            soundButton.gameObject.SetActive(true);

            vibrationButton.transform.DOScale(1, 0.5f);
            soundButton.transform.DOScale(1, 0.5f).SetDelay(0.25f);
        }
        else
        {

            // 이미 실행 중인 애니메이션 중지
            DOTween.Kill(vibrationButton.transform);
            DOTween.Kill(soundButton.transform);

            vibrationButton.transform.DOScale(0, 0.5f).OnComplete(() =>
            {
                vibrationButton.gameObject.SetActive(false);
            }).SetDelay(0.25f);

            soundButton.transform.DOScale(0, 0.5f).OnComplete(() =>
            {
                soundButton.gameObject.SetActive(false);
            });
        }
        
    }
    public void OnDrag(PointerEventData eventData)
    {
        if(Managers.Game.GameState == Define.GameState.Home && _isDrag == false)
        {
            _isDrag = true;
            StartCoroutine(co_DoFade());
        }
    }

    public void DoStart()
    {
        StartCoroutine(co_DoFade());
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
