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
    Dictionary<int, InGameObject> _inGameObjDict = new Dictionary<int, InGameObject>();
    // 공 오브젝트
    Dictionary<int, BallMovement> _ballDict = new Dictionary<int, BallMovement>();
    public Dictionary<int, InGameObject> InGameObjDict { get { return _inGameObjDict; } private set { _inGameObjDict = value; } }
    public Dictionary<int, BallMovement> BallDict { get { return _ballDict; } private set { _ballDict = value; } }

    private static int id = 0;


    public void Init()
    {
        Clear();
    }

    public T Spawn<T>(string key, Vector3 pos, Transform parent = null, bool isPool = true) where T : InGameObject
    {
        System.Type type = typeof(T);

        int id = GetId();

        if (type == typeof(BallMovement))
        {
            GameObject ball = Managers.Resource.Instantiate(key, parent , isPool);
            ball.transform.position = pos;

            BallMovement bm = ball.GetOrAddComponent<BallMovement>();
            bm.ObjId = id;
            if (_ballDict.ContainsKey(id) == false)
                _ballDict.Add(id, bm);
            else
                Debug.LogWarning("Aleady Has Key");

            return bm as T;
        }
        else if(type == typeof(InGameObject))
        {

            GameObject go = Managers.Resource.Instantiate(key, parent, isPool);
            go.gameObject.transform.position = pos;

            InGameObject component = go.GetOrAddComponent<InGameObject>();        
            component.ObjId = id;

            if (_inGameObjDict.ContainsKey(id) == false)
                _inGameObjDict.Add(id, component);
            else
                Debug.LogWarning("Aleady Has Key");

            return component as T;
        }

        return null as T;
    }

    public void Despawn<T>(T component) where T : InGameObject
    {
        System.Type type = typeof(T);

        int id = component.GetComponent<InGameObject>().ObjId;

        if (type == null)
        {
            Debug.LogWarning($"{type} has not IngameObejct Component");
        }
        else if(type == typeof(BallMovement))
        {
            _ballDict.Remove(id);
            Managers.Resource.Destroy(component.gameObject as GameObject);
        }
        else if(type == typeof(InGameObject))
        {
            _inGameObjDict.Remove(id);
            Managers.Resource.Destroy(component.gameObject as GameObject);
        }
    }

    public void Despawn<T>(int key) where T : InGameObject
    {
        System.Type type = typeof(T);

        if (type == null)
        {
            Debug.LogWarning($"{type} has not IngameObejct Component");
        }
        else if (type == typeof(BallMovement))
        {
            if(_ballDict.TryGetValue(key, out BallMovement bm))
            {
                _ballDict.Remove(key);
                Managers.Resource.Destroy(bm.gameObject as GameObject);
            }
        }
        else if (type == typeof(InGameObject))
        {

            if (_inGameObjDict.TryGetValue(key, out InGameObject ig))
            {
                _inGameObjDict.Remove(key);
                Managers.Resource.Destroy(ig.gameObject as GameObject);
            }
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
            Despawn<BallMovement>(item);
        }
    }

    public void DespawnAll()
    {
        // 복사 삭제
        List<int> objectKeysToRemove = new List<int>(_inGameObjDict.Keys);
        foreach (var item in objectKeysToRemove)
        {
            Despawn<InGameObject>(item);
        }

    }


    public void Clear()
    {
        _inGameObjDict.Clear();
        _ballDict.Clear();
    }
}
