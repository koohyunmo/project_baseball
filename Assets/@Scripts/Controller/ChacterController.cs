using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class ChacterController : MonoBehaviour
{
    // Start is called before the first frame update

    public Animator anim;

    enum CharState
    {
        Idle,
        IdleSwing,
        Hit,
    }
    CharState charState = CharState.Idle;
    public float lerpSpeed = 2.5f;
    private bool isMoving = false;
    [SerializeField] float lerpTimer;
    float maxTime = 0.3f;

    void Start()
    {
        anim = GetComponent<Animator>();

        Managers.Game.SetCharacter(this);
        Managers.Game.SetHitCallBack(UpdateSwing);


    }

    public void UpdateIdle()
    {
        charState = CharState.Idle;
        anim.Play(charState.ToString());
    }

    public void UpdateSwing()
    {
        var prevAnim = charState;
        charState = CharState.Hit;
        SmoothTransition(prevAnim.ToString(), charState.ToString(), 0.1f);

    }


    /// <summary>
    /// 두 애니메이션 간의 부드러운 전환을 수행합니다.
    /// </summary>
    /// <param name="fromAnimation">끝난 애니메이션의 이름</param>
    /// <param name="toAnimation">시작할 애니메이션의 이름</param>
    /// <param name="transitionDuration">보간값 (전환하는 데 걸리는 시간, 초 단위)</param>
    public void SmoothTransition(string fromAnimation, string toAnimation, float transitionDuration = 0.1f)
    {
        anim.CrossFade(toAnimation, transitionDuration,-1,0);
        anim.Play(toAnimation);
    }



    public Transform batHandle; // 배트의 핸들 위치
    public Transform aimTarget; // 에임의 목표 위치 (예: 공의 위치)

    private void OnAnimatorIK(int layerIndex)
    {
        if (aimTarget != null)
        {
            // 오른손의 위치를 배트 핸들 위치로 설정
            anim.SetIKPosition(AvatarIKGoal.RightHand, batHandle.position);
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);

            // 왼손의 위치를 조절하여 배트의 방향을 에임 목표 위치로 설정
            Vector3 leftHandPosition = batHandle.position + (aimTarget.position - batHandle.position) * 0.5f;
            anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandPosition);
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
        }
    }

}
