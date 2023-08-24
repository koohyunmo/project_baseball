using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatEffectCollider : MonoBehaviour
{
    public Transform effectPos;
    [SerializeField]public GameObject hitEffectPrefab; // ��Ʈ ����Ʈ ������
    private GameObject[] hitEffects; // Ǯ���� ��Ʈ ����Ʈ �迭
    private int poolSize = 5; // Ǯ ũ��

    private void Awake()
    {
        // ��ƼŬ Ǯ�� �ʱ�ȭ
        hitEffects = new GameObject[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            hitEffects[i] = Instantiate(hitEffectPrefab);
            hitEffects[i].SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ʈ���ſ� ����� �� ��Ʈ ����Ʈ Ȱ��ȭ
        GameObject effect = GetPooledEffect();
        if (effect != null)
        {
            effect.transform.position = other.transform.position;
            effect.SetActive(true);
            float duration = effect.GetComponent<ParticleSystem>().main.duration;

            StartCoroutine(co_ParticleOff(effect, duration));
        }
    }

    private GameObject GetPooledEffect()
    {
        for (int i = 0; i < poolSize; i++)
        {
            if (hitEffects[i].activeInHierarchy == false)
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
