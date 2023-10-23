using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Tusoo : MonoBehaviour
{
    public GameObject ballPrefab; // �� ��ü
    public Transform tusooTr; // ���� ���� ��ġ
    public Transform strikeZoneTr; // ��Ʈ����ũ ��
    public Transform ballControllTr; // Bezier ��� ������
    public LineRenderer pathRenderer; // ��θ� ǥ���ϴ� ���� ������
    private int currentPointIndex = 0; // ���� ��ǥ ����Ʈ
    float speed = 0.0f; // ���� ������ �ӵ�
    [SerializeField] float originalSpeed = 41.67f; // 150 km/h to m/s
    private Vector3 initialControlPointPosition; // �������� �ʱ� ��ġ
    [SerializeField] private ThrowType _throwType;
    private Transform ball = null;
    private List<Vector3> pathPoints = new List<Vector3>(); // ����� ��� ����Ʈ

    void Start()
    {
        Init();
        StartCoroutine(co_ThorwBall());
    }


    private void Init()
    {
        initialControlPointPosition = ballControllTr.position;
        tusooTr = transform;
        
    }

    IEnumerator co_ThorwBall()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.5f);
            ThrowBall();
        }
    }
    public void ThrowBall()
    {
        _throwType = (ThrowType)Random.RandomRange(0, (int)ThrowType.COUNT - 1);


        switch (_throwType)
        {
            case ThrowType.FastBall:
                CreateBall(ThrowFastBall);
                break;
            case ThrowType.Curve:
                CreateBall(ThrowCurve);
                break;
            case ThrowType.Slider:
                CreateBall(ThrowSlider);
                break;
            case ThrowType.ChangeUp:
                CreateBall(ThrowChangeUp);
                break;
            case ThrowType.Sinker:
                CreateBall(ThrowSinker);
                break;
            case ThrowType.ExCurve:
                CreateBall(ThrowExaggeratedCurveball);
                break;
            case ThrowType.NormalCurve:
                CreateBall(ThrowNormalCurveball);
                break;
            case ThrowType.Knuckleball:
                CreateBall(ThrowKnuckleball);
                break;
            case ThrowType.TwoSeamFastball:
                CreateBall(ThrowTwoSeamFastball);
                break;
            case ThrowType.Splitter:
                CreateBall(ThrowSplitter);
                break;
        }
    }

    void ResetPath()
    {
        pathPoints.Clear();
        currentPointIndex = 0;
        pathRenderer.positionCount = 0;
        transform.position = tusooTr.position;
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

    void CreateBall(System.Action generatePathMethod)
    {
        ball = null;


       // Managers.Game.SetBallInfo(speed, _throwType);

        ballControllTr.position = initialControlPointPosition; // �������� �ʱ� ��ġ�� �缳��

        //var pathEnd = new Vector3(endPoint.position.x, endPoint.position.y, endPoint.position.z - 0.5f);

        GameObject ballinst = Instantiate(ballPrefab, tusooTr.position, Quaternion.identity);
        BallController ballInstance = ballinst.GetOrAddComponent<BallController>();
        //var ballInstance = Managers.Object.Spawn<BallController>(ballPrefab.name, startPoint.position);

        ballInstance.playMode = BallController.PlayMode.Character;
        ballInstance.transform.position = ballControllTr.position;
        ballInstance.transform.rotation = Quaternion.identity;
        ballInstance.transform.name = ballPrefab.name;

        if(speed <= 1.0f)
        {
            Debug.LogWarning("�ӵ� ���� ����");
            speed = 35.0f;
        }

        ballInstance.speed = speed;
        ballInstance.startPoint = ballControllTr;
        ballInstance.endPoint = strikeZoneTr;
        ballInstance.controlPoint = ballControllTr;
        ballInstance.pathRenderer = pathRenderer;

        ball = ballInstance.transform;
        generatePathMethod.Invoke();

        ballInstance.SetPath(pathRenderer);

        CheckStrikeZone(tusooTr, strikeZoneTr);

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
            Debug.Log("��Ʈ����ũ �� �� ");
        }
        else
        {
            Debug.Log("��Ʈ����ũ �� ��");
        }
    }

    #region ����
    public void ThrowFastBall()
    {
        ResetPath();
        ballControllTr.position = (tusooTr.position + strikeZoneTr.position) / 2; // �߰� ����
        GeneratePath();
    }

    public void ThrowCurve()
    {
        ResetPath();
        ballControllTr.position += new Vector3(0, Random.Range(1f, 2f), Random.Range(-1f, 1f)); // ���� �ణ ��/�ڷ� �̵�
        GeneratePath();
    }

    public void ThrowSlider()
    {
        ResetPath();
        ballControllTr.position += new Vector3(Random.Range(-2f, 2f), 0, 0); // ��/��� �̵�
        GeneratePath();
    }

    public void ThrowChangeUp()
    {
        ResetPath();
        ballControllTr.position += new Vector3(0, Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)); // �ణ�� ������ ��ġ ��ȭ
        GeneratePath();
    }

    public void ThrowSinker()
    {
        ResetPath();
        ballControllTr.position += new Vector3(0, Random.Range(-2f, -1f), 0); // �Ʒ��� �̵�
        GeneratePath();
    }

    public void ThrowExaggeratedCurveball()
    {
        ResetPath();
        ballControllTr.position += new Vector3(Random.Range(-1f, 1f), Random.Range(2f, 3f), Random.Range(-2f, 2f)); // ũ�� ���� ��/�ڷ� �̵�
        GeneratePath();
    }

    public void ThrowNormalCurveball()
    {
        ResetPath();
        ballControllTr.position += new Vector3(0, Random.Range(1f, 2f), Random.Range(-1f, 1f)); // ���� �ణ ��/�ڷ� �̵�
        GeneratePath();
    }

    public void ThrowKnuckleball()
    {
        ResetPath();
        ballControllTr.position += new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)); // ũ�� ������ ��ġ ��ȭ
        GeneratePath();
    }

    public void ThrowTwoSeamFastball()
    {
        ResetPath();
        ballControllTr.position += new Vector3(Random.Range(-0.5f, 0), 0, 0); // �ణ �������� �̵� (���������� ���� ����)
        GeneratePath();
    }

    public void ThrowSplitter()
    {
        ResetPath();
        ballControllTr.position += new Vector3(0, Random.Range(-1.5f, -0.5f), 0); // �Ʒ��� �� ������
        GeneratePath();
    }

    #endregion


    void GeneratePath()
    {
        GeneratePathBazierRayCast();
    }

    void GeneratePathBazierRayCast()
    {
        int resolution = 20; // ����� �ػ�
        pathRenderer.positionCount = resolution;

        var randomPoint = strikeZoneTr.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), -5f);

        for (int i = 0; i < resolution; i++)
        {

            float t = i / (float)(resolution - 1);
            Vector3 position = CalculateBezierPoint(t, tusooTr.position + new Vector3(0, 0, 0), ballControllTr.position, randomPoint);
            pathRenderer.SetPosition(i, position);
            pathPoints.Add(position);
        }

        var aimPoint = CheckLineRendererHit();

    }

    Vector3 CheckLineRendererHit()
    {
        int numberOfPoints = pathRenderer.positionCount;


        Vector3 startPoint = pathRenderer.GetPosition(numberOfPoints - 2);
        Vector3 endPoint = pathRenderer.GetPosition(numberOfPoints - 3);
        Debug.DrawLine(startPoint, endPoint, Color.green, 5f);

        RaycastHit hit;
        if (Physics.Linecast(startPoint, endPoint, out hit))
        {
            // ����Ʈ �ݶ��̴��� �浹�ߴٸ�
            if (hit.collider.CompareTag("StrikeZone"))
            {
                var hitpoint = hit.point;
                hitpoint.z = endPoint.z;
                GameObject go = new GameObject();
                go.name = $"{hitpoint} : {endPoint}";
                return hitpoint;
            }

        }
        // ���� �������� ������ ���������� �׸�

        startPoint = pathRenderer.GetPosition(numberOfPoints - 1);
        endPoint = pathRenderer.GetPosition(numberOfPoints - 2);
        Debug.DrawLine(startPoint, endPoint, Color.yellow, 5f);
        if (Physics.Linecast(startPoint, endPoint, out hit))
        {
            // ����Ʈ �ݶ��̴��� �浹�ߴٸ�
            if (hit.collider.CompareTag("StrikeZone"))
            {
                var hitpoint = hit.point;
                hitpoint.z = endPoint.z;
                GameObject go = new GameObject();
                go.name = $"{hitpoint} : {endPoint}";
                return hitpoint;
            }

        }

        return Vector3.zero;
    }



}


