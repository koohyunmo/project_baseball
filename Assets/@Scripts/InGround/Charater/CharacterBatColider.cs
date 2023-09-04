using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBatColider : MonoBehaviour
{

    Taja taja;
    bool isHit = false;
    // Start is called before the first frame update
    void Start()
    {
        taja = GetComponentInParent<Taja>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != null && other.gameObject.CompareTag("Ball"))
        {
            Debug.Log("ĳ���� Hit");



            var hitPoint = other.transform.position;

            taja.Swing(hitPoint);

            // ���� ������
            Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();


            if (rb != null)
            {
                other.gameObject.GetComponent<BallController>().SetHit();

                if (isHit == false)
                {
                    isHit = true;
                    HitPointCheck(hitPoint, rb);
                }
            }

        }
    }

    private void HitPointCheck(Vector3 hitPoint, Rigidbody rb)
    {
        rb.velocity = transform.parent.forward + transform.parent.up;
        rb.AddForce(transform.parent.forward + transform.parent.up * 20.0f, ForceMode.Impulse);
        rb.useGravity = true;

        // ���� �׸���
        float rayLength = 5f; // ���ϴ� ������ ����
        Debug.DrawRay(transform.parent.position, transform.parent.forward + transform.parent.up * 5.0f, Color.cyan, 3f); // ������ ���̸� 2�� ���� ������

        isHit = false;
    }
}

