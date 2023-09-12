using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Vector3 offset;

    private Vector3 _cameraPos = Vector3.zero;
    private Quaternion _cameraRot;


    public Cinemachine.CinemachineVirtualCamera _virtualCamera;
    private Vector3 vcamPos;
    private Quaternion vcamRot;

    public Vector3 replayCamOffsetPos = Vector3.zero;
    public Quaternion replayCamOffsetRot;

    private void Awake()
    {
        transform.position = offset; 
        transform.rotation = Quaternion.identity;

        vcamPos = _virtualCamera.transform.localPosition;
        vcamRot = _virtualCamera.transform.rotation;
        _cameraPos = transform.position;
        _cameraRot = transform.rotation;

        _virtualCamera.gameObject.SetActive(false);
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        Managers.Game.SetMainCamera(this);
    }

    public void CameraMove(Vector3 pos)
    {
        gameObject.transform.DOMoveZ(pos.z + 5f,0.5f);
        gameObject.transform.DOMoveY(1f,0);

    }

    public void OnReplay(Transform target)
    {
        gameObject.transform.position = _cameraPos + replayCamOffsetPos;
        gameObject.transform.rotation = _cameraRot * replayCamOffsetRot;
        _virtualCamera.gameObject.SetActive(true);
        _virtualCamera.LookAt = target;
    }
    public void OnReplay(Transform target, Vector3 pos)
    {
        gameObject.transform.position = pos + replayCamOffsetPos;
        gameObject.transform.rotation = _cameraRot * replayCamOffsetRot;
        _virtualCamera.gameObject.SetActive(true);
        _virtualCamera.LookAt = target;
    }

    public void OffRePlay()
    {
        gameObject.transform.DOKill();
        if(_virtualCamera.LookAt != null)
        {
            Managers.Resource.Destroy(_virtualCamera.LookAt.transform.gameObject);
            _virtualCamera.LookAt = null;
        }
        MoveOriginaPos();
    }

    public void ReplayBack(Transform target, Vector3 pos)
    {
        gameObject.transform.DOMove(pos + replayCamOffsetPos,0f);
        gameObject.transform.DOMoveY(transform.position.y + 1f,0f);
        gameObject.transform.rotation = _cameraRot * replayCamOffsetRot;
        _virtualCamera.LookAt = target;
    }
    public void MoveOriginaPos()
    {
        _virtualCamera.transform.localPosition = vcamPos;
        _virtualCamera.transform.rotation = vcamRot;
        _virtualCamera.gameObject.SetActive(false);
        transform.position = _cameraPos;
        transform.transform.rotation = _cameraRot;
    }
    
}
