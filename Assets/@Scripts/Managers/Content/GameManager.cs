using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using static Define;
using static Keys;


using Debug = UnityEngine.Debug;
public class GameManager
{
    GameState _gameState = GameState.Home;


    public Stopwatch StopWatch { get; set; } = new Stopwatch();

    public BatType BatType { get; private set; } = BatType.NONE;
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

    public HitType HitType { get { return _hitType; } private set { _hitType = value; } }
    HitType _hitType = HitType.A;
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

    public void GetCSOReward()
    {
        bool rewarded = Managers.Game.PlayerInfo.csoAllRewawrd;
        string rewardID = "BAT_37";

#if UNITY_EDITOR
        rewarded = false;
#endif

        if (rewarded)
            return;

        var clearCount = Mathf.Clamp(Managers.Game.GameDB.challengeClearCount, 0, Managers.Game.GameDB.challengeData.Count);
        float ratio = clearCount / (float)Managers.Game.GameDB.challengeData.Count;

        if (ratio >= 1 && rewarded == false)
        {
            Managers.Game.PlayerInfo.csoAllRewawrd = true;
            var type = Managers.Game.GetItem(rewardID);
            var popup = Managers.UI.ShowPopupUI<UI_RoulletItemInfoPopup>();

            switch (type)
            {
                case Define.GetType.Failed:
                    throw new Exception("아이템 없음?");
                    break;
                case Define.GetType.Success:
                    popup.InitData(Define.GetType.Success, rewardID);
                    break;
                case Define.GetType.Duplicate:
                    Managers.Game.GetStar(500);
                    popup.InitData(Define.GetType.Duplicate,Define.Grade.Legendary, 500);
                    break;
            }
        }

    }

    public void GetBallReward()
    {
        bool rewarded = Managers.Game.PlayerInfo.ballAllReward;
        string rewardID = "SKILL_12";

#if UNITY_EDITOR
        rewarded = false;
#endif

        if (rewarded)
            return;

        var clearCount = Mathf.Clamp(Managers.Game.GameDB.playerBalls.Count, 0, Managers.Resource.ballOrderList.Count);
        float ratio = clearCount / (float)Managers.Resource.ballOrderList.Count;

        if (ratio >= 1 && rewarded == false)
        {
            Managers.Game.PlayerInfo.ballAllReward = true;
            var type = Managers.Game.GetItem(rewardID);
            var popup = Managers.UI.ShowPopupUI<UI_RoulletItemInfoPopup>();

            switch (type)
            {
                case Define.GetType.Failed:
                    throw new Exception("아이템 없음?");
                    break;
                case Define.GetType.Success:
                    popup.InitData(Define.GetType.Success,rewardID);
                    break;
                case Define.GetType.Duplicate:
                    Managers.Game.GetStar(500);
                    popup.InitData(Define.GetType.Duplicate,Define.Grade.Legendary, 500);
                    break;
            }

        }

    }

    public void GetBatReward()
    {
        bool rewarded = Managers.Game.PlayerInfo.batAllReward;
        string rewardID = "SKILL_2";

#if UNITY_EDITOR
        rewarded = false;
#endif

        if (rewarded)
            return;

        var clearCount = Mathf.Clamp(Managers.Game.GameDB.playerBats.Count, 0, Managers.Resource.batOrderList.Count);
        float ratio = clearCount / (float)Managers.Resource.batOrderList.Count;

        if (ratio >= 1 && rewarded == false)
        {
            Managers.Game.PlayerInfo.batAllReward = true;
            var type = Managers.Game.GetItem(rewardID);
            var popup = Managers.UI.ShowPopupUI<UI_RoulletItemInfoPopup>();

            switch (type)
            {
                case Define.GetType.Failed:
                    throw new Exception("아이템 없음?");
                    break;
                case Define.GetType.Success:
                    popup.InitData(Define.GetType.Success,rewardID);
                    break;
                case Define.GetType.Duplicate:
                    Managers.Game.GetStar(500);
                    popup.InitData(Define.GetType.Duplicate,Define.Grade.Legendary, 500);
                    break;
            }
        }

    }

