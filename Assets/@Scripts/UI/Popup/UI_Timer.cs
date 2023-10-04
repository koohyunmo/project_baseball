using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Timer : UI_Popup
{
    public TextMeshProUGUI timerTMP; // Unity �����Ϳ��� �Ҵ��� Text ������Ʈ ����

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

        timerTMP.gameObject.SetActive(false); // ī��Ʈ�ٿ� �ؽ�Ʈ�� ����ϴ�.

        StartGame(); // ���� ���� �Լ�
    }

    void StartGame()
    {
        Debug.Log("���� ����!");
        Managers.UI.ClosePopupUI(this);
        Managers.Game.GameStart();
        
    }

}
