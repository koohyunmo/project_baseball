using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatCollider : MonoBehaviour
{

    Bat bat;
    // Start is called before the first frame update
    void Start()
    {
        bat = GetComponentInParent<Bat>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            Debug.Log("Hit");
            bat.Swing();


            // 날려 보내기
            Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                float forceAmount = 5f;
                Vector3 forceDirection = transform.parent.forward + transform.parent.up;
                rb.velocity = forceDirection;
                rb.AddForce(forceDirection * forceAmount, ForceMode.Impulse);

                // 레이 그리기
                float rayLength = 5f; // 원하는 레이의 길이
                Debug.DrawRay(transform.parent.position, forceDirection * rayLength, Color.red, 3f); // 빨간색 레이를 2초 동안 보여줌
            }

        }
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        
        float forceAmount = 2f;

        Gizmos.color = Color.blue;
        Vector3 forceDirection = transform.parent.forward + transform.parent.up*2f;
        Gizmos.DrawRay(transform.position, forceDirection * forceAmount);

        Gizmos.color = Color.green;
        forceDirection = transform.parent.forward + transform.parent.up*1f;
        Gizmos.DrawRay(transform.position, forceDirection * forceAmount);

        Gizmos.color = Color.cyan;
        forceDirection = transform.parent.forward + transform.parent.up * 0.5f;
        Gizmos.DrawRay(transform.position, forceDirection * forceAmount);
    }

#endif


}
