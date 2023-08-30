using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.UIElements;

public class ObjectManager
{

    // 모든 오브젝트
    Dictionary<int, InGameObjectController> _inGameObjDict = new Dictionary<int, InGameObjectController>();
    // 공 오브젝트
    Dictionary<int, BallController> _ballDict = new Dictionary<int, BallController>();
    public Dictionary<int, InGameObjectController> InGameObjDict { get { return _inGameObjDict; } private set { _inGameObjDict = value; } }
    public Dictionary<int, BallController> BallDict { get { return _ballDict; } private set { _ballDict = value; } }

    private static int id = 0;


    public void Init()
    {
        Clear();
    }

    public T Spawn<T>(string key, Vector3 pos, Transform parent = null, bool isPool = true) where T : InGameObjectController
    {
        System.Type type = typeof(T);

        int id = GetId();

        if (type == typeof(BallController))
        {
            GameObject ball = Managers.Resource.Instantiate(key, parent , isPool);
            ball.transform.position = pos;

            BallController bm = ball.GetOrAddComponent<BallController>();

            bm.ObjId = id;

            if (_ballDict.ContainsKey(id) == false)
                _ballDict.Add(id, bm);
            else
                Debug.LogWarning("Aleady Has Key");

            return bm as T;
        }

        else if (type == typeof(TextController))
        {

            GameObject go = Managers.Resource.Instantiate(key, parent, isPool);
            go.transform.position = pos;

            TextController component = go.GetOrAddComponent<TextController>();

            component.ObjId = id;

            if (_inGameObjDict.ContainsKey(id) == false)
                _inGameObjDict.Add(id, component);
            else
                Debug.LogWarning("Aleady Has Key");

            return component as T;
        }
        else if (type == typeof(BallAimController))
        {

            GameObject go = Managers.Resource.Instantiate(key, parent, isPool);
            go.transform.position = pos;

            BallAimController component = go.GetOrAddComponent<BallAimController>();

            component.ObjId = id;

            if (_inGameObjDict.ContainsKey(id) == false)
                _inGameObjDict.Add(id, component);
            else
                Debug.LogWarning("Aleady Has Key");

            return component as T;
        }
        else if (type == typeof(InGameObjectController))
        {

            GameObject go = Managers.Resource.Instantiate(key, parent, isPool);
            go.transform.position = pos;

            InGameObjectController component = go.GetOrAddComponent<InGameObjectController>();

            component.ObjId = id;

            if (_inGameObjDict.ContainsKey(id) == false)
                _inGameObjDict.Add(id, component);
            else
                Debug.LogWarning("Aleady Has Key");

            return component as T;
        }
        return null as T;
    }

    public void Despawn<T>(T component) where T : InGameObjectController
    {
        System.Type type = typeof(T);

        int id = component.GetComponent<InGameObjectController>().ObjId;

        if (type == null)
        {
            Debug.LogWarning($"{type} has not IngameObejct Component");
        }
        else if(type == typeof(BallController))
        {
            _ballDict.Remove(id);
            Managers.Resource.Destroy(component.gameObject as GameObject);
        }
        else if (type == typeof(TextController))
        {
            _inGameObjDict.Remove(id);
            Managers.Resource.Destroy(component.gameObject as GameObject);
        }
        else if(type == typeof(InGameObjectController))
        {
            _inGameObjDict.Remove(id);
            Managers.Resource.Destroy(component.gameObject as GameObject);
        }
    }

    public void Despawn<T>(int key) where T : InGameObjectController
    {
        System.Type type = typeof(T);

        if (type == null)
        {
            Debug.LogWarning($"{type} has not IngameObejct Component");
        }
        else if (type == typeof(BallController))
        {
            if(_ballDict.TryGetValue(key, out BallController bm))
            {
                _ballDict.Remove(key);
                Managers.Resource.Destroy(bm.gameObject);
            }
        }
        else if (typeof(T) == typeof(TextController))
        {
            if (_inGameObjDict.TryGetValue(key, out InGameObjectController ig))
            {
                ig.Clear();
                _inGameObjDict.Remove(key);
                Managers.Resource.Destroy(ig.gameObject);
            }
        }
        else if (typeof(T) == typeof(BallAimController))
        {
            if (_inGameObjDict.TryGetValue(key, out InGameObjectController ig))
            {
                ig.Clear();
                _inGameObjDict.Remove(key);
                Managers.Resource.Destroy(ig.gameObject);
            }
        }
        else if (typeof(T) == typeof(InGameObjectController))
        {
            if (_inGameObjDict.TryGetValue(key, out InGameObjectController ig))
            {
                ig.Clear();
                _inGameObjDict.Remove(key);
                Managers.Resource.Destroy(ig.gameObject);
            }
        }
        else
        {
            Debug.LogWarning($"{typeof(T)} has not IngameObejct Component");
        }
    }

    public int GetId()
    {
        return ++id;
    }

    public void DespawnBall()
    {
        // 복사 삭제
        List<int> ballKeysToRemove = new List<int>(_ballDict.Keys);
        foreach (var item in ballKeysToRemove)
        {
            Debug.Log(item);
            Despawn<BallController>(item);
        }
    }

    public void DespawnAll()
    {
        // 복사 삭제
        List<int> objectKeysToRemove = new List<int>(_inGameObjDict.Keys);
        foreach (var item in objectKeysToRemove)
        {
            Despawn<InGameObjectController>(item);
        }

    }


    public void Clear()
    {
        _inGameObjDict.Clear();
        _ballDict.Clear();
    }
}
