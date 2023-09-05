using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

#if UNITY_IOS
    using UnityEngine.Apple.ReplayKit;
    using Unity.VisualScripting;
#endif


public class BallController : InGameObjectController
{
    public Transform startPoint;
    public Transform endPoint;
    public Transform controlPoint;
    public LineRenderer pathRenderer;
    public float speed;
    private bool _strike;
    private bool _hit;
    public Action<int> ballClearAction;

    private List<Vector3> pathPoints = new List<Vector3>();
    private int currentPointIndex = 0;

    Rigidbody _rigidbody;

    public enum PlayMode
    {
        None,
        Character
    }

    public PlayMode playMode  = PlayMode.None;


    private void Start()
    {
        if(_rigidbody == null)
        {
            gameObject.TryGetComponent<Rigidbody>(out _rigidbody);

            if (!_rigidbody)
                _rigidbody = gameObject.GetOrAddComponent<Rigidbody>();

            _rigidbody.useGravity = false;
        }
            
    }

    private void FixedUpdate()
    {
        MoveAlongPath();
    }

    public virtual void OnEnable()
    {
        if (_rigidbody == null)
            return;
        else
        {
            ResetRigid();
        }
        
        // 변수 초기화
        _strike = false;
        _hit = false;
        currentPointIndex = 0;
    }

    private void ResetRigid()
    {
        // 속도와 회전 초기화
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        // 방향 초기화 (옵션)
        transform.forward = Vector3.forward;

        // 회전 초기화 (옵션)
        transform.rotation = Quaternion.identity;

        // 만약 Rigidbody에 힘 또는 토크가 적용되어 있었다면, 이를 중지
        _rigidbody.isKinematic = true;
        _rigidbody.isKinematic = false;
        _rigidbody.useGravity = false;
    }

    public void SetPath(LineRenderer renderer)
    {
        pathPoints.Clear();
        for (int i = 0; i < renderer.positionCount; i++)
        {
            pathPoints.Add(renderer.GetPosition(i));
        }
    }

    public void MoveAlongPath()
    {
        if(_strike)
        {
            Debug.Log("스트라이크");
        }
        else if(_hit)
        {
            return;
        }
        else
        {
            if (currentPointIndex < pathPoints.Count)
            {
                transform.position = Vector3.MoveTowards(transform.position, pathPoints[currentPointIndex], speed * Time.deltaTime);

                if (transform.position == pathPoints[currentPointIndex])
                {
                    currentPointIndex++;
                }
            }

        }
    }

    public void SetHit()
    {
        _hit = true;
    }


    private void LateUpdate()
    {
       
        if(transform.position.z >= 100)
        {
            Debug.Log("TODO 수정 ");
            Managers.Object.Despawn<BallController>(ObjId);
        }
    }
}