    public GameState GameState { get { return _gameState; } private set { _gameState = value; } }

    public float ReplaySlowMode { get { return 0.25f; } private set { } }

    public DateTime freeRoulletTime;
    public DateTime adRoulletTime;
    public DateTime starShopADBonusTime;

    public float Speed { get { return (_speed * 3600) * 0.001f; } private set { _speed = value; } }
    public float _speed = 0;

    public ThrowType ThrowType { get; private set; }
    public LineRenderer StrikePath { get; private set; }

    public long HitScore { get; private set; }
    public Vector3 HitPos { get; private set; } = Vector3.zero;
    public long GameScore { get; private set; }

    public delegate void UIDelegate();
    public UIDelegate UiEvents;
    public Transform BuffSkillTr { get; private set; }
    public Transform ColliderSkillTr { get; private set; }
    public void SetBuffPos(Transform skillPos)
    {
        BuffSkillTr = skillPos;
    }

    public void SetColliderPos(Transform skillPos)
    {
        ColliderSkillTr = skillPos;
    }

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

    public float skillBonus = 0.0f;
    public long hitBonus = 0;
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

    public Action UpdateStar { get; private set; }

    public enum HitScoreType
    {
        NONE,
        BAD,
        COOL,
        GOOD,
        HOMERUN,  // 'E'를 제거했습니다.
        GRANDSLAM  // 'A'를 'E'로 변경했습니다.
    }

    public HitScoreType hitScoreType { get; private set; } = HitScoreType.NONE;

    public async void Init()
    {
        _path = Application.persistentDataPath + "/SaveFile.es3";
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
        Managers.Localization.LoadLocalizedText();
        _batSpeed = ES3.Load<float>("Gamdo", _settingPath);

        PlayerDataSettings();
        Managers.Resource.DoCache();
        StateUpdate();

        freeRoulletTime = ES3.Load<DateTime>("RTime");
        adRoulletTime = ES3.Load<DateTime>("ADTime");
        starShopADBonusTime = ES3.Load<DateTime>("AdBonus");

    }

    private void PlayerDataSettings()
    {
        ChangeBall(_gameData.playerInfo.equipBallId);
        ChangeBat(_gameData.playerInfo.equipBatId);
        ChangeSkill(_gameData.playerInfo.equipSkillId);
    }

    public async void LoadItemCount()
    {
        //BatCount = await Managers.Resource.ObjectGetAsyncCount("Bat");
        //Debug.Log(BatCount);
        //BallCount = await Managers.Resource.ObjectGetAsyncCount("Ball");
        //Debug.Log(BallCount);
        //SkillCount = await Managers.Resource.ObjectGetAsyncCount("Skill");
        //Debug.Log(SkillCount);
        //ChallengeCount = await Managers.Resource.ObjectGetAsyncCount("Challenge");
        //Debug.Log(ChallengeCount);
    }

    public bool CanPay(long star)
    {
        return Managers.Game.PlayerInfo.star >= star;
    }

    public void MinusStar(long star)
    {

        if (CanPay(star))
        {
            PlayerInfo.star -= star;
            Math.Clamp(PlayerInfo.star, 0, long.MaxValue);
            SaveGame();

            UpdateStar?.Invoke();
        }
        else
            return;

    }

    public void GetStar(long star)
    {
        if (star == 0)
            return;
#if UNITY_EDITOR
        Debug.Log("Star :" + star);
#endif
        PlayerInfo.star += star;
        Math.Clamp(PlayerInfo.star, 0, long.MaxValue);
        SaveGame();
        Managers.Sound.Play(Define.Sound.Effect, "Collectible01");
        UpdateStar?.Invoke();
    }

    public void SetStarUpdate(Action updateStar)
    {
        UpdateStar -= updateStar;
        UpdateStar += updateStar;
    }

