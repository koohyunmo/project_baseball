using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SkillScriptableObject", menuName = "SO/skillSO")]
public class SkillScriptableObject : ItemScriptableObject
{

    public Define.HawkeyeLevel HawkeyeLevel;
    public Define.SkillType SkillType;

    public override void Settings()
    {
        type = Define.ItemType.SKILL;
    }
}
