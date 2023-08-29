using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;
public class ThrowBall : MonoBehaviour
{
    // Prefab of the Ball object
    public GameObject ballPrefab;
    // Target object, the StrikeZone
    public Transform strikeZone;
    // Speed of the throw
    [SerializeField] float _throwSpeed = 25.0f;
    [SerializeField] float _curveThrowSpeed = 15.0f;
    [SerializeField] float _curveParabolicSpeed = 11.5f;
    [SerializeField] float _throwSliderSpeed = 35f;
    [SerializeField] float _curveTorqueAmount = 10.0f;
    // Random precision radius around the strike zone
    [SerializeField] float _randomPrecision = 0.5f;  // ��: 0.5 ���� �ݰ�

    public GameObject smallCubePrefab; // ���� ť���� Prefab

    Action[] _throwStyle;
    [SerializeField] ThrowStyle _type = 0;

    enum ThrowStyle
    {
        FastBall,
        Curve,
        Slider,
        ChangeUp,
        Sinker,
        ThrowFastball,
        ThrowExaggeratedCurveball,
        ThrowNormalCurveball,
        ThrowSliderPlus,
        ThrowChangeupPlus,
        ThrowSinkerPlus

    }

    private void Start()
    {
        //StartCoroutine(co_ThrowBall());

        _throwStyle  = new Action[] { 
            Throw,ThrowParabolicBall,
                ThrowSlider,ThrowChangeup,ThrowSinker,
                    ThrowFastball,ThrowExaggeratedCurveball,ThrowNormalCurveball,
                        ThrowSliderPlus, ThrowChangeupPlus,  ThrowSinkerPlus
        };
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // If the Spacebar key is pressed...
        {
            if ((int)_type > _throwStyle.Length - 1)
            {
                Debug.LogWarning("�迭�ʰ�");
                return;
            }
                
            _throwStyle[(int)_type]?.Invoke();
        }
    }

   IEnumerator co_ThrowBall()
    {

        while (true)
        {
            yield return new WaitForSeconds(1.5f);
            //Throw();
            //yield return new WaitForSeconds(1.5f);
            ThrowParabolicBall();

        }
    }

    #region �ߵǴ� ����
    void Throw()
    {
        // Set the spawn position to be above the Baller object
        Vector3 spawnPosition = transform.position + transform.up;

        // Instantiate the ball at the spawn position and rotation
        GameObject ball = Instantiate(ballPrefab, spawnPosition, transform.rotation);

        // Add a Rigidbody if the ball doesn't have one
        if (ball.GetComponent<Rigidbody>() == null)
        {
            ball.AddComponent<Rigidbody>();
        }


        // Calculate direction towards a random point near the strikeZone
        Vector3 randomOffset = new Vector3(
            Random.Range(-_randomPrecision, _randomPrecision),
            Random.Range(-_randomPrecision, _randomPrecision),
            Random.Range(-_randomPrecision, _randomPrecision)
        );

        Vector3 randomTarget = strikeZone.position + randomOffset;
        Vector3 direction = randomTarget - ball.transform.position;

        // Normalize the direction
        direction.Normalize();

        // Draw a Debug Ray in the direction of the throw
        Debug.DrawRay(ball.transform.position, direction * 60f, Color.magenta, 5f);  // Green ray with a length of 10 units, lasts for 2 seconds

        // Ray ���� ��ġ�� ���� ����
        Ray ray = new Ray(ball.transform.position, direction);

        

        // Ray�� �浹�� ������Ʈ�� ������ ������ ����
        RaycastHit hit;

        // Raycast �߻�
        if (Physics.Raycast(ray, out hit, 1000f)) // 100f�� Ray�� �ִ� �Ÿ�
        {
            Debug.DrawLine(ray.origin, hit.collider.transform.position, Color.yellow);
            // Ray�� StrikeZone�� �浹�ߴ��� Ȯ��
            if (hit.collider.CompareTag("StrikeZone"))
            {
                // Ray�� � ������Ʈ�� �浹�ߴ��� Ȯ��
                Debug.Log(hit.collider.name);
                // ���� ť�긦 �����մϴ�.
                // var go  = Instantiate(smallCubePrefab, hit.point, Quaternion.identity);
                // Destroy(go, 1.0f);
                smallCubePrefab.transform.position = hit.point;
            }
        }

        // Add force towards the random point near the strikeZone
        var rigid = ball.GetComponent<Rigidbody>();
        rigid.useGravity = false;
        rigid.AddForce(direction * _throwSpeed, ForceMode.VelocityChange);
    }
    void ThrowParabolicBall()
    {
        // ������ ���� �ڵ�� ������ �κ�
        Vector3 spawnPosition = transform.position + transform.up;
        GameObject ball = Instantiate(ballPrefab, spawnPosition, transform.rotation);
        Rigidbody rigid;
        if (ball.GetComponent<Rigidbody>() == null)
        {
            rigid = ball.AddComponent<Rigidbody>();
        }
        else
        {
            rigid = ball.GetComponent<Rigidbody>();
        }

        Vector3 randomOffset = new Vector3(
            Random.Range(-_randomPrecision, _randomPrecision),
            Random.Range(-_randomPrecision, _randomPrecision),
            Random.Range(-_randomPrecision, _randomPrecision)
        );

        Vector3 randomTarget = strikeZone.position + randomOffset;
        Vector3 direction = randomTarget - ball.transform.position;
        direction.Normalize();

        // �������� �׸��� ���� ���� �������� ���� �߰��մϴ�.
        Vector3 verticalForce = Vector3.up * _curveParabolicSpeed * 0.75f; // ���� ���������� ���� �� ���ϰ� �����մϴ�.
        Vector3 forwardForce = direction * _curveParabolicSpeed * 0.75f; // ������������ ���� �� ���ϰ� �����մϴ�.

        rigid.AddForce(verticalForce + forwardForce, ForceMode.VelocityChange);
    }
    #endregion

