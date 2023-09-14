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
    public UnityEvent<float> onSliderChanged = new UnityEvent<float>(); // �����̴� �� ���� �̺�Ʈ
    public float minRatio = 0.1f; // ���ϴ� �ּ� ratio ��
    public float maxRatio = 10f; // ���ϴ� �ִ� ratio ��
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
        if (isUpdating) return; // �̹� ������Ʈ ���̸� �Լ��� �����մϴ�.
        isUpdating = true;

        float SetSpeed = Mathf.Lerp(minRatio, maxRatio, sliderValue);
        Managers.Game.SetBatSpeed(SetSpeed);

        float ratio = (Managers.Game.BatSpeed) / (maxRatio);

        speedSlider.value = ratio;
        speedTMP.text = Managers.Game.BatSpeed.ToString("F2");

        isUpdating = false; // ������Ʈ ���Ḧ ǥ���մϴ�.
    }

    private void UpdateSlider()
    {
        float value = (Managers.Game.BatSpeed) / (maxRatio);

        speedSlider.value = value;
        speedTMP.text = Managers.Game.BatSpeed.ToString("F2");
    }


}
