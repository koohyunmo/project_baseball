using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using static Define;
using static UnityEngine.ParticleSystem;

public class BallPath : MonoBehaviour
{

    Define.GameState GameState { get { return Managers.Game.GameState; } }
    public GameObject ballPrefab; // �� ��ü
    public Transform startPoint; // ���� ���� ��ġ
    private Transform originPath; // ���� ���� ��ġ
    public Transform endPoint; // ��Ʈ����ũ ��
    public Transform controlPoint; // Bezier ��� ������
    public LineRenderer pathRenderer; // ��θ� ǥ���ϴ� ���� ������
    private List<Vector3> pathPoints = new List<Vector3>(); // ����� ��� ����Ʈ
    private int currentPointIndex = 0; // ���� ��ǥ ����Ʈ
    float speed = 0.0f; // ���� ������ �ӵ�
    [SerializeField]float originalSpeed = 41.67f; // 150 km/h to m/s
    private Dictionary<int,BallMovement> ballDict = new Dictionary<int, BallMovement>();
    private Vector3 initialControlPointPosition; // �������� �ʱ� ��ġ
    public GameObject ballAimPrefab; // ť�� ��ü
    [SerializeField] private bool _hEyes = false;
    [SerializeField] private float _ballerDistance = 0.0f;

    [SerializeField] private ThrowType _throwType;
    [SerializeField] private League League { get { return Managers.Game.League; } }

    [SerializeField] private List<GameObject> ballAims = new List<GameObject>();


    static int _ballerCount = 0;
    private bool _stopBaller = false;

    private bool first;


    void Start()
    {
        initialControlPointPosition = controlPoint.position;
        pathRenderer = GetComponent<LineRenderer>();
        originPath = startPoint;

        StartCoroutine(c_Baller());

        Managers.Game.SetHitCallBack(Thrw);
        Managers.Game.SetReplayGame(RePlay);
    }

    IEnumerator c_Baller()
    {

        while (true)
        {
            switch (GameState)
            {
                case Define.GameState.Home:
                    _stopBaller = false;
                    pathRenderer.enabled = false;
                    first = false;
                    yield return null;
                    break;
                case Define.GameState.Ready:
                    yield return null;
                    break;
                case Define.GameState.InGround:
                    if (first == false)
                    {
                        pathRenderer.enabled = true;
                       yield return new WaitForSeconds(1.5f);
                        first = true;
                        Thrw();
                    }
                    yield return null;
                    break;
                case Define.GameState.End:
                    if (_stopBaller == false)
                    {
                        _stopBaller = true;
                        ObjectClear();
                    }
                    yield return null;
                    break;
            }

        }
    }

    private void ObjectClear()
    {
        ClearLine();
        //StartCoroutine(co_Clear());
    }
    private void ClearLine()
    {
        //pathRenderer.positionCount = 0;
    }


    IEnumerator co_Clear()
    {
        foreach (var item in ballAims)
        {
            Managers.Resource.Destroy(item);
            yield return null;
        }

        ballAims.Clear();
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
        {
            if (ballDict.Count > 0)
            {
                foreach (var item in ballDict.Values)
                {
                    Destroy(item.gameObject);
                }
                ballDict.Clear();
            }
            else
                return;
        }

        if (ballDict.Count <= 0)
            return;

        var balls = new List<BallMovement>(ballDict.Values);
        foreach (var ball in balls)
        {
            ball.MoveAlongPath();
        }
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
        int resolution = 20; // ����� �ػ�
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

        GameObject ballInstance = Instantiate(ballPrefab, startPoint.position, Quaternion.identity);
        BallMovement ballMovement = ballInstance.AddComponent<BallMovement>();
        ballInstance.tag = "Ball";
        ballMovement.speed = speed;
        ballMovement.startPoint = startPoint;
        ballMovement.endPoint = endPoint;
        ballMovement.controlPoint = controlPoint;
        ballMovement.pathRenderer = pathRenderer;
        ballMovement.ballId = _ballerCount;
        ballMovement.ballClearAction = ((int id) =>
        {
            ballDict.Remove(id);

        });


        ball = ballInstance.transform;
        generatePathMethod.Invoke();

        ballMovement.SetPath(pathRenderer);
        //balls.Add(ballMovement);
        _ballerCount++;
        ballDict.Add(_ballerCount, ballMovement);

        CheckStrikeZone(startPoint, endPoint);

        Managers.Game.ThorwBallEvent();
        Managers.Game.SerStrikePath(pathRenderer);
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
            Debug.Log("��Ʈ����ũTODO");
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
    // ������ GenerateCurvePath �Լ��� GeneratePath�� �̸� ����
    void GeneratePath()
    {
        int resolution = 20; // ����� �ػ�
        pathRenderer.positionCount = resolution;

        var randomPoint = endPoint.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), -2f);

