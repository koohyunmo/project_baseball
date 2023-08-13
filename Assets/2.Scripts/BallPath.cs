using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPath : MonoBehaviour
{
    public GameObject ballPrefab; // 공 객체
    public Transform startPoint; // 공의 시작 위치
    private Transform originPath; // 공의 시작 위치
    public Transform endPoint; // 스트라이크 존
    public Transform controlPoint; // Bezier 곡선의 제어점
    public LineRenderer pathRenderer; // 경로를 표시하는 라인 렌더러
    private List<Vector3> pathPoints = new List<Vector3>(); // 경로의 모든 포인트
    private int currentPointIndex = 0; // 현재 목표 포인트
    public float speed = 5.0f; // 공의 움직임 속도
    private List<BallMovement> balls = new List<BallMovement>();
    private Vector3 initialControlPointPosition; // 제어점의 초기 위치
    public GameObject cubePrefab; // 큐브 객체

    void Start()
    {
        initialControlPointPosition = controlPoint.position;
        pathRenderer = GetComponent<LineRenderer>();
        originPath = startPoint;
    }

    void Update()
    {
        // 각 키에 따라 구종을 던집니다.
        if (Input.GetKeyDown(KeyCode.F))
        {

           ThrowBall(ThrowFastBall);
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            ThrowBall(ThrowCurve);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            ThrowBall(ThrowSlider);
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {

            ThrowBall(ThrowChangeUp);
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            ThrowBall(ThrowSinker);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            ThrowBall(ThrowExaggeratedCurveball);
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            ThrowBall(ThrowNormalCurveball);
        }

        foreach (var ball in balls)
        {
            ball.MoveAlongPath();
        }
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
        int resolution = 20; // 경로의 해상도
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

    void ThrowBall(System.Action generatePathMethod)
    {
        controlPoint.position = initialControlPointPosition; // 제어점을 초기 위치로 재설정

        //var pathEnd = new Vector3(endPoint.position.x, endPoint.position.y, endPoint.position.z - 0.5f);

        GameObject ballInstance = Instantiate(ballPrefab, startPoint.position, Quaternion.identity);
        BallMovement ballMovement = ballInstance.AddComponent<BallMovement>();
        ballMovement.speed = speed;
        ballMovement.startPoint = startPoint;
        ballMovement.endPoint = endPoint;
        ballMovement.controlPoint = controlPoint;
        ballMovement.pathRenderer = pathRenderer;

        generatePathMethod.Invoke();

        ballMovement.SetPath(pathRenderer);
        balls.Add(ballMovement);

        CheckStrikeZone(startPoint, endPoint);
    }

    void CheckStrikeZone(Transform sp, Transform ep)
    {
        // Ray를 시작 지점에서 도착 지점 방향으로 발사
        Ray ray = new Ray(sp.position, ep.position - sp.position);
        RaycastHit hit;

        // Ray가 스트라이크 존에 닿았는지 확인
        if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("StrikeZone"))
        {
            // 스트라이크 존에 닿았다면 큐브 생성
            var ballAim = Instantiate(cubePrefab, hit.point, Quaternion.identity);

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

    #endregion
    // 기존의 GenerateCurvePath 함수를 GeneratePath로 이름 변경
    void GeneratePath()
    {
        int resolution = 20; // 경로의 해상도
        pathRenderer.positionCount = resolution;

        for (int i = 0; i < resolution; i++)
        {
            float t = i / (float)(resolution - 1);
            Vector3 position = CalculateBezierPoint(t, startPoint.position, controlPoint.position, endPoint.position);
            pathRenderer.SetPosition(i, position);
            pathPoints.Add(position);
        }
    }
}
