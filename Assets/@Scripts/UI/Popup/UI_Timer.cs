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
        Managers.Sound.Play(Define.Sound.Effect, "retro_ui_menu_blip_click_01");
        timerTMP.text = "3";
        yield return new WaitForSeconds(1.0f);
        Managers.Sound.Play(Define.Sound.Effect, "retro_ui_menu_blip_click_01");
        timerTMP.text = "2";
        yield return new WaitForSeconds(1.0f);
        Managers.Sound.Play(Define.Sound.Effect, "retro_ui_menu_blip_click_01");
        timerTMP.text = "1";
        yield return new WaitForSeconds(1.0f);

        Managers.Sound.Play(Define.Sound.Effect, "retro_ui_menu_blip_click_02");
        timerTMP.text = Managers.Localization.GetLocalizedValue(LanguageKey.start.ToString());
        yield return new WaitForSeconds(0.5f);

        timerTMP.gameObject.SetActive(false); // ī��Ʈ�ٿ� �ؽ�Ʈ�� ����ϴ�.

        StartGame(); // ���� ���� �Լ�
    }

    void StartGame()
    {
#if UNITY_EDITOR
        Debug.Log("���� ����!");
#endif
        Managers.UI.ClosePopupUI(this);
        Managers.Game.GameStart();
        
    }

}
