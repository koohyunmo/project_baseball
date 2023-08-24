using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Apple.ReplayKit;

public class BallMovement : MonoBehaviour
{
    public int ballId;
    public Transform startPoint;
    public Transform endPoint;
    public Transform controlPoint;
    public LineRenderer pathRenderer;
    public float speed;
    private bool _strike;
    private bool _hit;
    public Action<int> ballClearAction;

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
        if(_strike)
        {
            ballClearAction?.Invoke(ballId);
        }
        else if(_hit)
        {
            return;
        }
        else
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

    public void SetHit()
    {
        _hit = true;
    }

    float customEpsilon = 1e-5f;
    float customEpsilon2 = 0.1f;

    private void LateUpdate()
    {
        
        if (Vector3.Distance(transform.position,Managers.Game.StrikeZone.transform.position) < customEpsilon2)
        {
            // 두 벡터는 충분히 가깝다고 판단
            Debug.Log("Vectors are close enough!");
        }
    }
}