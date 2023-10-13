using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

using Debug = UnityEngine.Debug;

public class Bat : MonoBehaviour
{
    Define.GameState GameState { get { return Managers.Game.GameState; } }

    public Transform HitColiderTransform { get { return batBoxCollider.transform; } }
    public Transform BatModelParent { get; private set; }
    public Transform HandleTransform;

    public BoxCollider batBoxCollider;
    private MeshCollider batMeshCollider;
    public GameObject model;

    public float lerpSpeed = 2.5f;
    private bool isMoving = false;

    public Transform startBat;
    public Transform endBat;

    [SerializeField] float lerpTimer;
    float maxTime = 0.3f;

    Vector3 originalBatPos = Vector3.zero;
    Quaternion originalBatRot;
    Vector3 originalCamPosition;

    AnimatorStateInfo stateInfo;
    Animator anim;

    public AnimationCurve swingCurve;

    Camera mainCamera;

    private float returnDelay = 0.2f; // 원래 위치로 돌아가기 전 대기 시간
    private float returnTimer = 0; // 대기 시간을 체크하는 타이머
    private float returnLerpTime = 0; // 원래 위치로 돌아갈 때 사용하는 lerp 타이머
    private float returnLerpSpeed = 1f; // 원래 위치로 돌아갈 때의 lerp 속도

    public GameObject modelParent;
    public GameObject colliderParent;

    private Action<Vector3, Rigidbody> _flyTheBall;
    [SerializeField]
    [Header("SKILL")]
    public Transform skillPos;
    public Transform skillPos2;
    public enum BatState
    {
        Idle,
        Swing,
    }

    public BatState batState { get; private set; } = BatState.Idle;

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

        BatModelParent = model.transform.parent;

        originalCamPosition = Camera.main.transform.position;

        mainCamera = Camera.main;

        anim = model.GetComponent<Animator>();

        Managers.Game.SetBat(this);
        Managers.Game.SetStrikeCallBack(HutSwing);
        Managers.Game.SetMoveBat(OnltMoveModel);
        Managers.Game.SetReplatMoveAction(() => StartCoroutine(co_BatMoveReplay()));
        Managers.Game.SetBatPositionSetting(ClampToCameraViewSetting);
        Managers.Game.batPositionSetting?.Invoke();
        Managers.Game.SetBuffPos(skillPos);
        Managers.Game.SetColliderPos(skillPos2);

        HitColiderTransform.gameObject.SetActive(false);

        SetBetHandle();

