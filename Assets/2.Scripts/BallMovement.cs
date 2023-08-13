using System.Collections.Generic;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public Transform controlPoint;
    public LineRenderer pathRenderer;
    public float speed;

    private List<Vector3> pathPoints = new List<Vector3>();
    private int currentPointIndex = 0;

    public void SetPath(LineRenderer renderer)
    {
        pathPoints.Clear();
        for (int i = 0; i < renderer.positionCount; i++)
        {
            pathPoints.Add(renderer.GetPosition(i));
        }
    }

    public void MoveAlongPath()
    {
        if (currentPointIndex < pathPoints.Count)
        {
            transform.position = Vector3.MoveTowards(transform.position, pathPoints[currentPointIndex], speed * Time.deltaTime);

            if (transform.position == pathPoints[currentPointIndex])
            {
                currentPointIndex++;
            }
        }
    }
}