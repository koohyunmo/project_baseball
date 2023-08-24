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

    public Vector2 BatPos { get { return _batPos; } private set { _batPos = value; } }
    private Vector2 _batPos;

    public StrikeZone StrikeZone { get { return _strikeZone; } private set { _strikeZone = value; } }
    private StrikeZone _strikeZone = null;

    public Vector3 AimPoint;
    #region 게임 UI
    private UI_DragPopup dragPopup = null;
    private UI_GameInfoPopup gameInfoPopup = null;
    #endregion

    public GameState GameState { get { return _gameState; } private set { _gameState = value; } }

    public float ReplaySlowMode { get { return 0.25f; } private set { } }


    public float Speed { get { return (_speed * 3600) *0.001f; } private set { _speed = value; } }
    public float _speed = 0;

    public ThrowType ThrowType { get; private set; }
    public LineRenderer StrikePath { get; private set; }

    public float HitScore { get; private set; }
    public float GameScore { get; private set; }
    
    public delegate void UIDelegate();
    public UIDelegate UiEvents;

    public delegate void GameUIDelegate();
    public GameUIDelegate GameUiEvent;

    public Action hutSwingCallBack;
    public Action hitCallBack;
    public Action moveBat;
    public Action movePosu;
    public Action<LineRenderer> makeReplayBallPathEvent;
    public bool isReplay = false;

    List<GameObject> gameObjects = new List<GameObject>();


    public League League { get { return _league; } private set { _league = value; } }
    private League _league = League.SemiPro;

    public CameraManager MainCam { get; private set; }



    public void Init()
    {
        _gameState = GameState.Home;
        Managers.UI.ShowPopupUI<UI_Main>();
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
                Managers.UI.ShowPopupUI<UI_Main>();
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
        movePosu?.Invoke();
        makeReplayBallPathEvent?.Invoke(StrikePath);
    }

    #region 게터세터
    public void GetGameScore(float score)
    {
        if (score < 85)
        {
            HitScore = 1;
        }
        else if (score >= 85 && score < 95)
        {
            HitScore = 3;
        }
        else if (score >= 95 && score < 98)
        {
            HitScore = 5;
        }
        else if (score >= 98 && score <= 100)
        {
            HitScore = 10;
        }
        else
        {
            // 범위 밖의 점수에 대한 처리 (이 경우 에러 값 반환)
            HitScore = -1;
        }

        GameScore += HitScore;
        GameUiEvent?.Invoke();
    }


    #endregion

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

    public void SetStrikePath(LineRenderer pathRenderer)
    {
       StrikePath = pathRenderer;
    }


    public void SetBallPath(Action<LineRenderer> replayAc)
    {
        makeReplayBallPathEvent -= replayAc;
        makeReplayBallPathEvent += replayAc;
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

    public void SetGameUiEvent(GameUIDelegate evt)
    {
        GameUiEvent -= evt;
        GameUiEvent += evt;
    }

    public void SetBatPos(Vector2 pos)
    {
        _batPos = pos;
    }

    public void SetStrikeZone(StrikeZone zone)
    {
        _strikeZone = zone;
    }

    public void SetMovePosu(Action movePos)
    {
        movePosu -= movePos;
        movePosu += movePos;
    }
    #endregion

}
