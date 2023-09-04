using DG.Tweening.Plugins.Core.PathCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Define;

public class GameManager
{
    GameState _gameState = GameState.Home;

    public Bat Bat { get { return _bat; } private set { _bat = value; } }
    private Bat _bat = null;

    public Vector2 BatPos { get { return _batPos; } private set { _batPos = value; } }
    private Vector2 _batPos;

    public StrikeZone StrikeZone { get { return _strikeZone; } private set { _strikeZone = value; } }
    private StrikeZone _strikeZone = null;

    public BatCollider BatCollider { get { return _batCollider; } private set { _batCollider = value; } }
    private BatCollider _batCollider = null;

    public BatPosition BatPosition { get { return _batPosition; } private set { _batPosition = value; } }
    private BatPosition _batPosition = BatPosition.Left;

    public Vector3 AimPoint;

    public Vector2 AimPointScreen
    {
        get
        {
            // 2. ���� ��ǥ�� ��ũ�� ��ǥ�� ��ȯ
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(AimPoint);
            return screenPosition;
        }
    }

    public Vector2 BatColiderPointScreen
    {
        get
        {
            // 2. ���� ��ǥ�� ��ũ�� ��ǥ�� ��ȯ
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(BatCollider.BatMid());
            return screenPosition;
        }
    }
    #region ���� UI
    private UI_DragPopup dragPopup = null;
    private UI_GameInfoPopup gameInfoPopup = null;
    #endregion

    public GameState GameState { get { return _gameState; } private set { _gameState = value; } }

    public float ReplaySlowMode { get { return 0.25f; } private set { } }


    public float Speed { get { return (_speed * 3600) * 0.001f; } private set { _speed = value; } }
    public float _speed = 0;

    public ThrowType ThrowType { get; private set; }
    public LineRenderer StrikePath { get; private set; }

    public float HitScore { get; private set; }
    public float GameScore { get; private set; }

    public delegate void UIDelegate();
    public UIDelegate UiEvents;

    public delegate void GameUIDelegate();
    public GameUIDelegate GameUiEvent;

    // ���� �Լ�
    public Action hutSwingCallBack;
    public Action hitCallBack;
    public Action moveBat;
    public Action movePosu;
    public Action batPositionSetting;
    public Action gameReplayObjectClear;
    // ���÷��� �Լ�
    public Action<LineRenderer> makeReplayBallPathEvent;
    public List<CustomReplayData> batMoveReplayData = new List<CustomReplayData>();
    public bool isReplay = false;
    public bool isRecord = false;


    // ���̵�
    public League League { get { return _league; } private set { _league = value; } }
    private League _league = League.SemiPro;

    // ȣũ����

    HawkeyeLevel hawkeyeLevel = HawkeyeLevel.ZERO;
    public HawkeyeLevel HawkeyeLevel { get { return hawkeyeLevel; } private set { hawkeyeLevel = value; } }

    // ���� Ÿ��
    private GameType _gameType = GameType.None;
    public GameType GameType { get { return _gameType; } private set { _gameType = value; } }

    public CameraManager MainCam { get; private set; }

    public ChacterController ChacterController { get; private set; }



    public void Init()
    {
        _path = Application.persistentDataPath + "/SaveData.json";
        _settingPath = Application.persistentDataPath + "/SettingData.json";

        LoadGameSaveFile();
        {
            _gameState = GameState.Home;
            Managers.UI.ShowPopupUI<UI_Main>();
            var go = Managers.Resource.Instantiate("InGround");
            go.transform.position = Vector3.zero;
            go.transform.rotation = Quaternion.identity;
        }

        GameScore = 0;
    }