        var aimPoint = randomPoint + new Vector3(0, 0, +2.25f);

        Managers.Game.AimPoint = aimPoint;

        if (_hEyes == true)
        {
            var go = Instantiate(ballAimPrefab, aimPoint, Quaternion.identity);
            go.GetOrAddComponent<BallAim>().DataInit(aimPoint, ball);
            ballAims.Add(go);
        }
            
        // var posu = Instantiate(cubePrefab, randomPoint, Quaternion.identity);
        // Destroy(posu, 2);

        for (int i = 0; i < resolution; i++)
        {
            float t = i / (float)(resolution - 1);
            Vector3 position = CalculateBezierPoint(t, startPoint.position + new Vector3(0,0, _ballerDistance), controlPoint.position, randomPoint);
            pathRenderer.SetPosition(i, position);
            pathPoints.Add(position);
        }
    }

    private void RePlay(LineRenderer renderer)
    {
        List<GameObject> balls = new List<GameObject>();

        //for (int i = 0; i < renderer.positionCount; i++)
        //{
        //    GameObject go = Instantiate(ballPrefab, renderer.GetPosition(i), Quaternion.identity);
        //    balls.Add(go);
        //}

        StartCoroutine(FollowPath());

        foreach (var item in balls)
        {
            Destroy(item, 3.0f);
        }

        
    }


    /// <summary>
    /// ���÷��� �Լ�
    /// </summary>
    /// <returns></returns>
    private IEnumerator FollowPath()
    {
        // ī�޶� ��ġ��
        // ����, ����, �װ��� ,�� 4��
        var cam = Camera.main;

        var replayBall = Instantiate(ballPrefab);
        replayBall.transform.position = pathRenderer.GetPosition(0);
        //cam.transform.rotation = Quaternion.Euler(0, 180, 0);
        cam.transform.rotation = Quaternion.Euler(0, -90, 0);
        float replaySpeed = speed;

        for (int i = 0; i < pathRenderer.positionCount; i++)
        {
            Vector3 targetPosition = pathRenderer.GetPosition(i); // ���� �ݺ��� ��ġ�� ����

            while (Vector3.Distance(replayBall.transform.position, targetPosition) > 0.05f)
            {
                if (GameState == GameState.Home)
                {
                    Managers.Game.isReplay = false;
                    yield break;
                }

                float step = replaySpeed * Time.deltaTime;
                Vector3 moveDirection = (targetPosition - replayBall.transform.position).normalized;
                replayBall.transform.position += moveDirection * step;
                cam.transform.position = replayBall.transform.position + new Vector3(10f, 0, 0);
                yield return null;
            }

            if (i == pathRenderer.positionCount - 2)
            {
                replaySpeed = replaySpeed * Managers.Game.ReplaySlowMode;
                Managers.Game.StrikeEvent();
            }

            if (GameState == GameState.Home)
            {
                Managers.Game.isReplay = false;
                yield break;
            }
                


            // ���� ������ �Ѿ�� ���� ��� ��ٸ�
            yield return null;
        }

        Managers.Game.isReplay = false;



    }


}
