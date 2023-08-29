using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DG.Tweening.DOTweenAnimation;

public class AnimationController : MonoBehaviour
{
    // Start is called before the first frame update

    enum AnimationType
    {
        Bat_Idle,
        Bat_Swing,
        Bat_Swing_2,
        Bat_Swing_3,
    }
    private Animator anim;
    private bool isPlaying = false;

    private Dictionary<AnimationType, string> animDict;

   [SerializeField]private AnimationType _animType = AnimationType.Bat_Idle;

    void Start()
    {
        animDict = new Dictionary<AnimationType, string>()
    {
        { AnimationType.Bat_Idle, "Bat_Idle" },
        { AnimationType.Bat_Swing_2, "Bat_Swing_2" },
        { AnimationType.Bat_Swing_3, "Bat_Swing_3" },
        { AnimationType.Bat_Swing, "Bat_Swing" },
    };

        anim = GetComponent<Animator>();
        StartCoroutine(PlayAnimation());
    }

    IEnumerator PlayAnimation()
    {
        while (true)
        {
            // 애니메이션이 재생 중이 아니라면 애니메이션을 시작합니다.
            if (!isPlaying)
            {
                isPlaying = true;
                anim.Play(animDict[_animType]);

                // 애니메이션의 길이만큼 대기합니다.
                yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);

                isPlaying = false;
            }

            // 다음 프레임까지 대기합니다.
            yield return null;
        }
    }
}
