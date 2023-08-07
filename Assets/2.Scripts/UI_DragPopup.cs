using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class UI_DragPopup : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
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
    public GameObject batObj;
    public AiBat bat;

    Vector2 _touchPosition;
    Vector2 _moveDir;
    float _joystickRadius = 0.0f;
    float _speed = 5.0f;


    private void Start()
    {
        _joystickRadius = _background.gameObject.GetComponent<RectTransform>().sizeDelta.y / 2f;

        bat = batObj.GetComponent<AiBat>();
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
        batObj.transform.localPosition += dir;
    }
}
