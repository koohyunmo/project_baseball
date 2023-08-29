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
                // �浹 ������ ������ �����ɴϴ�.
                ContactPoint contact = collision.contacts[0];

                // �浹 ������ �������� ���� ƨ�ܳ��ϴ�.
                Vector3 direction = contact.point - transform.position;
                direction = direction.normalized;

                float forceMultiplier = 2.0f; // ���� �����Դϴ�. �� ���� ������Ű�� ���� �� �ָ� ���ư��ϴ�.
                rb.AddForce(direction * swingSpeed * forceMultiplier, ForceMode.Impulse);

                // ī�޶� ȿ���� �߰��մϴ�.
                StartCoroutine(CameraShake());
            }
        }
    }

    IEnumerator CameraShake()
    {
        // ī�޶� ��鸲 ȿ���� �����մϴ�.
        float shakeDuration = 0.5f; // ��鸲�� ���� �ð��Դϴ�.
        float shakeMagnitude = 0.05f; // ��鸲�� ũ���Դϴ�.

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
