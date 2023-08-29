using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatCollision : MonoBehaviour
{

    float swingSpeed = 5.0f;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Baseball"))
        {
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // 충돌 지점의 정보를 가져옵니다.
                ContactPoint contact = collision.contacts[0];

                // 충돌 지점의 방향으로 공을 튕겨냅니다.
                Vector3 direction = contact.point - transform.position;
                direction = direction.normalized;

                float forceMultiplier = 2.0f; // 힘의 배율입니다. 이 값을 증가시키면 공이 더 멀리 날아갑니다.
                rb.AddForce(direction * swingSpeed * forceMultiplier, ForceMode.Impulse);

                // 카메라 효과를 추가합니다.
                StartCoroutine(CameraShake());
            }
        }
    }

    IEnumerator CameraShake()
    {
        // 카메라 흔들림 효과를 구현합니다.
        float shakeDuration = 0.5f; // 흔들림의 지속 시간입니다.
        float shakeMagnitude = 0.05f; // 흔들림의 크기입니다.

        Vector3 originalPosition = Camera.main.transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            float x = originalPosition.x + Random.Range(-1f, 1f) * shakeMagnitude;
            float z = originalPosition.z + Random.Range(-1f, 1f) * shakeMagnitude;

            Camera.main.transform.localPosition = new Vector3(x, originalPosition.y, z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        Camera.main.transform.localPosition = originalPosition;
    }



}
