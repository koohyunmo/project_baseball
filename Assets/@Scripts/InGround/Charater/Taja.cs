using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Taja : MonoBehaviour
{
    [SerializeField] Transform batPos;


    public void Swing(Vector3 hitPoint)
    {
        Debug.Log("Hit");
    }
}