    public void RemoveStarUpdate(Action updateStar)
    {
        UpdateStar -= updateStar;
    }

    public void GameReady(GameMode gameType, Action callBack = null)
    {
        if (GameState != GameState.Home)
            return;

        _gameMode = gameType;
        GameState = GameState.Ready;

        callBack?.Invoke();
        StateUpdate();
    }


    public void GameStart(Action callBack = null)
    {
        if (GameState != GameState.Ready)
            return;

        GameState = GameState.InGround;

        callBack?.Invoke();
        StateUpdate();
    }

    public void GameEndCallback(Action callBack = null)
    {
        if (GameState != GameState.InGround)
            return;

        GameState = GameState.End;

        callBack?.Invoke();
        StateUpdate();
    }

    public void GameEnd()
    {
        if (GameState != GameState.InGround)
            return;

        GameState = GameState.End;
        StateUpdate();
    }




    public void GoHome(Action callBack = null)
    {
        if (GameState != GameState.End)
            return;

        SaveBestScore();


        GameState = GameState.Home;

        GameScore = 0;
#if UNITY_EDITOR
        Debug.Log($"{GameScore} 게임 리셋");
#endif


        StateUpdate();
        callBack?.Invoke();
    }

    private void SaveBestScore()
    {
        if (GameMode != GameMode.Challenge && _gameData.playerInfo.playerBestScore[_league] < GameScore)
        {
            //Debug.Log("최고 점수 갱신");
            _gameData.playerInfo.playerBestScore[_league] = GameScore;
            SaveGame();
       
        }

        if (Managers.Game.GameMode == GameMode.Challenge)
            return;
        else
        {
            if (_league < League.Master)
                GetStar(Math.Clamp((GameScore / Define.POINT), 0, (int)League + 1));
            else
                GetStar(Math.Clamp((GameScore / Define.POINT), 0, (int)League + 2));
        }




    }

