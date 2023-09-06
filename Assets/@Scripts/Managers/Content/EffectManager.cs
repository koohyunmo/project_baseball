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
    
    public void Play(string key, Vector3 pos, Transform parent = null)
    {
        var effect = Managers.Object.Spawn<EffectController>(key, pos);

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

            EffectDataDict.Add(key,efd);
        }
        else
        {
            var duration = EffectDataDict[key].duration;
            effect.SetData(duration);
            ps.Play();
        }
     
    }

}