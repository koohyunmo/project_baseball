using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEditor;
using UnityEngine;

public class GUIDebugger : MonoBehaviour
{
#if UNITY_EDITOR

    public HashSet<string> playerInventory;
    public PlayerInfo playerInfo;

    bool isManagerLoading = false;


    private void Start()
    {
        StartCoroutine(co_Loading());
    }

    IEnumerator co_Loading()
    {
        yield return new WaitForSeconds(1f);
        isManagerLoading = true;

    }

    private Rect windowRect = new Rect(20, 20, 800, 800);
    private Vector2 scrollPosition = Vector2.zero;
    private int fontSize = 30;
    private Color fontColor = Color.black;

    private bool showPlayerInfo = true; // PlayerInfo 섹션 표시 여부
    private bool showPlayerInventory = true; // PlayerInventory 섹션 표시 여부
    private bool showOptions = true; // 옵션 섹션 표시 여부

    void OnGUI()
    {
        if (isManagerLoading == false)
            return;

        //windowRect = GUI.Window(0, windowRect, DisplayDebugWindow, "Debug Info");
        windowRect = GUILayout.Window(0, windowRect, DisplayDebugWindow, "Debug Info", GUILayout.Width(500), GUILayout.Height(500));
    }


    void DisplayDebugWindow(int windowID)
    {
        playerInventory = Managers.Game.GameDB.playerInventory;
        playerInfo = Managers.Game.GameDB.playerInfo;

        // Font 설정
        GUI.skin.label.fontSize = fontSize;
        GUI.contentColor = fontColor;

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        // Player Inventory 출력
        showPlayerInventory = GUILayout.Toggle(showPlayerInventory, "Player Inventory");
        if (showPlayerInventory)
        {
            foreach (var item in playerInventory)
            {
                GUILayout.Label("- " + item);
            }
        }

        // PlayerInfo 정보 출력
        showPlayerInfo = GUILayout.Toggle(showPlayerInfo, "Player Info");
        if (showPlayerInfo)
        {
            GUILayout.Label("Player ID: " + playerInfo.playerId);
            GUILayout.Label("Money: " + playerInfo.money);
            GUILayout.Label("Level: " + playerInfo.level);
            GUILayout.Label("Experience: " + playerInfo.exp);
            GUILayout.Label("Equipped Bat ID: " + playerInfo.equipBatId);
            GUILayout.Label("Equipped Ball ID: " + playerInfo.equipBallId);
            GUILayout.Label("Equipped Background ID: " + playerInfo.equipBackgroundID);
        }

        

        // 옵션 섹션
        showOptions = GUILayout.Toggle(showOptions, "Options");
        if (showOptions)
        {
            // 폰트 크기 및 색상 조절
            GUILayout.Label("Font Size:");
            fontSize = (int)GUILayout.HorizontalSlider(fontSize, 10, 30, GUILayout.Width(400), GUILayout.Height(20));
            GUILayout.Label("Font Color:");
            fontColor.r = GUILayout.HorizontalSlider(fontColor.r, 0, 1, GUILayout.Width(400), GUILayout.Height(20));
            fontColor.g = GUILayout.HorizontalSlider(fontColor.g, 0, 1, GUILayout.Width(400), GUILayout.Height(20));
            fontColor.b = GUILayout.HorizontalSlider(fontColor.b, 0, 1, GUILayout.Width(400), GUILayout.Height(20));
            fontColor.a = GUILayout.HorizontalSlider(fontColor.a, 0, 1, GUILayout.Width(400), GUILayout.Height(20));
        }

        GUILayout.EndScrollView();

        // 윈도우 드래그
        GUI.DragWindow(new Rect(0, 0, 10000, 20));
    }
#endif

}
