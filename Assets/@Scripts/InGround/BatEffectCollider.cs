using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatEffectCollider : MonoBehaviour
{
    public Transform effectPos;
    [SerializeField]public ParticleSystem hitEffectPrefab; // 히트 이펙트 프리팹
    private ParticleSystem[] hitEffects; // 풀링된 히트 이펙트 배열
    private int poolSize = 3; // 풀 크기

    private void Start()
    {
        // 파티클 풀링 초기화
        hitEffects = new ParticleSystem[poolSize];
        hitEffectPrefab.gameObject.SetActive(false);
        for (int i = 0; i < poolSize; i++)
        {

            hitEffects[i] = Instantiate(hitEffectPrefab);
            hitEffects[i].gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 트리거에 닿았을 때 히트 이펙트 활성화
        ParticleSystem effect = GetPooledEffect();
        if (effect != null)
        {
            effect.transform.position = other.transform.position;
            effect.gameObject.SetActive(true);
            float duration = effect.GetComponent<ParticleSystem>().main.duration;

            StartCoroutine(co_ParticleOff(effect.gameObject, duration));
        }
    }

    private ParticleSystem GetPooledEffect()
    {
        for (int i = 0; i < poolSize; i++)
        {
            if (hitEffects[i].gameObject.activeInHierarchy == false)
            {
                return hitEffects[i];
            }
        }
        return null;
    }

    IEnumerator co_ParticleOff(GameObject effect, float duration)
    {
        yield return new WaitForSeconds(duration);
        effect.SetActive(false);

    }

}
