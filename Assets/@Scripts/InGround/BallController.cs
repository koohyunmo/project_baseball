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
        
        // ���� �ʱ�ȭ
        _strike = false;
        _hit = false;
        currentPointIndex = 0;
    }

    private void ResetRigid()
    {
        // �ӵ��� ȸ�� �ʱ�ȭ
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        // ���� �ʱ�ȭ (�ɼ�)
        transform.forward = Vector3.forward;

        // ȸ�� �ʱ�ȭ (�ɼ�)
        transform.rotation = Quaternion.identity;

        // ���� Rigidbody�� �� �Ǵ� ��ũ�� ����Ǿ� �־��ٸ�, �̸� ����
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
            Debug.Log("��Ʈ����ũ");
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
            Debug.Log("TODO ���� ");
            Managers.Object.Despawn<BallController>(ObjId);
        }
    }
}