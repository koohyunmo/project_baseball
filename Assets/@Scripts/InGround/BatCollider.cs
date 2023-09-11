using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatCollider : MonoBehaviour
{

    Bat bat;

    [SerializeField] Transform _top;
    [SerializeField] Transform _mid;
    [SerializeField] Transform _midRow;
    [SerializeField] Transform _bottom;
    [SerializeField] HitPos _hitPos = HitPos.NONE;

    Vector3 originalScale;

    bool isHit = false;

    enum HitPos
    {
        NONE,
        Top,
        Mid,
        MidRow,
        Bottom,
    }

    // Start is called before the first frame update
    void Start()
    {
        bat = GetComponentInParent<Bat>();

        originalScale = transform.localScale;

        Managers.Game.SetBatCollider(this);
    }

    private void OnEnable()
    {
        Debug.Log("베트콜리얻 초기화");
        isHit = false;
        _hitPos = HitPos.NONE;
    }

    public Vector3 BatMid()
    {
        return _mid.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            Time.timeScale = 1f;

            var hitPoint = other.gameObject.transform.position;

            // 날려 보내기
            var bc = other.gameObject.GetComponent<BallController>();



            if (bc.GetRigid() != null && bc.GetHit() == false)
            {
                Managers.Game.StopWatch.Reset();
                Managers.Game.StopWatch.Start();


                if (Managers.Game.GameState == Define.GameState.InGround)
                {
                    bc.SetHit();
                    CalHitScore(hitPoint);
                    Debug.Log("Hit");
                    // 람다로 공날리는 함수 보내기
                    bat.SwingCollision(hitPoint, HitAction, bc.GetRigid());

                    Managers.Effect.PlayEffect(Keys.BAT_EFFECT_KEY.HitA.ToString(), hitPoint);
                    transform.DOShakeScale(0.3f, 0.7f).OnComplete(() => transform.DOScale(originalScale, 0));

                }

            }

        }
    }


    private void HitAction(Vector3 hitPoint, Rigidbody rb)
    {
        if (isHit == false)
        {
            isHit = true;
            FlyTheBall(hitPoint, rb);
        }

    }

    private void CalHitScore(Vector3 hitPoint)
    {
        if (Managers.Game.GameState != Define.GameState.InGround)
            return;

        var topCheck = (_top.transform.position - hitPoint).sqrMagnitude;
        var midCheck = (_mid.transform.position - hitPoint).sqrMagnitude;
        var bottomCheck = (_bottom.transform.position - hitPoint).sqrMagnitude;
        var midRowCheck = (_midRow.transform.position - hitPoint).sqrMagnitude;

        var min = Mathf.Min(topCheck, midCheck, bottomCheck, midRowCheck);

        if (min == topCheck)
            _hitPos = HitPos.Top;
        else if (min == midCheck)
            _hitPos = HitPos.Mid;
        else if (min == bottomCheck)
            _hitPos = HitPos.Bottom;
        else if (min == midRowCheck)
            _hitPos = HitPos.MidRow;
        else
        {
            Debug.LogWarning("없는 타입");
        }


        float scroe = GetScoreFromDistance(topCheck, midRowCheck, midCheck, bottomCheck);
        Managers.Game.GetGameScoreAndGetPosition(scroe, hitPoint);
    }

    private void FlyTheBall(Vector3 hitPoint, Rigidbody rb)
    {

        Debug.Log("TODO 볼 날라가는 효과");

        if (Managers.Game.GameState != Define.GameState.InGround)
            return;

        var topCheck = (_top.transform.position - hitPoint).sqrMagnitude;
        var midCheck = (_mid.transform.position - hitPoint).sqrMagnitude;
        var bottomCheck = (_bottom.transform.position - hitPoint).sqrMagnitude;
        var midRowCheck = (_midRow.transform.position - hitPoint).sqrMagnitude;

        float forceAmount = 20f;
        Vector3 forceDirection = Vector3.zero;
        Color hitColor = Color.white;

        forceDirection = GetDirection(topCheck, midCheck, midRowCheck, bottomCheck, ref hitColor);


        forceDirection += new Vector3(Random.Range(-0.3f, 0.3f), 0, 0);

        rb.velocity = forceDirection;


        var ballFlyVelocity = forceDirection;


        ballFlyVelocity.x = ballFlyVelocity.x * (forceAmount + (Managers.Game.Speed * 0.2f));
        ballFlyVelocity.y = ballFlyVelocity.y * (forceAmount + (Managers.Game.Speed * 0.2f));
        ballFlyVelocity.z = ballFlyVelocity.z * (forceAmount + (Managers.Game.Speed * 0.2f));

        ballFlyVelocity.y = Mathf.Clamp(ballFlyVelocity.y, 1f, 55f);

        rb.AddForce(ballFlyVelocity, ForceMode.Impulse);
        rb.useGravity = true;

        // 레이 그리기
        Debug.DrawRay(transform.parent.position, ballFlyVelocity, Color.yellow, 10f); // 빨간색 레이를 2초 동안 보여줌
        Debug.DrawRay(transform.parent.position, forceDirection*forceAmount, Color.magenta, 10f); // 빨간색 레이를 2초 동안 보여줌

        isHit = false;

    }

    private void HitPointCheck(Vector3 hitPoint, Rigidbody rb)
    {
        if (Managers.Game.GameState != Define.GameState.InGround)
            return;

        var topCheck = (_top.transform.position - hitPoint).sqrMagnitude;
        var midCheck = (_mid.transform.position - hitPoint).sqrMagnitude;
        var bottomCheck = (_bottom.transform.position - hitPoint).sqrMagnitude;
        var midRowCheck = (_midRow.transform.position - hitPoint).sqrMagnitude;

        var min = Mathf.Min(topCheck, midCheck, bottomCheck, midRowCheck);

        if (min == topCheck)
            _hitPos = HitPos.Top;
        else if (min == midCheck)
            _hitPos = HitPos.Mid;
        else if (min == bottomCheck)
            _hitPos = HitPos.Bottom;
        else if (min == midRowCheck)
            _hitPos = HitPos.MidRow;
        else
        {
            Debug.LogWarning("없는 타입");
        }

        float forceAmount = 20f;
        Vector3 forceDirection = Vector3.zero;
        Color hitColor = Color.white;

        forceDirection = GetDirection(topCheck, midCheck, midRowCheck, bottomCheck, ref hitColor);
        float scroe = GetScoreFromDistance(topCheck, midRowCheck, midCheck, bottomCheck);


        rb.velocity = forceDirection;
        rb.AddForce(forceDirection * forceAmount, ForceMode.Impulse);
        rb.useGravity = true;

        // 레이 그리기
        float rayLength = 5f; // 원하는 레이의 길이
        Debug.DrawRay(transform.parent.position, forceDirection * (forceAmount + Managers.Game.Speed*0.5f), hitColor, 3f); // 빨간색 레이를 2초 동안 보여줌

        Debug.Log($"HitPoint :  {_hitPos}  Score : {scroe} : Color {hitColor}");


        Managers.Game.GetGameScoreAndGetPosition(scroe, hitPoint);

        isHit = false;

    }



    private Vector3 GetDirection(float topDist, float midDist, float midRowDist, float bottomDist, ref Color hitColor)
    {
        Vector3 forceDirection = Vector3.zero;

        Vector3 topDir = transform.parent.forward + transform.parent.up * 2f;
        Vector3 midDir = transform.parent.forward + transform.parent.up;
        Vector3 midRowDir = transform.parent.forward + transform.parent.up * 0.75f;
        Vector3 bottomDir = transform.parent.forward + transform.parent.up * 0.25f;

        Color topColor = Color.yellow;
        Color midColor = Color.green;
        Color midRowColor = Color.cyan;
        Color bottomColor = Color.red;

        // 거리에 따라 forceDirection과 hitColor를 결정
        if (topDist < midDist && topDist < midRowDist && topDist < bottomDist) // Top에 가장 가까울 때
        {
            float t = topDist / (topDist + midDist); // 보간 비율 계산
            forceDirection = Vector3.Lerp(topDir, midDir, t);
            hitColor = Color.Lerp(topColor, midColor, t);
        }
        else if (midDist < topDist && midDist < midRowDist && midDist < bottomDist) // Mid에 가장 가까울 때
        {
            if (topDist < midRowDist)
            {
                float t = midDist / (midDist + topDist);
                forceDirection = Vector3.Lerp(midDir, topDir, t);
                hitColor = Color.Lerp(midColor, topColor, t);
            }
            else
            {
                float t = midDist / (midDist + midRowDist);
                forceDirection = Vector3.Lerp(midDir, midRowDir, t);
                hitColor = Color.Lerp(midColor, midRowColor, t);
            }
        }
        else if (midRowDist < topDist && midRowDist < midDist && midRowDist < bottomDist) // MidRow에 가장 가까울 때
        {
            if (midDist < bottomDist)
            {
                float t = midRowDist / (midRowDist + midDist);
                forceDirection = Vector3.Lerp(midRowDir, midDir, t);
                hitColor = Color.Lerp(midRowColor, midColor, t);
            }
            else
            {
                float t = midRowDist / (midRowDist + bottomDist);
                forceDirection = Vector3.Lerp(midRowDir, bottomDir, t);
                hitColor = Color.Lerp(midRowColor, bottomColor, t);
            }
        }
        else // Bottom에 가장 가까울 때
        {
            if (midRowDist < midDist)
            {
                float t = bottomDist / (bottomDist + midRowDist);
                forceDirection = Vector3.Lerp(bottomDir, midRowDir, t);
                hitColor = Color.Lerp(bottomColor, midRowColor, t);
            }
            else
            {
                float t = bottomDist / (bottomDist + midDist);
                forceDirection = Vector3.Lerp(bottomDir, midDir, t);
                hitColor = Color.Lerp(bottomColor, midColor, t);
            }
        }

        return forceDirection;
    }


    public float GetScoreFromDistance(float topDist, float midRow, float midDist, float bottomDist)
    {
        float score = 0f;

        float midRowDist = midRow;

        //TODO
        var batPower = Managers.Resource.GetScriptableObjet<BatScriptableObject>(Managers.Game.PlayerInfo.equipBatId).power;

        if (topDist < midDist && topDist < midRowDist && topDist < bottomDist) // Top에 가장 가까울 때
        {
            float t = topDist / (topDist + midDist); // 보간 비율 계산
            score = Mathf.Lerp(40 + batPower, 100 + batPower, t);
        }
        else if (midDist < topDist && midDist < midRowDist && midDist < bottomDist) // Mid에 가장 가까울 때
        {
            if (topDist < midRowDist) // Mid에서 Top이 더 가깝다면
            {
                float t = midDist / (midDist + topDist);
                score = Mathf.Lerp(100 + batPower, 40 + batPower, t);
            }
            else // Mid에서 MidRow가 더 가깝다면
            {
                float t = midDist / (midDist + midRowDist);
                score = Mathf.Lerp(100 + batPower, 50 + batPower, t);
            }
        }
        else if (midRowDist < topDist && midRowDist < midDist && midRowDist < bottomDist) // MidRow에 가장 가까울 때
        {
            if (midDist < bottomDist) // MidRow에서 Mid가 더 가깝다면
            {
                float t = midRowDist / (midRowDist + midDist);
                score = Mathf.Lerp(50 + batPower, 100 + batPower, t);
            }
            else // MidRow에서 Bottom이 더 가깝다면
            {
                float t = midRowDist / (midRowDist + bottomDist);
                score = Mathf.Lerp(50 + batPower, 1 + batPower, t);
            }
        }
        else // Bottom에 가장 가까울 때
        {
            float t = bottomDist / (bottomDist + midRowDist);
            score = Mathf.Lerp(1 + batPower, 50 + batPower, t);
        }


        Debug.Log(score);

        return score;

    }

    private void OnDestroy()
    {
        transform.DOKill();
        transform.localScale = originalScale;
    }


