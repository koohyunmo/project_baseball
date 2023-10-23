using Actopolus.FakeLeaderboard.Src.UI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using Random = UnityEngine.Random;

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
    public bool isSpinning { get; private set; } = false;  // 룰렛이 회전 중인지 확인하는 플래그
    private float currentSpeed;       // 현재 회전 속도
    public float deceleration = 100.0f; // 감속률

    private int numberOfPrizes = 0; // 상품의 수
    [Header("기즈모 디버그용")]
    public float radius = 1.0f; // 룰렛의 반지름
    public float rotationOffset = 0f; // 기즈모의 회전 오프셋


    [Header("상품 목록")]
    //[SerializeField] private RandomItemData[] prizes = new RandomItemData[8];
    private Define.Grade[] prizes = { Define.Grade.Common, Define.Grade.Common, Define.Grade.Uncommon, Define.Grade.Uncommon, Define.Grade.Rare, Define.Grade.Rare, Define.Grade.Epic, Define.Grade.Legendary };
    public Sprite[] gradeImages;
    private int itemCount;
    [Header("색상 목록")]
    private Color[] colors = {
        Color.green,
        Color.green,
        Color.gray,
        Color.gray,
        Color.blue,
        Color.blue,
        Color.magenta,
        Color.red
    };

    List<Image> icons = new List<Image>();

    bool first = true;

    private readonly string ITEMLIST = "ITEMLIST";

    struct RandomItemData
    {
        public string id;
        public Color color;
        public Define.Grade grade;
        public string name;
        public Sprite icon;

        public bool IsNull()
        {
            if (string.IsNullOrEmpty(id))
            {
                return true;
            }

            return false;
        }

    }

    private void Start()
    {

        Init();
    }

    private void Init()
    {
        icons.Clear();
        SetUI();
    }

    void Update()
    {

        if (isSpinning)
        {
            roullet.Rotate(0, 0, -currentSpeed * Time.deltaTime); // Z축을 중심으로 회전
            currentSpeed -= deceleration * Time.deltaTime; // 속도 감소
            //UpdateUI();

            if (currentSpeed <= 0)
            {
                isSpinning = false;
                DeterminePrize2();
            }
        }
    }

    public void RollFreeRoullet()
    {
#if UNITY_EDITOR
        if (!isSpinning)
        {
            currentSpeed = Random.Range(spinSpeed, spinSpeed * 1.5f);
            isSpinning = true;
            first = false;

            Managers.Game.RollFreeRoullet();
        }
#else
        if (!isSpinning && Managers.Game.RollFreeRoullet())
        {
            currentSpeed = Random.Range(spinSpeed, spinSpeed * 1.5f);
            isSpinning = true;

            first = false;
        }
#endif

    }

    public void RollADRoullet()
    {

#if UNITY_EDITOR
        if (!isSpinning && Managers.Ad.CanShowRewardAd())
        {
            Managers.Ad.ShowRewardedAd(() =>
            {

                currentSpeed = Random.Range(spinSpeed, spinSpeed * 1.5f);
                isSpinning = true;
                first = false;

                Managers.Game.RollADRoullet();
            });
        }
#else
        if (!isSpinning && Managers.Game.RollADRoullet() && Managers.Ad.CanShowRewardAd())
        {
            Managers.Ad.ShowRewardedAd(() =>
        {

        currentSpeed = Random.Range(spinSpeed, spinSpeed * 1.5f);
        isSpinning = true;

        first = false;
        });
        }
#endif


    }

    //private async void SetData()
    //{
    //    prizes = await makeList();

    //    for (int i = 0; i < icons.Count; i++)
    //    {
    //        icons[i].sprite = prizes[i].icon;
    //    }

    //}

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
        Define.Grade data = prizes[minIndex % prizes.Length];
        resultText.text = $"<color=#{colorHex}> Prize: {Managers.Localization.GetLocalizedValue(data.ToString())}</color>";

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
            if (min > (hand.position - item.position).sqrMagnitude)
            {
                min = Mathf.Min(min, (hand.position - item.position).sqrMagnitude);
                minIndex = index;
            }
            index++;

        }
        Color prizeColor = colors[minIndex % colors.Length];
        string colorHex = ColorUtility.ToHtmlStringRGB(prizeColor);

        Define.Grade data = prizes[minIndex % prizes.Length];
        string id = "";

        switch (data)
        {
            case Define.Grade.Common:
                var rnd = Random.Range(0, Managers.Resource.CommonList.Count - 1);
                id = Managers.Resource.CommonList[rnd];
                break;
            case Define.Grade.Uncommon:
                rnd = Random.Range(0, Managers.Resource.UncommonList.Count - 1);
                id = Managers.Resource.UncommonList[rnd];
                break;
            case Define.Grade.Rare:
                rnd = Random.Range(0, Managers.Resource.RareList.Count - 1);
                id = Managers.Resource.RareList[rnd];
                break;
            case Define.Grade.Epic:
                rnd = Random.Range(0, Managers.Resource.EpicList.Count - 1);
                id = Managers.Resource.EpicList[rnd];
                break;
            case Define.Grade.Legendary:
                rnd = Random.Range(0, Managers.Resource.LegendaryList.Count - 1);
                id = Managers.Resource.LegendaryList[rnd];
                break;
        }


        Define.GetType get = Managers.Game.GetItem(id);

        if (data == Define.Grade.Legendary)
        {
            get = Define.GetType.Star;
        }


        //resultText.text = $"<color=#{colorHex}> Prize: {Managers.Localization.GetLocalizedValue(id)}</color>";
        var so = Managers.Resource.GetItemScriptableObjet<ItemScriptableObject>(id);
        resultText.text = $"<color=#{colorHex}> {Managers.Localization.GetLocalizedValue(LanguageKey.getprize.ToString())} : {Managers.Localization.GetLocalizedValue(so.grade.ToString())}</color>";


        long star = 0;


        if (get == Define.GetType.Duplicate || get == Define.GetType.Star)
        {
            switch (data)
            {
                case Define.Grade.Common:
                    star = 10;
                    break;
                case Define.Grade.Uncommon:
                    star = 30;
                    break;
                case Define.Grade.Rare:
                    star = 50;
                    break;
                case Define.Grade.Epic:
                    star = 100;
                    break;
                case Define.Grade.Legendary:
                    star = 300;
                    break;
            }

            Managers.Game.GetStar(star);

            resultText.text = $"<color=#{colorHex}> {Managers.Localization.GetLocalizedValue(LanguageKey.getprize.ToString())}: {star} {Managers.Localization.GetLocalizedValue(LanguageKey.star.ToString())}</color>";

        }


        var popup = Managers.UI.ShowPopupUI<UI_RoulletItemInfoPopup>();

        switch (get)
        {
            case Define.GetType.Failed:
                throw new Exception("아이템 없음?");
                break;
            case Define.GetType.Success:
                popup.InitData(get, id);
                break;
            case Define.GetType.Duplicate:
                popup.InitData(get,data, star);
                break;
            case Define.GetType.Star:
                popup.InitData(get,data, star);
                break;
        }



        //SetItemList();

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
        float radius2 = 25; // 룰렛의 반지름

        for (int i = 0; i < images.Length; i++)
        {
            Vector3 endAnglePosition = position + Quaternion.Euler(0, 0, currentAngle) * Vector3.right * radius2;

            // 선과 원점 중간에 큐브 그리기
            Vector3 middlePosition = (position + endAnglePosition) * 0.5f; // Vector3로 변경
            images[i].transform.position = middlePosition;
            var img = images[i].GetComponent<Image>();

            // 원점에서 기즈모 끝 방향으로의 방향 벡터 계산
            Vector3 direction = (endAnglePosition - position).normalized;

            // 방향 벡터를 기반으로 회전 계산
            Quaternion rotation = Quaternion.LookRotation(Vector3.forward, direction);

            // 회전 적용
            images[i].transform.rotation = rotation;




            Sprite icon = null;

            switch (prizes[i])
            {
                case Define.Grade.Common:
                    icon = gradeImages[0];
                    break;
                case Define.Grade.Uncommon:
                    icon = gradeImages[1];
                    break;
                case Define.Grade.Rare:
                    icon = gradeImages[2];
                    break;
                case Define.Grade.Epic:
                    icon = gradeImages[3];
                    break;
                case Define.Grade.Legendary:
                    icon = gradeImages[4];
                    break;
            }

            img.sprite = icon;
            var rect = img.GetComponent<RectTransform>();
            // 아이콘 변경
            //images[i].GetComponent<Image>().sprite = Managers.Resource.GetItemScriptableObjet<ItemScriptableObject>(prizes[i]).icon;
            //images[i].SetParent(roullet);

            icons.Add(img);
            currentAngle += angleStep;
        }


        foreach (var item in prizes)
        {
            Debug.Log(item);

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
