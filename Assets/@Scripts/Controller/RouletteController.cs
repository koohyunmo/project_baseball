using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RouletteController : MonoBehaviour
{
    [Header("돌림판")]
    [SerializeField] Transform roullet;
    [Header("돌림판 아이템 이미지")]
    [SerializeField] Transform[] images;
    [Header("돌림판 핸드")]
    [SerializeField] Transform hand;
    [Header("결과 텍스트(TMP)")]
    public TextMeshProUGUI resultText;           // 결과를 표시할 Text UI

    [Header("돌림판 셋팅")]
    public float spinSpeed = 3000.0f; // 초당 회전 속도
    private bool isSpinning = false;  // 룰렛이 회전 중인지 확인하는 플래그
    private float currentSpeed;       // 현재 회전 속도
    public float deceleration = 100.0f; // 감속률

    private int numberOfPrizes = 0; // 상품의 수
    [Header("기즈모 디버그용")]
    public float radius = 1.0f; // 룰렛의 반지름
    public float rotationOffset = 0f; // 기즈모의 회전 오프셋


    [Header("상품 목록")]
    [SerializeField]private string[] prizes = { "Product 1", "Product 2", "Product 3", "Product 4", "Product 5", "Product 6", "Product7", "Product 8" }; // 상품 배열

    [SerializeField]
    [Header("색상 목록")]
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
            roullet.Rotate(0, 0, -currentSpeed * Time.deltaTime); // Z축을 중심으로 회전
            currentSpeed -= deceleration * Time.deltaTime; // 속도 감소
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
    /// 실시간 UI 업데이트
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
    /// 핸드와 이미지 거리를 기반으로 당첨상품을 정해주는 함수
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
    /// 룰렛아이템의 위치 설정
    /// </summary>
    private void SetUI()
    {
        numberOfPrizes = prizes.Length; // 이 부분을 추가
        Vector3 position = roullet.position; // 룰렛의 중심 위치
        float angleStep = 360.0f / numberOfPrizes; // 각 상품 섹션의 각도
        float currentAngle = rotationOffset;
        float radius2 = 25.0f; // 룰렛의 반지름

        for (int i = 0; i < images.Length; i++)
        {
            Vector3 endAnglePosition = position + Quaternion.Euler(0, 0, currentAngle) * Vector3.right * radius2;

            // 선과 원점 중간에 큐브 그리기
            Vector3 middlePosition = (position + endAnglePosition) * 0.5f; // Vector3로 변경
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
        Vector3 position = roullet.position; // 룰렛의 중심 위치
        float angleStep = 360.0f / numberOfPrizes; // 각 상품 섹션의 각도
        float currentAngle = rotationOffset;

        for (int i = 0; i < numberOfPrizes; i++)
        {
            Vector3 endAnglePosition = position + Quaternion.Euler(0, 0, currentAngle) * Vector3.right * radius;

            // 선 그리기
            Gizmos.color = colors[i % colors.Length];
            Gizmos.DrawLine(position, endAnglePosition);

            // 선과 원점 중간에 큐브 그리기
            Vector3 middlePosition = (position + endAnglePosition) * 0.5f;
            float cubeSize = 0.05f; // 큐브 크기
            Gizmos.DrawCube(middlePosition, new Vector3(cubeSize, cubeSize, cubeSize));

            currentAngle += angleStep;
        }
    }

#endif
}
