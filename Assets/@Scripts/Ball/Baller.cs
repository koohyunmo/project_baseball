using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using static Define;

public class Baller : MonoBehaviour
{

    Define.GameState GameState { get { return Managers.Game.GameState; } }
    public GameObject ballPrefab; // �� ��ü
    public BallScriptableObject ballSO;
    public Transform startPoint; // ���� ���� ��ġ
    private Transform originPath; // ���� ���� ��ġ
    public Transform endPoint; // ��Ʈ����ũ ��
    public Transform controlPoint; // Bezier ��� ������
    public LineRenderer pathRenderer; // ��θ� ǥ���ϴ� ���� ������
    private List<Vector3> pathPoints = new List<Vector3>(); // ����� ��� ����Ʈ
    private int currentPointIndex = 0; // ���� ��ǥ ����Ʈ
    float speed = 0.0f; // ���� ������ �ӵ�
    [SerializeField] float originalSpeed = 41.67f; // 150 km/h to m/s
    private Vector3 initialControlPointPosition; // �������� �ʱ� ��ġ
    public GameObject ballAimPrefab; // ȣũ���̿���
    public GameObject strikeAimPrefab; // ��Ʈ����ũ����
    [SerializeField] private bool _hEyes = false;
    [SerializeField] private float _ballerDistance = 0.0f;

    [SerializeField] private ThrowType _throwType;
    [SerializeField] private League League { get { return Managers.Game.League; } }


    static int _ballerCount = 0;
    private bool _stopBaller = false;

    private bool first;

    private string _prevId;


    void Start()
    {
        Init();
    }


    private void Init()
    {
        Managers.Game.SetBaller(this);

        initialControlPointPosition = controlPoint.position;
        pathRenderer = GetComponent<LineRenderer>();
        originPath = startPoint;

        StartCoroutine(c_Baller());

        Managers.Game.SetHitCallBack(Thrw);
        Managers.Game.SetBallPath(RePlay);
        Managers.Game.SetStrikeCallBack(StrikeAim);


        var ballId = Managers.Game.EquipBallId;
        ballSO = Managers.Resource.GetScriptableObjet<BallScriptableObject>(ballId);

        _prevId = ballId;

        ballPrefab = ballSO.model;
        ballPrefab.name = ballSO.id;
        ballAimPrefab = Managers.Resource.Load<GameObject>("BallAim");
        ballAimPrefab.name = ballAimPrefab.name;
        strikeAimPrefab = Managers.Resource.Load<GameObject>("StrikeBallAim");
        strikeAimPrefab.name = strikeAimPrefab.name;
    }

    IEnumerator c_Baller()
    {

        while (true)
        {
            switch (GameState)
            {
                case Define.GameState.Home:
                    TriggersReset();
                    yield return null;
                    break;
                case Define.GameState.Ready:
                    TriggersReset();
                    yield return null;
                    break;
                case Define.GameState.InGround:
                    if (first == false)
                    {
                        first = true;
                        pathRenderer.enabled = true;
                        yield return new WaitForSeconds(1.5f);
                        Managers.Game.isRecord = true;
                        Thrw();
                    }
                    yield return null;
                    break;
                case Define.GameState.End:
                    if (_stopBaller == false)
                    {
                        _stopBaller = true;
                    }
                    yield return null;
                    break;
            }

        }
    }


    private void TriggersReset()
    {
        pathRenderer.enabled = false;
        first = false;
        _stopBaller = false;
        pathRenderer.widthMultiplier = 0.0f;
    }

    private void StrikeAim()
    {
        var go = Managers.Object.Spawn<InGameObjectController>(strikeAimPrefab.name, Managers.Game.AimPoint);
        //var go = Managers.Resource.Instantiate("ballTestAimPrefab.name");
        go.transform.position = Managers.Game.AimPoint;
        go.name = $"{_throwType} : {Managers.Game.Speed.ToString("F2")}km/s";
    }

