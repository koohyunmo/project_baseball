using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TestMove : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.DOMoveZ(-10, 4).SetLoops(-1);
    }


}
