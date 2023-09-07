using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingCollider : MonoBehaviour
{

    Bat _bat;

    private void Start()
    {
        _bat = transform.parent.GetComponentInParent<Bat>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            Debug.Log("SwingANim");
            _bat.SwingBatAnim();
        }
    }
}