    private void Thrw()
    {
        if (GameState != Define.GameState.InGround)
            return;

        _throwType = (ThrowType)Random.RandomRange(0, (int)ThrowType.COUNT - 1);


        switch (_throwType)
        {
            case ThrowType.FastBall:
                ThrowBall(ThrowFastBall);
                break;
            case ThrowType.Curve:
                ThrowBall(ThrowCurve);
                break;
            case ThrowType.Slider:
                ThrowBall(ThrowSlider);
                break;
            case ThrowType.ChangUp:
                ThrowBall(ThrowChangeUp);
                break;
            case ThrowType.Sinker:
                ThrowBall(ThrowSinker);
                break;
            case ThrowType.ExCurve:
                ThrowBall(ThrowExaggeratedCurveball);
                break;
            case ThrowType.NormalCurve:
                ThrowBall(ThrowNormalCurveball);
                break;
            case ThrowType.Knuckleball:
                ThrowBall(ThrowKnuckleball);
                break;
            case ThrowType.TwoSeamFastball:
                ThrowBall(ThrowTwoSeamFastball);
                break;
            case ThrowType.Splitter:
                ThrowBall(ThrowSplitter);
                break;
        }
    }



    void FixedUpdate()
    {
        if (_stopBaller == true)
            return;


        /*
        if (Managers.Object.BallDict.Count < 0)
            return;
        // ���� ����
        List<int> ballKeysToRemove = new List<int>(Managers.Object.BallDict.Keys);
        foreach (var key in ballKeysToRemove)
        {
            Managers.Object.BallDict[key].MoveAlongPath();
        }
        */
    }

    public void ThrowCurveBall()
    {
        ResetPath();
        GenerateCurvePath();
        // ���⼭ ���� ������ ������ �߰��� �� �ֽ��ϴ�.
    }

    void ResetPath()
    {
        pathPoints.Clear();
        currentPointIndex = 0;
        pathRenderer.positionCount = 0;
        transform.position = originPath.position;
    }

