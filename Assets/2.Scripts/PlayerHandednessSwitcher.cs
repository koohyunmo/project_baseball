using DG.Tweening;
using UnityEngine;

public class PlayerHandednessSwitcher : MonoBehaviour
{
    public BoxCollider batBoxCollider;
    public Transform playerTransform; // �÷��̾��� Transform
    public Transform batTransform;    // ��Ʈ�� Transform
    public float batOffsetX = 0.5f;   // ��Ʈ�� x�� ������ (�÷��̾�κ����� �Ÿ�)

    public GameObject cube;

    private bool isRightHanded = true; // �ʱ� ���¸� ��Ÿ�ڷ� ����

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
        // �ڽ� �ݶ��̴��� z ũ�⸦ �����ɴϴ�. (��Ʈ�� ����)
        float batBox = inputOffset;
        // �������� ����� ��ġ�� ����մϴ�. ���⼭�� z ũ���� 20%�� ����Ͽ����ϴ�.
        // 0.3f �������� ��ġ
        float handleOffset = batBox * offset;
        // ȸ���� ����Ͽ� ���� ��ǥ�� ��ȯ�մϴ�. �����̴� z�� �������� ��ġ�˴ϴ�.
        Vector3 handleLocalPosition = new Vector3(batBoxCollider.center.x , batBoxCollider.center.y + handleOffset, batBoxCollider.center.z - offset);
        Vector3 handleWorldPosition = batBoxCollider.transform.TransformPoint(handleLocalPosition);
        // �������� Ʈ�������� �����մϴ�.
        Transform HandleTransform = Instantiate(cube).transform;
        HandleTransform.gameObject.name = name + offset.ToString();
        HandleTransform.position = handleWorldPosition;
        HandleTransform.parent = batBoxCollider.transform;
    }


    private void MakeHandleZ(float inputOffset, string name)
    {
        // �߽ɿ��� ���� �������� �Ÿ��� ����Ͽ� �������� ��ġ�� ����մϴ�.
        float handleOffset = inputOffset;

        // ȸ���� ����Ͽ� ���� ��ǥ�� ��ȯ�մϴ�. �����̴� z�� �������� ��ġ�˴ϴ�.
        Vector3 handleLocalPosition = new Vector3(batBoxCollider.center.x, batBoxCollider.center.y, batBoxCollider.center.z - handleOffset);
        Vector3 handleWorldPosition = batBoxCollider.transform.TransformPoint(handleLocalPosition);

        // �������� Ʈ�������� �����մϴ�.
        Transform HandleTransform = Instantiate(cube).transform;
        HandleTransform.position = handleWorldPosition;
        HandleTransform.gameObject.name = name;
        HandleTransform.parent = batBoxCollider.transform;
    }

    public void ToggleHandedness()
    {
        if (isRightHanded)
        {
            // ��Ÿ�� ���Ľ��� ����
            playerTransform.eulerAngles = new Vector3(0, 180, 0); // y������ 180�� ȸ��
            batTransform.localPosition = new Vector3(-batOffsetX, batTransform.localPosition.y, batTransform.localPosition.z); // ��Ʈ�� �÷��̾��� �������� �̵�
        }
        else
        {
            // ��Ÿ�� ���Ľ��� ����
            playerTransform.eulerAngles = Vector3.zero; // ���� �������� ȸ��
            batTransform.localPosition = new Vector3(batOffsetX, batTransform.localPosition.y, batTransform.localPosition.z); // ��Ʈ�� �÷��̾��� ���������� �̵�
        }

        // ���� ���� ������Ʈ
        isRightHanded = !isRightHanded;
    }
}