#if UNITY_EDITOR

    private void OnDrawGizmos()
    {

        float forceAmount = 2f;

        Gizmos.color = Color.blue;
        Vector3 forceDirection = transform.parent.forward + transform.parent.up * 2f;
        Gizmos.DrawRay(transform.position, forceDirection * forceAmount);
        forceDirection += new Vector3(1, 0, 0);
        Gizmos.DrawRay(transform.position, forceDirection * forceAmount);
        forceDirection += new Vector3(-1, 0, 0);
        Gizmos.DrawRay(transform.position, forceDirection * forceAmount);

        Gizmos.color = Color.green;
        forceDirection = transform.parent.forward + transform.parent.up * 1f;
        Gizmos.DrawRay(transform.position, forceDirection * forceAmount);
        forceDirection += new Vector3(0, 0, 1);
        Gizmos.DrawRay(transform.position, forceDirection * forceAmount);
        forceDirection += new Vector3(0, 0, -1);
        Gizmos.DrawRay(transform.position, forceDirection * forceAmount);

        Gizmos.color = Color.cyan;
        forceDirection = transform.parent.forward + transform.parent.up * 0.5f;
        Gizmos.DrawRay(transform.position, forceDirection * forceAmount);
        forceDirection += new Vector3(1, 0, 1);
        Gizmos.DrawRay(transform.position, forceDirection * forceAmount);
        forceDirection += new Vector3(-1, 0, -1);
        Gizmos.DrawRay(transform.position, forceDirection * forceAmount);
    }

#endif


}