    private void StateUpdate()
    {
        switch (GameState)
        {
            case GameState.Home:
                {


                    Managers.Obj.DespawnAll();
                    batMoveReplayData.Clear();
                    MainCam.MoveOriginaPos();
                    Managers.UI.ShowPopupUI<UI_Main>();

                    if (challengeProc == ChallengeProc.Complete || challengeProc == ChallengeProc.Fail)
                        Managers.UI.ShowPopupUI<UI_ChallengePopup>();

                    if (_gameMode == GameMode.Challenge)
                    {
                        SetLeague(challengePrevLeague);
                        _gameMode = GameMode.None;
                    }


                    ScoreAndCountClear();

                    if (_bat)
                        _bat.ColiderObjOff();


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
                        var skillSo = Managers.Resource.GetItemScriptableObjet<SkillScriptableObject>(Managers.Game.EquipSkillId);
                        hawkeyeLevel = skillSo.HawkeyeLevel;

                        if (hawkeyeLevel == HawkeyeLevel.ZERO)
                            HawkEyes = false;
                        else
                            HawkEyes = true;

                        Managers.Skill.SkillInjection(skillSo);
                    }
                }
                break;
            case GameState.InGround:
                {
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
                        if (challengeProc == ChallengeProc.None)
                            Managers.UI.ShowPopupUI<UI_ReplayPopupTimer>();
                        else
                        {

                            SaveChallengeData();
                            if (challengeProc == ChallengeProc.Complete)
                                Managers.UI.ShowPopupUI<UI_ChallengeClearPopup>();
                            else
                            {
                                Managers.UI.ShowPopupUI<UI_ChallengeClearPopup>().Failed();
                            }
                        }


                        isRecord = false;

                    });


                }
                break;
        }

    }

    private void SaveChallengeData()
    {
        if (challengeProc == ChallengeProc.Complete && _gameData.challengeData[ChallengeGameID] == false)
        {
            _gameData.challengeData[ChallengeGameID] = true;
            _gameData.challengeClearCount += 1;
            SaveGame();
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
        challengeProc = ChallengeProc.None;
    }

    public void SaveBatGamdo(float gamdo)
    {
        ES3.Save<float>("Gamdo", gamdo, _settingPath);
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

    public bool RollFreeRoullet()
    {
        if (DateTime.Now >= freeRoulletTime)
        {
            freeRoulletTime = DateTime.Now.AddMinutes(10);
            ES3.Save<DateTime>("RTime", freeRoulletTime);
            return true;
        }

        return false;
    }

    public bool GetFreeRoullet()
    {
        if (DateTime.Now >= freeRoulletTime)
        {
            return true;
        }

        return false;
    }

    public bool RollADRoullet()
    {
        if (DateTime.Now >= adRoulletTime)
        {
            adRoulletTime = DateTime.Now.AddHours(1);
            ES3.Save<DateTime>("ADTime", adRoulletTime);
            return true;
        }

        return false;
    }

    public bool GetADRoullet()
    {
        if (DateTime.Now >= adRoulletTime)
        {
            return true;
        }

        return false;
    }

    
    private void GetAdBonus()
    {
        long starShopBonus = 50;

        Managers.Game.GetStar(starShopBonus);

        var popup = Managers.UI.ShowPopupUI<UI_RoulletItemInfoPopup>();
        popup.InitData(Define.GetType.Star,Define.Grade.Rare, starShopBonus);

        SaveGame();

    }

    public bool CanCllickAdButton()
    {
        if (DateTime.Now >= starShopADBonusTime)
        {
            return true;
        }

        return false;
    }


    public string ADBounusTimeDisplay()
    {
        TimeSpan timeRemaining; // 남은 시간

        timeRemaining = starShopADBonusTime - DateTime.Now; // 남은 시간 계산

        string remainTimeText = "";

        // 남은 시간을 문자열로 변환하여 화면에 표시
        // 남은 시간이 없거나 지난 경우
        if (timeRemaining.TotalSeconds <= 0)
        {
            return remainTimeText = Managers.Localization.GetLocalizedValue(LanguageKey.ad.ToString()); ;
        }

        // 남은 시간을 문자열로 변환하여 화면에 표시
        return remainTimeText = string.Format("{0:D2}:{1:D2}:{2:D2}", timeRemaining.Hours, timeRemaining.Minutes, timeRemaining.Seconds);

    }
    public string FreeRTimeDisplay()
    {
        TimeSpan timeRemaining; // 남은 시간

        timeRemaining = freeRoulletTime - DateTime.Now; // 남은 시간 계산

        string remainTimeText = "";

        // 남은 시간을 문자열로 변환하여 화면에 표시
        // 남은 시간이 없거나 지난 경우
        if (timeRemaining.TotalSeconds <= 0)
        {
            return remainTimeText = "00:00:00";
        }

        // 남은 시간을 문자열로 변환하여 화면에 표시
        return remainTimeText = string.Format("{0:D2}:{1:D2}:{2:D2}", timeRemaining.Hours, timeRemaining.Minutes, timeRemaining.Seconds);
    }

    public string ADRTimeDisplay()
    {
        TimeSpan timeRemaining; // 남은 시간

        timeRemaining = adRoulletTime - DateTime.Now; // 남은 시간 계산

        string remainTimeText = "";

        // 남은 시간을 문자열로 변환하여 화면에 표시
        // 남은 시간이 없거나 지난 경우
        if (timeRemaining.TotalSeconds <= 0)
        {
            return remainTimeText = Managers.Localization.GetLocalizedValue(LanguageKey.spinwithad.ToString());
        }

        // 남은 시간을 문자열로 변환하여 화면에 표시
        return remainTimeText = string.Format("{0:D2}:{1:D2}:{2:D2}", timeRemaining.Hours, timeRemaining.Minutes, timeRemaining.Seconds);
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
        StateUpdate();
    }

    #region 게터세터
    public void GetGameScoreAndGetPosition(float score, Vector3 hitPos)
    {
        HitPos = hitPos;

        score += skillBonus;

        if (score < 50 && score > 0)
        {
            hitScoreType = HitScoreType.BAD;
            _hitType = HitType.A;
            HitScore = 1 + hitBonus;
            SwingCount++;
        }
        else if (score >= 51 && score < 80)
        {
            hitScoreType = HitScoreType.COOL;
            _hitType = HitType.A;
            HitScore += 1 + hitBonus;
            SwingCount++;
        }
        else if (score >= 80 && score < 95)
        {
            hitScoreType = HitScoreType.GOOD;
            _hitType = HitType.B;
            HitScore += 2 + hitBonus;
            SwingCount++;
        }
        else if (score >= 95 && score < 100)
        {
            hitScoreType = HitScoreType.HOMERUN;
            _hitType = HitType.D;
            HitScore += 4 + hitBonus;
            SwingCount++;
            HomeRunCount++;
        }
        else if (score >= 100 && score <= float.MaxValue)
        {
            hitScoreType = HitScoreType.GRANDSLAM;
            _hitType = HitType.D;
            HitScore += 10 + hitBonus;
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
        if (_bat)
            _bat.gameObject.SetActive(true);
    }


    private League challengePrevLeague;
    public void SetChallengeMode(ChallengeScriptableObject cso)
    {
        ChallengeScore = cso.score;
        ChallengeMode = cso.mode;
        ChallengeGameID = cso.key;
        challengeProc = ChallengeProc.Fail;
        challengePrevLeague = _league;
        SetLeague(cso.league);
    }

    // -1 실패 0 성공  1이미 있는 아이템
    public Define.GetType GetItem(string key)
    {

        var itemSO = Managers.Resource.GetItemScriptableObjet<ItemScriptableObject>(key);

        if (!itemSO)
        {
            Debug.LogError("없는 아이템");
            Managers.Sound.Play(Define.Sound.Effect, "");
            return Define.GetType.Failed;
        }


        if (_gameData.playerInventory.Contains(key) == true)
        {
#if UNITY_EDITOR
            Debug.Log("이미 있는 아이템");
#endif
            Managers.Sound.Play(Define.Sound.Effect, "Collectible01");
            return Define.GetType.Duplicate;
        }


        var type = Managers.Resource.GetItemScriptableObjet<ItemScriptableObject>(key).type;

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
#if UNITY_EDITOR
        Debug.Log("TODO");
#endif
        notifyItemAction();

        LobbyUIEvent?.Invoke();

        Managers.Sound.Play(Define.Sound.Effect, "collect_item_02");
        return Define.GetType.Success;
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
        ChageBackgroundColor();
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

    public string GetSpeedColorString()
    {
        float minValue = 41.67f * 0.8f; // 초록색
        float maxValue = 62f; // 빨간색

        // 값의 위치 계산 (0에서 1 사이)
        float t = Mathf.InverseLerp(minValue, maxValue, _speed);

        // 두 색상 사이를 선형 보간
        Color interpolatedColor = Color.Lerp(Color.green, Color.red, t);

        string colorCode = Utils.ColorToHex_1(interpolatedColor);

        string speedKM = ((_speed * 3600) * 0.001f).ToString("F2") + "km/h";

        return colorCode;
    }

    #endregion


    #region 첼린지 모드 체크
    public ChallengeProc challengeProc { get; private set; } = ChallengeProc.None;


    public void SaveChallengGridPos()
    {
        ChallengeGridPos = new Vector3(0,ChallengeGrid.localPosition.y,0);
    }

    public void DeleteChallengeGridPos()
    {
        ChallengeGridPos = Vector3.zero;
    }
    private void ChallengeModeCheck()
    {

        if (GameMode != GameMode.Challenge)
            return;

        switch (ChallengeMode)
        {
            case ChallengeType.ScoreMode:
                if (GameScore == ChallengeScore)
                {
                    challengeProc = ChallengeProc.Complete;
                    ManagerAsyncFunction(GameEnd, 0.5f);
                }
                else if (GameScore > ChallengeScore)
                {
                    challengeProc = ChallengeProc.Fail;
                    ManagerAsyncFunction(GameEnd, 0.5f);
                }
                return;
            case ChallengeType.HomeRunMode:
                if (HomeRunCount >= ChallengeScore)
                {
                    challengeProc = ChallengeProc.Complete;
                    ManagerAsyncFunction(GameEnd, 0.5f);
                }
                return;
            case ChallengeType.RealMode:
                if (SwingCount >= ChallengeScore)
                {
                    challengeProc = ChallengeProc.Complete;
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
    public float BatSpeed { get { return Mathf.Clamp(_batSpeed, 0.1f, 10.0f); } private set { _batSpeed = value; } }

    public Vector3 ChallengeGridPos { get; internal set; } = Vector3.zero;
    public Transform ChallengeGrid { get; internal set; } = null;

    private float _batSpeed = 5.0f;


    public void SaveGame()
    {
        //string jsonStr = JsonConvert.SerializeObject(SaveData);
        //File.WriteAllTextAsync(_path, jsonStr);

        ES3.Save<GameDB>(Keys.DB.GameDB.ToString(), SaveData);
#if UNITY_EDITOR
        Debug.Log($"Save Game Completed : {_path}");
#endif
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

                // 플레이어 데이터 설정
                if (Managers.Resource.Resources[BAT_KEY.BAT_0.ToString()] is ItemScriptableObject so)
                {
                    // Test Data
                    PlayerItem startItem = new PlayerItem(so.id, so.name, so.name, ItemType.BAT);
                    //StartData.playerItem.Add(startItem.itemId, startItem);
#if UNITY_EDITOR
                    //StartData.playerInfo.gold = 100000;
                    StartData.playerInfo.star = 10000;
#endif
                    StartData.playerInfo.level = 1;
                    StartData.playerInfo.equipBatId = BAT_KEY.BAT_0.ToString();
                    StartData.playerInventory.Add(startItem.itemId);
                    StartData.playerBats.Add(startItem.itemId);
                }

                if (Managers.Resource.Resources[BALL_KEY.BALL_0.ToString()] is ItemScriptableObject ball)
                {
                    // Test Data
                    PlayerItem startItem = new PlayerItem(ball.id, ball.name, ball.name, ItemType.BALL);
                    //StartData.playerItem.Add(startItem.itemId, startItem);

                    StartData.playerInfo.level = 1;
                    StartData.playerInfo.equipBallId = BALL_KEY.BALL_0.ToString();

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

            {
                _gameData.playerInfo.playerBestScore[League.Bronze] = 0;
                _gameData.playerInfo.playerBestScore[League.Silver] = 0;
                _gameData.playerInfo.playerBestScore[League.Gold] = 0;
                _gameData.playerInfo.playerBestScore[League.Platinum] = 0;
                _gameData.playerInfo.playerBestScore[League.Diamond] = 0;
                _gameData.playerInfo.playerBestScore[League.Master] = 0;

                _gameData.playerInfo.playerClearLeague[League.Bronze] = true;
                _gameData.playerInfo.playerClearLeague[League.Silver] = false;
                _gameData.playerInfo.playerClearLeague[League.Gold] = false;
                _gameData.playerInfo.playerClearLeague[League.Platinum] = false;
                _gameData.playerInfo.playerClearLeague[League.Diamond] = false;
                _gameData.playerInfo.playerClearLeague[League.Master] = false;
            }

            _gameData.challengeData = challengeData;

            // 옵션설정
            ES3.Save<float>("Gamdo", 5f, _settingPath);
            // TODO

            SystemLanguage userLanguage = Application.systemLanguage;

            _settingPath = Application.persistentDataPath + "/SettingData.json";
            Language currentLanguage;

            {
                // 기본 언어 설정
                switch (userLanguage)
                {
                    case SystemLanguage.Korean:
                        currentLanguage = Language.KR;
                        break;
                    case SystemLanguage.Japanese:
                        currentLanguage = Language.JP;
                        break;
                    default:
                        currentLanguage = Language.EN;
                        break;
                }
            }

            ES3.Save<Language>("Lang", currentLanguage, _settingPath);
            ES3.Save<DateTime>("RTime", DateTime.Now);
            ES3.Save<DateTime>("AdBonus", DateTime.Now);
            ES3.Save<DateTime>("ADTime", DateTime.Now);


            SaveGame();



            Debug.LogWarning("SaveFile is not Existed");
            return false;
        }

        //string fileStr = File.ReadAllText(_path);
        //GameDB data = JsonConvert.DeserializeObject<GameDB>(fileStr);
        GameDB data = ES3.Load<GameDB>(Keys.DB.GameDB.ToString());
        if (data != null)
        {
            //_playerData = data;
            SaveData = data;
#if UNITY_EDITOR
            Debug.Log(SaveData);
#endif

        }
#if UNITY_EDITOR
        Debug.Log($"Save Game Loaded : {_path}");
#endif
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
        if (BatType == BatType.NONE)
        {
            var batSO = Managers.Resource.GetItemScriptableObjet<BatScriptableObject>(key);
            BatType = batSO.batType;
        }

        if (EquipBatId.Equals(key) == false)
        {
            EquipBatId = key;
            var batSO = Managers.Resource.GetItemScriptableObjet<BatScriptableObject>(key);
            BatType = batSO.batType;

            BatSetting(batSO);
        }
        else
            return;

        SaveGame();
    }

    private void BatSetting(BatScriptableObject _item)
    {
        List<Material> _mats;
        Mesh _mesh;

        if (_item.model)
        {
            MeshRenderer renderer = _item.model.GetComponent<MeshRenderer>();

            if (renderer)
            {
                var meshfilter = _item.model.GetComponent<MeshFilter>();

                if (meshfilter)
                {
                    _mats = new List<Material>();

                    var modelMats = renderer.sharedMaterials;
                    _mats.AddRange(modelMats);
                    _mesh = meshfilter.sharedMesh;

                    Managers.Game.Bat.ChangeBatMat(_mats);
                    Managers.Game.Bat.ChangeBatMesh(_mesh);
                }
            }
        }

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

    public long GetBestScore()
    {
        if (_gameMode == GameMode.Challenge)
            GameScore = 0;

        if (Managers.Game.GameScore > _gameData.playerInfo.playerBestScore[_league])
        {
            SaveBestScore();
            //Debug.Log("최고 점수 갱신 이벤트");
            return GameScore;
        }
        else
            return _gameData.playerInfo.playerBestScore[_league];
    }

    public long GetPrice(Grade grade)
    {
        switch (grade)
        {
            case Grade.Common:
                return 50;
            case Grade.Uncommon:
                return 150;
            case Grade.Rare:
                return 300;
            case Grade.Epic:
                return 700;
            case Grade.Legendary:
                return 2000;
        }

        return -2323;
    }

    public void SetBatSpeed(float setSpeed)
    {
        _batSpeed = setSpeed;
    }
    #endregion

#if UNITY_EDITOR
    #region 테스트 코드

    public void GetAllBat()
    {
        foreach(string key in Managers.Resource.batOrderList)
        {
            GetItem(key);
        }
    }

    public void GetAllBall()
    {
        foreach (string key in Managers.Resource.ballOrderList)
        {
            GetItem(key);
        }
    }

    public void GetAllSkill()
    {
        foreach (string key in Managers.Resource.skillOrderList)
        {
            GetItem(key);
        }
    }

    public void GetAllCSO()
    {
        foreach (string key in Managers.Resource.challengeOrderList)
        {
            _gameData.challengeData[key] = true;
            _gameData.challengeClearCount += 1;
            SaveGame();
        }
    }


    #endregion

#endif

}
