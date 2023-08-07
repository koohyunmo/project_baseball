using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Bat : MonoBehaviour
{
    public BoxCollider batCollider;
    public Transform targetPosition; // 스윙의 목표 위치
    public Transform targetRotation; // 스윙의 목표 회전
    public GameObject model;

    public float lerpSpeed = 1f;
    private bool isMoving = false;
    private float lerpTime = 0f;
    private float maxTime = 0.8f;
    private Vector3 initialPosition; // 스윙의 시작 위치
    private Quaternion initialRotation; // 스윙의 시작 회전

    void Start()
    {
        batCollider = GetComponentInChildren<BoxCollider>();

        initialPosition = model.transform.position;
        initialRotation = model.transform.rotation;

        targetPosition = batCollider.transform;
        targetRotation = batCollider.transform;
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            lerpTime += Time.deltaTime * lerpSpeed;

            if (targetPosition == null)
                return;

            // Position Lerp
            model.transform.position = Vector3.Lerp(initialPosition, targetPosition.position, lerpTime);

            // Rotation Slerp
            model.transform.rotation = Quaternion.Slerp(initialRotation, targetRotation.rotation, lerpTime);

            if (lerpTime >= maxTime)
            {
                isMoving = false;

                lerpTime = 0f;

                model.transform.position = initialPosition;

                // Rotation Slerp
                model.transform.rotation = initialRotation;
            }
        }
    }

    public void Swing(Transform target)
    {
        if (!isMoving) // 스윙 중이 아닐 때만 스윙 시작
        {
            targetPosition.position = target.position;
            targetPosition.rotation = target.rotation;

            initialPosition = model.transform.position; // 현재 위치를 시작 위치로 설정
            initialRotation = model.transform.rotation; // 현재 회전을 시작 회전으로 설정
            isMoving = true;
        }
    }
}

