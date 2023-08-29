using System.Collections;
using UnityEngine;

public class PlayerSwing : MonoBehaviour
{
    public GameObject bat; // 야구방망이 오브젝트를 연결합니다.
    public Camera playerCamera; // 플레이어의 카메라를 연결합니다.
    public float windupSpeed = 50.0f; // 야구방망이를 뒤로 땡기는 속도입니다.
    public float initialSwingSpeed = 200.0f; // 야구방망이를 휘두르는 초기 속도입니다.
    public float swingAcceleration = 50.0f; // 야구방망이를 휘두를 때의 가속도입니다.
    public float returnSpeed = 200.0f; // 야구방망이를 원래 위치로 돌려놓는 속도입니다.
    public float windupTime = 0.5f; // 배트를 뒤로 땡기는 시간입니다.

    private Quaternion originalRotation; // 야구방망이의 원래 회전을 저장합니다.
    private float windupAngle; // 배트를 뒤로 땡길 때의 회전 각도를 저장합니다.
    private float swingSpeed; // 야구방망이를 휘두르는 속도입니다.

    private bool isSwinging = false; // 야구방망이가 휘두르는 중인지를 나타내는 변수입니다.

    void Start()
    {
        originalRotation = transform.rotation; // 원래 회전을 저장합니다.
    }

    void Update()
    {
        if (!isSwinging && Input.GetMouseButtonDown(0)) // 마우스 왼쪽 버튼을 누르면
        {
            StartCoroutine(SwingBat());
        }
    }

    IEnumerator SwingBat()
    {
        isSwinging = true; // 야구방망이가 휘두르는 중임을 나타냅니다.

        // 배트를 뒤로 땡깁니다.
        float windupElapsedTime = 0;
        while (windupElapsedTime < windupTime)
        {
            float rotationAmount = -windupSpeed * Time.deltaTime * 1.5f;
            transform.Rotate(Vector3.up, rotationAmount);
            windupElapsedTime += Time.deltaTime;
            windupAngle += rotationAmount; // 회전 각도를 저장합니다.

            if(Input.GetMouseButtonUp(0))
            {
                break;
            }

            yield return null;
        }

        // 배트를 휘두릅니다.
        swingSpeed = initialSwingSpeed; // 휘두르는 속도를 초기화합니다.
        float swingTime = 2 * Mathf.Abs(windupAngle * 1.2f) / swingSpeed; // 배트를 뒤로 땡긴 각도의 두 배에 따라 스윙 시간을 계산합니다.
        float swingElapsedTime = 0;
        while (swingElapsedTime < swingTime)
        {
            swingSpeed += swingAcceleration * Time.deltaTime*1.5f; // 가속도를 적용하여 속도를 증가시킵니다.
            transform.Rotate(Vector3.up, swingSpeed * Time.deltaTime);
            swingElapsedTime += Time.deltaTime;
            yield return null;
        }

        // 스윙이 끝나면 원래 회전으로 돌아갑니다.
        StartCoroutine(ReturnBat());
        windupAngle = 0; // 회전 각도를 초기화합니다.
    }

    IEnumerator ReturnBat()
    {
        // 배트를 원래 회전으로 부드럽게 돌려놓습니다.
        while (Quaternion.Angle(transform.rotation, originalRotation) > 0.1f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, originalRotation, returnSpeed * Time.deltaTime);
            yield return null;
        }
        transform.rotation = originalRotation; // 정확한 회전을 보장합니다.

        isSwinging = false; // 야구방망이가 휘두르는 중이 아님을 나타냅니다.
    }


    public void Collision(Rigidbody rb)
    {
        Debug.Log("충돌");

        if (rb != null)
        {
            rb.AddForce(transform.forward * swingSpeed, ForceMode.Impulse);
        }
    }


}

