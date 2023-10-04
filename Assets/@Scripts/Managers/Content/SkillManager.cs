using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                break;
            case Define.SkillType.Strong:
                Managers.Game.skillBonus = 5f;
                break;
            case Define.SkillType.Collider:
                Managers.Game.Bat.HitColiderTransform.localScale = new Vector3(colliderLocalScale.x *2 , colliderLocalScale.y, colliderLocalScale.z);
                Managers.Game.Bat.model.transform.localScale = new Vector3(modelLocalScale.x*2f, modelLocalScale.y, modelLocalScale.z);
                break;
            case Define.SkillType.Bonus:
                Managers.Game.hitBonus = 1;
                break;
            case Define.SkillType.None:
                break;
        }

    }

    public void SkillClear()
    {
        Clear();
    }


    private void Clear()
    {
        if (colliderLocalScale.Equals(Vector3.zero) || modelLocalScale.Equals(Vector3.zero))
            return;

        Debug.Log(colliderLocalScale);
        Debug.Log(modelLocalScale);

        Managers.Game.hitBonus = 0;
        Managers.Game.skillBonus = 0.0f;
        Managers.Game.Bat.HitColiderTransform.localScale = colliderLocalScale;
        Managers.Game.Bat.model.transform.localScale = modelLocalScale;
    }
}
