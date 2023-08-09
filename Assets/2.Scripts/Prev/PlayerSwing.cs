using System.Collections;
using UnityEngine;

public class PlayerSwing : MonoBehaviour
{
    public GameObject bat; // �߱������ ������Ʈ�� �����մϴ�.
    public Camera playerCamera; // �÷��̾��� ī�޶� �����մϴ�.
    public float windupSpeed = 50.0f; // �߱�����̸� �ڷ� ����� �ӵ��Դϴ�.
    public float initialSwingSpeed = 200.0f; // �߱�����̸� �ֵθ��� �ʱ� �ӵ��Դϴ�.
    public float swingAcceleration = 50.0f; // �߱�����̸� �ֵθ� ���� ���ӵ��Դϴ�.
    public float returnSpeed = 200.0f; // �߱�����̸� ���� ��ġ�� �������� �ӵ��Դϴ�.
    public float windupTime = 0.5f; // ��Ʈ�� �ڷ� ����� �ð��Դϴ�.

    private Quaternion originalRotation; // �߱�������� ���� ȸ���� �����մϴ�.
    private float windupAngle; // ��Ʈ�� �ڷ� ���� ���� ȸ�� ������ �����մϴ�.
    private float swingSpeed; // �߱�����̸� �ֵθ��� �ӵ��Դϴ�.

    private bool isSwinging = false; // �߱�����̰� �ֵθ��� �������� ��Ÿ���� �����Դϴ�.

    void Start()
    {
        originalRotation = transform.rotation; // ���� ȸ���� �����մϴ�.
    }

    void Update()
    {
        if (!isSwinging && Input.GetMouseButtonDown(0)) // ���콺 ���� ��ư�� ������
        {
            StartCoroutine(SwingBat());
        }
    }

    IEnumerator SwingBat()
    {
        isSwinging = true; // �߱�����̰� �ֵθ��� ������ ��Ÿ���ϴ�.

        // ��Ʈ�� �ڷ� ����ϴ�.
        float windupElapsedTime = 0;
        while (windupElapsedTime < windupTime)
        {
            float rotationAmount = -windupSpeed * Time.deltaTime * 1.5f;
            transform.Rotate(Vector3.up, rotationAmount);
            windupElapsedTime += Time.deltaTime;
            windupAngle += rotationAmount; // ȸ�� ������ �����մϴ�.

            if(Input.GetMouseButtonUp(0))
            {
                break;
            }

            yield return null;
        }

        // ��Ʈ�� �ֵθ��ϴ�.
        swingSpeed = initialSwingSpeed; // �ֵθ��� �ӵ��� �ʱ�ȭ�մϴ�.
        float swingTime = 2 * Mathf.Abs(windupAngle * 1.2f) / swingSpeed; // ��Ʈ�� �ڷ� ���� ������ �� �迡 ���� ���� �ð��� ����մϴ�.
        float swingElapsedTime = 0;
        while (swingElapsedTime < swingTime)
        {
            swingSpeed += swingAcceleration * Time.deltaTime*1.5f; // ���ӵ��� �����Ͽ� �ӵ��� ������ŵ�ϴ�.
            transform.Rotate(Vector3.up, swingSpeed * Time.deltaTime);
            swingElapsedTime += Time.deltaTime;
            yield return null;
        }

        // ������ ������ ���� ȸ������ ���ư��ϴ�.
        StartCoroutine(ReturnBat());
        windupAngle = 0; // ȸ�� ������ �ʱ�ȭ�մϴ�.
    }

    IEnumerator ReturnBat()
    {
        // ��Ʈ�� ���� ȸ������ �ε巴�� ���������ϴ�.
        while (Quaternion.Angle(transform.rotation, originalRotation) > 0.1f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, originalRotation, returnSpeed * Time.deltaTime);
            yield return null;
        }
        transform.rotation = originalRotation; // ��Ȯ�� ȸ���� �����մϴ�.

        isSwinging = false; // �߱�����̰� �ֵθ��� ���� �ƴ��� ��Ÿ���ϴ�.
    }


    public void Collision(Rigidbody rb)
    {
        Debug.Log("�浹");

        if (rb != null)
        {
            rb.AddForce(transform.forward * swingSpeed, ForceMode.Impulse);
        }
    }


}

