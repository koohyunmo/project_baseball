using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallRotate : MonoBehaviour
{
    float speed = 10f;
    private void Update()
    {
        // Z���� �߽����� ȸ���մϴ�. ȸ�� �ӵ��� ���� �ӵ��� ����մϴ�.
        transform.Rotate(Managers.Game.Speed * Time.deltaTime * speed * 0.1f, Managers.Game.Speed * Time.deltaTime * speed, Managers.Game.Speed * Time.deltaTime * 0.1f);
    }
}
