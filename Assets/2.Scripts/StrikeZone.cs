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

    private void OnTriggerExit(Collider other)
    {
        if (other != null && other.CompareTag("Ball"))
        {
            Debug.Log("Strike");
        }
    }


}
