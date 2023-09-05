using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallRotate : MonoBehaviour
{
    float speed = 10f;
    private void Update()
    {
        // Z축을 중심으로 회전합니다. 회전 속도는 공의 속도에 비례합니다.
        transform.Rotate(Managers.Game.Speed * Time.deltaTime * speed * 0.1f, Managers.Game.Speed * Time.deltaTime * speed, Managers.Game.Speed * Time.deltaTime * 0.1f);
    }
}
