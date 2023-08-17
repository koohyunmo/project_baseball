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

    public BoxCollider batCollider;
    public GameObject model;

    public float lerpSpeed = 2.5f;
    private bool isMoving = false;

    public Transform startBat;
    public Transform endBat;

    bool isSwinging;
    [SerializeField] float lerpTimer;
    float maxTime = 0.3f;

    Vector3 originalBatPos;
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
        batCollider = GetComponentInChildren<BoxCollider>();


        originalBatPos = model.transform.position;
        originalBatRot = model.transform.rotation;

        originalCamPosition = Camera.main.transform.position;

        mainCamera = Camera.main;

        anim = model.GetComponent<Animator>();

        Managers.Game.SetBat(this);
    }


    private void Update()
    {
        switch (GameState)
        {
            case Define.GameState.Home:
                BatOff();
                break;
            case Define.GameState.Ready:
                break;
            case Define.GameState.InGround:
                BatOn();
                break;
            case Define.GameState.End:
                break;
        }
    }


    private void BatOff()
    {
        if (batCollider.gameObject.activeSelf == true)
        {
            batCollider.gameObject.SetActive(false);
        }
    }

    private void BatOn()
    {
        if (batCollider.gameObject.activeSelf == false)
        {
            batCollider.gameObject.SetActive(true);
        }
    }

    private void FixedUpdate()
    {
        SwingAndBack();
        ClampToCameraView();
    }

    private void ClampToCameraView()
    {
        if (mainCamera == null) return; // ī�޶� �Ҵ���� �ʾҴٸ� �Լ� ����

        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(model.transform.position);

        // 0�� 1 ���̷� ����Ʈ ��ǥ ����
        viewportPosition.x = Mathf.Clamp(viewportPosition.x, 0f, 1f);
        viewportPosition.y = Mathf.Clamp(viewportPosition.y, 0f, 1f);

        // ���ѵ� ����Ʈ ��ǥ�� ���� ��ǥ�� ��ȯ ��, ������Ʈ ��ġ ������Ʈ
        model.transform.position = mainCamera.ViewportToWorldPoint(viewportPosition);
    }

    private void SwingAndBack()
    {
        if (isSwinging)
        {
            lerpTimer += Time.deltaTime * lerpSpeed;

            float curveValue = swingCurve.Evaluate(lerpTimer);  // 0�� 1 ������ lerpTime ���� ������� � �� ����

            // ȸ�� �� ��ġ ����
            model.transform.position = Vector3.Lerp(originalBatPos, endBat.position, curveValue);
            anim.Play("Bat_Swing");

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

    public void Swing()
    {
        if (!isSwinging) // ���� ���� �ƴ� ���� ���� ����
        {
            isSwinging = true;
        }

        // ī�޶� ȿ���� �߰��մϴ�.
        StartCoroutine(CameraShake());
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
