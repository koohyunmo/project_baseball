using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static Define;
using static EPOOutline.TargetStateListener;

public class GameManager
{
    GameState _gameState = GameState.Home;

    public Bat Bat { get { return _bat; } private set { _bat = value; } }
    private Bat _bat = null;

    #region 게임 UI
    private UI_DragPopup dragPopup = null;
    private UI_GameInfoPopup gameInfoPopup = null;
    #endregion

    public GameState GameState { get { return _gameState; } private set { _gameState = value; } }

    public float Speed { get { return (_speed * 3600) *0.001f; } private set { _speed = value; } }
    public float _speed = 0;

    public ThrowType ThrowType { get; private set; }
    public LineRenderer StrikePath { get; private set; }

    
    public delegate void UIDelegate();
    public UIDelegate UiEvents;
    public Action hutSwingCallBack;
    public Action hitCallBack;
    public Action moveBat;
    public Action<LineRenderer> makeReplayEvent;
    public bool isReplay = false;


    public League League { get { return _league; } private set { _league = value; } }
    private League _league = League.SemiPro;

    public CameraManager MainCam { get; private set; }



    public void Init()
    {
        _gameState = GameState.Home;
        Managers.UI.ShowPopupUI<UI_MainTest>();
    }


    public void GameReady(Action callBack = null)
    {
        if (GameState != GameState.Home)
            return;


        GameState = GameState.Ready;

        callBack?.Invoke();
        StateChangeEvent();
    }


    public void GameStart(Action callBack = null)
    {
        if (GameState != GameState.Ready)
            return;

        GameState = GameState.InGround;

        callBack?.Invoke();
        StateChangeEvent();
    }

    public void GameEnd(Action callBack = null)
    {
        if (GameState != GameState.InGround)
            return;

        GameState = GameState.End;

        callBack?.Invoke();
        StateChangeEvent();
    }

    public void GoHome(Action callBack = null)
    {
        if (GameState != GameState.End)
            return;

        GameState = GameState.Home;

        callBack?.Invoke();
        StateChangeEvent();
    }
    private void StateChangeEvent()
    {
        switch (GameState)
        {
            case GameState.Home:
                MainCam.MoveOriginaPos();
                Managers.UI.ShowPopupUI<UI_MainTest>();
                break;
            case GameState.Ready:
                Managers.UI.ShowPopupUI<UI_Timer>();
                break;
            case GameState.InGround:
                gameInfoPopup = Managers.UI.ShowPopupUI<UI_GameInfoPopup>();
                dragPopup = Managers.UI.ShowPopupUI<UI_DragPopup>();
                break;
            case GameState.End:
                Managers.UI.ClosePopupUI(dragPopup);
                Managers.UI.ClosePopupUI(gameInfoPopup);
                Managers.UI.ShowPopupUI<UI_EndPopup>();
                break;
        }

    }

    public void ThorwBallEvent()
    {
        UiEvents?.Invoke();
    }

    public void StrikeEvent()
    {
        hutSwingCallBack?.Invoke();
    }

    public void HitEvent()
    {
        hitCallBack?.Invoke();
    }

    public void Replay()
    {
        if (isReplay)
            return;
        isReplay = true;
        //moveBat?.Invoke();
        makeReplayEvent?.Invoke(StrikePath);
    }

    #region 오브젝트 바인딩
    public void SetBat(Bat bat)
    {
        _bat = bat;
    }

    public void SetDragPopup(UI_DragPopup popup)
    {
        dragPopup = popup;
    }

    public void SetLeague(League lg)
    {
        _league = lg;
    }
    public void SetBallInfo(float speed, ThrowType throwType)
    {
        _speed = speed;
        ThrowType = throwType;
    }

    public void SetThrowBallEvent(UIDelegate uiEvent)
    {
        UiEvents -= uiEvent;
        UiEvents += uiEvent;
    }

    public void SetStrikeCallBack(Action callback)
    {
        hutSwingCallBack -= callback;
        hutSwingCallBack += callback;
    }

    public void SetHitCallBack(Action callback)
    {
        hitCallBack -= callback;
        hitCallBack += callback;
    }

    public void SerStrikePath(LineRenderer pathRenderer)
    {
       StrikePath = pathRenderer;
    }


    public void SetReplayGame(Action<LineRenderer> replayAc)
    {
        makeReplayEvent -= replayAc;
        makeReplayEvent += replayAc;
    }

    public void SetMainCamera(CameraManager mainCam)
    {
        MainCam = mainCam;
    }

    public void SetMoveBat(Action ac)
    {
        moveBat -= ac;
        moveBat += ac;
    }
    #endregion

}
