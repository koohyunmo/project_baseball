using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BackgroundMove : MonoBehaviour
{

    public GameObject[] obj;


    private void Start()
    {
        foreach (GameObject go in obj) 
        {
            go.transform.DOPunchRotation(new Vector3(0, 5, 0), 20f,1).SetLoops(-1,LoopType.Yoyo);
        }
    }
}
