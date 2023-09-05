using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatEffectCollider : MonoBehaviour
{
    public Transform effectPos;
    [SerializeField]public ParticleSystem hitEffectPrefab; // ��Ʈ ����Ʈ ������
    private ParticleSystem[] hitEffects; // Ǯ���� ��Ʈ ����Ʈ �迭
    private int poolSize = 3; // Ǯ ũ��

    private void Start()
    {
        // ��ƼŬ Ǯ�� �ʱ�ȭ
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
        // Ʈ���ſ� ����� �� ��Ʈ ����Ʈ Ȱ��ȭ
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
