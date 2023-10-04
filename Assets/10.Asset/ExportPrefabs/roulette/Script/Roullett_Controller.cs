using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq; // ToArray() ����� ���� namespace
using System.Collections.Generic;

public class Roullett_Controller : MonoBehaviour
{
    [Header("������")]
    [SerializeField] Transform roullet;
    [Header("������ ������ �̹���")]
    [SerializeField] Transform[] images;
    [Header("������ �ڵ�")]
    [SerializeField] Transform hand;
    [Header("��� �ؽ�Ʈ(TMP)")]
    public Text resultText;           // ����� ǥ���� Text UI

    [Header("������ ����")]
    public float spinSpeed = 3000.0f; // �ʴ� ȸ�� �ӵ�
    private bool isSpinning = false;  // �귿�� ȸ�� ������ Ȯ���ϴ� �÷���
    private float currentSpeed;       // ���� ȸ�� �ӵ�
    public float deceleration = 100.0f; // ���ӷ�
    public float imagePos = 300.0f; // �귿�� ������


    private int numberOfPrizes = 0; // ��ǰ�� ��

    [Header("����� ����׿�")]
    public float radius = 1.0f; // �귿�� ������
    public float rotationOffset = 0f; // ������� ȸ�� ������

    [Header("��ǰ ���")]
    [SerializeField] private string[] prizes = { "Product 1", "Product 2", "Product 3", "Product 4", "Product 5", "Product 6", "Product7", "Product 8" }; // ��ǰ �迭
    private int itemCount;

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


    struct RandomItemData
    {
        public string id;
        public Color color;
        public Define.Grade grade;

    }

    private void Start()
    {
        Init();
        gameObject.GetComponent<Button>().onClick.AddListener(StartSpin);
    }

    private void Init()
    {
        //roullet.gameObject.BindEvent(StartSpin);
        SetUI();
        
    }

    private async void SetData()
    {
        prizes = await makeList();
        SetUI();
        currentSpeed = Random.Range(spinSpeed, spinSpeed * 1.5f);
        isSpinning = true;
    }

    private async Task<string[]> makeList()
    {
        itemCount = prizes.Length;

        string[] list = new string[itemCount];
        List<string> itemList = new List<string>();

        for (int i = 0; i < itemCount; i++)
        {
            int index = Random.Range(0, 100);
            Define.Grade itemGrade = Define.Grade.Common;
            Color color = Color.green;
            itemList.Clear();
          


            if (index > 98)
            {
                itemGrade = Define.Grade.Legendary;
                color = Color.red;
                itemList = Managers.Resource.LegendaryList;
            }
            else if (index > 96)
            {
                itemGrade = Define.Grade.Epic;
                color = new Color(100f / 255f, 64.7f / 255f, 0, 1);
                itemList = Managers.Resource.EpicList;
            }
            else if (index > 85)
            {
                itemGrade = Define.Grade.Rare;
                color = Color.magenta;
                itemList = Managers.Resource.RareList;
            }
            else if (index > 70)
            {
                itemGrade = Define.Grade.Uncommon;
                color = Color.blue;
                itemList = Managers.Resource.UncommonList;
            }
            else
            {
                itemList = Managers.Resource.CommonList;
            }

            int randomIndex = UnityEngine.Random.Range(0, itemList.Count);

            list[i] = itemList[randomIndex];
        }
        return list;
    }

    void Update()
    {

        if (isSpinning)
        {
            roullet.Rotate(0, 0, -currentSpeed * Time.deltaTime); // Z���� �߽����� ȸ��
            currentSpeed -= deceleration * Time.deltaTime; // �ӵ� ����
            //UpdateUI();

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
            SetData();
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
            if (min > (hand.position - item.position).sqrMagnitude)
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

        for (int i = 0; i < images.Length; i++)
        {
            Vector3 endAnglePosition = position + Quaternion.Euler(0, 0, currentAngle) * Vector3.right * imagePos;

            // ���� ���� �߰��� ť�� �׸���
            Vector3 middlePosition = (position + endAnglePosition) * 0.5f; // Vector3�� ����
            images[i].transform.position = middlePosition;
            images[i].GetComponent<Image>().color = colors[i];
            // ������ ����
            images[i].GetComponent<Image>().sprite = Managers.Resource.GetItemScriptableObjet<ItemScriptableObject>(prizes[i]).icon;

            currentAngle += angleStep;
        }
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