        Managers.Skill.SetModelLocalScale(model.transform.localScale);
        Managers.Skill.SetColliderLocalScale(HitColiderTransform.lossyScale);

    }

    public void SetBetHandle()
    {

        if (Managers.Game.BatPosition == Define.BatPosition.Left)
            MakeHandle(batBoxCollider.bounds.max.x, "batBoxCollider.bounds.max.x", -0.4f, -0.2f);
        else
            MakeHandle(batBoxCollider.bounds.min.x, "batBoxCollider.bounds.min.x", -0.4f, -0.2f);

    }

    private void MakeHandle(float inputOffset, string name, float offsetX, float offsetZ)
    {
        // 박스 콜라이더의 z 크기를 가져옵니다. (배트의 길이)
        float batBox = inputOffset;
        // 손잡이의 상대적 위치를 계산합니다. 여기서는 z 크기의 20%를 사용하였습니다.
        // 0.3f 가손잡이 위치
        float handleOffset = batBox * offsetX;
        // 회전을 고려하여 월드 좌표로 변환합니다. 손잡이는 z축 방향으로 배치됩니다.
        Vector3 handleLocalPosition = new Vector3(batBoxCollider.center.x, batBoxCollider.center.y + handleOffset, batBoxCollider.center.z + offsetZ);
        Vector3 handleWorldPosition = batBoxCollider.transform.TransformPoint(handleLocalPosition);
        // 손잡이의 트랜스폼을 생성합니다.
        Transform tr = new GameObject().transform;
        HandleTransform = tr;
        HandleTransform.gameObject.name = name + offsetX.ToString();
        HandleTransform.position = handleWorldPosition;
        HandleTransform.parent = batBoxCollider.transform;
    }

    public void ChangeBatMat(List<Material> mats)
    {
        model.GetComponent<MeshRenderer>().sharedMaterials = mats.ToArray();
    }

    public void ChangeBatMesh(Mesh mesh)
    {
        model.GetComponent<MeshFilter>().sharedMesh = mesh;
        endBat.GetComponent<MeshFilter>().sharedMesh = mesh;
    }


    private void OnltMoveModel()
    {
        model.transform.position = originalBatPos;
        model.transform.rotation = originalBatRot;
    }

    public void BatOff()
    {
        {
            batBoxCollider.gameObject.SetActive(false);
        }
    }

    public void BatOn()
    {
        {

            batBoxCollider.gameObject.SetActive(true);
        }
    }

    public void ColiderOff()
    {
        {
            //batMeshCollider.enabled = false;
            batBoxCollider.enabled = false;
        }
    }

    public void ColiderOn()
    {
        {
            HitColiderTransform.gameObject.SetActive(true);
            batBoxCollider.enabled = true;
        }
    }

    public void ColiderObjOff()
    {
        HitColiderTransform.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        SwingAndBack();
        ClampToCameraView();
    }

    private void ClampToCameraViewOriginal()
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



    private void ClampToCameraViewSetting()
    {
        var mainCamea = Camera.main;

        Vector3 firstBatPos = new Vector3(0.78f, 0.37f, -6.45f);

        Vector3 viewportPosition = mainCamea.WorldToViewportPoint(firstBatPos);
        //Debug.Log($"World : {HitColiderTransform.position} View : {viewportPosition}");
        //World : (0.78, 0.37, -6.45) View : (0.84, 0.35, 3.55);

        // 0과 1 사이로 뷰포트 좌표 제한
        viewportPosition.x = Mathf.Clamp(viewportPosition.x, 0f, 1f);
        viewportPosition.y = Mathf.Clamp(viewportPosition.y, 0f, 1f);

        // 제한된 뷰포트 좌표를 월드 좌표로 변환
        Vector3 clampedWorldPosition = mainCamera.ViewportToWorldPoint(viewportPosition);

        // z 좌표를 원래의 z 좌표로 유지
        clampedWorldPosition.z = HitColiderTransform.position.z;

        // HitColider 위치 업데이트
        HitColiderTransform.position = clampedWorldPosition;

        var moveVec = Vector3.Lerp(BatModelParent.position, HandleTransform.position, 1f);

        BatModelParent.position = moveVec;
    }


    private void ClampToCameraView()
    {

        if (mainCamera == null) return; // 카메라가 할당되지 않았다면 함수 종료
        if (GameState != Define.GameState.InGround) return; // 게임중이 아니라면 함수 종료

        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(HitColiderTransform.position);

        // 0과 1 사이로 뷰포트 좌표 제한
        viewportPosition.x = Mathf.Clamp(viewportPosition.x, 0f, 1f);
        viewportPosition.y = Mathf.Clamp(viewportPosition.y, 0f, 1f);

        // 제한된 뷰포트 좌표를 월드 좌표로 변환
        Vector3 clampedWorldPosition = mainCamera.ViewportToWorldPoint(viewportPosition);

        // z 좌표를 원래의 z 좌표로 유지
        clampedWorldPosition.z = HitColiderTransform.position.z;

        // HitColider 위치 업데이트
        HitColiderTransform.position = clampedWorldPosition;

        var moveVec =  Vector3.Lerp(BatModelParent.position, HandleTransform.position, 0.1f);

        BatModelParent.position = moveVec;
    }


    private void SwingAndBack()
    {
        float slowSpeed = 1f;

        if (batState == BatState.Swing)
        {
           

            if (GameState != Define.GameState.InGround && Managers.Game.isReplay)
            {
                //slowSpeed = 1f * Managers.Game.ReplaySlowMode;
                //anim.speed = 1f * Managers.Game.ReplaySlowMode;
            }
            else
            {
                anim.speed = 1f;
            }


            lerpTimer += Time.deltaTime * lerpSpeed * slowSpeed;


            float curveValue = swingCurve.Evaluate(lerpTimer);  // 0과 1 사이의 lerpTime 값을 기반으로 곡선 값 추출

            // 회전 및 위치 보간
            //model.transform.position = Vector3.Lerp(originalBatPos, endBat.position, curveValue);
            anim.Play("Bat_Swing");


            // 이시점에서 애니메이션 실행
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

            // stateInfo.normalizedTime은 애니메이션의 현재 진행 시간을 0.0 ~ 1.0의 범위로 반환합니다.
            // 따라서 이 값이 0.5f 이상이면 애니메이션의 중간 지점에 도달했다고 볼 수 있습니다.
            if (stateInfo.normalizedTime >= 0.15f)
            {

                _flyTheBall?.Invoke(_hitPoint,_ballRigid);
                _flyTheBall = null;
                Managers.Game.StopWatch.Stop();
                //Debug.Log($"FunctionB 실행 시간: {Managers.Game.StopWatch.ElapsedMilliseconds}ms");

            }


            // 최대시간 도달후 배트 스윙 + 상태 종료
            if (lerpTimer >= maxTime)
            {
                lerpTimer = 0;
                batState = BatState.Idle;
                anim.Play("Bat_Idle");
                Time.timeScale = 1f;
            }

        }



        if (batState == BatState.Idle)
        {
            returnTimer += Time.deltaTime;

            if (returnTimer >= returnDelay)
            {
                returnLerpTime += Time.deltaTime * returnLerpSpeed;

                // 선형적으로 원래 위치 및 회전으로 돌아오기
                //model.transform.position = Vector3.Lerp(endBat.position, originalBatPos, returnLerpTime);
                //model.transform.rotation = Quaternion.Lerp(endBat.rotation, originalBatRot, returnLerpTime);

                if (returnLerpTime >= 1)
                {
                    returnLerpTime = 0;
                    returnTimer = 0;
                    //model.transform.position = originalBatPos;
                    //model.transform.rotation = originalBatRot;
                }
            }
        }
    }


    private Rigidbody _ballRigid;
    private Vector3 _hitPoint;

    public void SwingCollision(Vector3 hitPoint, Action<Vector3,Rigidbody> flyTheBallAction, Rigidbody ballRigid)
    {
        //Debug.Log("공 타격");
        if (batState == BatState.Swing) //
        {

            Managers.Sound.Play(Define.Sound.Effect, "hit1");
            //Debug.Log("SwingCollision");
            var go = Managers.Obj.Spawn<TextController>("HitScoreText", hitPoint);
            go.transform.position = hitPoint;

            _flyTheBall -= flyTheBallAction;
            _flyTheBall += flyTheBallAction;

            _ballRigid = null;
            _ballRigid = ballRigid;

            _hitPoint = Vector3.zero;
            _hitPoint = hitPoint;

            Managers.Game.HitEvent();
        }

        // 카메라 효과를 추가합니다.
        StartCoroutine(CameraShake());
    }

    public void SwingBatAnim()
    {
        if (batState == BatState.Idle) // 스윙 중이 아닐 때만 스윙 시작
        {
            Managers.Sound.Play(Define.Sound.Effect, "swing1");
            batState = BatState.Swing;
            anim.Play("Bat_Swing");
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }
    }
    public void HutSwing()
    {
        return;

        //if (batState == BatState.Idle) // 스윙 중이 아닐 때만 스윙 시작
        //{
        //    batState = BatState.Swing;
        //}
    }

    IEnumerator co_BatMoveReplay()
    {

        Debug.Log("TODO : 코루틴 관리");

        var data = Managers.Game.batMoveReplayData;

        for (int i = 0; i < data.Count -1; i++)
        {
            // 현재 위치와 다음 위치 사이의 대기 시간을 계산합니다.
            float waitTime = data[i + 1].time - data[i].time;
            HitColiderTransform.localPosition = data[i].position;
            float elapsedTime = 0f;
            Debug.Log(waitTime);

            while (elapsedTime < waitTime)
            {
                if (Managers.Game.GameState != Define.GameState.End)
                {
                    Managers.Game.isReplay = false;
                    Managers.Game.MainCam.MoveOriginaPos();
                    yield break;
                }
                else
                {
                    float t = elapsedTime / waitTime;
                    Vector3 interpolatedPosition = Vector3.Lerp(HitColiderTransform.localPosition, data[i].position, t);
                    HitColiderTransform.localPosition = interpolatedPosition;
                    elapsedTime += Time.deltaTime;
                    yield return new WaitForFixedUpdate();
                }
               
            }
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
            float x = originalCamPosition.x + UnityEngine.Random.Range(-1f, 1f) * shakeMagnitude;
            float z = originalCamPosition.z + UnityEngine.Random.Range(-1f, 1f) * shakeMagnitude;

            Camera.main.transform.position = new Vector3(x, originalCamPosition.y, z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        Camera.main.transform.position = originalCamPosition;
    }


}
