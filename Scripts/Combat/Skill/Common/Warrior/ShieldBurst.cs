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
                // 대미지 적용
                float damage = warrior.AttackDamage * damageMultiplier;
                target.TakeDamage(damage, DamageType.Physical, AttackType.Slash, false);

                // 방어력 감소 효과 적용
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
        // 방어력 감소 적용
        target.ApplyDefenseReduction(defenseReductionPercentage);

        // 지속 시간 대기
        yield return new WaitForSeconds(defenseReductionDuration);

        // 방어력 복구
        target.RestoreDefense();
    }

}
