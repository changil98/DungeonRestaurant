using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Priest Repentance Skill", menuName = "Skills/Priest/Repentance")]
public class Repentance : BaseSkill
{
    public float damageMultiplier = 2f;
    public float healMultiplier = 0.4f;
    public float healRadius = 5f; // 힐 범위
    public GameObject damageEffectPrefab;
    public GameObject healEffectPrefab;

    public override void ExecuteSkill(BaseCombatAI character)
    {
        if (character is Priest priest)
        {
            priest.PlayAnimation(AnimationData.Skill_MagicHash);
            SoundManager.Instance.PlaySound("SFX_MagicSkill");
            // 대상에게 피해 입히기
            if (priest.cachedNearestTarget != null)
            {
                float damage = priest.AttackDamage * damageMultiplier;
                priest.cachedNearestTarget.TakeDamage(damage,DamageType.Magical, AttackType.Blunt, false);
                PlayDamageEffect(priest.cachedNearestTarget);
               
            }

            // 주변 아군 회복
            var allies = FindNearbyAllies(priest);
            float healAmount = priest.AttackDamage * healMultiplier;

            foreach (var ally in allies)
            {
                ally.Heal(healAmount);
                PlayHealEffect(ally);
                SoundManager.Instance.PlaySound("SFX_Heal");
            }
        }
    }

    private IEnumerable<BaseCombatAI> FindNearbyAllies(Priest priest)
    {
        return Physics2D.OverlapCircleAll(priest.transform.position, healRadius, LayerMask.GetMask("Character"))
            .Select(collider => collider.GetComponent<BaseCombatAI>())
            .Where(ally => ally != null && ally.team == priest.team && ally != priest);
    }

    private void PlayDamageEffect(BaseCombatAI target)
    {
        if (damageEffectPrefab != null)
        {
            Vector3 effectPosition = target.transform.position;
            effectPosition.y += 1f;
            EffectManager.Instance.PlayAnimatedSpriteEffect(damageEffectPrefab, effectPosition, false);
        }
    }

    private void PlayHealEffect(BaseCombatAI target)
    {
        if (healEffectPrefab != null)
        {
            Vector3 effectPosition = target.transform.position;
            effectPosition.y += 1f; // 이펙트를 캐릭터 위에 표시
            EffectManager.Instance.PlayAnimatedSpriteEffect(healEffectPrefab, effectPosition, false);
        }
    }
}
