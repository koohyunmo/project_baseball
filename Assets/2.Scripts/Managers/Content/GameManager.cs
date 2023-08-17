using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

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

    
    public delegate void UIDelegate();
    public UIDelegate UiEvents;

    public League League { get { return _league; } private set { _league = value; } }
    private League _league = League.SemiPro;



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
        StateChangEvent();
    }


    public void GameStart(Action callBack = null)
    {
        if (GameState != GameState.Ready)
            return;

        GameState = GameState.InGround;

        callBack?.Invoke();
        StateChangEvent();
    }

    public void GameEnd(Action callBack = null)
    {
        if (GameState != GameState.InGround)
            return;

        GameState = GameState.End;

        callBack?.Invoke();
        StateChangEvent();
    }

    public void GoHome(Action callBack = null)
    {
        if (GameState != GameState.End)
            return;

        GameState = GameState.Home;

        callBack?.Invoke();
        StateChangEvent();

    }
    private void StateChangEvent()
    {
        switch (GameState)
        {
            case GameState.Home:
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

    public void ThorwBall()
    {
        UiEvents?.Invoke();
    }

    public void SetLeague(League lg)
    {
        _league = lg;
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

    #endregion

}
