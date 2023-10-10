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
    public static ObjectManager Obj { get { return Instance?._object; } }
    public static EffectManager Effect { get { return Instance?._effect; } }
    public static LocalizationManager Localization { get { return Instance?._local; } }
    public static VibrationManager Vibration { get { return Instance?._vibration; } }
    public static AdManager Ad { get { return Instance?._ad; } }
    public static ReplayManager Replay { get; private set; }

    public static SkillManager Skill { get{ return Instance?._skill; } }

    [SerializeField] UIManager _ui = new UIManager();
    [SerializeField] PoolManager _pool = new PoolManager();
    [SerializeField] ResourceManager _resource = new ResourceManager();
    [SerializeField] GameManager _game = new GameManager();
    [SerializeField] DataManager _db = new DataManager();
    [SerializeField] ObjectManager _object = new ObjectManager();
    [SerializeField] EffectManager _effect = new EffectManager();
    [SerializeField] LocalizationManager _local = new LocalizationManager();
    [SerializeField] VibrationManager _vibration = new VibrationManager();
    [SerializeField] SkillManager _skill = new SkillManager();
    [SerializeField] AdManager _ad = new AdManager();


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
            Application.targetFrameRate = 60;
            Localization.Init();

            Replay = go.GetOrAddComponent<ReplayManager>();
        }
    }


    public static void Clear()
    {
        Resource.Resources.Clear();
        Obj.Clear();
        UI.Clear();
        s_instance._resource.Clear();

    }


    public void OnDestroy()
    {
        Clear();
    }
}
