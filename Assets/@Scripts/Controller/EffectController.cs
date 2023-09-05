using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectController : InGameObjectController
{
    public void SetData(float time)
    {
        StartCoroutine(co_Delay(time));
    }

    IEnumerator co_Delay(float time)
    {
        yield return new WaitForSeconds(time);
        Managers.Object.Despawn<EffectController>(ObjId);
    }
}
