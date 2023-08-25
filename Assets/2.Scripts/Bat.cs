using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UI.Image;

public class Bat : MonoBehaviour
{
    Define.GameState GameState { get { return Managers.Game.GameState; } }

    public Transform HitColider { get { return batBoxCollider.transform; } }

    public BoxCollider batBoxCollider;
    private MeshCollider batMeshCollider;
    public GameObject model;

    public float lerpSpeed = 2.5f;
    private bool isMoving = false;

    public Transform startBat;
    public Transform endBat;

    bool isSwinging;
    [SerializeField] float lerpTimer;
    float maxTime = 0.3f;

    Vector3 originalBatPos = Vector3.zero;
    Quaternion originalBatRot;
    Vector3 originalCamPosition;
    Animator anim;

    public AnimationCurve swingCurve;

    Camera mainCamera;

    private bool isReturning = false; // 배트가 원래 위치로 돌아가는 중인지 체크하는 변수
    private float returnDelay = 0.2f; // 원래 위치로 돌아가기 전 대기 시간
    private float returnTimer = 0; // 대기 시간을 체크하는 타이머
    private float returnLerpTime = 0; // 원래 위치로 돌아갈 때 사용하는 lerp 타이머
    private float returnLerpSpeed = 1f; // 원래 위치로 돌아갈 때의 lerp 속도





    void Start()
    {
        First();

    }

    private void First()
    {
        batBoxCollider = GetComponentInChildren<BoxCollider>();
        batMeshCollider = GetComponentInChildren<MeshCollider>();


        originalBatPos = model.transform.position;
        originalBatRot = model.transform.rotation;

        originalCamPosition = Camera.main.transform.position;

        mainCamera = Camera.main;

        anim = model.GetComponent<Animator>();

        Managers.Game.SetBat(this);
        Managers.Game.SetStrikeCallBack(HutSwing);
        Managers.Game.SetMoveBat(OnltMoveModel);
    }

    public void ChangeBatMat(Material mat)
    {
        model.GetComponent<MeshRenderer>().sharedMaterial = mat;
    }

    public void ChangeBatMesh(Mesh mesh)
    {
        model.GetComponent<MeshFilter>().sharedMesh = mesh;
    }

    private void OnltMoveModel()
    {
        model.transform.position = originalBatPos;
        model.transform.rotation = originalBatRot;
    }


    private void Update()
    {
        switch (GameState)
        {
            case Define.GameState.Home:
                BatOff();
                break;
            case Define.GameState.Ready:
                ColiderOn();
                break;
            case Define.GameState.InGround:
                BatOn();
                break;
            case Define.GameState.End:
                ColiderOff();
                break;
        }
    }


    private void BatOff()
    {
        if (batBoxCollider != null && batBoxCollider.gameObject.activeSelf == true)
        {
            batBoxCollider.gameObject.SetActive(false);
        }
    }

    private void BatOn()
    {
        if (batBoxCollider != null &&  batBoxCollider.gameObject.activeSelf == false)
        {
            batBoxCollider.gameObject.SetActive(true);
        }
    }

    private void ColiderOff()
    {
        if (batBoxCollider != null && batBoxCollider.enabled == true)
        {
            //batMeshCollider.enabled = false;
            batBoxCollider.enabled = false;
        }
    }

    private void ColiderOn()
    {
        if (batBoxCollider != null && batBoxCollider.enabled == false)
        {
            //batMeshCollider.enabled = true;
            batBoxCollider.enabled = true;
        }
    }

    private void FixedUpdate()
    {
        SwingAndBack();
        ClampToCameraView();
    }

    private void ClampToCameraView()
    {
        if (mainCamera == null) return; // 카메라가 할당되지 않았다면 함수 종료
        if (GameState != Define.GameState.InGround) return; // 게임중이 아니라면 함수 종료

        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(model.transform.position);

        // 0과 1 사이로 뷰포트 좌표 제한
        viewportPosition.x = Mathf.Clamp(viewportPosition.x, 0f, 1f);
        viewportPosition.y = Mathf.Clamp(viewportPosition.y, 0f, 1f);

        // 제한된 뷰포트 좌표를 월드 좌표로 변환 후, 오브젝트 위치 업데이트
        model.transform.position = mainCamera.ViewportToWorldPoint(viewportPosition);
    }

    private void SwingAndBack()
    {
        float slowSpeed = 1f;

        if (isSwinging)
        {
            if (GameState != Define.GameState.InGround && Managers.Game.isReplay)
            {
                slowSpeed = 1f * Managers.Game.ReplaySlowMode;
                anim.speed = 1f * Managers.Game.ReplaySlowMode;
            }
            else
            {
                anim.speed = 1f;
            }
                

            lerpTimer += Time.deltaTime * lerpSpeed * slowSpeed;
            

            float curveValue = swingCurve.Evaluate(lerpTimer);  // 0과 1 사이의 lerpTime 값을 기반으로 곡선 값 추출

            // 회전 및 위치 보간
            model.transform.position = Vector3.Lerp(originalBatPos, endBat.position, curveValue);
            anim.Play("Bat_Swing");
            //anim.Play("Bat_Weak_Swing");
            //anim.Play("Bat_Swing");

            if (lerpTimer >= maxTime)
            {
                lerpTimer = 0;
                isSwinging = false;
                isReturning = true;
                anim.Play("Bat_Idle");
            }
                
        }



        if (isReturning)
        {
            returnTimer += Time.deltaTime;

            if (returnTimer >= returnDelay)
            {
                returnLerpTime += Time.deltaTime * returnLerpSpeed;

                // 선형적으로 원래 위치 및 회전으로 돌아오기
                model.transform.position = Vector3.Lerp(endBat.position, originalBatPos, returnLerpTime);
                model.transform.rotation = Quaternion.Lerp(endBat.rotation, originalBatRot, returnLerpTime);

                if (returnLerpTime >= 1)
                {
                    returnLerpTime = 0;
                    returnTimer = 0;
                    isReturning = false;
                    model.transform.position = originalBatPos;
                    model.transform.rotation = originalBatRot;
                }
            }
        }
    }

    private void SwingAnim()
    {
        if (GameState == Define.GameState.End)
        {
            string animationName = "Bat_Swing"; // 애니메이션 클립 이름
            isSwinging = false;
            float stopAt = 0.2f; // 애니메이션의 중간에서 멈추려면 0.5 (0은 시작, 1은 끝)

            // 애니메이션의 정확한 시점에서 멈추기
            anim.Play(animationName, 0, stopAt);
            anim.speed = 0;
        }
    }

    public void Swing()
    {
        if (!isSwinging) // 스윙 중이 아닐 때만 스윙 시작
        {
            Managers.Game.HitEvent();
            isSwinging = true;
        }

        // 카메라 효과를 추가합니다.
        StartCoroutine(CameraShake());
    }

    public void HutSwing()
    {
        if (!isSwinging) // 스윙 중이 아닐 때만 스윙 시작
        {
            isSwinging = true;
        }
    }

    IEnumerator CameraShake()
    {
        // 카메라 흔들림 효과를 구현합니다.
        float shakeDuration = 0.5f; // 흔들림의 지속 시간입니다.
        float shakeMagnitude = 0.05f; // 흔들림의 크기입니다.

        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            float x = originalCamPosition.x + Random.Range(-1f, 1f) * shakeMagnitude;
            float z = originalCamPosition.z + Random.Range(-1f, 1f) * shakeMagnitude;

            Camera.main.transform.position = new Vector3(x, originalCamPosition.y, z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        Camera.main.transform.position = originalCamPosition;
    }

}
