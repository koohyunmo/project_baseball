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
    private Vector3 baseShadowScale = new Vector3(0.1f, 0.1f, 0.1f);

    private Transform _trailRoot;
    private Transform TrailRoot 
    {
        get
        {
            if(_trailRoot == null)
            {
                var go = new GameObject { name = "@TrailRoot" };
                go.transform.SetParent(transform);
                _trailRoot = go.transform;
            }

            return _trailRoot;
        }

        set
        {
            _trailRoot = value;
        }
    }

    Rigidbody _rigidbody;
    private GameObject Shadow;
    private Vector3 shadowStartPoint;


    public float baseHeight = 1.0f; // ���� ���� (�� ������ �� �⺻ ũ��)
    public float scaleFactor = 0.5f; // ���̰� ���� �� ũ�� ���� ����

    public enum PlayMode
    {
        None,
        Character
    }

    public PlayMode playMode  = PlayMode.None;


    private void Start()
    {
        FirstInit();
    }


    private void FirstInit()
    {
        if (_rigidbody == null)
        {
            gameObject.TryGetComponent<Rigidbody>(out _rigidbody);

            if (!_rigidbody)
                _rigidbody = gameObject.GetOrAddComponent<Rigidbody>();

            _rigidbody.useGravity = false;

        }

        RaycastHit hit;
        // �Ʒ� �������� ����ĳ��Ʈ�� �߻��մϴ�.
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            // ���̰� Plane�� ��Ҵٸ�
            if (hit.collider.CompareTag("Plane")) // "Ground" �±׸� ���� Plane���� �����ϰ� �˴ϴ�.
            {
                // �׸��ڰ� ���� ���ٸ� �����մϴ�.
                if (Shadow == null)
                {
                    Shadow = Managers.Resource.Instantiate("Shadow", transform);
                    shadowStartPoint = hit.point;
                    Shadow.transform.position = shadowStartPoint + new Vector3(0, 0.01f, 0); // 0.01f�� �߰��Ͽ� �ٴڿ��� �ణ ���� ��ġ�ϰ� �մϴ�.
                    Shadow.transform.localScale = baseShadowScale;
                }
                else
                {
                    // �׸��ڰ� �̹� �ִٸ� ��ġ�� ������Ʈ �մϴ�.
                    Shadow.transform.position = hit.point + new Vector3(0, 0.01f, 0); // 0.01f�� �߰��Ͽ� �ٴڿ��� �ణ ���� ��ġ�ϰ� �մϴ�.
                    shadowStartPoint = hit.point;
                }
            }
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
        Shadow.gameObject.SetActive(true);
        Shadow.transform.position = shadowStartPoint + new Vector3(0, 0.001f, 0); // 0.01f�� �߰��Ͽ� �ٴڿ��� �ణ ���� ��ġ�ϰ� �մϴ�.
        Shadow.transform.localScale = baseShadowScale;
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
        if( Managers.Game.GameMode == Define.GameMode.Challenge &&Managers.Game.GameState == Define.GameState.End)
        {
            if(gameObject.activeSelf == true)
            {
                gameObject.SetActive(false);
            }
            return;
        }

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

                {
                    var moveShadowVec = new Vector3(transform.position.x, shadowStartPoint.y + 0.001f, transform.position.z);
                    Shadow.transform.position = moveShadowVec;
                }

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
        Shadow.gameObject.SetActive(false);
    }

    public bool GetHit()
    {
        return _hit;
    }

    public Rigidbody GetRigid()
    {
        return _rigidbody;
    }


    private void LateUpdate()
    {
       
        if(transform.position.z >= 100)
        {
#if UNITY_EDITOR
            Debug.Log("TODO ���� ");
#endif
            Managers.Obj.Despawn<BallController>(ObjId);
        }
    }

    public void CreateEffect()
    {

        if(Managers.Game.HitScore > 30)
        {
            TrailClear();
            Managers.Effect.PlayTrail(Keys.BALL_EFFECT_KEY.Trail_3_BigFire.ToString(), transform.localPosition, TrailRoot);
        }
        else if(Managers.Game.HitScore > 10)
        {
            TrailClear();
            Managers.Effect.PlayTrail(Keys.BALL_EFFECT_KEY.Trail_2_SmallFire.ToString(), transform.localPosition, TrailRoot);
        }
        else if(Managers.Game.HitScore > 3)
        {
            TrailClear();
            Managers.Effect.PlayTrail(Keys.BALL_EFFECT_KEY.Trail_1_Smoke.ToString(), transform.localPosition, TrailRoot);
        }
        else
        {
            TrailClear();
        }
    }


    private void TrailClear()
    {
        foreach (Transform child in TrailRoot.transform)
        {
            Managers.Resource.Destroy(child.gameObject);
        }
    }
}