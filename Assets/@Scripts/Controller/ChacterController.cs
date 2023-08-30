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
        Swing,
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
        Managers.Game.SetHitCallBack(SwingAnim);

        charState = CharState.Idle;
        anim.Play(charState.ToString());
    }

    public void SwingAnim()
    {
        //anim.Play(CharState.Swing.ToString());

        var prevAnim = charState;
        charState = CharState.Swing;
        SmoothTransition(prevAnim.ToString(), charState.ToString(), 0.5f);
        StartCoroutine(co_AnimCallBack(anim, charState.ToString()));

    }

    IEnumerator co_AnimCallBack(Animator anim, string animClipName)
    {
        while (!anim.GetCurrentAnimatorStateInfo(0).IsName(animClipName) || anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }

        charState = CharState.IdleSwing;

        SmoothTransition(animClipName, charState.ToString(), 0.5f);
    }


    /// <summary>
    /// �� �ִϸ��̼� ���� �ε巯�� ��ȯ�� �����մϴ�.
    /// </summary>
    /// <param name="fromAnimation">���� �ִϸ��̼��� �̸�</param>
    /// <param name="toAnimation">������ �ִϸ��̼��� �̸�</param>
    /// <param name="transitionDuration">������ (��ȯ�ϴ� �� �ɸ��� �ð�, �� ����)</param>
    public void SmoothTransition(string fromAnimation, string toAnimation, float transitionDuration)
    {
        // ���� ���� ���� �ִϸ��̼��� 'fromAnimation'���� Ȯ��
        if (anim.GetCurrentAnimatorStateInfo(0).IsName(fromAnimation))
        {
            anim.CrossFade(toAnimation, transitionDuration);
        }
    }



    public Transform batHandle; // ��Ʈ�� �ڵ� ��ġ
    public Transform aimTarget; // ������ ��ǥ ��ġ (��: ���� ��ġ)

    private void OnAnimatorIK(int layerIndex)
    {
        if (aimTarget != null)
        {
            // �������� ��ġ�� ��Ʈ �ڵ� ��ġ�� ����
            anim.SetIKPosition(AvatarIKGoal.RightHand, batHandle.position);
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);

            // �޼��� ��ġ�� �����Ͽ� ��Ʈ�� ������ ���� ��ǥ ��ġ�� ����
            Vector3 leftHandPosition = batHandle.position + (aimTarget.position - batHandle.position) * 0.5f;
            anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandPosition);
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
        }
    }

}
