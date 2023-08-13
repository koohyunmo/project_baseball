using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPath : MonoBehaviour
{
    public GameObject ballPrefab; // �� ��ü
    public Transform startPoint; // ���� ���� ��ġ
    private Transform originPath; // ���� ���� ��ġ
    public Transform endPoint; // ��Ʈ����ũ ��
    public Transform controlPoint; // Bezier ��� ������
    public LineRenderer pathRenderer; // ��θ� ǥ���ϴ� ���� ������
    private List<Vector3> pathPoints = new List<Vector3>(); // ����� ��� ����Ʈ
    private int currentPointIndex = 0; // ���� ��ǥ ����Ʈ
    public float speed = 5.0f; // ���� ������ �ӵ�
    private List<BallMovement> balls = new List<BallMovement>();
    private Vector3 initialControlPointPosition; // �������� �ʱ� ��ġ
    public GameObject cubePrefab; // ť�� ��ü

    void Start()
    {
        initialControlPointPosition = controlPoint.position;
        pathRenderer = GetComponent<LineRenderer>();
        originPath = startPoint;
    }

    void Update()
    {
        // �� Ű�� ���� ������ �����ϴ�.
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

    void ThrowBall(System.Action generatePathMethod)
    {
        controlPoint.position = initialControlPointPosition; // �������� �ʱ� ��ġ�� �缳��

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
        // Ray�� ���� �������� ���� ���� �������� �߻�
        Ray ray = new Ray(sp.position, ep.position - sp.position);
        RaycastHit hit;

        // Ray�� ��Ʈ����ũ ���� ��Ҵ��� Ȯ��
        if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("StrikeZone"))
        {
            // ��Ʈ����ũ ���� ��Ҵٸ� ť�� ����
            var ballAim = Instantiate(cubePrefab, hit.point, Quaternion.identity);

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

    #endregion
    // ������ GenerateCurvePath �Լ��� GeneratePath�� �̸� ����
    void GeneratePath()
    {
        int resolution = 20; // ����� �ػ�
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
