using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PosuController : MonoBehaviour
{
    public float moveSpeed = 5.0f; // 움직임의 속도를 조절하기 위한 변수
    private Vector3 targetPosition;
    private float threshold = 0.05f; // 위치가 얼마나 가까워져야 while문을 탈출할지 결정하는 임계값
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
            yield return null; // 다음 프레임까지 대기
        }

        transform.position = targetPosition; // 마지막으로 정확한 위치로 설정
    }

    public void CatchBall()
    {
        transform.DOPunchScale(new Vector3(0.2f,0.2f), 0.5f, 1).OnComplete(() => 
        {

        });

    }
}
