using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallAim : InGameObject
{

    Vector3 _targetPos = Vector3.zero;
    Transform _ball = null;
    float _initialDistance = 0f;

    float hValue = 0.1f;

    private void OnEnable()
    {
        _initialDistance = 0f;
        hValue = 0.1f;
    }

    public void DataInit(Vector3 vec, Transform ball)
    {
        _targetPos = vec;
        _ball = ball;
        _initialDistance = (_targetPos - ball.position).magnitude;


        Managers.Game.SetHitCallBack(() => { gameObject.SetActive(false); });

    }
    private void Update()
    {
        if (_targetPos == Vector3.zero)
            return;

        // 크기 조절

        if (Mathf.Approximately(_initialDistance, 0f))
        {
            return;
        }
        else
        {

            if (_targetPos == null || _ball == null)
                return;
            // 현재 거리 계산
            float currentDistance = Vector3.Distance(_ball.position, _targetPos);
            float scaleValue = Mathf.Lerp((float)Managers.Game.League*hValue, 1f, currentDistance / _initialDistance);

            transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
        }

    }
}
