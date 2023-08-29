using DG.Tweening;
using UnityEngine;

public class PlayerHandednessSwitcher : MonoBehaviour
{
    public BoxCollider batBoxCollider;
    public Transform playerTransform; // 플레이어의 Transform
    public Transform batTransform;    // 배트의 Transform
    public float batOffsetX = 0.5f;   // 배트의 x축 오프셋 (플레이어로부터의 거리)

    public GameObject cube;

    private bool isRightHanded = true; // 초기 상태를 우타자로 설정

    private void Start()
    {
        ToggleHandedness();

        MakeHandle(batBoxCollider.bounds.max.x, "batBoxCollider.bounds.max.x", 0.9f);
        MakeHandle(batBoxCollider.bounds.max.x, "batBoxCollider.bounds.max.x", 0.8f);
        MakeHandle(batBoxCollider.bounds.max.x, "batBoxCollider.bounds.max.x", 0.7f);
        MakeHandle(batBoxCollider.bounds.max.x, "batBoxCollider.bounds.max.x",0.6f);
        MakeHandle(batBoxCollider.bounds.max.x, "batBoxCollider.bounds.max.x", 0.5f);
        MakeHandle(batBoxCollider.bounds.max.x, "batBoxCollider.bounds.max.x", 0.4f);
        MakeHandle(batBoxCollider.bounds.max.x, "batBoxCollider.bounds.max.x" , 0.3f);
        MakeHandle(batBoxCollider.bounds.max.x, "batBoxCollider.bounds.max.x" , 0.2f);
        MakeHandle(batBoxCollider.bounds.max.x, "batBoxCollider.bounds.max.x" , 0.1f);
        MakeHandle(batBoxCollider.bounds.max.x, "batBoxCollider.bounds.max.x" , 0.0f);
        MakeHandle(batBoxCollider.bounds.max.x, "batBoxCollider.bounds.max.x" , -0.1f);
        MakeHandle(batBoxCollider.bounds.max.x, "batBoxCollider.bounds.max.x" , -0.2f);
        MakeHandle(batBoxCollider.bounds.max.x, "batBoxCollider.bounds.max.x" , -0.3f);



        MakeHandleZ(batBoxCollider.bounds.extents.z, "Bat_Handle");
    }

    private void MakeHandle(float inputOffset, string name, float offset)
    {
        // 박스 콜라이더의 z 크기를 가져옵니다. (배트의 길이)
        float batBox = inputOffset;
        // 손잡이의 상대적 위치를 계산합니다. 여기서는 z 크기의 20%를 사용하였습니다.
        // 0.3f 가손잡이 위치
        float handleOffset = batBox * offset;
        // 회전을 고려하여 월드 좌표로 변환합니다. 손잡이는 z축 방향으로 배치됩니다.
        Vector3 handleLocalPosition = new Vector3(batBoxCollider.center.x , batBoxCollider.center.y + handleOffset, batBoxCollider.center.z - offset);
        Vector3 handleWorldPosition = batBoxCollider.transform.TransformPoint(handleLocalPosition);
        // 손잡이의 트랜스폼을 생성합니다.
        Transform HandleTransform = Instantiate(cube).transform;
        HandleTransform.gameObject.name = name + offset.ToString();
        HandleTransform.position = handleWorldPosition;
        HandleTransform.parent = batBoxCollider.transform;
    }


    private void MakeHandleZ(float inputOffset, string name)
    {
        // 중심에서 한쪽 끝까지의 거리를 사용하여 손잡이의 위치를 계산합니다.
        float handleOffset = inputOffset;

        // 회전을 고려하여 월드 좌표로 변환합니다. 손잡이는 z축 방향으로 배치됩니다.
        Vector3 handleLocalPosition = new Vector3(batBoxCollider.center.x, batBoxCollider.center.y, batBoxCollider.center.z - handleOffset);
        Vector3 handleWorldPosition = batBoxCollider.transform.TransformPoint(handleLocalPosition);

        // 손잡이의 트랜스폼을 생성합니다.
        Transform HandleTransform = Instantiate(cube).transform;
        HandleTransform.position = handleWorldPosition;
        HandleTransform.gameObject.name = name;
        HandleTransform.parent = batBoxCollider.transform;
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
