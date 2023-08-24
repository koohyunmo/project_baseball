using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PosuController : MonoBehaviour
{
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

        transform.position = originPos;
        targetPosition = new Vector3(Managers.Game.AimPoint.x, Managers.Game.AimPoint.y, transform.position.z);
        float distance = Vector3.Distance(transform.position, targetPosition);

        while (distance > threshold)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            distance = Vector3.Distance(transform.position, targetPosition);
            yield return null; // ���� �����ӱ��� ���
        }

        transform.position = targetPosition; // ���������� ��Ȯ�� ��ġ�� ����
    }

    public void CatchBall()
    {
        transform.DOPunchScale(new Vector3(0.2f,0.2f), 0.5f, 1).OnComplete(() => 
        {

        });

    }
}
