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
    [SerializeField] Transform _effectPos;

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
        _effectPos.gameObject.SetActive(false);
        Managers.Game.SetBatCollider(this);
    }

    public Vector3 BatMid()
    {
        return _mid.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            Debug.Log("Hit");


            _effectPos.gameObject.SetActive(true);

            var hitPoint = other.gameObject.transform.position;

            // ���� ������
            Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();

            if (rb != null)
            {
                if(Managers.Game.GameState == Define.GameState.InGround)
                {
                    other.gameObject.GetComponent<BallController>().SetHit();

                }   

                if(isHit == false)
                {
                    isHit = true;
                    HitPointCheck(hitPoint, rb);
                }
               
            }

            bat.Swing(hitPoint);

            Managers.Effect.Play("HitA", hitPoint);

        }
    }

    private void HitPointCheck(Vector3 hitPoint,Rigidbody rb)
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
        else if(min == midRowCheck)
            _hitPos = HitPos.MidRow;
        else
        {
            Debug.LogWarning("���� Ÿ��");
        }

        float forceAmount = 20f;
        Vector3 forceDirection = Vector3.zero;
        Color hitColor = Color.white;

        forceDirection = GetDirection(topCheck, midCheck, midRowCheck, bottomCheck,ref hitColor);
        float scroe = GetScoreFromDistance(topCheck,midRowCheck, midCheck, bottomCheck);


        rb.velocity = forceDirection;
        rb.AddForce(forceDirection * forceAmount, ForceMode.Impulse);
        rb.useGravity = true;

        // ���� �׸���
        float rayLength = 5f; // ���ϴ� ������ ����
        Debug.DrawRay(transform.parent.position, forceDirection * rayLength, hitColor, 3f); // ������ ���̸� 2�� ���� ������

        Debug.Log($"HitPoint :  {_hitPos}  Score : {scroe} : Color {hitColor}");


        Managers.Game.GetGameScoreAndGetPosition(scroe,hitPoint);

        isHit = false;

    }



    private Vector3 GetDirection(float topDist, float midDist,float midRowDist, float bottomDist, ref Color hitColor)
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

        // �Ÿ��� ���� forceDirection�� hitColor�� ����
        if (topDist < midDist && topDist < midRowDist && topDist < bottomDist) // Top�� ���� ����� ��
        {
            float t = topDist / (topDist + midDist); // ���� ���� ���
            forceDirection = Vector3.Lerp(topDir, midDir, t);
            hitColor = Color.Lerp(topColor, midColor, t);
        }
        else if (midDist < topDist && midDist < midRowDist && midDist < bottomDist) // Mid�� ���� ����� ��
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
        else if (midRowDist < topDist && midRowDist < midDist && midRowDist < bottomDist) // MidRow�� ���� ����� ��
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
        else // Bottom�� ���� ����� ��
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

        if (topDist < midDist && topDist < midRowDist && topDist < bottomDist) // Top�� ���� ����� ��
        {
            float t = topDist / (topDist + midDist); // ���� ���� ���
            score = Mathf.Lerp(40, 100, t);
        }
        else if (midDist < topDist && midDist < midRowDist && midDist < bottomDist) // Mid�� ���� ����� ��
        {
            if (topDist < midRowDist) // Mid���� Top�� �� �����ٸ�
            {
                float t = midDist / (midDist + topDist);
                score = Mathf.Lerp(100, 40, t);
            }
            else // Mid���� MidRow�� �� �����ٸ�
            {
                float t = midDist / (midDist + midRowDist);
                score = Mathf.Lerp(100, 50, t);
            }
        }
        else if (midRowDist < topDist && midRowDist < midDist && midRowDist < bottomDist) // MidRow�� ���� ����� ��
        {
            if (midDist < bottomDist) // MidRow���� Mid�� �� �����ٸ�
            {
                float t = midRowDist / (midRowDist + midDist);
                score = Mathf.Lerp(50, 100, t);
            }
            else // MidRow���� Bottom�� �� �����ٸ�
            {
                float t = midRowDist / (midRowDist + bottomDist);
                score = Mathf.Lerp(50, 1, t);
            }
        }
        else // Bottom�� ���� ����� ��
        {
            float t = bottomDist / (bottomDist + midRowDist);
            score = Mathf.Lerp(1, 50, t);
        }

        return score;

    }


#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        
        float forceAmount = 2f;

        Gizmos.color = Color.blue;
        Vector3 forceDirection = transform.parent.forward + transform.parent.up*2f;
        Gizmos.DrawRay(transform.position, forceDirection * forceAmount);

        Gizmos.color = Color.green;
        forceDirection = transform.parent.forward + transform.parent.up*1f;
        Gizmos.DrawRay(transform.position, forceDirection * forceAmount);

        Gizmos.color = Color.cyan;
        forceDirection = transform.parent.forward + transform.parent.up * 0.5f;
        Gizmos.DrawRay(transform.position, forceDirection * forceAmount);
    }

#endif


}
