using UnityEngine;
[CreateAssetMenu(fileName = "New Priest Skill Set", menuName = "Skill Sets/Priest Skill Set")]
public class PriestSkillSet : SkillSet
{
    public BaseSkill skill1; // Cure
    public BaseSkill skill2; // Blessing
    public BaseSkill skill3; // Repentance

    public override BaseSkill GetRandomSkill()
    {
        BaseSkill[] skills = { skill1, skill2, skill3 };
        return skills[Random.Range(0, skills.Length)];
    }
}
