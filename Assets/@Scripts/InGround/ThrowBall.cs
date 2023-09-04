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
    [SerializeField] float _randomPrecision = 0.5f;  // 예: 0.5 미터 반경

    public GameObject smallCubePrefab; // 작은 큐브의 Prefab

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
                Debug.LogWarning("배열초과");
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

    #region 잘되는 구종
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

        // Ray 시작 위치와 방향 정의
        Ray ray = new Ray(ball.transform.position, direction);

        

        // Ray에 충돌한 오브젝트의 정보를 저장할 변수
        RaycastHit hit;

        // Raycast 발사
        if (Physics.Raycast(ray, out hit, 1000f)) // 100f는 Ray의 최대 거리
        {
            Debug.DrawLine(ray.origin, hit.collider.transform.position, Color.yellow);
            // Ray가 StrikeZone과 충돌했는지 확인
            if (hit.collider.CompareTag("StrikeZone"))
            {
                // Ray가 어떤 오브젝트와 충돌했는지 확인
                Debug.Log(hit.collider.name);
                // 작은 큐브를 생성합니다.
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
        // 기존의 직구 코드와 동일한 부분
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

        // 포물선을 그리기 위해 수직 방향으로 힘을 추가합니다.
        Vector3 verticalForce = Vector3.up * _curveParabolicSpeed * 0.75f; // 수직 방향으로의 힘을 더 강하게 조절합니다.
        Vector3 forwardForce = direction * _curveParabolicSpeed * 0.75f; // 전방향으로의 힘을 더 강하게 조절합니다.

        rigid.AddForce(verticalForce + forwardForce, ForceMode.VelocityChange);
    }
    #endregion

    #region Test중
    void ThrowSlider()
    {
        PrepareBall(out GameObject ball, out Rigidbody rigid);
        rigid.useGravity = false; // 중력 비활성화

        Vector3 targetPosition = GetTargetPosition();
        Vector3 direction = (targetPosition - ball.transform.position).normalized;

        rigid.AddForce(direction * _throwSpeed, ForceMode.VelocityChange);
        rigid.AddTorque(new Vector3(0, -0.05f, 0), ForceMode.VelocityChange); // 사이드 스핀 추가

        Debug.DrawLine(ball.transform.position, targetPosition, Color.blue, 5f);
    }

    void ThrowChangeup()
    {
        PrepareBall(out GameObject ball, out Rigidbody rigid);
        rigid.useGravity = false; // 중력 비활성화

        Vector3 targetPosition = GetTargetPosition();
        Vector3 direction = (targetPosition - ball.transform.position).normalized;

        rigid.AddForce(direction * _throwSpeed, ForceMode.VelocityChange); // 체인지업은 속도가 느림

        Debug.DrawLine(ball.transform.position, targetPosition, Color.green, 5f);
    }

    void ThrowSinker()
    {
        PrepareBall(out GameObject ball, out Rigidbody rigid);
        rigid.useGravity = false; // 중력 비활성화

        Vector3 targetPosition = GetTargetPosition();
        Vector3 direction = (targetPosition - ball.transform.position).normalized;

        rigid.AddForce(direction * _throwSpeed, ForceMode.VelocityChange);
        rigid.AddTorque(new Vector3(-0.05f, 0, 0), ForceMode.VelocityChange); // 다운워드 스핀 추가

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

        Vector3 verticalForce = Vector3.up * _throwSpeed * 0.5f; // 수직 방향으로의 힘을 더 강하게 조절합니다.
        Vector3 forwardForce = direction * _throwSpeed * 0.5f; // 전방향으로의 힘을 더 강하게 조절합니다.

        rigid.AddForce(verticalForce + forwardForce, ForceMode.VelocityChange);
        rigid.AddTorque(new Vector3(-_throwSpeed * 0.5f, 0, 0), ForceMode.VelocityChange); // 큰 다운워드 스핀 추가

        Debug.DrawLine(ball.transform.position, targetPosition, Color.blue, 5f);
    }

    void ThrowNormalCurveball()
    {
        PrepareBall(out GameObject ball, out Rigidbody rigid);

        _throwSpeed = 20;

        Vector3 targetPosition = GetTargetPosition();
        Vector3 direction = (targetPosition - ball.transform.position).normalized;

        Vector3 verticalForce = Vector3.up * _throwSpeed * 0.25f; // 수직 방향으로의 힘을 조절합니다.
        Vector3 forwardForce = direction * _throwSpeed * 0.75f; // 전방향으로의 힘을 조절합니다.

        rigid.AddForce(verticalForce + forwardForce, ForceMode.VelocityChange);
        rigid.AddTorque(new Vector3(-_throwSpeed * 0.25f, 0, 0), ForceMode.VelocityChange); // 다운워드 스핀 추가

        Debug.DrawLine(ball.transform.position, targetPosition, Color.green, 5f);
    }

    void ThrowSliderPlus()
    {
        PrepareBall(out GameObject ball, out Rigidbody rigid);

        Vector3 targetPosition = GetTargetPosition();
        Vector3 direction = (targetPosition - ball.transform.position).normalized;

        rigid.AddForce(direction * _throwSpeed, ForceMode.VelocityChange);
        rigid.AddTorque(new Vector3(0, -_throwSpeed * 0.5f, 0), ForceMode.VelocityChange); // 사이드 스핀 추가

        Debug.DrawLine(ball.transform.position, targetPosition, Color.blue, 5f);
    }

    void ThrowChangeupPlus()
    {
        PrepareBall(out GameObject ball, out Rigidbody rigid);

        Vector3 targetPosition = GetTargetPosition();
        Vector3 direction = (targetPosition - ball.transform.position).normalized;

        rigid.AddForce(direction * (_throwSpeed * 0.7f), ForceMode.VelocityChange); // 체인지업은 속도가 약간 느림

        Debug.DrawLine(ball.transform.position, targetPosition, Color.green, 5f);
    }

    void ThrowSinkerPlus()
    {
        PrepareBall(out GameObject ball, out Rigidbody rigid);

        Vector3 targetPosition = GetTargetPosition();
        Vector3 direction = (targetPosition - ball.transform.position).normalized;

        rigid.AddForce(direction * _throwSpeed, ForceMode.VelocityChange);
        rigid.AddTorque(new Vector3(-_throwSpeed * 0.5f, 0, 0), ForceMode.VelocityChange); // 다운워드 스핀 추가

        Debug.DrawLine(ball.transform.position, targetPosition, Color.yellow, 5f);
    }

    #endregion


    #region 유틸
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
        // 공 준비
        PrepareBall(out GameObject ball, out Rigidbody rigid);
        rigid.useGravity = false; // 중력 비활성화

        // 스트라이크 존의 중앙에서 ±0.5의 오차를 허용하는 위치 계산
        Vector3 targetPosition = strikeZone.position + new Vector3(
            Random.Range(-0.5f, 0.5f),
            Random.Range(-0.5f, 0.5f),
            Random.Range(-0.5f, 0.5f)
        );

        // 공을 해당 위치로 보내는 방향 계산
        Vector3 direction = (targetPosition - ball.transform.position).normalized;

        // 공에게 해당 방향으로 힘을 가함 (속도는 0.1로 설정)
        rigid.AddForce(direction * 0.1f, ForceMode.VelocityChange);

        // 디버그 라인 그리기 (투구 경로 확인용)
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
