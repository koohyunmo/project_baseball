using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Managers : MonoBehaviour
{
    static Managers s_instance; // 유일성이 보장된다
    static bool s_init = false;


    public static UIManager UI { get { return Instance?._ui; } }
    public static PoolManager Pool { get { return Instance?._pool; } }
    public static ResourceManager Resource { get { return Instance?._resource; } }

    UIManager _ui = new UIManager();
    PoolManager _pool = new PoolManager();
    ResourceManager _resource = new ResourceManager();

    public static Managers Instance
    {
        get
        {
            if (s_init == false)
            {
                s_init = true;

                GameObject go = GameObject.Find("@Managers");
                if (go == null)
                {
                    go = new GameObject() { name = "@Managers" };
                    go.AddComponent<Managers>();
                }

                DontDestroyOnLoad(go);
                s_instance = go.GetComponent<Managers>();
            }

            return s_instance;
        }
    }

    public static void Clear()
    {

    }
}
