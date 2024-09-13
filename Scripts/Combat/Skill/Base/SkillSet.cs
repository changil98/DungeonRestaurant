using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill Set", menuName = "Skill Sets/Skill Set")]
public class SkillSet : ScriptableObject
{
    [SerializeField, EnumPaging] public eCharacterClass skillClass;
    [SerializeField] protected List<BaseSkill> skills = new List<BaseSkill>();
    public virtual BaseSkill GetRandomSkill()
    {
        return skills[Random.Range(0, skills.Count)];
    }

    public BaseSkill GetSkill(string rcode)
    {
        foreach (BaseSkill skill in skills)
        {
            if (skill.rcode == rcode)
                return skill;
        }

        Debug.Log("잘못된 rcode입니다! return null");
        return null;
    }
}