    public void GameReady(GameType gameType, Action callBack = null)
    {
        if (GameState != GameState.Home)
            return;

        _gameType = gameType;
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

        Debug.Log($"{GameScore} ���� ����");

        GameState = GameState.Home;

        GameScore = 0;
        Debug.Log($"{GameScore} ���� ����");

        callBack?.Invoke();
        StateChangeEvent();
    }
    private void StateChangeEvent()
    {
        switch (GameState)
        {
            case GameState.Home:
                Managers.Object.DespawnAll();
                batMoveReplayData.Clear();
                MainCam.MoveOriginaPos();
                Managers.UI.ShowPopupUI<UI_Main>();
                _gameType = GameType.None;
                break;
            case GameState.Ready:
                batPositionSetting?.Invoke();
                Managers.UI.ShowPopupUI<UI_Timer>();
                Managers.Object.DespawnAll();
                break;
            case GameState.InGround:
                gameInfoPopup = Managers.UI.ShowPopupUI<UI_GameInfoPopup>();
                dragPopup = Managers.UI.ShowPopupUI<UI_DragPopup>();
                break;
            case GameState.End:
                Managers.UI.ClosePopupUI(dragPopup);
                Managers.UI.ClosePopupUI(gameInfoPopup);
                //Managers.UI.ShowPopupUI<UI_EndPopup>();
                Managers.UI.ShowPopupUI<UI_ReplayPopupTimer>();
                isRecord = false;
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

    public void ReplayReview()
    {
        if (isReplay)
            return;
        isReplay = true;
        //moveBat?.Invoke();
        movePosu?.Invoke();
        makeReplayBallPathEvent?.Invoke(StrikePath);
    }

    public void GameRetry(Action callBack = null)
    {
        GameState = GameState.Ready;
        callBack?.Invoke();
        StateChangeEvent();
    }

    #region ���ͼ���
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
            // ���� ���� ������ ���� ó�� (�� ��� ���� �� ��ȯ)
            HitScore = -1;
        }

        GameScore += HitScore;
        GameUiEvent?.Invoke();
    }

    public void SetBatPosition(BatPosition batPosition)
    {
        BatPosition = batPosition;
    }

    #endregion

    #region ������Ʈ ���ε�
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
    public void RemoveCallBack(Action callback)
    {
        hitCallBack -= callback;
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

    public void SetReplatMoveAction(Action movePos)
    {
        movePosu -= movePos;
        movePosu += movePos;
    }
    public void SetBatPositionSetting(Action setting)
    {
        batPositionSetting -= setting;
        batPositionSetting += setting;
    }

    public void SetReplayGameClearAction(Action clear)
    {
        gameReplayObjectClear -= clear;
        gameReplayObjectClear += clear;
    }

    public void ReplayData(List<CustomReplayData> moveList)
    {
        batMoveReplayData = moveList;
    }

    public void SetBatCollider(BatCollider batCollider)
    {
        _batCollider = batCollider;
    }
    #endregion


    #region Save & Load	
    public string _path;
    public string _settingPath;
    public GameDB GameDB { get { return _gameData; } private set { _gameData = value; } }
    private GameDB _gameData = new GameDB();
    public GameDB SaveData
    {
        get
        {
            return _gameData;
        }
        set
        {
            _gameData = value;
        }
    }

    public BallPath Baller { get; private set; }

    public void SaveGame()
    {
        string jsonStr = JsonConvert.SerializeObject(SaveData);
        File.WriteAllTextAsync(_path, jsonStr);
        Debug.Log($"Save Game Completed : {_path}");
    }

    public bool LoadGameSaveFile()
    {
        if (File.Exists(_path) == false)
        {

            string path = "TestData";
            TextAsset textAsset = Managers.Resource.Load<TextAsset>($"{path}");
            GameDB StartData = new GameDB();



            if (textAsset != null)
                StartData = JsonConvert.DeserializeObject<GameDB>(textAsset.text);

            {


                if (Managers.Resource.Bats["BAT_2"] is ItemScriptableObject so)
                {
                    // Test Data
                    GameItem startItem = new GameItem(so.id, so.name, so.name);
                    StartData.playerItem.Add(startItem.itemId, startItem);
#if UNITY_EDITOR
                    StartData.playerInfo.money = 100000;
#endif
                    StartData.playerInfo.level = 1;
                    StartData.playerInfo.equipBatId = startItem.itemId;
                }

            }


            _gameData = StartData;


            SaveGame();

            Debug.LogWarning("SaveFile is not Existed");
            return false;
        }

        string fileStr = File.ReadAllText(_path);
        GameDB data = JsonConvert.DeserializeObject<GameDB>(fileStr);
        if (data != null)
        {
            //_playerData = data;
            SaveData = data;

            Debug.Log(SaveData);

        }

        Debug.Log($"Save Game Loaded : {_path}");
        return true;
    }

    public void SetCharacter(ChacterController chacterController)
    {
        ChacterController = chacterController;
    }

    public void SetBaller(BallPath ballPath)
    {
        Baller = ballPath;
    }
    #endregion

}