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
    public Dictionary<string,bool> csoData;

    bool isManagerLoading = false;


    private void Start()
    {
        StartCoroutine(co_Loading());
    }

    IEnumerator co_Loading()
    {
        showPlayerInfo = false; // PlayerInfo 섹션 표시 여부
        showPlayerInventory = false; // PlayerInventory 섹션 표시 여부
        showOptions = false; // 옵션 섹션 표시 여부
        yield return new WaitForSeconds(1f);
        isManagerLoading = true;

    }

    private Rect windowRect = new Rect(50, 200, 800, 800);
    private Vector2 scrollPosition = Vector2.zero;
    private Vector2 scrollPosition2 = Vector2.zero;
    private int fontSize = 60;
    private Color fontColor = Color.white;

    private bool showPlayerInfo = false; // PlayerInfo 섹션 표시 여부
    private bool showPlayerInventory = false; // PlayerInventory 섹션 표시 여부
    private bool showChallenge = false; // PlayerInventory 섹션 표시 여부
    private bool showOptions = false; // 옵션 섹션 표시 여부
    private bool showGUIWindow = true; // GUI 창 표시 여부
    private bool isResizing = false;
    private Vector2 lastMousePosition = Vector2.zero;
    void OnGUI()
    {
        if (isManagerLoading == false)
            return;

        if (showGUIWindow)
        {
            windowRect = GUILayout.Window(0, windowRect, DisplayDebugWindow, "Debug Info", GUILayout.Width(1000), GUILayout.Height(1000));
            ResizeWindow(); 
        }

        // GUI 창 열기/닫기 버튼
        if (GUI.Button(new Rect(10, 10, 100, 50), showGUIWindow ? "Close GUI" : "Open GUI"))
        {
            showGUIWindow = !showGUIWindow;
        }
    }


    void DisplayDebugWindow(int windowID)
    {
        playerInventory = Managers.Game.GameDB.playerInventory;
        playerInfo = Managers.Game.GameDB.playerInfo;
        csoData = Managers.Game.GameDB.challengeData;
        // Font 설정
        GUI.skin.toggle.fontSize = 60; // 토글 버튼 폰트 크기 조절
        GUI.skin.toggle.fontStyle = FontStyle.Bold;
        GUI.skin.label.fontSize = fontSize;
        

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        // Player Inventory 출력
        GUI.contentColor = Color.green;
        showPlayerInventory = GUILayout.Toggle(showPlayerInventory, "Player Inventory");
        if (showPlayerInventory)
        {
            GUI.contentColor = Color.white;
            GUI.skin.label.fontSize = 40;
            GUILayout.BeginVertical();
            int num = 0;
            foreach (var item in playerInventory)
            {
                GUILayout.Label($"{num++}" + item + ",");
            }
            GUI.skin.label.fontSize = fontSize;
            GUILayout.EndVertical();
        }

        // PlayerInfo 정보 출력
        GUI.contentColor = Color.green;
        showPlayerInfo = GUILayout.Toggle(showPlayerInfo, "Player Info");
        if (showPlayerInfo)
        {
            GUI.contentColor = Color.white;
            GUILayout.Label("Player ID: " + playerInfo.playerId);
            GUILayout.Label("Money: " + playerInfo.money);
            GUILayout.Label("Level: " + playerInfo.level);
            GUILayout.Label("Experience: " + playerInfo.exp);
            GUILayout.Label("Equipped Bat ID: " + playerInfo.equipBatId);
            GUILayout.Label("Equipped Ball ID: " + playerInfo.equipBallId);
            GUILayout.Label("Equipped Background ID: " + playerInfo.equipBackgroundID);


        }

        GUI.contentColor = Color.red;
        // 첼린지 섹션
        showChallenge = GUILayout.Toggle(showChallenge, "Challenge");
        if (showChallenge)
        {
            GUI.contentColor = Color.white;
            GUILayout.BeginVertical();
            foreach (var item in csoData.Keys)
            {
                GUILayout.Label($"{item} Clear : {csoData[item]}");
            }
            GUILayout.EndVertical();

            GUILayout.Label("ChallengeType " + Managers.Game.ChallengeMode.ToString());
            GUILayout.Label("ChallengeScore " + Managers.Game.ChallengeScore);
            GUILayout.Label("Score " + Managers.Game.GameScore);
            GUILayout.Label("SwingCount: " + Managers.Game.SwingCount);
            GUILayout.Label("Homerun: " + Managers.Game.HomeRunCount);
        }


        GUI.contentColor = Color.white;
        // 옵션 섹션
        showOptions = GUILayout.Toggle(showOptions, "Options");
        if (showOptions)
        {
            // 폰트 크기 및 색상 조절
            GUILayout.Label($"Font Size: {fontSize}");
            fontSize = (int)GUILayout.HorizontalSlider(fontSize, 11, 80, GUILayout.Width(500), GUILayout.Height(120));
            //GUILayout.Label("Font Color:");
            //fontColor.r = GUILayout.HorizontalSlider(fontColor.r, 0, 1, GUILayout.Width(400), GUILayout.Height(50));
            //fontColor.g = GUILayout.HorizontalSlider(fontColor.g, 0, 1, GUILayout.Width(400), GUILayout.Height(50));
            //fontColor.b = GUILayout.HorizontalSlider(fontColor.b, 0, 1, GUILayout.Width(400), GUILayout.Height(50));
            //fontColor.a = GUILayout.HorizontalSlider(fontColor.a, 0, 1, GUILayout.Width(400), GUILayout.Height(50));
        }

        GUILayout.EndScrollView();

        // 윈도우 드래그
        GUI.DragWindow(new Rect(0, 0, 10000, 20));
    }

    void ResizeWindow()
    {
        Vector2 mouse = GUIUtility.ScreenToGUIPoint(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y));
        Rect r = new Rect(windowRect.x + windowRect.width - 10, windowRect.y + windowRect.height - 10, 10, 10);
        if (Event.current.type == EventType.MouseDown && r.Contains(mouse))
        {
            isResizing = true;
            lastMousePosition = mouse;
        }
        if (isResizing)
        {
            if (Event.current.type == EventType.MouseUp)
                isResizing = false;
            else
            {
                windowRect.width += mouse.x - lastMousePosition.x;
                windowRect.height += mouse.y - lastMousePosition.y;
                lastMousePosition = mouse;
            }
        }
    }
#endif

}
