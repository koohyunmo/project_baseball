using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Bat : MonoBehaviour
{
    public BoxCollider batCollider;
    public Transform targetPosition; // ������ ��ǥ ��ġ
    public Transform targetRotation; // ������ ��ǥ ȸ��
    public GameObject model;

    public float lerpSpeed = 1f;
    private bool isMoving = false;
    private float lerpTime = 0f;
    private float maxTime = 0.8f;
    private Vector3 initialPosition; // ������ ���� ��ġ
    private Quaternion initialRotation; // ������ ���� ȸ��

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
        if (!isMoving) // ���� ���� �ƴ� ���� ���� ����
        {
            targetPosition.position = target.position;
            targetPosition.rotation = target.rotation;

            initialPosition = model.transform.position; // ���� ��ġ�� ���� ��ġ�� ����
            initialRotation = model.transform.rotation; // ���� ȸ���� ���� ȸ������ ����
            isMoving = true;
        }
    }
}

