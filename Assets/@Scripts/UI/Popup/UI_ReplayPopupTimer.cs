using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_ReplayPopupTimer : UI_Popup
{
    public Image timeSlider;
    public Image adImage;
    public float maxTime = 30f;
    Coroutine Co_timer;
    private float currentTime;

    public TextMeshProUGUI reaplayInfoTMP;

    Tween adTweenAnim;

    // ���� ����� �� ������ ����
    private Color startColor = Color.green;
    private Color endColor = Color.red;

    public bool useColorBlend = true;  // �� ���� ���� ���� ������ ���� ���� �����մϴ�.

    private void Start()
    {
        Init();

        if(Managers.Game.GameScore <= 50)
        {
            adImage.gameObject.BindEvent(Retry);
            reaplayInfoTMP.text = "����";
            return;
        }

        if(Managers.Game.CanPay(3))
        {
            adImage.gameObject.BindEvent(Replay);
            reaplayInfoTMP.text = "3�� ����ϰ�\n�絵��";
        }
        else
        {
            Managers.UI.ClosePopupUI(this);
            Managers.UI.ShowPopupUI<UI_EndPopup>();
        }
    }



    public override bool Init()
    {
        base.Init();

        Co_timer = StartCoroutine(CountdownTimer());

        adImage.gameObject.BindEvent(Replay);
        adImage.gameObject.SetActive(true);

        return true;
    }

    private void Retry()
    {
        Managers.Game.GameRetry();
    }

    private void Replay()
    {


        Managers.UI.ClosePopupUI(this);

        if (Managers.Game.CanPay(3))
        {
            Managers.Game.MinusStar(3);
            Retry();
        }

        if (Co_timer != null)
        {
            StopCoroutine(Co_timer);
            Co_timer = null;
        }



        //Managers.Game.GameRetry();
    }

    // Ÿ�̸� �ڷ�ƾ ����
    private IEnumerator CountdownTimer()
    {
        currentTime = maxTime;

        // ������ �ʷϻ����� �ʱ�ȭ
        timeSlider.color = startColor;

        adImage.transform.DOScale(0.7f, 0.4f).SetLoops(-1,LoopType.Yoyo);

        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            timeSlider.fillAmount = currentTime / maxTime;

            // useColorBlend ���� ���� ���� ������ �����մϴ�.
            if (useColorBlend)
            {
                timeSlider.color = Color.Lerp(endColor, startColor, currentTime / maxTime);
            }

            // �ð� ������ �����մϴ�.
            TimeSpan time = TimeSpan.FromSeconds(currentTime);
           

            yield return null;
        }

        yield return new WaitForSeconds(0.3f);

        ClearDisplay();
    }


    // UI �ʱ�ȭ
    public void ClearDisplay()
    {
        if (Co_timer != null)
        {
            StopCoroutine(Co_timer);
            Co_timer = null;
        }

        TimeSpan time = TimeSpan.FromSeconds(maxTime);

        // �ڵ� �ۼ�
        Managers.UI.ClosePopupUI(this);
        Managers.UI.ShowPopupUI<UI_EndPopup>();
    }


    public void OnDestroy()
    {
        adImage.transform.DOKill();
    }
}
