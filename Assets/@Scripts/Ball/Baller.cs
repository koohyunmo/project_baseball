using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using static Define;

public class Baller : MonoBehaviour
{

    Define.GameState GameState { get { return Managers.Game.GameState; } }
    public GameObject ballPrefab; // 공 객체
    public BallScriptableObject ballSO;
    public Transform startPoint; // 공의 시작 위치
    private Transform originPath; // 공의 시작 위치
    public Transform endPoint; // 스트라이크 존
    public Transform controlPoint; // Bezier 곡선의 제어점
    public LineRenderer pathRenderer; // 경로를 표시하는 라인 렌더러
    private List<Vector3> pathPoints = new List<Vector3>(); // 경로의 모든 포인트
    private int currentPointIndex = 0; // 현재 목표 포인트
    float speed = 0.0f; // 공의 움직임 속도
    [SerializeField] float originalSpeed = 41.67f; // 150 km/h to m/s
    private Vector3 initialControlPointPosition; // 제어점의 초기 위치
    public GameObject ballAimPrefab; // 호크가이에임
    public GameObject strikeAimPrefab; // 스트라이크에임
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
        // 복사 삭제
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
        // 여기서 공을 던지는 로직을 추가할 수 있습니다.
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
        int resolution = 10; // 경로의 해상도
        pathRenderer.positionCount = resolution;

        // 제어점의 위치를 약간 변경하여 경로의 변화를 만듭니다.
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
        // 난이도별 구속
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

        controlPoint.position = initialControlPointPosition; // 제어점을 초기 위치로 재설정

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
        // Ray를 시작 지점에서 도착 지점 방향으로 발사
        Ray ray = new Ray(sp.position, ep.position - sp.position);
        RaycastHit hit;

        // Ray가 스트라이크 존에 닿았는지 확인
        if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("StrikeZone"))
        {
            Debug.DrawRay(ray.origin, ray.direction);
            Debug.Log("스트라이크 존안 ");
        }
        else
        {
            Debug.Log("Ball");
        }
    }

    #region 구종
    public void ThrowFastBall()
    {
        ResetPath();
        controlPoint.position = (startPoint.position + endPoint.position) / 2; // 중간 지점
        GeneratePath();
    }

    public void ThrowCurve()
    {
        ResetPath();
        controlPoint.position += new Vector3(0, Random.Range(1f, 2f), Random.Range(-1f, 1f)); // 위와 약간 앞/뒤로 이동
        GeneratePath();
    }

    public void ThrowSlider()
    {
        ResetPath();
        controlPoint.position += new Vector3(Random.Range(-2f, 2f), 0, 0); // 좌/우로 이동
        GeneratePath();
    }

    public void ThrowChangeUp()
    {
        ResetPath();
        controlPoint.position += new Vector3(0, Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)); // 약간의 무작위 위치 변화
        GeneratePath();
    }

    public void ThrowSinker()
    {
        ResetPath();
        controlPoint.position += new Vector3(0, Random.Range(-2f, -1f), 0); // 아래로 이동
        GeneratePath();
    }

    public void ThrowExaggeratedCurveball()
    {
        ResetPath();
        controlPoint.position += new Vector3(Random.Range(-1f, 1f), Random.Range(2f, 3f), Random.Range(-2f, 2f)); // 크게 위와 앞/뒤로 이동
        GeneratePath();
    }

    public void ThrowNormalCurveball()
    {
        ResetPath();
        controlPoint.position += new Vector3(0, Random.Range(1f, 2f), Random.Range(-1f, 1f)); // 위와 약간 앞/뒤로 이동
        GeneratePath();
    }

    public void ThrowKnuckleball()
    {
        ResetPath();
        controlPoint.position += new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)); // 크게 무작위 위치 변화
        GeneratePath();
    }

    public void ThrowTwoSeamFastball()
    {
        ResetPath();
        controlPoint.position += new Vector3(Random.Range(-0.5f, 0), 0, 0); // 약간 왼쪽으로 이동 (오른손잡이 투수 기준)
        GeneratePath();
    }

    public void ThrowSplitter()
    {
        ResetPath();
        controlPoint.position += new Vector3(0, Random.Range(-1.5f, -0.5f), 0); // 아래로 더 떨어짐
        GeneratePath();
    }

    #endregion

    #region 마구


    public void ThrowMagicBall()
    {
        ResetPath();
        controlPoint.position = (startPoint.position + endPoint.position) * 2; // 중간 지점
        GeneratePath();

    }

    #endregion
    // 기존의 GenerateCurvePath 함수를 GeneratePath로 이름 변경
    void GeneratePath()
    {
        GeneratePathBazierRayCast();
    }
    void GeneratePathBazierRayCast()
    {
        int resolution = 20; // 경로의 해상도
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

        // 호크아이여부
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
            // 포인트 콜라이더와 충돌했다면
            if (hit.collider.CompareTag("StrikeZone"))
            {
                var hitpoint = hit.point;
                hitpoint.z = Managers.Game.StrikeZone.transform.position.z;
                return hitpoint;
            }
        }
        // 라인 랜더러의 선분을 빨간색으로 그림


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


        // widthCurve를 설정하는 부분
        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0, 1f);
        curve.AddKey(1, 1f);

        pathRenderer.widthCurve = curve;

        // 절대적인 너비 설정
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
