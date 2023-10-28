using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;

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
        B_NextLeague,
        B_PrevLeague,
        B_Language,
        B_BatOption,
        B_Star,
        B_QuitGame
    }

    enum TMPS
    {
        StarTMP
    }

    enum Images
    {
        NotificationItem,
        NotificationReward,
        StartDrag,
    }

    private List<Image> images = new List<Image>();
    public TextMeshProUGUI leagueTMP;
    public TextMeshProUGUI dragTMP;
    Button nextLeague;
    Button prevLeague;

    private Button vibrationButton;
    private Button soundButton;

    private bool _isDrag = true;
    private bool _isAnim = false;
    Image NotifyItem;
    Image NotifyReward;

    private TextMeshProUGUI starTMP;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));
        BindImage(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(TMPS));

        GetButton((int)Buttons.B_Skin).gameObject.BindEvent(B_SkinClick);
        GetButton((int)Buttons.B_Challenge).gameObject.BindEvent(B_ChanllengeClick);
        GetButton((int)Buttons.B_Store).gameObject.BindEvent(B_StoreClick);
        GetButton((int)Buttons.B_Options).gameObject.BindEvent(B_OptionsClick);
        GetButton((int)Buttons.B_Star).gameObject.BindEvent(()=>Managers.UI.ShowPopupUI<UI_StarShop>());
        GetImage((int)Images.StartDrag).gameObject.BindEvent(null, OnDrag, Define.UIEvent.Drag);

        leagueTMP.text = Managers.Game.League.ToString();

        nextLeague = GetButton((int)Buttons.B_NextLeague);
        prevLeague = GetButton((int)Buttons.B_PrevLeague);

        nextLeague.gameObject.BindEvent(OnClickNextLeague);
        prevLeague.gameObject.BindEvent(OnClickPrevLeague);

        vibrationButton = GetButton((int)Buttons.B_Vibration);
        soundButton = GetButton((int)Buttons.B_Sound);

        // 옵션 애니메이션
        vibrationButton.gameObject.SetActive(false);
        soundButton.gameObject.SetActive(false);
        GetButton((int)Buttons.B_BatOption).gameObject.SetActive(false);
        GetButton((int)Buttons.B_Language).gameObject.SetActive(false);
        GetButton((int)Buttons.B_QuitGame).gameObject.SetActive(false);


        vibrationButton.transform.DOScale(0, 0);
        soundButton.transform.DOScale(0, 0);
        GetButton((int)Buttons.B_BatOption).transform.DOScale(0, 0);
        GetButton((int)Buttons.B_Language).transform.DOScale(0, 0);
        GetButton((int)Buttons.B_QuitGame).transform.DOScale(0, 0);
        // 끝

        vibrationButton.gameObject.BindEvent(VibrateOnOff);
        soundButton.gameObject.BindEvent(SoundOnOff);

        starTMP = Get<TextMeshProUGUI>((int)TMPS.StarTMP);
        starTMP.text = Managers.Game.PlayerInfo.star.ToString();



        NotifyItem = GetImage((int)Images.NotificationItem);
        NotifyReward = GetImage((int)Images.NotificationReward);

        NotifyItem.gameObject.SetActive(false);
        NotifyReward.gameObject.SetActive(false);

        StartCoroutine(co_GetAllImages());

        Managers.Game.SetNotifyItemAction(NotifyItemAnim);
        Managers.Game.SetNotifyRewardAction(NotifyItemAnim);
        Managers.Game.ChageBackgroundColor();

        GetButton((int)Buttons.B_Language).gameObject.BindEvent(() => Managers.UI.ShowPopupUI<UI_LanguageOptionPopup>());
        GetButton((int)Buttons.B_BatOption).gameObject.BindEvent(() => Managers.UI.ShowPopupUI<UI_BatOptionPopup>());
        GetButton((int)Buttons.B_QuitGame).gameObject.BindEvent(() => Application.Quit());


        MainTMPUpdate();

        Managers.Localization.SetLocalChangeUpdateTMP(MainTMPUpdate);


        Managers.Game.SetStarUpdate(() => starTMP.text = Managers.Game.PlayerInfo.star.ToString());

        return true;

    }

    private void MainTMPUpdate()
    {
        starTMP.text = Managers.Game.PlayerInfo.star.ToString();
        leagueTMP.text = Managers.Localization.GetLocalizedValue((Managers.Game.League).ToString().ToLower());
        dragTMP.text = Managers.Localization.GetLocalizedValue(LanguageKey.startdrag.ToString());
#if UNITY_EDITOR
        Debug.Log("TODO 드래그 텍스트");
#endif
    }

    public void NotifyItemAnim()
    {
        if (NotifyItem.gameObject.activeSelf == false)
            NotifyItem.gameObject.SetActive(true);

        NotifyItem.transform.DOScale(0.7f, 1f).OnComplete(() =>
        {
            NotifyItem.transform.DOScale(1f, 1f);
        }).SetLoops(-1, LoopType.Yoyo);
    }

    public void NotifyRewardAnim()
    {
        if (NotifyReward.gameObject.activeSelf == false)
            NotifyReward.gameObject.SetActive(true);

        NotifyReward.transform.DOScale(0.7f, 1f).OnComplete(() =>
        {
            NotifyReward.transform.DOScale(1f, 1f);
        }).SetLoops(-1, LoopType.Yoyo);
    }

    private void VibrateOnOff()
    {
        Managers.Vibration.ClickVibrateButton();
        UpdateOptionButton();
    }

    private void SoundOnOff()
    {
        Managers.Sound.ClickSoundButton();
        UpdateOptionButton();
    }

    private void UpdateOptionButton()
    {
        vibrationButton.interactable = Managers.Vibration.GetVibrate();
        soundButton.interactable = Managers.Sound.GetSound();
    }

    private void OnClickNextLeague()
    {
        float league = (int)Managers.Game.League + 1;
        league = Mathf.Clamp(league, 0, (int)Define.League.COUNT - 1);

        Managers.Game.SetLeague((Define.League)league);
        leagueTMP.text = Managers.Localization.GetLocalizedValue(((Define.League)league).ToString().ToLower());


        Managers.Game.ChageBackgroundColor();


    }

    private void OnClickPrevLeague()
    {


        float league = (int)Managers.Game.League - 1;
        league = Mathf.Clamp(league, 0, (int)Define.League.COUNT - 1);

        Managers.Game.SetLeague((Define.League)league);
        leagueTMP.text = Managers.Localization.GetLocalizedValue(((Define.League)league).ToString().ToLower());

        Managers.Game.ChageBackgroundColor();
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


    private bool isOptionAnim = false;
    private bool isOn = false;
    private void B_OptionsClick()
    {


        UpdateOptionButton();

        if (isOptionAnim == false && isOn == false)
        {
            isOptionAnim = true;
            isOn = true;
            // 이미 실행 중인 애니메이션 중지
            DOTween.Kill(vibrationButton.transform);
            DOTween.Kill(soundButton.transform);
            DOTween.Kill(GetButton((int)Buttons.B_Language).transform);
            DOTween.Kill(GetButton((int)Buttons.B_BatOption).transform);
            DOTween.Kill(GetButton((int)Buttons.B_QuitGame).transform);

            vibrationButton.gameObject.SetActive(true);
            soundButton.gameObject.SetActive(true);
            GetButton((int)Buttons.B_Language).gameObject.SetActive(true);
            GetButton((int)Buttons.B_BatOption).gameObject.SetActive(true);
            GetButton((int)Buttons.B_QuitGame).gameObject.SetActive(true);


            vibrationButton.transform.DOScale(Vector3.one, 0.5f);
            soundButton.transform.DOScale(Vector3.one, 0.5f).SetDelay(0.25f);
            GetButton((int)Buttons.B_Language).transform.DOScale(Vector3.one, 0.5f).SetDelay(0.5f);
            GetButton((int)Buttons.B_BatOption).transform.DOScale(Vector3.one, 0.5f).SetDelay(0.75f);
            GetButton((int)Buttons.B_QuitGame).transform.DOScale(Vector3.one, 0.5f).SetDelay(1f).OnComplete(()=> isOptionAnim = false);
        }
        if(isOptionAnim == false && isOn == true)
        {
            isOptionAnim = true;
            isOn = false;

            // 이미 실행 중인 애니메이션 중지
            DOTween.Kill(vibrationButton.transform);
            DOTween.Kill(soundButton.transform);
            DOTween.Kill(GetButton((int)Buttons.B_Language).transform);
            DOTween.Kill(GetButton((int)Buttons.B_BatOption).transform);

            vibrationButton.transform.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
            {
                
            });

            soundButton.transform.DOScale(Vector3.zero, 0.5f).SetDelay(0.25f).OnComplete(() =>
            {
                
            });

            GetButton((int)Buttons.B_Language).transform.DOScale(Vector3.zero, 0.5f).SetDelay(0.5f).OnComplete(() =>
            {
                
            });

            GetButton((int)Buttons.B_BatOption).transform.DOScale(Vector3.zero, 0.5f).SetDelay(0.75f).OnComplete(() =>
            {

            });
            GetButton((int)Buttons.B_QuitGame).transform.DOScale(Vector3.zero, 0.5f).SetDelay(1f).OnComplete(() =>
            {
                vibrationButton.gameObject.SetActive(false);
                soundButton.gameObject.SetActive(false);
                GetButton((int)Buttons.B_Language).gameObject.SetActive(false);
                GetButton((int)Buttons.B_BatOption).gameObject.SetActive(false);
                GetButton((int)Buttons.B_QuitGame).gameObject.SetActive(false);
                isOptionAnim = false;
            });
        }

    }
    public void OnDrag(BaseEventData eventData)
    {
        if (Managers.Game.GameState == Define.GameState.Home && _isDrag == false)
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
        Managers.Game.GameReady(Define.GameMode.Normal);
        yield break;
    }


    Color buttonColor(bool isActive)
    {
        if(isActive)
        {
            return new Color(252, 170, 75, 199);
        }
        else
        {
            return Color.gray;
        }
    }
    private void OnDestroy()
    {
        foreach (Transform child in transform)
        {
            child.DOKill();
        }

        Managers.Game.RemoveStarUpdate(() => starTMP.text = Managers.Game.PlayerInfo.star.ToString());
    }
}
