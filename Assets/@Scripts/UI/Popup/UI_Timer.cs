using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Timer : UI_Popup
{
    public TextMeshProUGUI timerTMP; // Unity 에디터에서 할당할 Text 컴포넌트 참조

    private void Start()
    {
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        timerTMP.text = "3";
        yield return new WaitForSeconds(1.0f);

        timerTMP.text = "2";
        yield return new WaitForSeconds(1.0f);

        timerTMP.text = "1";
        yield return new WaitForSeconds(1.0f);

        timerTMP.text = Managers.Localization.GetLocalizedValue(LanguageKey.start.ToString());
        yield return new WaitForSeconds(0.5f);

        timerTMP.gameObject.SetActive(false); // 카운트다운 텍스트를 숨깁니다.

        StartGame(); // 게임 시작 함수
    }

    void StartGame()
    {
        Debug.Log("게임 시작!");
        Managers.UI.ClosePopupUI(this);
        Managers.Game.GameStart();
        
    }

}
