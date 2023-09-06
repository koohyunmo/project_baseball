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
        showPlayerInfo = false; // PlayerInfo ���� ǥ�� ����
        showPlayerInventory = false; // PlayerInventory ���� ǥ�� ����
        showOptions = false; // �ɼ� ���� ǥ�� ����
        yield return new WaitForSeconds(1f);
        isManagerLoading = true;

    }

    private Rect windowRect = new Rect(50, 200, 800, 800);
    private Vector2 scrollPosition = Vector2.zero;
    private Vector2 scrollPosition2 = Vector2.zero;
    private int fontSize = 60;
    private Color fontColor = Color.white;

    private bool showPlayerInfo = false; // PlayerInfo ���� ǥ�� ����
    private bool showPlayerInventory = false; // PlayerInventory ���� ǥ�� ����
    private bool showChallenge = false; // PlayerInventory ���� ǥ�� ����
    private bool showOptions = false; // �ɼ� ���� ǥ�� ����
    private bool showGUIWindow = true; // GUI â ǥ�� ����
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

        // GUI â ����/�ݱ� ��ư
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
        // Font ����
        GUI.skin.toggle.fontSize = 60; // ��� ��ư ��Ʈ ũ�� ����
        GUI.skin.toggle.fontStyle = FontStyle.Bold;
        GUI.skin.label.fontSize = fontSize;
        

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        // Player Inventory ���
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

        // PlayerInfo ���� ���
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
        // ÿ���� ����
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
        // �ɼ� ����
        showOptions = GUILayout.Toggle(showOptions, "Options");
        if (showOptions)
        {
            // ��Ʈ ũ�� �� ���� ����
            GUILayout.Label($"Font Size: {fontSize}");
            fontSize = (int)GUILayout.HorizontalSlider(fontSize, 11, 80, GUILayout.Width(500), GUILayout.Height(120));
            //GUILayout.Label("Font Color:");
            //fontColor.r = GUILayout.HorizontalSlider(fontColor.r, 0, 1, GUILayout.Width(400), GUILayout.Height(50));
            //fontColor.g = GUILayout.HorizontalSlider(fontColor.g, 0, 1, GUILayout.Width(400), GUILayout.Height(50));
            //fontColor.b = GUILayout.HorizontalSlider(fontColor.b, 0, 1, GUILayout.Width(400), GUILayout.Height(50));
            //fontColor.a = GUILayout.HorizontalSlider(fontColor.a, 0, 1, GUILayout.Width(400), GUILayout.Height(50));
        }

        GUILayout.EndScrollView();

        // ������ �巡��
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
