using DG.Tweening.Plugins.Core.PathCore;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

public class ResourceManager
{
    // 실제 로드한 리소스.
    Dictionary<string, UnityEngine.Object> _resources = new Dictionary<string, UnityEngine.Object>();
    // 벳 데이터
   // Dictionary<string, UnityEngine.Object> _bats = new Dictionary<string, UnityEngine.Object>();

   // public Dictionary<string, UnityEngine.Object> Bats {get { return _bats; } private set { _bats = value; } }
   public Dictionary<string, UnityEngine.Object> Bats {get { return _resources; } private set { _resources = value; } }

    public float LoadBytes { get; private set; }

    #region 리소스 로드
    public T Load<T>(string key) where T : Object
    {
        if (_resources.TryGetValue(key, out Object resource))
        {
            return resource as T;
        }

        //스프라이트 로드할때 항상 .sprite가 붙어 있어야하는데 데이터시트에 .sprite가 붙어있지 않은 데이터가 많음
        //임시로 붙임 -드래곤
        if (typeof(T) == typeof(Sprite))
        {
            key = key + ".sprite";
            if (_resources.TryGetValue(key, out Object temp))
            {
                return temp as T;
            }
        }

        return null;
    }

    public GameObject Instantiate(string key, Transform parent = null, bool pooling = false)
    {
        GameObject prefab = Load<GameObject>($"{key}");
        if (prefab == null)
        {
            Debug.LogError($"Failed to load prefab : {key}");
            return null;
        }

        if (pooling)
            return Managers.Pool.Pop(prefab);

        GameObject go = Object.Instantiate(prefab, parent);

        go.name = prefab.name;
        return go;
    }


    public GameObject Instantiate(GameObject prefab, Transform parent = null, bool pooling = false)
    {
        if (prefab == null)
        {
            Debug.LogError($"Failed to load prefab : {prefab.name}");
            return null;
        }

        if (pooling)
            return Managers.Pool.Pop(prefab);

        GameObject go = Object.Instantiate(prefab, parent);

        go.name = prefab.name;
        return go;
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
            return;

        if (Managers.Pool.Push(go))
            return;

        Object.Destroy(go);
    }

    #endregion
    #region 어드레서블

    public void GetLoadBytes()
    {
        Debug.Log($"Load Bytes size: {LoadBytes} KB");
    }

    public void LoadAsync<T>(string key, Define.Prefabs type = Define.Prefabs.None, Action<T> callback = null) where T : UnityEngine.Object
    {
        //스프라이트인 경우 하위객체의 찐이름으로 로드하면 스프라이트로 로딩이 됌
        string loadKey = key;
        if (key.Contains(".sprite"))
            loadKey = $"{key}[{key.Replace(".sprite", "")}]";

        float beforeLoad = System.GC.GetTotalMemory(false); // 메모리 사용량 측정 시작

        var asyncOperation = Addressables.LoadAssetAsync<T>(loadKey);
        asyncOperation.Completed += (op) =>
        {

            float afterLoad = System.GC.GetTotalMemory(false); // 메모리 사용량 측정 종료
            float sizeInBytes = afterLoad - beforeLoad;
            Debug.Log($"Resource {key} size: {sizeInBytes / 1024f} KB");

            LoadBytes += sizeInBytes / 1024f;

            // 캐시 확인.
            if (_resources.TryGetValue(key, out Object resource))
            {
                callback?.Invoke(op.Result);
                return;
            }

            switch (type)
            {
                case Define.Prefabs.None:
                    _resources.Add(key, op.Result);
                    break;
                case Define.Prefabs.Bat:
                    _resources.Add(key, op.Result);
                    break;
                case Define.Prefabs.Ball:
                    _resources.Add(key, op.Result);
                    break;
            }
            
            callback?.Invoke(op.Result);
        };
    }

    public void LoadAllAsync<T>(string label, Define.Prefabs type = Define.Prefabs.None, Action<string, int, int> callback = null) where T : UnityEngine.Object
    {

        var opHandle = Addressables.LoadResourceLocationsAsync(label, typeof(T));
        opHandle.Completed += (op) =>
        {
            int loadCount = 0;

            int totalCount = op.Result.Count;

            foreach (var result in op.Result)
            {
                if (result.PrimaryKey.Contains(".sprite"))
                {
                    LoadAsync<Sprite>(result.PrimaryKey, type ,(obj) =>
                    {
                        loadCount++;
                        callback?.Invoke(result.PrimaryKey, loadCount, totalCount);
                    });
                }
                else
                {
                    LoadAsync<T>(result.PrimaryKey,type, (obj) =>
                    {
                        loadCount++;
                        callback?.Invoke(result.PrimaryKey, loadCount, totalCount);
                    });

                }
            }
        };
    }

    #endregion

    public void Clear()
    {
        _resources.Clear();
        //_bats.Clear();
    }
}
