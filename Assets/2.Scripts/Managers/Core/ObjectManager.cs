using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{

    List<GameObject> _inGameObjects = new List<GameObject>();
    public List<GameObject> InGameObejct { get { return _inGameObjects; } private set{ _inGameObjects = value; } }


    public void Init()
    {
        _inGameObjects.Clear();
    }

    public T Spawn<T>(string key, Vector3 pos) where T : Object
    {
        var go = Managers.Resource.Instantiate(key);
        go.gameObject.transform.position = pos;

        _inGameObjects.Add(go);

        return null;
    }

    public GameObject SpawnObj(string key, Vector3 pos, Transform parent = null)
    {
        var go = Managers.Resource.Instantiate(key, parent, true);
        go.gameObject.transform.position = pos;

        _inGameObjects.Add(go);

        return go;
    }

    public T Spawn<T>(string key) where T : Object
    { 

        return null;
    }

    public void Clear()
    {
        if(_inGameObjects.Count > 0)

        foreach (GameObject obj in _inGameObjects)
        {
            Managers.Resource.Destroy(obj);
        }

        _inGameObjects.Clear();
    }
}
