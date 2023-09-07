using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallAimController : InGameObjectController
{

    Vector3 _targetPos = Vector3.zero;
    Transform _ball = null;
    float _initialDistance = 0f;

    float hValue = 1f;

    private void OnEnable()
    {
        _initialDistance = 0f;
        hValue = 1f;
    }

    public void DataInit(Vector3 vec, Transform ball)
    {
        _targetPos = vec;
        _ball = ball;
        _initialDistance = (_targetPos - ball.position).magnitude;
        hValue = Managers.Game.HawkEyesAmount;


        Managers.Game.SetHitCallBack(DespawnBall);
        Managers.Game.SetStrikeCallBack(DespawnBall);

    }

    private void DespawnBall()
    {
        if (gameObject.IsValid())
        {
            Managers.Game.RemoveCallBack(DespawnBall);
            Managers.Obj.Despawn<BallAimController>(ObjId);       
        }
            
    }
    private void Update()
    {
        if (_targetPos == Vector3.zero)
            return;



        if (Mathf.Approximately(_initialDistance, 0f))
        {
            
            return;
        }
        else
        {

            if (_targetPos == null || _ball == null)
                return;

            float currentDistance = Vector3.Distance(_ball.position, _targetPos);
            float scaleValue = Mathf.Lerp(hValue, 1f, currentDistance / _initialDistance);

            transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
        }

    }
}
