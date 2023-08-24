using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PosuController : MonoBehaviour
{
    public Transform model;
    public float moveSpeed = 5.0f; // �������� �ӵ��� �����ϱ� ���� ����
    private Vector3 targetPosition;
    private float threshold = 0.05f; // ��ġ�� �󸶳� ��������� while���� Ż������ �����ϴ� �Ӱ谪
    private Vector3 originPos;

    private void Start()
    {
        originPos = transform.position;
        Managers.Game.SetMovePosu(() => StartCoroutine(MoveToTarget()));
    }

    private IEnumerator MoveToTarget()
    {

        model.transform.position = originPos;
        targetPosition = new Vector3(Managers.Game.AimPoint.x, Managers.Game.AimPoint.y, transform.position.z);
        float distance = Vector3.Distance(model.transform.position, targetPosition);

        while (distance > threshold)
        {
            model.transform.position = Vector3.Lerp(model.transform.position, targetPosition, moveSpeed * Time.deltaTime);
            distance = Vector3.Distance(model.transform.position, targetPosition);
            yield return null; // ���� �����ӱ��� ���
        }

        model.transform.position = targetPosition; // ���������� ��Ȯ�� ��ġ�� ����
    }

    public void CatchBall()
    {
        model.transform.DOPunchScale(new Vector3(0.2f,0.2f), 0.5f, 1).OnComplete(() => 
        {

        });

    }
}
