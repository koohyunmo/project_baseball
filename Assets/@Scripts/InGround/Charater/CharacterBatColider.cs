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
            Debug.Log("캐릭터 Hit");



            var hitPoint = other.transform.position;

            taja.Swing(hitPoint);

            // 날려 보내기
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

        // 레이 그리기
        float rayLength = 5f; // 원하는 레이의 길이
        Debug.DrawRay(transform.parent.position, transform.parent.forward + transform.parent.up * 5.0f, Color.cyan, 3f); // 빨간색 레이를 2초 동안 보여줌

        isHit = false;
    }
}

