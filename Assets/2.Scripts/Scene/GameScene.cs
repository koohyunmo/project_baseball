using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LoadData();
    }

    private void LoadData()
    {
        Managers.Resource.LoadAllAsync<GameObject>("PreLoad",Define.Prefabs.None, (key, count, totalCount) =>
        {
            Debug.Log($"{key} {count}/{totalCount}");

            if (count == totalCount)
            {
                StartLoaded();
            }
        });

        Managers.Resource.LoadAllAsync<GameObject>("Prefabs", Define.Prefabs.Bat ,(key, count, totalCount) =>
        {
            Debug.Log($"{key} {count}/{totalCount}");
        });
    }

    void StartLoaded()
    {
        Object obj = GameObject.FindObjectOfType(typeof(EventSystem));
        if (obj == null)
            Managers.Resource.Instantiate("EventSystem").name = "@EventSystem";

        Managers.Game.Init();

        //var ui_Main = Managers.UI.ShowSceneUI<UI_MainScene>();
        //var ui_Drag = Managers.UI.ShowPopupUI<UI_DragPopup>();

    }


}
