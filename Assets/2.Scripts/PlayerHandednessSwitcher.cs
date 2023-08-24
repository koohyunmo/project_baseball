using DG.Tweening;
using UnityEngine;

public class PlayerHandednessSwitcher : MonoBehaviour
{
    public Transform playerTransform; // �÷��̾��� Transform
    public Transform batTransform;    // ��Ʈ�� Transform
    public float batOffsetX = 0.5f;   // ��Ʈ�� x�� ������ (�÷��̾�κ����� �Ÿ�)

    private bool isRightHanded = true; // �ʱ� ���¸� ��Ÿ�ڷ� ����

    private void Start()
    {
        ToggleHandedness();
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
