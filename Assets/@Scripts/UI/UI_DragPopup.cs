using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;

public class UI_DragPopup : UI_Popup, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    enum Images
    {
        TouchBG,
        BG,
        Handler,
        Icon,
        ZoneImage,
        BallAim,
        BatAim,
    }


    public Image _bg;
    public Image _handler;
    public Image _icon;
    public Image _touchBG;
    public Image _zoneImage;
    public Image _batAim;
    public Image _ballAim;

    public Bat bat;

    Vector2 _touchPosition;
    Vector2 _moveDir;
    float _joystickRadius = 0.0f;
    //float _speed = 2.5f;
    float _speed = 5f;


    RectTransform zoneRt;


    public bool isRight = false;


    List<CustomReplayData> _batReplayData = new List<CustomReplayData>();


    private void Start()
    {
        BindUI();
    }

    private void BindUI()
    {
        BindImage(typeof(Images));



        bat = Managers.Game.Bat;

        _zoneImage = GetImage((int)Images.ZoneImage);
        _batAim = GetImage((int)Images.BatAim);
        _ballAim = GetImage((int)Images.BallAim);
        _touchBG = GetImage((int)Images.TouchBG);
        _icon = GetImage((int)Images.Icon);
        _handler = GetImage((int)Images.Handler);
        _bg = GetImage((int)Images.BG);

        Managers.Game.SetDragPopup(this);
        zoneRt = _zoneImage.GetComponent<RectTransform>();


        Managers.Game.SetHitCallBack(MovePosReset);
        Managers.Game.SetStrikeCallBack(SaveMovePos);



        _batReplayData.Clear();

        ZoneSetting();
        _joystickRadius = _bg.gameObject.GetComponent<RectTransform>().sizeDelta.y / 2f;
    }


    private void ZoneSetting()
    {
        var zone = Utils.GetScreenRectanglePoints(Managers.Game.StrikeZone.transform.position,0.7f);

        // RectTransform을 가져옵니다.

        // 스크린 좌표의 너비와 높이를 계산합니다.
        float width = Mathf.Abs(zone.TopRight.x - zone.TopLeft.x);
        float height = Mathf.Abs(zone.TopLeft.y - zone.BottomLeft.y);


        // 이미지의 중심 위치를 설정합니다.
        Vector2 centerPosition = new Vector2((zone.TopLeft.x + zone.BottomRight.x) / 2, (zone.TopLeft.y + zone.BottomRight.y) / 2);

        zoneRt.position = centerPosition;

        // 이미지의 크기를 설정합니다.
        zoneRt.sizeDelta = new Vector2(width, height);
    }


    private void MovePosReset()
    {
        if (_batReplayData.Count > 0)
            _batReplayData.Clear();
    }

    private void LateUpdate()
    {
        return;
        _ballAim.transform.position = Managers.Game.AimPointScreen;
        _batAim.transform.position = Managers.Game.BatColiderPointScreen;
    }

    private void SaveMovePos()
    {
        Managers.Game.ReplayData(_batReplayData);
    }

    public void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
    {
        //Debug.Log("OnPointerClick");


    }

    public void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
    {
        //Debug.Log("OnPointerDown");
        _bg.transform.position = eventData.position;
        _handler.transform.position = eventData.position;
        _touchPosition = eventData.position;
    }

    public void OnPointerUp(UnityEngine.EventSystems.PointerEventData eventData)
    {
        //Debug.Log("OnPointerUp");
        _handler.transform.position = _touchPosition;
        _moveDir = Vector2.zero;

        // TEMP1
        //dragObj.MoveDir = _moveDir;
        // TEMP2
        //Managers.MoveDir = _moveDir;

    }

    public void OnDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        //Debug.Log("OnDrag");
        Vector2 touchDir = (eventData.position - _touchPosition);

        float moveDist = Mathf.Min(touchDir.magnitude, _joystickRadius);
        _moveDir = touchDir.normalized;
        Vector2 newPosition = _touchPosition + _moveDir * moveDist;
        _handler.transform.position = newPosition;

        // TEMP1
        //_player.MoveDir = _moveDir;

        // TEMP2
        //Managers.MoveDir = _moveDir;

        Vector3 newDir = new Vector3(_moveDir.x, _moveDir.y, 0);

        Vector3 dir = newDir * _speed * Time.deltaTime;


        if (Managers.Game.BatPosition == Define.BatPosition.Right) // 우타인 경우
        {

            dir = -dir;
            dir.y *= -1;
        }

        if ( Managers.Game.isRecord == true && Managers.Game.GameState == Define.GameState.InGround)
        {
            CustomReplayData replatData;
            replatData.position = bat.HitColiderTransform.localPosition;
            replatData.time = Time.timeSinceLevelLoad;

            _batReplayData.Add(replatData);
        }

        bat.HitColiderTransform.localPosition += dir;
    }
}
