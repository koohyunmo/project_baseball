using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowBall : MonoBehaviour
{
    // Prefab of the Ball object
    public GameObject ballPrefab;
    // Target object, the StrikeZone
    public Transform strikeZone;
    // Speed of the throw
    public float throwSpeed = 40.0f;
    // Random precision radius around the strike zone
    public float randomPrecision = 0.5f;  // 예: 0.5 미터 반경

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // If the Spacebar key is pressed...
        {
            Throw();
        }
    }

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
            Random.Range(-randomPrecision, randomPrecision),
            Random.Range(-randomPrecision, randomPrecision),
            Random.Range(-randomPrecision, randomPrecision)
        );

        Vector3 randomTarget = strikeZone.position + randomOffset;
        Vector3 direction = randomTarget - ball.transform.position;

        // Normalize the direction
        direction.Normalize();

        // Draw a Debug Ray in the direction of the throw
        Debug.DrawRay(ball.transform.position, direction * 60f, Color.magenta, 5f);  // Green ray with a length of 10 units, lasts for 2 seconds

        // Add force towards the random point near the strikeZone
        var rigid = ball.GetComponent<Rigidbody>();
        rigid.AddForce(direction * throwSpeed, ForceMode.VelocityChange);
    }
}
