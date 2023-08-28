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

    // 시작 색상과 끝 색상을 정의
    private Color startColor = Color.green;
    private Color endColor = Color.red;

    public bool useColorBlend = true;  // 이 값에 따라 색상 보간을 할지 말지 결정합니다.

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
        Debug.Log("TODO 광고");
    }

    // 타이머 코루틴 시작
    private IEnumerator CountdownTimer()
    {
        currentTime = maxTime;

        // 색상을 초록색으로 초기화
        timeSlider.color = startColor;
        if (adTweenAnim != null)
            adTweenAnim.Kill();
        adTweenAnim = adImage.transform.DOScale(0.8f, 1).SetLoops(-1,LoopType.Yoyo);

        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            timeSlider.fillAmount = currentTime / maxTime;

            // useColorBlend 값에 따라 색상 보간을 실행합니다.
            if (useColorBlend)
            {
                timeSlider.color = Color.Lerp(endColor, startColor, currentTime / maxTime);
            }

            // 시간 형식을 수정합니다.
            TimeSpan time = TimeSpan.FromSeconds(currentTime);
           

            yield return null;
        }

        yield return new WaitForSeconds(0.3f);

        ClearDisplay();
    }


    // UI 초기화
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

        // 코드 작성
        Managers.UI.ClosePopupUI(this);
        Managers.UI.ShowPopupUI<UI_EndPopup>();
    }
}
