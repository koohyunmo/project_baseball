using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallAimController : InGameObjectController
{

    Vector3 _targetPos = Vector3.zero;
    Transform _ball = null;
    float _initialDistance = 0f;

    readonly float testValue = 0.1f;

    float hValue = 0.1f;

    private void OnEnable()
    {
        _initialDistance = 0f;
        hValue = testValue;
        var tt = (int)(10 - Managers.Game.HawkeyeLevel) * 0.1f;

        Debug.Log("hValue : " + tt);
    }

    public void DataInit(Vector3 vec, Transform ball)
    {
        _targetPos = vec;
        _ball = ball;
        _initialDistance = (_targetPos - ball.position).magnitude;


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
            float scaleValue = Mathf.Lerp((float)Managers.Game.League * hValue, 1f, currentDistance / _initialDistance);

            transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
        }

    }
}
