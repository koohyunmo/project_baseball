using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct EffectData
{
    public EffectController ec;
    public GameObject obj;
    public ParticleSystem ps;
    public float duration;

}

public class EffectManager
{
    public Dictionary<string, EffectData> EffectDataDict = new Dictionary<string, EffectData>();

    readonly string treeEffectKey = "Tree_Hit_";
    readonly string aluEffectKey = "Alu_Hit_";
    readonly string sp1EffectKey = "Sp1_Hit_";
    readonly string sp2EffectKey = "Sp2_Hit_";
    readonly string sp3EffectKey = "Sp3_Hit_";
    readonly string sp4EffectKey = "Sp4_Hit_";

    public void PlayEffect(string key, Vector3 pos, Transform parent = null)
    {
        var effect = Managers.Obj.Spawn<EffectController>(key, pos, parent);

        if (effect == null)
            return;

        var ps = effect.GetComponent<ParticleSystem>();

        if (EffectDataDict.ContainsKey(key) == false)
        {
            EffectData efd = new EffectData
            {
                ec = effect,
                obj = effect.gameObject,
                ps = ps,
                duration = ps.main.duration
            };

            EffectDataDict.Add(key, efd);
        }


        var duration = EffectDataDict[key].duration;
        effect.SetData(duration);
        ps.Play();
    }

    public void PlayBatEffect(Vector3 pos, Transform parent = null)
    {
        var key = "";

        switch (Managers.Game.BatType)
        {
            case Define.BatType.Tree:
                key = treeEffectKey;
                break;
            case Define.BatType.Alu:
                key = aluEffectKey;
                break;
            case Define.BatType.Sp1:
                key = sp1EffectKey;
                break;
            case Define.BatType.Sp2:
                key = sp2EffectKey; 
                break;
            case Define.BatType.Sp3:
                key = sp3EffectKey;
                break;
            case Define.BatType.Sp4:
                key = sp4EffectKey;
                break;
        }

        key = key + Managers.Game.HitType.ToString();

     

        var effect = Managers.Obj.Spawn<EffectController>(key, pos, parent);

        if (effect == null)
        {
            Debug.LogError($"{key}의 이펙트가 없습니다");
            return;
        }
            

        var ps = effect.GetComponent<ParticleSystem>();

        if (EffectDataDict.ContainsKey(key) == false)
        {
            EffectData efd = new EffectData
            {
                ec = effect,
                obj = effect.gameObject,
                ps = ps,
                duration = ps.main.duration
            };

            EffectDataDict.Add(key, efd);
        }


        var duration = EffectDataDict[key].duration;
        effect.SetData(duration);
        ps.Play();
    }


    public void PlayTrail(string key, Vector3 pos, Transform parent = null)
    {
        //GameObject ball = Managers.Resource.Instantiate(key, parent);
        //ball.transform.SetParent(parent);
        //ball.transform.position = pos;
       
        var trail = Managers.Obj.Spawn<EffectController>(key, pos, parent);
        var ps = trail.GetComponent<ParticleSystem>();
        trail.transform.SetParent(parent);
        ps.Play();
    }
}
