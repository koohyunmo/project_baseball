using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class Managers : MonoBehaviour
{
    static Managers s_instance; // 유일성이 보장된다
    static bool s_init = false;


    public static UIManager UI { get { return Instance?._ui; } }
    public static DataManager Data { get { return Instance?._db; } }
    public static PoolManager Pool { get { return Instance?._pool; } }
    public static ResourceManager Resource { get { return Instance?._resource; } }
    public static GameManager Game { get { return Instance?._game; } }

    [SerializeField] UIManager _ui = new UIManager();
    [SerializeField] PoolManager _pool = new PoolManager();
    [SerializeField] ResourceManager _resource = new ResourceManager();
    [SerializeField] GameManager _game = new GameManager();
    [SerializeField] DataManager _db = new DataManager();


    public static Managers Instance { get { Init();  return s_instance; } } 


    public static void Init()
    {
        if (s_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<Managers>();

        }
    }


    public static void Clear()
    {

    }
}
