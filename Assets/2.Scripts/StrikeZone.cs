using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class StrikeZone : MonoBehaviour
{
    public Vector3 size { get; private set; }
    private Vector3 center;
    public BoxCollider boxCollider { get; private set; }

    [SerializeField] Transform Top, Mid, MidRow, Bottom;



    public Vector2 TopPos { get { return Top.position; }  }
    public Vector3 WorldVector { get { return TopPos - BottomPos; }  }
    public Vector3 MidPos { get { return Mid.position; }  }
    public Vector2 BottomPos { get { return Bottom.position; }  }
    public GameObject cube;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();

        size = boxCollider.size;
        center = boxCollider.center;
        Managers.Game.SetStrikeZone(this);


    }


}
