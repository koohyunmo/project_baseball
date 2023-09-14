using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI_BatOptionPopup : UI_OptionPopup
{

    public Slider speedSlider;
    public UnityEvent<float> onSliderChanged = new UnityEvent<float>(); // 슬라이더 값 변경 이벤트
    public float minRatio = 0.1f; // 원하는 최소 ratio 값
    public float maxRatio = 10f; // 원하는 최대 ratio 값
    public TextMeshProUGUI speedTMP;

    private void Start()
    {
        Init();
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        titleTMP.text = "BatSpeed";

        UpdateSlider();
        speedSlider.onValueChanged.AddListener(ChageBVolume);

        return true;
    }

    private bool isUpdating = false;
    public void ChageBVolume(float sliderValue)
    {
        if (isUpdating) return; // 이미 업데이트 중이면 함수를 종료합니다.
        isUpdating = true;

        float SetSpeed = Mathf.Lerp(minRatio, maxRatio, sliderValue);
        Managers.Game.SetBatSpeed(SetSpeed);

        float ratio = (Managers.Game.BatSpeed) / (maxRatio);

        speedSlider.value = ratio;
        speedTMP.text = Managers.Game.BatSpeed.ToString("F2");

        isUpdating = false; // 업데이트 종료를 표시합니다.
    }

    private void UpdateSlider()
    {
        float value = (Managers.Game.BatSpeed) / (maxRatio);

        speedSlider.value = value;
        speedTMP.text = Managers.Game.BatSpeed.ToString("F2");
    }


}
