using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RouletteController : MonoBehaviour
{
    [Header("������")]
    [SerializeField] Transform roullet;
    [Header("������ ������ �̹���")]
    [SerializeField] Transform[] images;
    [Header("������ �ڵ�")]
    [SerializeField] Transform hand;
    [Header("��� �ؽ�Ʈ(TMP)")]
    public TextMeshProUGUI resultText;           // ����� ǥ���� Text UI

    [Header("������ ����")]
    public float spinSpeed = 3000.0f; // �ʴ� ȸ�� �ӵ�
    private bool isSpinning = false;  // �귿�� ȸ�� ������ Ȯ���ϴ� �÷���
    private float currentSpeed;       // ���� ȸ�� �ӵ�
    public float deceleration = 100.0f; // ���ӷ�

    private int numberOfPrizes = 0; // ��ǰ�� ��
    [Header("����� ����׿�")]
    public float radius = 1.0f; // �귿�� ������
    public float rotationOffset = 0f; // ������� ȸ�� ������


    [Header("��ǰ ���")]
    [SerializeField]private string[] prizes = { "Product 1", "Product 2", "Product 3", "Product 4", "Product 5", "Product 6", "Product7", "Product 8" }; // ��ǰ �迭

    [SerializeField]
    [Header("���� ���")]
    private Color[] colors = {
        Color.red,
        Color.yellow,
        Color.green,
        Color.blue,
        Color.magenta,
        Color.black,
        Color.white,
        Color.cyan
    };


    private void Start()
    {
        Init();
    }

    private void Init()
    {
        roullet.gameObject.BindEvent(StartSpin);
        SetUI();
    }

    void Update()
    {

        if (isSpinning)
        {
            roullet.Rotate(0, 0, -currentSpeed * Time.deltaTime); // Z���� �߽����� ȸ��
            currentSpeed -= deceleration * Time.deltaTime; // �ӵ� ����
            UpdateUI();

            if (currentSpeed <= 0)
            {
                isSpinning = false;
                DeterminePrize2();
            }
        }
    }

    public void StartSpin()
    {
        if (!isSpinning)
        {
            currentSpeed = Random.Range(spinSpeed, spinSpeed * 1.5f);
            isSpinning = true;
        }
    }

    /// <summary>
    /// �ǽð� UI ������Ʈ
    /// </summary>
    private void UpdateUI()
    {
        float min = float.MaxValue;
        int minIndex = 0;
        int index = 0;

        foreach (var item in images)
        {
            if (min > (hand.position - item.position).sqrMagnitude)
            {
                min = Mathf.Min(min, (hand.position - item.position).sqrMagnitude);
                minIndex = index;
            }
            index++;

        }


        Color prizeColor = colors[minIndex % colors.Length];
        string colorHex = ColorUtility.ToHtmlStringRGB(prizeColor);

        resultText.text = $"<color=#{colorHex}> Prize: {prizes[minIndex % prizes.Length]}</color>";

    }

    /// <summary>
    /// �ڵ�� �̹��� �Ÿ��� ������� ��÷��ǰ�� �����ִ� �Լ�
    /// </summary>
    private void DeterminePrize2()
    {
        float min = float.MaxValue;
        int minIndex = 0;
        int index = 0;

        foreach (var item in images)
        {
            if(min > (hand.position - item.position).sqrMagnitude)
            {
                min = Mathf.Min(min, (hand.position - item.position).sqrMagnitude);
                minIndex = index;
            }
            index++;

        }
        Color prizeColor = colors[minIndex % colors.Length];
        string colorHex = ColorUtility.ToHtmlStringRGB(prizeColor);

        resultText.text = $"<color=#{colorHex}> Winning Prize: {prizes[minIndex % prizes.Length]}</color>";

    }

    /// <summary>
    /// �귿�������� ��ġ ����
    /// </summary>
    private void SetUI()
    {
        numberOfPrizes = prizes.Length; // �� �κ��� �߰�
        Vector3 position = roullet.position; // �귿�� �߽� ��ġ
        float angleStep = 360.0f / numberOfPrizes; // �� ��ǰ ������ ����
        float currentAngle = rotationOffset;
        float radius2 = 25.0f; // �귿�� ������

        for (int i = 0; i < images.Length; i++)
        {
            Vector3 endAnglePosition = position + Quaternion.Euler(0, 0, currentAngle) * Vector3.right * radius2;

            // ���� ���� �߰��� ť�� �׸���
            Vector3 middlePosition = (position + endAnglePosition) * 0.5f; // Vector3�� ����
            images[i].transform.position = middlePosition;
            images[i].GetComponent<Image>().color = colors[i];

            currentAngle += angleStep;
        }

        resultText.text = null;
    }


#if UNITY_EDITOR


    




    void OnDrawGizmos()
    {
        numberOfPrizes = prizes.Length;
        DrawRouletteGizmo();
    }

    void DrawRouletteGizmo()
    {
        Vector3 position = roullet.position; // �귿�� �߽� ��ġ
        float angleStep = 360.0f / numberOfPrizes; // �� ��ǰ ������ ����
        float currentAngle = rotationOffset;

        for (int i = 0; i < numberOfPrizes; i++)
        {
            Vector3 endAnglePosition = position + Quaternion.Euler(0, 0, currentAngle) * Vector3.right * radius;

            // �� �׸���
            Gizmos.color = colors[i % colors.Length];
            Gizmos.DrawLine(position, endAnglePosition);

            // ���� ���� �߰��� ť�� �׸���
            Vector3 middlePosition = (position + endAnglePosition) * 0.5f;
            float cubeSize = 0.05f; // ť�� ũ��
            Gizmos.DrawCube(middlePosition, new Vector3(cubeSize, cubeSize, cubeSize));

            currentAngle += angleStep;
        }
    }

#endif
}