    #region Test��
    void ThrowSlider()
    {
        PrepareBall(out GameObject ball, out Rigidbody rigid);
        rigid.useGravity = false; // �߷� ��Ȱ��ȭ

        Vector3 targetPosition = GetTargetPosition();
        Vector3 direction = (targetPosition - ball.transform.position).normalized;

        rigid.AddForce(direction * _throwSpeed, ForceMode.VelocityChange);
        rigid.AddTorque(new Vector3(0, -0.05f, 0), ForceMode.VelocityChange); // ���̵� ���� �߰�

        Debug.DrawLine(ball.transform.position, targetPosition, Color.blue, 5f);
    }

    void ThrowChangeup()
    {
        PrepareBall(out GameObject ball, out Rigidbody rigid);
        rigid.useGravity = false; // �߷� ��Ȱ��ȭ

        Vector3 targetPosition = GetTargetPosition();
        Vector3 direction = (targetPosition - ball.transform.position).normalized;

        rigid.AddForce(direction * _throwSpeed, ForceMode.VelocityChange); // ü�������� �ӵ��� ����

        Debug.DrawLine(ball.transform.position, targetPosition, Color.green, 5f);
    }

    void ThrowSinker()
    {
        PrepareBall(out GameObject ball, out Rigidbody rigid);
        rigid.useGravity = false; // �߷� ��Ȱ��ȭ

        Vector3 targetPosition = GetTargetPosition();
        Vector3 direction = (targetPosition - ball.transform.position).normalized;

        rigid.AddForce(direction * _throwSpeed, ForceMode.VelocityChange);
        rigid.AddTorque(new Vector3(-0.05f, 0, 0), ForceMode.VelocityChange); // �ٿ���� ���� �߰�

        Debug.DrawLine(ball.transform.position, targetPosition, Color.yellow, 5f);
    }

    void ThrowFastball()
    {
        PrepareBall(out GameObject ball, out Rigidbody rigid);

        Vector3 targetPosition = GetTargetPosition();
        Vector3 direction = (targetPosition - ball.transform.position).normalized;

        rigid.AddForce(direction * _throwSpeed, ForceMode.VelocityChange);

        Debug.DrawLine(ball.transform.position, targetPosition, Color.red, 5f);
    }

    void ThrowExaggeratedCurveball()
    {
        PrepareBall(out GameObject ball, out Rigidbody rigid);

        _throwSpeed = 18;

        Vector3 targetPosition = GetTargetPosition();
        Vector3 direction = (targetPosition - ball.transform.position).normalized;

        Vector3 verticalForce = Vector3.up * _throwSpeed * 0.5f; // ���� ���������� ���� �� ���ϰ� �����մϴ�.
        Vector3 forwardForce = direction * _throwSpeed * 0.5f; // ������������ ���� �� ���ϰ� �����մϴ�.

        rigid.AddForce(verticalForce + forwardForce, ForceMode.VelocityChange);
        rigid.AddTorque(new Vector3(-_throwSpeed * 0.5f, 0, 0), ForceMode.VelocityChange); // ū �ٿ���� ���� �߰�

        Debug.DrawLine(ball.transform.position, targetPosition, Color.blue, 5f);
    }

    void ThrowNormalCurveball()
    {
        PrepareBall(out GameObject ball, out Rigidbody rigid);

        _throwSpeed = 20;

        Vector3 targetPosition = GetTargetPosition();
        Vector3 direction = (targetPosition - ball.transform.position).normalized;

        Vector3 verticalForce = Vector3.up * _throwSpeed * 0.25f; // ���� ���������� ���� �����մϴ�.
        Vector3 forwardForce = direction * _throwSpeed * 0.75f; // ������������ ���� �����մϴ�.

        rigid.AddForce(verticalForce + forwardForce, ForceMode.VelocityChange);
        rigid.AddTorque(new Vector3(-_throwSpeed * 0.25f, 0, 0), ForceMode.VelocityChange); // �ٿ���� ���� �߰�

        Debug.DrawLine(ball.transform.position, targetPosition, Color.green, 5f);
    }

