using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SkillScriptableObject", menuName = "SO/skillSO")]
public class SkillScriptableObject : ItemScriptableObject
{

    public Define.HawkeyeLevel HawkeyeLevel;
    public Define.SkillType SkillType;
    public float titanNum = 0;
    public int bonusNum = 0;
    public float powerNum = 0;


    public override void Settings()
    {
        type = Define.ItemType.SKILL;
    }
}