    void GenerateCurvePath()
    {
        int resolution = 10; // ����� �ػ�
        pathRenderer.positionCount = resolution;

        // �������� ��ġ�� �ణ �����Ͽ� ����� ��ȭ�� ����ϴ�.
        controlPoint.position += new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));

        for (int i = 0; i < resolution; i++)
        {
            float t = i / (float)(resolution - 1);
            Vector3 position = CalculateBezierPoint(t, startPoint.position, controlPoint.position, endPoint.position);
            pathRenderer.SetPosition(i, position);
            pathPoints.Add(position);
        }
    }

    Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;

        return p;
    }

    private void SetLeagueSpeed()
    {
        // ���̵��� ����
        switch (League)
        {
            case League.Major:
                originalSpeed = Random.Range(41.67f, 44.44f);
                break;
            case League.Mainor:
                originalSpeed = Random.Range(41.67f, 44.44f) * Random.Range(0.9f, 0.95f);
                break;
            case League.Amateur:
                originalSpeed = Random.Range(41.67f, 44.44f) * Random.Range(0.8f, 0.88f);
                break;
            case League.SemiPro:
                originalSpeed = Random.Range(41.67f, 44.44f) * Random.Range(0.7f, 0.82f);
                break;
            case League.TEST:
                originalSpeed = Random.Range(41.67f, 44.44f) * Random.Range(0.3f, 0.4f);
                break;

        }
    }
    private void SetThrowTypeSpeed()
    {

        switch (_throwType)
        {
            case ThrowType.FastBall:
                speed = originalSpeed * 1.0f;
                break;
            case ThrowType.Curve:
                speed = originalSpeed * 0.83f;
                break;
            case ThrowType.Slider:
                speed = originalSpeed * 0.92f;
                break;
            case ThrowType.ChangUp:
                speed = originalSpeed * 0.87f;
                break;
            case ThrowType.Sinker:
                speed = originalSpeed * 0.98f;
                break;
            case ThrowType.ExCurve:
                speed = originalSpeed * 0.65f;
                break;
            case ThrowType.NormalCurve:
                speed = originalSpeed * 0.75f;
                break;
            case ThrowType.Knuckleball:
                speed = originalSpeed * 0.72f;
                break;
            case ThrowType.TwoSeamFastball:
                speed = originalSpeed * 0.98f;
                break;
            case ThrowType.Splitter:
                speed = originalSpeed * 0.92f;
                break;
        }
    }

    private Transform ball = null;
    void ThrowBall(System.Action generatePathMethod)
    {
        ball = null;

        SetLeagueSpeed();
        SetThrowTypeSpeed();


        Managers.Game.SetBallInfo(speed, _throwType);

        controlPoint.position = initialControlPointPosition; // �������� �ʱ� ��ġ�� �缳��

        //var pathEnd = new Vector3(endPoint.position.x, endPoint.position.y, endPoint.position.z - 0.5f);

        //GameObject ballInstance = Instantiate(ballPrefab, startPoint.position, Quaternion.identity);


        if(_prevId != Managers.Game.EquipBallId)
        {
            var ballId = Managers.Game.EquipBallId;
            ballPrefab = Managers.Resource.GetScriptableObjet<BallScriptableObject>(ballId).model;
        }


        var ballInstance = Managers.Object.Spawn<BallController>(ballPrefab, startPoint.position);
        

        ballInstance.transform.position = startPoint.position;
        ballInstance.transform.rotation = Quaternion.identity;
        ballInstance.transform.name = ballPrefab.name;

        ballInstance.speed = speed;
        ballInstance.startPoint = startPoint;
        ballInstance.endPoint = endPoint;
        ballInstance.controlPoint = controlPoint;
        ballInstance.pathRenderer = pathRenderer;

        ball = ballInstance.transform;
        generatePathMethod.Invoke();

        ballInstance.SetPath(pathRenderer);

        CheckStrikeZone(startPoint, endPoint);

        Managers.Game.ThorwBallEvent();
        Managers.Game.SetStrikePath(pathRenderer);

        _ballerCount++;
    }

    void CheckStrikeZone(Transform sp, Transform ep)
    {
        // Ray�� ���� �������� ���� ���� �������� �߻�
        Ray ray = new Ray(sp.position, ep.position - sp.position);
        RaycastHit hit;

        // Ray�� ��Ʈ����ũ ���� ��Ҵ��� Ȯ��
        if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("StrikeZone"))
        {
            Debug.DrawRay(ray.origin, ray.direction);
            Debug.Log("��Ʈ����ũ ���� ");
        }
        else
        {
            Debug.Log("Ball");
        }
    }

    #region ����
    public void ThrowFastBall()
    {
        ResetPath();
        controlPoint.position = (startPoint.position + endPoint.position) / 2; // �߰� ����
        GeneratePath();
    }

    public void ThrowCurve()
    {
        ResetPath();
        controlPoint.position += new Vector3(0, Random.Range(1f, 2f), Random.Range(-1f, 1f)); // ���� �ణ ��/�ڷ� �̵�
        GeneratePath();
    }

    public void ThrowSlider()
    {
        ResetPath();
        controlPoint.position += new Vector3(Random.Range(-2f, 2f), 0, 0); // ��/��� �̵�
        GeneratePath();
    }

    public void ThrowChangeUp()
    {
        ResetPath();
        controlPoint.position += new Vector3(0, Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)); // �ణ�� ������ ��ġ ��ȭ
        GeneratePath();
    }

    public void ThrowSinker()
    {
        ResetPath();
        controlPoint.position += new Vector3(0, Random.Range(-2f, -1f), 0); // �Ʒ��� �̵�
        GeneratePath();
    }

    public void ThrowExaggeratedCurveball()
    {
        ResetPath();
        controlPoint.position += new Vector3(Random.Range(-1f, 1f), Random.Range(2f, 3f), Random.Range(-2f, 2f)); // ũ�� ���� ��/�ڷ� �̵�
        GeneratePath();
    }

    public void ThrowNormalCurveball()
    {
        ResetPath();
        controlPoint.position += new Vector3(0, Random.Range(1f, 2f), Random.Range(-1f, 1f)); // ���� �ణ ��/�ڷ� �̵�
        GeneratePath();
    }

    public void ThrowKnuckleball()
    {
        ResetPath();
        controlPoint.position += new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)); // ũ�� ������ ��ġ ��ȭ
        GeneratePath();
    }

    public void ThrowTwoSeamFastball()
    {
        ResetPath();
        controlPoint.position += new Vector3(Random.Range(-0.5f, 0), 0, 0); // �ణ �������� �̵� (���������� ���� ����)
        GeneratePath();
    }

    public void ThrowSplitter()
    {
        ResetPath();
        controlPoint.position += new Vector3(0, Random.Range(-1.5f, -0.5f), 0); // �Ʒ��� �� ������
        GeneratePath();
    }

    #endregion

    #region ����


    public void ThrowMagicBall()
    {
        ResetPath();
        controlPoint.position = (startPoint.position + endPoint.position) * 2; // �߰� ����
        GeneratePath();

    }

    #endregion
    // ������ GenerateCurvePath �Լ��� GeneratePath�� �̸� ����
    void GeneratePath()
    {
        GeneratePathBazierRayCast();
    }
    void GeneratePathBazierRayCast()
    {
        int resolution = 20; // ����� �ػ�
        pathRenderer.positionCount = resolution;

        var randomPoint = endPoint.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), -5f);

        for (int i = 0; i < resolution; i++)
        {

            float t = i / (float)(resolution - 1);
            Vector3 position = CalculateBezierPoint(t, startPoint.position + new Vector3(0, 0, _ballerDistance), controlPoint.position, randomPoint);
            pathRenderer.SetPosition(i, position);
            pathPoints.Add(position);
        }

        var aimPoint = CheckLineRendererHit();
        Managers.Game.AimPoint = aimPoint;

        // ȣũ���̿���
        if (_hEyes == true)
        {
            var go = Managers.Object.Spawn<BallAimController>(ballAimPrefab.name, aimPoint);
            go.DataInit(aimPoint, ball);
        }


    }


    Vector3 CheckLineRendererHit()
    {
        int numberOfPoints = pathRenderer.positionCount;


        Vector3 startPoint = pathRenderer.GetPosition(numberOfPoints - 2);
        Vector3 endPoint = pathRenderer.GetPosition(numberOfPoints - 3);
        Debug.DrawLine(startPoint, endPoint, Color.red, 5f);

        RaycastHit hit;
        if (Physics.Linecast(startPoint, endPoint, out hit))
        {
            // ����Ʈ �ݶ��̴��� �浹�ߴٸ�
            if (hit.collider.CompareTag("StrikeZone"))
            {
                var hitpoint = hit.point;
                hitpoint.z = Managers.Game.StrikeZone.transform.position.z;
                return hitpoint;
            }
        }
        // ���� �������� ������ ���������� �׸�


        return Vector3.zero;
    }
    private void RePlay(LineRenderer renderer)
    {

        StartCoroutine(co_RelplayFollowBall());
    }

    private IEnumerator co_RelplayFollowBall()
    {
        var cam = Camera.main;
        var camManager = Camera.main.gameObject.GetComponent<CameraManager>();


        // widthCurve�� �����ϴ� �κ�
        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0, 1f);
        curve.AddKey(1, 1f);

        pathRenderer.widthCurve = curve;

        // �������� �ʺ� ����
        pathRenderer.widthMultiplier = 0.01f;

        if (_prevId != Managers.Game.EquipBallId)
        {
            var ballId = Managers.Game.EquipBallId;
            ballPrefab = Managers.Resource.GetScriptableObjet<BallScriptableObject>(ballId).model;
        }
        var replayBall = Managers.Object.Spawn<BallController>(ballPrefab, pathRenderer.GetPosition(0));
        //camManager.OnReplay(replayBall.transform);
        camManager.OnReplay(replayBall.transform, transform.position);

        float replaySpeed = speed;

        var forwardDistacne = (transform.position - Managers.Game.StrikeZone.transform.position) / (pathRenderer.positionCount - 1);

        for (int i = 0; i < pathRenderer.positionCount - 1; i++)
        {
            Vector3 startPoint = replayBall.transform.position;
            Vector3 endPoint = pathRenderer.GetPosition(i + 1);

            

            float journeyLength = Vector3.Distance(startPoint, endPoint);
            float journeyProgress = 0;

            while (journeyProgress < journeyLength)
            {
                if (GameState == GameState.Home)
                {
                    Managers.Game.isReplay = false;
                    yield break;
                }

                float distanceToMove = replaySpeed * Time.deltaTime;
                journeyProgress += distanceToMove;

                float fractionOfJourney = journeyProgress / journeyLength;

                replayBall.transform.position = Vector3.Lerp(startPoint, endPoint, fractionOfJourney);

                camManager.CameraMove(replayBall.transform.position);

                yield return new FixedUpdate();
            }

            if (i == pathRenderer.positionCount - 2)
            {
                replaySpeed = replaySpeed * Managers.Game.ReplaySlowMode;
                Managers.Game.StrikeEvent();
            }


        }


        Managers.Game.isReplay = false;
        
        yield return new WaitForSeconds(5f);

        if(Managers.Game.GameState == GameState.End)
            camManager.ReplayBack(replayBall.transform, transform.position);

        //yield break;
    }



}
