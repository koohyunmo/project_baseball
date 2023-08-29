using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameObject : MonoBehaviour
{
    public int id;

    public virtual void OnEnable()
    {
        id = Managers.Object.GetId();
    }

    public virtual void OnDisable()
    {
        id = -1;
    }
}
