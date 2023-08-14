using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
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
    float speed = 0.0f; // ���� ������ �ӵ�
    [SerializeField]float originalSpeed = 15.0f; // ���� ������ �ӵ�
    private List<BallMovement> balls = new List<BallMovement>();
    private Vector3 initialControlPointPosition; // �������� �ʱ� ��ġ
    public GameObject cubePrefab; // ť�� ��ü
    [SerializeField] private bool _hEyes = false;
    [SerializeField] private float _ballerDistance = 0.0f;

    [SerializeField] private ThrowType _throwType;

    enum ThrowType
    {
        FastBall,
        Curve,
        Slider,
        ChangUp,
        Sinker,
        ExCurve,
        NormalCurve,
        Knuckleball,
        TwoSeamFastball,
        Splitter,
        COUNT
    }

    void Start()
    {
        initialControlPointPosition = controlPoint.position;
        pathRenderer = GetComponent<LineRenderer>();
        originPath = startPoint;

        StartCoroutine(c_Baller());
    }

    IEnumerator c_Baller()
    {
        while (true)
        {

            yield return new WaitForSeconds(2f);
            _throwType = (ThrowType)Random.RandomRange(0, (int)ThrowType.COUNT-1);

            Debug.Log($"Throw Type {_throwType}");

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



        

        var randomPoint = endPoint.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), -0.37f);

        if(_hEyes == true)
            cubePrefab.transform.position = randomPoint + new Vector3(0, 0, +0.37f);
        else
            cubePrefab.transform.position = randomPoint + new Vector3(999, 999, 999);

        for (int i = 0; i < resolution; i++)
        {
            float t = i / (float)(resolution - 1);
            Vector3 position = CalculateBezierPoint(t, startPoint.position + new Vector3(0,0, _ballerDistance), controlPoint.position, randomPoint);
            pathRenderer.SetPosition(i, position);
            pathPoints.Add(position);
        }
    }
}
