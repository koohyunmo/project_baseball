using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class StrikeZone : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 size;
    float sizeNorm;
    private BoxCollider boxCollider;
   

    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();

        size = boxCollider.size;

        sizeNorm = Vector3.Dot(size, size);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            var localDetectedPos = boxCollider.transform.InverseTransformPoint(other.transform.position); // World position을 Local position으로 변환



        }
    }


}
