using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SkillManager
{

    Vector3 colliderLocalScale = Vector3.zero;
    Vector3 modelLocalScale = Vector3.zero;


    public void SetColliderLocalScale(Vector3 localScale)
    {
        colliderLocalScale = localScale;
    }

    public void SetModelLocalScale(Vector3 localScale)
    {
        modelLocalScale = localScale;
    }

    public void SkillInjection(SkillScriptableObject so)
    {

        Clear();

        switch (so.SkillType)
        {
            case Define.SkillType.HwakEye:
                Managers.Effect.PlayEffect("FX_Swirl_03 Variant", Managers.Game.BuffSkillTr.position);
                break;
            case Define.SkillType.Strong:
                Managers.Effect.PlayEffect("FX_MagicBlast_Ground_02", Managers.Game.BuffSkillTr.position);
                Managers.Game.skillBonus = so.powerNum;
                break;
            case Define.SkillType.Collider:
                Managers.Effect.PlayEffect("FX_PowerDraw_01", Managers.Game.ColliderSkillTr.position);
                //Managers.Game.Bat.HitColiderTransform.localScale = new Vector3(colliderLocalScale.x *2 , colliderLocalScale.y, colliderLocalScale.z);
                //Managers.Game.Bat.model.transform.localScale = new Vector3(modelLocalScale.x*2f, modelLocalScale.y, modelLocalScale.z);

                Managers.Game.Bat.model.layer = 0;
                Managers.Game.Bat.HitColiderTransform.DOScale(new Vector3(colliderLocalScale.x * so.titanNum, colliderLocalScale.y, colliderLocalScale.z), 1.5f);
                Managers.Game.Bat.model.transform.DOScale(new Vector3(modelLocalScale.x * so.titanNum, modelLocalScale.y, modelLocalScale.z), 1.5f).OnComplete(()=> Managers.Game.Bat.model.layer = 6);
                break;
            case Define.SkillType.Bonus:
                Managers.Effect.PlayEffect("FX_ExperienceGain_01 Variant", Managers.Game.BuffSkillTr.position);
                Managers.Game.hitBonus = so.bonusNum;
                break;
            case Define.SkillType.None:
                break;
        }

#if UNITY_EDITOR
        Debug.Log(so.SkillType);
#endif

    }

    public void SkillClear()
    {
        Clear();
    }


    private void Clear()
    {
        if (colliderLocalScale.Equals(Vector3.zero) || modelLocalScale.Equals(Vector3.zero))
            return;

#if UNITY_EDITOR
        Debug.Log(colliderLocalScale);
        Debug.Log(modelLocalScale);
#endif

        Managers.Game.hitBonus = 0;
        Managers.Game.skillBonus = 0.0f;
        Managers.Game.Bat.HitColiderTransform.localScale = colliderLocalScale;
        Managers.Game.Bat.model.transform.localScale = modelLocalScale;
    }
}
