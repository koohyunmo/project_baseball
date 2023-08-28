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

    Tween adTweenAnim;

    // ���� ����� �� ������ ����
    private Color startColor = Color.green;
    private Color endColor = Color.red;

    public bool useColorBlend = true;  // �� ���� ���� ���� ������ ���� ���� �����մϴ�.

    private void Start()
    {
        Init();
    }



    public override bool Init()
    {
        base.Init();

        Co_timer = StartCoroutine(CountdownTimer());

        adImage.gameObject.BindEvent(Replay);

        return true;
    }

    private void Replay()
    {
        if(Co_timer != null)
        {
            StopCoroutine(Co_timer);
            Co_timer = null;
        }

        Managers.UI.ClosePopupUI(this);
        Managers.Game.GameRetry();
        Debug.Log("TODO ����");
    }

    // Ÿ�̸� �ڷ�ƾ ����
    private IEnumerator CountdownTimer()
    {
        currentTime = maxTime;

        // ������ �ʷϻ����� �ʱ�ȭ
        timeSlider.color = startColor;
        if (adTweenAnim != null)
            adTweenAnim.Kill();
        adTweenAnim = adImage.transform.DOScale(0.8f, 1).SetLoops(-1,LoopType.Yoyo);

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
        adTweenAnim.Kill();
        adTweenAnim = null;

        adImage.transform.DOScale(1, 0);

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
}
