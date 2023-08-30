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
        Icon
    }

    public Image _background;
    public Image _handler;
    public Image _icon;
    public Image _touchBG;
    public Image _zoneImage;
    public Bat bat;
    public Transform _batAim;
    public RectTransform _ballAim;

    Vector2 _touchPosition;
    Vector2 _moveDir;
    float _joystickRadius = 0.0f;
    float _speed = 2.5f;

    float ratioX;
    float ratioY;

    RectTransform zoneRt;
    RectTransform batRect;


    float offsetX = 18f;
    float offsetY = -95f;

    public bool isRight = false;


    List<CustomReplayData> _batReplayData = new List<CustomReplayData>();


    private void Start()
    {
        _joystickRadius = _background.gameObject.GetComponent<RectTransform>().sizeDelta.y / 2f;

        bat = Managers.Game.Bat;

        Managers.Game.SetDragPopup(this);
        zoneRt = _zoneImage.GetComponent<RectTransform>();
        batRect = _batAim.GetComponent<RectTransform>();

        offsetX = 18f;
        offsetY = -95f;

        Managers.Game.SetHitCallBack(MovePosReset);
        Managers.Game.SetStrikeCallBack(SaveMovePos);


        _batReplayData.Clear();

        ZoneSetting();

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
        _ballAim.position = Managers.Game.AimPointScreen;
        _batAim.position = Managers.Game.BatColiderPointScreen;
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
        _background.transform.position = eventData.position;
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
