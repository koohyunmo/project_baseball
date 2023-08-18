using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    Vector3 _cameraPos = Vector3.zero;
    Vector3 _cameraRot = Vector3.zero;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        _cameraPos = Camera.main.transform.position;
        _cameraRot = Camera.main.transform.eulerAngles;
        Managers.Game.SetMainCamera(this);
    }

    public void MoveOriginaPos()
    {
        Camera.main.transform.position = _cameraPos;
        Camera.main.transform.eulerAngles = _cameraRot;
    }
    
}
