using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Sockets;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    private void Start()
    {
        _joystickRadius = _background.gameObject.GetComponent<RectTransform>().sizeDelta.y / 2f;

        bat = Managers.Game.Bat;

        Managers.Game.SetDragPopup(this);
        zoneRt = _zoneImage.GetComponent<RectTransform>();
        batRect = _batAim.GetComponent<RectTransform>();

        CalculateZoneRatio();

        offsetX = 18f;
        offsetY = -95f;


    }

    private void LateUpdate()
    {
        // UI 좌표 + Vec2(x 정규화. y 정규화)
        //batRect.anchoredPosition =
        //    new Vector2(bat.HitColider.localPosition.x * ratioX, (bat.HitColider.localPosition.y) * ratioY);

        //if(Managers.Game.AimPoint != null)
        //{
        //    _ballAim.anchoredPosition = new Vector2(Managers.Game.AimPoint.x * ratioX, Managers.Game.AimPoint.y * ratioY) ;
        //}

        //batRect.anchoredPosition = CalculateZoneRatio4(bat.HitColider.localPosition);
        //_ballAim.anchoredPosition = CalculateZoneRatio4(Managers.Game.AimPoint);

    }

    public void CalculateZoneRatio()
    {
        // 월드 좌표 크기
        Vector3 worldSize = Managers.Game.StrikeZone.size; // 'size'가 StrikeZone의 가로와 세로 크기를 반환한다고 가정


        Debug.Log(Managers.Game.StrikeZone.boxCollider.bounds.max);
        Debug.Log(Managers.Game.StrikeZone.boxCollider.bounds.center);
        Debug.Log(Managers.Game.StrikeZone.boxCollider.bounds.min);

        Debug.Log("World Size" + worldSize);

        // UI 좌표 크기
        float distanceZoneX = zoneRt.rect.width;
        float distanceZoneY = zoneRt.rect.height;

        Debug.Log(distanceZoneX);
        Debug.Log(distanceZoneY);

        var bounds = Managers.Game.StrikeZone.boxCollider.bounds;

        ratioX = distanceZoneX * 2f / Mathf.Abs(bounds.max.x - bounds.min.x);
        ratioY = distanceZoneY * 2f / Mathf.Abs(bounds.max.y - bounds.min.y);
    }

    public Vector2 CalculateZoneRatio2(Vector3 worldVector)
    {
        float worldXoffset = 0.45f;
        float worldYoffset = 0.25f;
        Vector2 worldX = new Vector2(Managers.Game.StrikeZone.boxCollider.bounds.min.x, Managers.Game.StrikeZone.boxCollider.bounds.max.x) + new Vector2(worldXoffset, worldXoffset);
        Vector2 worldY = new Vector2(Managers.Game.StrikeZone.boxCollider.bounds.min.y, Managers.Game.StrikeZone.boxCollider.bounds.max.y) + new Vector2(worldYoffset, worldYoffset);

        float zoneOffset = 250.0f;
        Vector2 zoneX = new Vector2(zoneRt.rect.xMin, zoneRt.rect.xMax) + new Vector2(zoneOffset, zoneOffset);
        Vector2 zoneY = new Vector2(zoneRt.rect.yMin, zoneRt.rect.yMax) + new Vector2(zoneOffset, zoneOffset);

        // Calculate scale ratios
        float scaleX = Mathf.Abs(zoneX.y - zoneX.x) / Mathf.Abs(worldX.y - worldX.x);
        float scaleY = Mathf.Abs(zoneY.y - zoneY.x) / Mathf.Abs(worldY.y - worldY.x);

        // Transform world coordinates to zone coordinates
        float transformedWorldX = (worldVector.x - worldX.x) * scaleX + zoneX.x;
        float transformedWorldY = (worldVector.y - worldY.x) * scaleY + zoneY.x; // Adjusted the y transformation logic


        // Assuming there's an offset between the origins of the two coordinate systems
        Vector2 originOffset = new Vector2(0, 0); // Adjust this value as needed
        transformedWorldX += originOffset.x;
        transformedWorldY += originOffset.y;



        return new Vector2(transformedWorldX, transformedWorldY);
    }


    public Vector2 CalculateZoneRatio3(Vector3 worldVector)
    {
        float worldXoffset = 0.45f;
        float worldYoffset = 0.25f;
        Vector2 worldX = new Vector2(Managers.Game.StrikeZone.boxCollider.bounds.min.x, Managers.Game.StrikeZone.boxCollider.bounds.max.x) + new Vector2(worldXoffset, worldXoffset);
        Vector2 worldY = new Vector2(Managers.Game.StrikeZone.boxCollider.bounds.min.y, Managers.Game.StrikeZone.boxCollider.bounds.max.y) + new Vector2(worldYoffset, worldYoffset);

        float zoneOffset = 250.0f;
        Vector2 zoneX = new Vector2(zoneRt.rect.xMin, zoneRt.rect.xMax) + new Vector2(zoneOffset, zoneOffset);
        Vector2 zoneY = new Vector2(zoneRt.rect.yMin, zoneRt.rect.yMax) + new Vector2(zoneOffset, zoneOffset);

        // Calculate scale ratios
        float scaleX = (zoneX.y - zoneX.x) / (worldX.y - worldX.x);
        float scaleY = (zoneY.y - zoneY.x) / (worldY.y - worldY.x);

        float aspect = (float)Screen.width / (float)Screen.height;
    
        // Transform world coordinates to zone coordinates
        float transformedWorldX = (worldVector.x - worldX.x) * scaleX + zoneX.x; // Subtract zoneOffset to adjust the range
        float transformedWorldY = (worldVector.y - worldY.x) * (scaleY) + zoneY.x ; // Subtract zoneOffset to adjust the range



        return new Vector2(transformedWorldX, transformedWorldY);
    }


    public Vector2 ConvertWorldToUIPosition(Vector3 worldPosition, RectTransform canvasRectTransform, Camera camera = null)
    {

        camera = Camera.main;
        // 월드 좌표를 스크린 좌표로 변환
        Vector2 screenPosition = camera.WorldToScreenPoint(worldPosition);

        // 스크린 좌표를 canvasRectTransform 내의 로컬 좌표로 변환
        Vector2 localUIPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, screenPosition, camera, out localUIPosition);

        return localUIPosition;
    }


    public Vector2 CalculateZoneRatio4(Vector3 worldVector)
    {
        float worldXoffset = 0.45f;
        float worldYoffset = 0.25f;
        Vector2 worldX = new Vector2(Managers.Game.StrikeZone.boxCollider.bounds.min.x, Managers.Game.StrikeZone.boxCollider.bounds.max.x) + new Vector2(worldXoffset, worldXoffset);
        Vector2 worldY = new Vector2(Managers.Game.StrikeZone.boxCollider.bounds.min.y, Managers.Game.StrikeZone.boxCollider.bounds.max.y) + new Vector2(worldYoffset, worldYoffset);

        float zoneOffset = 250.0f;
        Vector2 zoneX = new Vector2(zoneRt.rect.xMin, zoneRt.rect.xMax) + new Vector2(zoneOffset, zoneOffset);
        Vector2 zoneY = new Vector2(zoneRt.rect.yMin, zoneRt.rect.yMax) + new Vector2(zoneOffset, zoneOffset);

        // Calculate scale ratios
        float scaleX = Mathf.Abs(zoneX.y - zoneX.x) / Mathf.Abs(worldX.y - worldX.x);
        float scaleY = Mathf.Abs(zoneY.y - zoneY.x) / Mathf.Abs(worldY.y - worldY.x);

        // Transform world coordinates to zone coordinates
        float transformedWorldX = (worldVector.x - worldX.x) * scaleX + zoneX.x;
        float transformedWorldY = (worldVector.y - worldY.x) * scaleY + zoneY.y; // Adjusted the y transformation logic

        // Assuming there's an offset between the origins of the two coordinate systems
        Vector2 originOffset = new Vector2(0, 0); // Adjust this value as needed
        transformedWorldX += originOffset.x;
        transformedWorldY += originOffset.y;

        return new Vector2(transformedWorldX, transformedWorldY);
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
        bat.HitColider.localPosition += dir;
    }
}
