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

    private void Update()
    {
        if(Managers.Game.GameState != Define.GameState.InGround)
        {
            gameObject.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        if (_strike)
            return;

        if (transform.position.z <= -8.45 && _strike == false)
        {
            //Relplay()
            //pathRenderer
            Managers.Game.StrikeEvent();
            _strike = true;
            gameObject.SetActive(true);
            Managers.Game.GameEnd();
        }
    }


}