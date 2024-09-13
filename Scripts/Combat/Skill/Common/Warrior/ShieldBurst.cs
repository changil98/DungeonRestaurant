using System.Collections;
using UnityEngine;
[CreateAssetMenu(fileName = "New Warrior Bash Skill", menuName = "Skills/Warrior/ShieldBurst")]
public class ShieldBurst : BaseSkill
{
    public float damageMultiplier = 2f;
    public float defenseReductionPercentage = 0.3f;
    public float defenseReductionDuration = 3f;

    public override void ExecuteSkill(BaseCombatAI character)
    {
        if (character is Warrior warrior)
        {
            warrior.PlayAnimation(AnimationData.Skill_NormalHash);
            SoundManager.Instance.PlaySound("SFX_WarriorSkill1,2");
            Vector3 targetPos = warrior.cachedNearestTarget.transform.position;
            targetPos.y += 0.8f;
            EffectManager.Instance.PlayAnimatedSpriteEffect(skillEffectObject, targetPos, false);

            BaseCombatAI target = warrior.cachedNearestTarget;
            if (target != null && target.team != warrior.team)
            {
                // ����� ����
                float damage = warrior.AttackDamage * damageMultiplier;
                target.TakeDamage(damage, DamageType.Physical, AttackType.Slash, false);

                // ���� ���� ȿ�� ����
                ApplyDefenseReduction(target);
            }
        }
    }

    private void ApplyDefenseReduction(BaseCombatAI target)
    {
        target.StartCoroutine(DefenseReductionCoroutine(target));
    }

    private IEnumerator DefenseReductionCoroutine(BaseCombatAI target)
    {
        // ���� ���� ����
        target.ApplyDefenseReduction(defenseReductionPercentage);

        // ���� �ð� ���
        yield return new WaitForSeconds(defenseReductionDuration);

        // ���� ����
        target.RestoreDefense();
    }

}
