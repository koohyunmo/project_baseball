using DG.Tweening.Plugins.Core.PathCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using static Define;
using static Keys;


using Debug = UnityEngine.Debug;
public class GameManager
{
    GameState _gameState = GameState.Home;


    public Stopwatch StopWatch { get; set; } = new Stopwatch();

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
            // 2. 월드 좌표를 스크린 좌표로 변환
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(AimPoint);
            return screenPosition;
        }
    }

    public Vector2 BatColiderPointScreen
    {
        get
        {
            // 2. 월드 좌표를 스크린 좌표로 변환
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(BatCollider.BatMid());
            return screenPosition;
        }
    }

    public GameState GameState { get { return _gameState; } private set { _gameState = value; } }

    public float ReplaySlowMode { get { return 0.25f; } private set { } }


    public float Speed { get { return (_speed * 3600) * 0.001f; } private set { _speed = value; } }
    public float _speed = 0;

    public ThrowType ThrowType { get; private set; }
    public LineRenderer StrikePath { get; private set; }

    public float HitScore { get; private set; }
    public Vector3 HitPos { get; private set; } = Vector3.zero;
    public float GameScore { get; private set; }

    public delegate void UIDelegate();
    public UIDelegate UiEvents;

    public delegate void GameUIDelegate();
    public GameUIDelegate GameUiEvent;

    public delegate void LobbyUIDelegate();
    public LobbyUIDelegate LobbyUIEvent;


    // 게임 함수
    public Action hutSwingCallBack;
    public Action hitCallBack;
    public Action moveBat;
    public Action movePosu;
    public Action batPositionSetting;
    public Action gameReplayObjectClear;
    // 리플레이 함수
    public Action<LineRenderer> makeReplayBallPathEvent;
    public List<CustomReplayData> batMoveReplayData = new List<CustomReplayData>();
    public bool isReplay = false;
    public bool isRecord = false;

    //첼린지 모드
    public int ChallengeScore;
    public int HomeRunCount;
    public int SwingCount;

    // 알람
    public Action notifyItemAction;
    public Action notifyRewardAction;
    public Action EquipItemAction;

    // 난이도
    public League League { get { return _league; } private set { _league = value; } }
    private League _league = League.Bronze;

    // 호크아이
    public bool HawkEyes { get; private set; }
    HawkeyeLevel hawkeyeLevel = HawkeyeLevel.ZERO;
    public HawkeyeLevel HawkeyeLevel { get { return hawkeyeLevel; } private set { hawkeyeLevel = value; } }

    public float HawkEyesAmount
    {
        get
        {
            return (100 - (int)hawkeyeLevel) * 0.01f;
        }
    }


    // 게임 타입
    private GameMode _gameMode = GameMode.None;
    public GameMode GameMode { get { return _gameMode; } private set { _gameMode = value; } }

    public CameraManager MainCam { get; private set; }

    public ChacterController ChacterController { get; private set; }

    public ChallengeType ChallengeMode { get; private set; } = ChallengeType.RealMode;
    public string ChallengeGameID { get; private set; }

    public int BatCount { get; private set; }
    public int BallCount { get; private set; }
    public int SkillCount { get; private set; }
    public int ChallengeCount { get; private set; }

    public async void Init()
    {
        _path = Application.persistentDataPath + "/SaveData.json";
        _settingPath = Application.persistentDataPath + "/SettingData.json";

        LoadGameSaveFile();
        {
            _gameState = GameState.Home;
            //Managers.UI.ShowPopupUI<UI_Main>();
            var go = Managers.Resource.Instantiate(IN_GAME_OBJ_KEY.InGround.ToString());
            go.transform.position = Vector3.zero;
            go.transform.rotation = Quaternion.identity;

            EquipBallId = _gameData.playerInfo.equipBallId;
            EquipBatId = _gameData.playerInfo.equipBatId;

            LoadItemCount();
        }

        GameScore = 0;
        StateChangeEvent();
    }

    public async void LoadItemCount()
    {
        BatCount = await Managers.Resource.ObjectGetAsyncCount("Bat");
        BallCount = await Managers.Resource.ObjectGetAsyncCount("Ball");
        SkillCount = await Managers.Resource.ObjectGetAsyncCount("Skill");
        ChallengeCount = await Managers.Resource.ObjectGetAsyncCount("Challenge");
    }


    public void GameReady(GameMode gameType, Action callBack = null)
    {
        if (GameState != GameState.Home)
            return;

        _gameMode = gameType;
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

    public void GameEndCallback(Action callBack = null)
    {
        if (GameState != GameState.InGround)
            return;

        GameState = GameState.End;

        callBack?.Invoke();
        StateChangeEvent();
    }

    public void GameEnd()
    {
        if (GameState != GameState.InGround)
            return;

        GameState = GameState.End;
        StateChangeEvent();
    }

    


    public void GoHome(Action callBack = null)
    {
        if (GameState != GameState.End)
            return;

        Debug.Log($"{GameScore} 게임 저장");

        GameState = GameState.Home;

        GameScore = 0;
        Debug.Log($"{GameScore} 게임 리셋");

        
        StateChangeEvent();
        callBack?.Invoke();
    }
    private void StateChangeEvent()
    {
        switch (GameState)
        {
            case GameState.Home:
                {
                    if(_bat)
                        _bat.BatOff();

                    Managers.Obj.DespawnAll();
                    batMoveReplayData.Clear();
                    MainCam.MoveOriginaPos();
                    Managers.UI.ShowPopupUI<UI_Main>();

                    if (isChallengeClear == ChallengeProc.Complete || isChallengeClear == ChallengeProc.Fail)
                        Managers.UI.ShowPopupUI<UI_ChallengePopup>();


                    _gameMode = GameMode.None;
                    ScoreAndCountClear();


                }
                break;
            case GameState.Ready:
                {
                    if (_bat)
                        _bat.ColiderOn();

                    MainCam.MoveOriginaPos();
                    HitScore = 0;
                    batPositionSetting?.Invoke();
                    Managers.UI.ShowPopupUI<UI_Timer>();
                    Managers.Obj.DespawnAll();
                    {
                        var skillSo = Managers.Resource.GetScriptableObjet<SkillScriptableObject>(Managers.Game.EquipSkillId);
                        hawkeyeLevel = skillSo.HawkeyeLevel;

                        if (hawkeyeLevel == HawkeyeLevel.ZERO)
                            HawkEyes = false;
                        else
                            HawkEyes = true;
                    }
                }
                break;
            case GameState.InGround:
                {
                    if (_bat)
                        _bat.BatOn();
                    Managers.UI.ShowPopupUI<UI_GameInfoPopup>();
                    Managers.UI.ShowPopupUI<UI_DragPopup>();
                }
                break;
            case GameState.End:
                {

                    if (_bat)
                        _bat.ColiderOff();
                    Managers.UI.CloseAllPopupUIAndCall(() =>
                    {
                        if (isChallengeClear == ChallengeProc.None)
                            Managers.UI.ShowPopupUI<UI_ReplayPopupTimer>();
                        else
                        {
                            if (_gameData.challengeData[ChallengeGameID] == false)
                            {
                                _gameData.challengeData[ChallengeGameID] = true;
                                _gameData.challengeClearCount += 1;
                                SaveGame();
                            }

                            _gameMode = GameMode.None;
                            if(isChallengeClear == ChallengeProc.Complete)
                                Managers.UI.ShowPopupUI<UI_ChallengeClearPopup>();
                            else if(isChallengeClear == ChallengeProc.Fail)
                                Managers.UI.ShowPopupUI<UI_ChallengeClearPopup>().Failed();
                        }
                           

                        isRecord = false;

                    });


                }
                break;
        }

    }

    private void ScoreAndCountClear()
    {
        HitScore = 0;
        GameScore = 0;
        SwingCount = 0;
        HomeRunCount = 0;
        ChallengeScore = 0;
        ChallengeGameID = "";
        isChallengeClear = ChallengeProc.None;
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
        ChallengeModeCheck();
    }

    #region 챌린지

    private async void ManagerAsyncFunction(Action action, float timer)
    {
        timer *= 1000;

        await Task.Delay((int)timer);  // 0.5초 대기
        action?.Invoke();
    }
    #endregion

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
        MainCam.OffRePlay();
        StateChangeEvent();
    }

    #region 게터세터
    public void GetGameScoreAndGetPosition(float score, Vector3 hitPos)
    {
        HitPos = hitPos;

        if (score < 50 && score > 0)
        {
            HitScore = 1;
            SwingCount++;
        }
        else if (score >= 51 && score < 85)
        {
            HitScore += 1;
            SwingCount++;
        }
        else if (score >= 85 && score < 95)
        {
            HitScore += 2;
            SwingCount++;
        }
        else if (score >= 95 && score < 100)
        {
            HitScore += 4;
            SwingCount++;
            HomeRunCount++;
        }
        else if (score >= 100 && score <= float.MaxValue)
        {
            HitScore += 10;
            SwingCount++;
            HomeRunCount++;
        }

        GameScore += HitScore;
        GameUiEvent?.Invoke();

        //ChallengeModeCheck();
    }

    public void SetBatPosition(BatPosition batPosition)
    {
        BatPosition = batPosition;
    }

    public void MainBatOff()
    {
        if (_bat)
            _bat.gameObject.SetActive(false);
    }

    public void MainBatOn()
    {
        if(_bat)
            _bat.gameObject.SetActive(true);
    }


    public void SetChallengeMode(int score, ChallengeType mode, string csoKey)
    {
        ChallengeScore = score;
        ChallengeMode = mode;
        ChallengeGameID = csoKey;
    }

    public void GetItem(string key)
    {

        if (_gameData.playerInventory.Contains(key) == true)
            return;

        var type = Managers.Resource.GetScriptableObjet<ItemScriptableObject>(key).type;

        switch (type)
        {

            case ItemType.BALL:
                _gameData.playerBalls.Add(key);
                break;
            case ItemType.BAT:
                _gameData.playerBats.Add(key);
                break;
            case ItemType.SKILL:
                _gameData.playerSkills.Add(key);
                break;
        }

        _gameData.playerInventory.Add(key);
        SaveGame();
        Debug.Log("TODO");
        notifyItemAction();

        LobbyUIEvent?.Invoke();
    }

    #endregion

    #region 오브젝트 바인딩
    public void SetBat(Bat bat)
    {
        _bat = bat;
    }

    public void SetDragPopup(UI_DragPopup popup)
    {
        //dragPopup = popup;
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


    #region 첼린지 모드 체크
    private ChallengeProc isChallengeClear = ChallengeProc.None;
    private void ChallengeModeCheck()
    {

        if (GameMode != GameMode.Challenge)
            return;

        switch (ChallengeMode)
        {
            case ChallengeType.Score:
                if (GameScore == ChallengeScore)
                {
                    isChallengeClear = ChallengeProc.Complete;
                    ManagerAsyncFunction(GameEnd, 0.5f);
                }
                else if (GameScore > ChallengeScore)
                {
                    isChallengeClear = ChallengeProc.Fail;
                    ManagerAsyncFunction(GameEnd, 0.5f);
                }
                return;
            case ChallengeType.HomeRun:
                if (HomeRunCount >= ChallengeScore)
                {
                    isChallengeClear = ChallengeProc.Complete; 
                    ManagerAsyncFunction(GameEnd, 0.5f);
                }
                return;
            case ChallengeType.RealMode:
                if (SwingCount >= ChallengeScore)
                {
                    isChallengeClear = ChallengeProc.Complete; 
                    ManagerAsyncFunction(GameEnd, 0.5f);
                }
                return;
        }

    }
    #endregion

    #region Save & Load	
    public string _path;
    public string _settingPath;
    public GameDB GameDB { get { return _gameData; } private set { _gameData = value; } }
    private GameDB _gameData = new GameDB();




    public PlayerInfo PlayerInfo { get { return _gameData.playerInfo; } private set { _gameData.playerInfo = value; } }
    public string EquipBallId { get { return _gameData.playerInfo.equipBallId; } private set { _gameData.playerInfo.equipBallId = value; } }
    public string EquipBatId { get { return _gameData.playerInfo.equipBatId; } private set { _gameData.playerInfo.equipBatId = value; } }
    public string EquipSkillId { get { return _gameData.playerInfo.equipSkillId; } private set { _gameData.playerInfo.equipSkillId = value; } }
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

    public Baller Baller { get; private set; }

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


                if (Managers.Resource.Resources[BAT_KEY.BAT_2.ToString()] is ItemScriptableObject so)
                {
                    // Test Data
                    PlayerItem startItem = new PlayerItem(so.id, so.name, so.name, ItemType.BAT);
                    //StartData.playerItem.Add(startItem.itemId, startItem);
#if UNITY_EDITOR
                    StartData.playerInfo.money = 100000;
#endif
                    StartData.playerInfo.level = 1;
                    StartData.playerInfo.equipBatId = BAT_KEY.BAT_2.ToString();
                    StartData.playerInventory.Add(startItem.itemId);
                    StartData.playerBats.Add(startItem.itemId);
                }

                if (Managers.Resource.Resources[BALL_KEY.BALL_1.ToString()] is ItemScriptableObject ball)
                {
                    // Test Data
                    PlayerItem startItem = new PlayerItem(ball.id, ball.name, ball.name, ItemType.BALL);
                    //StartData.playerItem.Add(startItem.itemId, startItem);

                    StartData.playerInfo.level = 1;
                    StartData.playerInfo.equipBallId = BALL_KEY.BALL_1.ToString();

                    StartData.playerInventory.Add(startItem.itemId);
                    StartData.playerBalls.Add(startItem.itemId);
                }

                if (Managers.Resource.Resources[HAWK_EYE_KEY.SKILL_0.ToString()] is ItemScriptableObject so2)
                {
                    // Test Data
                    PlayerItem startItem = new PlayerItem(so2.id, so2.name, so2.name, ItemType.SKILL);
                    StartData.playerInfo.equipSkillId = HAWK_EYE_KEY.SKILL_0.ToString();
                    StartData.playerInventory.Add(startItem.itemId);
                    StartData.playerSkills.Add(startItem.itemId);
                }


            }

            Dictionary<string, bool> challengeData = new Dictionary<string, bool>();

            foreach (var item in Managers.Resource.Resources)
            {
                if (item.Key.Contains("CSO_"))
                {
                    challengeData.Add(item.Key, false);
                }

            }


            _gameData = StartData;
            _gameData.challengeData = challengeData;


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

    public void SetBaller(Baller ballPath)
    {
        Baller = ballPath;
    }


    public void ChangeItem(string key)
    {
        if (key.Contains("BALL_"))
        {
            ChangeBall(key);
        }
        else if (key.Contains("BAT_"))
        {
            ChangeBat(key);
        }
        else if (key.Contains("SKILL_"))
        {
            ChangeSkill(key);
        }
        else
        {
            Debug.LogError($"{key} is not item");
        }
    }

    public void ChangeBall(string key)
    {
        if (EquipBallId.Equals(key) == false)
            EquipBallId = key;
        else
            return;

        SaveGame();
    }

    public void ChangeBat(string key)
    {
        if (EquipBatId.Equals(key) == false)
            EquipBatId = key;
        else
            return;

        SaveGame();
    }

    public void ChangeSkill(string key)
    {
        if (EquipSkillId.Equals(key) == false)
            EquipSkillId = key;
        else
            return;

        SaveGame();
    }


    public void SetNotifyItemAction(Action notifyItemAnim)
    {
        notifyItemAction = notifyItemAnim;
    }

    public void SetNotifyRewardAction(Action notifyItemAnim)
    {
        notifyRewardAction = notifyItemAnim;
    }

    public void SetEquipUIItemAction(Action choiceUIUpdate)
    {
        EquipItemAction -= choiceUIUpdate;
        EquipItemAction += choiceUIUpdate;
    }

    public void RemoveEqupUIItemAction(Action choiceUIUpdate)
    {
        EquipItemAction -= choiceUIUpdate;
    }

    public void ChageBackgroundColor()
    {

        Color color = Color.white;
        switch (League)
        {
            case League.Bronze:
                color = Bronze;
                break;
            case League.Silver:
                color = Silver;
                break;
            case League.Gold:
                color = Gold;
                break;
            case League.Platinum:
                color = Platinum;
                break;
            case League.Diamond:
                color = Diamond;
                break;
            case League.Master:
                color = Master;
                break;
        }

        Camera.main.backgroundColor = color;
    }

    public void RemoveThrowBallEvent(UIDelegate updateUI)
    {
        UiEvents -= updateUI;
    }

    public void RemoveGameUiEvent(GameUIDelegate updateGameUI)
    {
        GameUiEvent -= updateGameUI;
    }

    public void SetLobbyUIUpdate(LobbyUIDelegate uiEvent)
    {
        LobbyUIEvent -= uiEvent;
        LobbyUIEvent += uiEvent;
    }

    public void RemoveLobbyUIUpdate(LobbyUIDelegate uiEvent)
    {
        LobbyUIEvent -= uiEvent;
    }
    #endregion

}
