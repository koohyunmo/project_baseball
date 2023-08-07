using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiBat : MonoBehaviour
{
    enum State { Moving, Hitting}

    [SerializeField]private Transform target;
    [SerializeField] private BoxCollider batCollider;
    float moveSpeed = 1.5f;
    State state;
    private bool canDectedHit;

    [SerializeField]private LayerMask ballMask;
    // Start is called before the first frame update
    void Start()
    {
        state = State.Moving;
        Move();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Moving:
                break;
            case State.Hitting:
                //if (canDectedHit)
                    //CheckForHits();
                break;

        }
    }

    public void JoyStickMove(Vector2 dir)
    {
        transform.position = transform.position + new Vector3(dir.x, dir.y,0 ) * moveSpeed * Time.deltaTime;

    }

    private void Move()
    {
        Vector3 targetPosition = transform.position;
        targetPosition.x = target.position.x;


        targetPosition.x = Mathf.Clamp(targetPosition.x, -1.83f, 1.83f);

        //cal target
        float difference = targetPosition.x - transform.position.x;

        if (difference == 0)
        {
            // idle
            Debug.Log("Idle");
        }
        else if (difference > 0)
        {
            // left
            Debug.Log("left");
        }
        else
        {
            // right
            Debug.Log("right");
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }


    private float GetTargetX()
    {
        Vector3 ballerShootPosition = new Vector3(-1, 0, 9.5f);
        Vector3 shootDirection = (target.position - ballerShootPosition).normalized;
        float shootAngle = Vector3.Angle(Vector3.right, shootDirection);

        float bc = target.position.z - ballerShootPosition.z;
        float ab = bc/Mathf.Tan(shootAngle * Mathf.Deg2Rad);

        Vector3 targetAiPosition =  ballerShootPosition + shootDirection * ab;

        return targetAiPosition.x + 0.5f;
    }

    public void StartDetectingHits()
    {
        canDectedHit = true;

    }

    private void CheckForHits()
    {
        Vector3 center = batCollider.transform.TransformPoint(batCollider.center);
        Vector3 halfExtents = 1.5f* batCollider.size / 2;

        Quaternion rotation = batCollider.transform.rotation;
        Collider[] detectedBalls =  Physics.OverlapBox(center, halfExtents, rotation, ballMask);

        for (int i = 0; i < detectedBalls.Length; i++)
        {
            BallDetectedCallback(detectedBalls[i]);
            return;
        }
    }

    private void BallDetectedCallback(Collider ballCollider)
    {
        canDectedHit = false;

        ShootBall(ballCollider.transform);

        Debug.Log("Ball Detected");
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider's object is on the target layer
        if (other.gameObject.layer == LayerMask.NameToLayer("Ball"))
        {
            other.GetComponent<Rigidbody>().velocity = Vector3.back + Vector3.up * 10;
            Debug.Log("Collided with target layer!");
        }
    }

    private void ShootBall(Transform ball)
    {
        ball.GetComponent<Rigidbody>().velocity = Vector3.back + Vector3.up * 10;
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (batCollider != null)
        {
            Gizmos.color = Color.red;  // Set the color to red

            Vector3 center = batCollider.transform.TransformPoint(batCollider.center);
            Vector3 halfExtents = 1.5f * batCollider.size / 2;

            Gizmos.DrawWireCube(center, 2 * halfExtents);  // Draw the wireframe cube


            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(batCollider.bounds.center, batCollider.size);  // Draw the wireframe cube
        }
    }
#endif
}
