using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Vector3 offset;

    Vector3 _cameraPos = Vector3.zero;
    Vector3 _cameraRot = Vector3.zero;


    public Cinemachine.CinemachineVirtualCamera _virtualCamera;

    private void Awake()
    {
        transform.position = offset; 
        transform.rotation = Quaternion.identity;
        _virtualCamera.gameObject.SetActive(false);
    }

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

    public void OnReplay(Transform target)
    {
        gameObject.transform.position = _cameraPos;
        gameObject.transform.eulerAngles = _cameraRot;
        _virtualCamera.gameObject.SetActive(true);
        _virtualCamera.LookAt = target;
    }

    public void OffRePlay()
    {
        Managers.Resource.Destroy(_virtualCamera.LookAt.transform.gameObject);
        _virtualCamera.LookAt = null;
        StartCoroutine(co_MoveCam());
    }

    public IEnumerator co_MoveCam()
    {
        Vector3 startingPosition = _virtualCamera.transform.position;
        Quaternion startingRotation = _virtualCamera.transform.rotation;

        Vector3 targetPosition = _cameraPos;
        Quaternion targetRotation = Quaternion.Euler(_cameraRot);

        float duration = 1.0f; // 전환에 걸리는 시간, 원하는 대로 조절
        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            _virtualCamera.transform.position = Vector3.Lerp(startingPosition, targetPosition, t);
            _virtualCamera.transform.rotation = Quaternion.Slerp(startingRotation, targetRotation, t);

            elapsedTime += Time.deltaTime;
            yield return null;

            if(Managers.Game.GameState == Define.GameState.Home)
            {
                // 보간이 완료된 후의 최종 값으로 설정
                _virtualCamera.transform.position = _cameraPos;
                _virtualCamera.transform.rotation = Quaternion.Euler(_cameraRot);
                _virtualCamera.gameObject.SetActive(false);
                yield break;
            }
        }

        // 보간이 완료된 후의 최종 값으로 설정
        _virtualCamera.transform.position = _cameraPos;
        _virtualCamera.transform.rotation = Quaternion.Euler(_cameraRot);
        _virtualCamera.gameObject.SetActive(false);

        yield break;
    }


    public void MoveOriginaPos()
    {
        Camera.main.transform.position = _cameraPos;
        Camera.main.transform.eulerAngles = _cameraRot;
    }
    
}