    void ThrowSliderPlus()
    {
        PrepareBall(out GameObject ball, out Rigidbody rigid);

        Vector3 targetPosition = GetTargetPosition();
        Vector3 direction = (targetPosition - ball.transform.position).normalized;

        rigid.AddForce(direction * _throwSpeed, ForceMode.VelocityChange);
        rigid.AddTorque(new Vector3(0, -_throwSpeed * 0.5f, 0), ForceMode.VelocityChange); // ���̵� ���� �߰�

        Debug.DrawLine(ball.transform.position, targetPosition, Color.blue, 5f);
    }

    void ThrowChangeupPlus()
    {
        PrepareBall(out GameObject ball, out Rigidbody rigid);

        Vector3 targetPosition = GetTargetPosition();
        Vector3 direction = (targetPosition - ball.transform.position).normalized;

        rigid.AddForce(direction * (_throwSpeed * 0.7f), ForceMode.VelocityChange); // ü�������� �ӵ��� �ణ ����

        Debug.DrawLine(ball.transform.position, targetPosition, Color.green, 5f);
    }

    void ThrowSinkerPlus()
    {
        PrepareBall(out GameObject ball, out Rigidbody rigid);

        Vector3 targetPosition = GetTargetPosition();
        Vector3 direction = (targetPosition - ball.transform.position).normalized;

        rigid.AddForce(direction * _throwSpeed, ForceMode.VelocityChange);
        rigid.AddTorque(new Vector3(-_throwSpeed * 0.5f, 0, 0), ForceMode.VelocityChange); // �ٿ���� ���� �߰�

        Debug.DrawLine(ball.transform.position, targetPosition, Color.yellow, 5f);
    }

    #endregion


    #region ��ƿ
    Rigidbody GetOrCreateRigidbody(GameObject ball)
    {
        if (ball.GetComponent<Rigidbody>() == null)
        {
            return ball.AddComponent<Rigidbody>();
        }
        return ball.GetComponent<Rigidbody>();
    }

    Vector3 GetThrowDirection(GameObject ball)
    {
        Vector3 randomOffset = new Vector3(
            Random.Range(-_randomPrecision, _randomPrecision),
            Random.Range(-_randomPrecision, _randomPrecision),
            Random.Range(-_randomPrecision, _randomPrecision)
        );
        Vector3 randomTarget = strikeZone.position + randomOffset;
        Vector3 direction = randomTarget - ball.transform.position;
        return direction.normalized;
    }

    void PrepareBall(out GameObject ball, out Rigidbody rigid)
    {
        Vector3 spawnPosition = transform.position + transform.up;
        ball = Instantiate(ballPrefab, spawnPosition, transform.rotation);
        rigid = ball.GetComponent<Rigidbody>() ?? ball.AddComponent<Rigidbody>();
    }

    Vector3 CalculateDirectionToStrikeZone(GameObject ball)
    {
        Vector3 toStrikeZone = strikeZone.position - ball.transform.position;
        float distance = toStrikeZone.magnitude;
        Vector3 direction = toStrikeZone.normalized * Mathf.Sqrt(9.8f * distance);
        return direction;
    }

    void ThrowBallToStrikeZone()
    {
        // �� �غ�
        PrepareBall(out GameObject ball, out Rigidbody rigid);
        rigid.useGravity = false; // �߷� ��Ȱ��ȭ

        // ��Ʈ����ũ ���� �߾ӿ��� ��0.5�� ������ ����ϴ� ��ġ ���
        Vector3 targetPosition = strikeZone.position + new Vector3(
            Random.Range(-0.5f, 0.5f),
            Random.Range(-0.5f, 0.5f),
            Random.Range(-0.5f, 0.5f)
        );

        // ���� �ش� ��ġ�� ������ ���� ���
        Vector3 direction = (targetPosition - ball.transform.position).normalized;

        // ������ �ش� �������� ���� ���� (�ӵ��� 0.1�� ����)
        rigid.AddForce(direction * 0.1f, ForceMode.VelocityChange);

        // ����� ���� �׸��� (���� ��� Ȯ�ο�)
        Debug.DrawLine(ball.transform.position, targetPosition, Color.red, 5f);
    }

    Vector3 GetTargetPosition()
    {
        return strikeZone.position + new Vector3(
            Random.Range(-0.5f, 0.5f),
            Random.Range(-0.5f, 0.5f),
            Random.Range(-0.5f, 0.5f)
        );
    }
    #endregion



}
