using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameObjectController : MonoBehaviour
{
    public int ObjId { get; set; }

    public virtual void Clear()
    {
        Debug.Log($"Obj Clear {ObjId}");
    }
}
