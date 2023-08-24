using DG.Tweening;
using UnityEngine;

public class PlayerHandednessSwitcher : MonoBehaviour
{
    public Transform playerTransform; // 플레이어의 Transform
    public Transform batTransform;    // 배트의 Transform
    public float batOffsetX = 0.5f;   // 배트의 x축 오프셋 (플레이어로부터의 거리)

    private bool isRightHanded = true; // 초기 상태를 우타자로 설정

    private void Start()
    {
        ToggleHandedness();
    }

    public void ToggleHandedness()
    {
        if (isRightHanded)
        {
            // 좌타자 스탠스로 변경
            playerTransform.eulerAngles = new Vector3(0, 180, 0); // y축으로 180도 회전
            batTransform.localPosition = new Vector3(-batOffsetX, batTransform.localPosition.y, batTransform.localPosition.z); // 배트를 플레이어의 왼쪽으로 이동
        }
        else
        {
            // 우타자 스탠스로 변경
            playerTransform.eulerAngles = Vector3.zero; // 원래 방향으로 회전
            batTransform.localPosition = new Vector3(batOffsetX, batTransform.localPosition.y, batTransform.localPosition.z); // 배트를 플레이어의 오른쪽으로 이동
        }

        // 현재 상태 업데이트
        isRightHanded = !isRightHanded;
    }
}
