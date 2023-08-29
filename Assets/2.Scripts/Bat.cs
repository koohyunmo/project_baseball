using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UI.Image;

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

    bool isSwinging;
    [SerializeField] float lerpTimer;
    float maxTime = 0.3f;

    Vector3 originalBatPos = Vector3.zero;
    Quaternion originalBatRot;
    Vector3 originalCamPosition;
    Animator anim;

    public AnimationCurve swingCurve;

    Camera mainCamera;

    private bool isReturning = false; // ��Ʈ�� ���� ��ġ�� ���ư��� ������ üũ�ϴ� ����
    private float returnDelay = 0.2f; // ���� ��ġ�� ���ư��� �� ��� �ð�
    private float returnTimer = 0; // ��� �ð��� üũ�ϴ� Ÿ�̸�
    private float returnLerpTime = 0; // ���� ��ġ�� ���ư� �� ����ϴ� lerp Ÿ�̸�
    private float returnLerpSpeed = 1f; // ���� ��ġ�� ���ư� ���� lerp �ӵ�



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


        SetBetHandle();

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
        // �ڽ� �ݶ��̴��� z ũ�⸦ �����ɴϴ�. (��Ʈ�� ����)
        float batBox = inputOffset;
        // �������� ����� ��ġ�� ����մϴ�. ���⼭�� z ũ���� 20%�� ����Ͽ����ϴ�.
        // 0.3f �������� ��ġ
        float handleOffset = batBox * offsetX;
        // ȸ���� ����Ͽ� ���� ��ǥ�� ��ȯ�մϴ�. �����̴� z�� �������� ��ġ�˴ϴ�.
        Vector3 handleLocalPosition = new Vector3(batBoxCollider.center.x, batBoxCollider.center.y + handleOffset, batBoxCollider.center.z + offsetZ);
        Vector3 handleWorldPosition = batBoxCollider.transform.TransformPoint(handleLocalPosition);
        // �������� Ʈ�������� �����մϴ�.
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

    private void ClampToCameraViewOriginal()
    {

        if (mainCamera == null) return; // ī�޶� �Ҵ���� �ʾҴٸ� �Լ� ����
        if (GameState != Define.GameState.InGround) return; // �������� �ƴ϶�� �Լ� ����

        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(model.transform.position);

        // 0�� 1 ���̷� ����Ʈ ��ǥ ����
        viewportPosition.x = Mathf.Clamp(viewportPosition.x, 0f, 1f);
        viewportPosition.y = Mathf.Clamp(viewportPosition.y, 0f, 1f);

        // ���ѵ� ����Ʈ ��ǥ�� ���� ��ǥ�� ��ȯ ��, ������Ʈ ��ġ ������Ʈ
        model.transform.position = mainCamera.ViewportToWorldPoint(viewportPosition);


    }


    private void ClampToCameraView()
    {

        if (mainCamera == null) return; // ī�޶� �Ҵ���� �ʾҴٸ� �Լ� ����
        if (GameState != Define.GameState.InGround) return; // �������� �ƴ϶�� �Լ� ����

        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(HitColiderTransform.position);

        // 0�� 1 ���̷� ����Ʈ ��ǥ ����
        viewportPosition.x = Mathf.Clamp(viewportPosition.x, 0f, 1f);
        viewportPosition.y = Mathf.Clamp(viewportPosition.y, 0f, 1f);

        // ���ѵ� ����Ʈ ��ǥ�� ���� ��ǥ�� ��ȯ
        Vector3 clampedWorldPosition = mainCamera.ViewportToWorldPoint(viewportPosition);

        // z ��ǥ�� ������ z ��ǥ�� ����
        clampedWorldPosition.z = HitColiderTransform.position.z;

        // HitColider ��ġ ������Ʈ
        HitColiderTransform.position = clampedWorldPosition;

        var moveVec =  Vector3.Lerp(BatModelParent.position, HandleTransform.position, 0.1f);

        BatModelParent.position = moveVec;
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


            float curveValue = swingCurve.Evaluate(lerpTimer);  // 0�� 1 ������ lerpTime ���� ������� � �� ����

            // ȸ�� �� ��ġ ����
            //model.transform.position = Vector3.Lerp(originalBatPos, endBat.position, curveValue);
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

                // ���������� ���� ��ġ �� ȸ������ ���ƿ���
                //model.transform.position = Vector3.Lerp(endBat.position, originalBatPos, returnLerpTime);
                //model.transform.rotation = Quaternion.Lerp(endBat.rotation, originalBatRot, returnLerpTime);

                if (returnLerpTime >= 1)
                {
                    returnLerpTime = 0;
                    returnTimer = 0;
                    isReturning = false;
                    //model.transform.position = originalBatPos;
                    //model.transform.rotation = originalBatRot;
                }
            }
        }
    }

    private void SwingAnim()
    {
        if (GameState == Define.GameState.End)
        {
            string animationName = "Bat_Swing"; // �ִϸ��̼� Ŭ�� �̸�
            isSwinging = false;
            float stopAt = 0.2f; // �ִϸ��̼��� �߰����� ���߷��� 0.5 (0�� ����, 1�� ��)

            // �ִϸ��̼��� ��Ȯ�� �������� ���߱�
            anim.Play(animationName, 0, stopAt);
            anim.speed = 0;
        }
    }

    public void Swing()
    {
        if (!isSwinging) // ���� ���� �ƴ� ���� ���� ����
        {
            Managers.Game.HitEvent();
            isSwinging = true;
        }

        // ī�޶� ȿ���� �߰��մϴ�.
        StartCoroutine(CameraShake());
    }

    public void HutSwing()
    {
        if (!isSwinging) // ���� ���� �ƴ� ���� ���� ����
        {
            isSwinging = true;
        }
    }

    IEnumerator co_BatMoveReplay()
    {
        var data = Managers.Game.batMoveReplayData;

        for (int i = 0; i < data.Count -1; i++)
        {
            // ���� ��ġ�� ���� ��ġ ������ ��� �ð��� ����մϴ�.
            float waitTime = data[i + 1].time - data[i].time;
            HitColiderTransform.localPosition = data[i].position;
            float elapsedTime = 0f;

            while (elapsedTime < waitTime)
            {
                float t = elapsedTime / waitTime;
                Vector3 interpolatedPosition = Vector3.Lerp(HitColiderTransform.localPosition, data[i].position, t);;
                HitColiderTransform.localPosition = interpolatedPosition;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }

    IEnumerator CameraShake()
    {
        // ī�޶� ��鸲 ȿ���� �����մϴ�.
        float shakeDuration = 0.5f; // ��鸲�� ���� �ð��Դϴ�.
        float shakeMagnitude = 0.05f; // ��鸲�� ũ���Դϴ�.

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
