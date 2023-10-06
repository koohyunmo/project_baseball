﻿using Actopolus.FakeLeaderboard.Src.UI;
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
    private bool isSpinning = false;  // 룰렛이 회전 중인지 확인하는 플래그
    private float currentSpeed;       // 현재 회전 속도
    public float deceleration = 100.0f; // 감속률

    private int numberOfPrizes = 0; // 상품의 수
    [Header("기즈모 디버그용")]
    public float radius = 1.0f; // 룰렛의 반지름
    public float rotationOffset = 0f; // 기즈모의 회전 오프셋


    [Header("상품 목록")]
    //[SerializeField] private RandomItemData[] prizes = new RandomItemData[8];
    private Define.Grade[] prizes = { Define.Grade.Common, Define.Grade.Common, Define.Grade.Uncommon, Define.Grade.Uncommon, Define.Grade.Rare, Define.Grade.Rare, Define.Grade.Epic, Define.Grade.Legendary };
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
            if(string.IsNullOrEmpty(id))
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
        if (!isSpinning && Managers.Game.RollRoullet())
        {
            currentSpeed = Random.Range(spinSpeed, spinSpeed * 1.5f);
            isSpinning = true;

            first = false;
        }
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
        resultText.text = $"<color=#{colorHex}> Prize: {data.ToString()}</color>";

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

        Define.Grade data = prizes[minIndex % prizes.Length];
        string id = "";

        switch (data)
        {
            case Define.Grade.Common:
                id = Managers.Resource.CommonList[0];
                break;
            case Define.Grade.Uncommon:
                id = Managers.Resource.UncommonList[0];
                break;
            case Define.Grade.Rare:
                id = Managers.Resource.RareList[0];
                break;
            case Define.Grade.Epic:
                id = Managers.Resource.EpicList[0];
                break;
            case Define.Grade.Legendary:
                id = Managers.Resource.LegendaryList[0];
                break;
        }


        //resultText.text = $"<color=#{colorHex}> Prize: {Managers.Localization.GetLocalizedValue(id)}</color>";
        resultText.text = $"<color=#{colorHex}> Get Prize: {data.ToString()}</color>";


        Define.GetType get = Managers.Game.GetItem(id);
        long gold = 0;


        if (get == Define.GetType.duplicate)
        {


            switch (data)
            {
                case Define.Grade.Common:
                    gold = 10;
                    break;
                case Define.Grade.Uncommon:
                    gold = 30;
                    break;
                case Define.Grade.Rare:
                    gold = 100;
                    break;
                case Define.Grade.Epic:
                    gold = 300;
                    break;
                case Define.Grade.Legendary:
                    gold = 500;
                    break;
            }

            Debug.Log($"Get Gold {gold}");
        }


        var popup = Managers.UI.ShowPopupUI<UI_RoulletItemInfoPopup>();

        switch (get)
        {
            case Define.GetType.Failed:
                throw new Exception("아이템 없음?");
                break;
            case Define.GetType.Success:
                popup.InitData(id);
                break;
            case Define.GetType.duplicate:
                popup.InitData(data,gold);
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
            img.color = colors[i];
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
