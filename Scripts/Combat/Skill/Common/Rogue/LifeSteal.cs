using UnityEngine;

[CreateAssetMenu(fileName = "New Rogue LifeSteal Skill", menuName = "Skills/Rogue/LifeSteal")]
public class LifeSteal : BaseSkill
{
    public float skillDamageMultiplier = 1.5f;
    public GameObject lifeStealEffectPrefab;
    public GameObject healEffectPrefab;

    public override void ExecuteSkill(BaseCombatAI character)
    {
        if (character is Rogue rogue)
        {
            PerformLifeSteal(rogue);
        }
    }

    private void PerformLifeSteal(Rogue rogue)
    {
        if (rogue.cachedNearestTarget != null)
        {
            rogue.PlayAnimation(AnimationData.Skill_NormalHash);
            SoundManager.Instance.PlaySound("SFX_DaggerSwing");
            // 데미지 계산
            float damage = rogue.AttackDamage * skillDamageMultiplier;

            // 대상에게 데미지 적용
            rogue.cachedNearestTarget.TakeDamage(damage, DamageType.Physical, AttackType.Slash, false);

            // 흡혈량 계산 (데미지의 절반)
            float healAmount = damage * 0.5f;

            // 로그에게 체력 회복
            rogue.Heal(healAmount);

            // 이펙트 재생
            PlayLifeStealEffect(rogue, rogue.cachedNearestTarget.transform.position);

            
        }
              
    }

    private void PlayLifeStealEffect(Rogue rogue, Vector3 targetPosition)
    {
        if (lifeStealEffectPrefab != null)
        {
            // 대상 위치에 이펙트 재생
            EffectManager.Instance.PlayAnimatedSpriteEffect(lifeStealEffectPrefab, targetPosition, false);

            Vector3 offset = rogue.transform.position;
            offset.y += 2f;

            // 로그 위치에 이펙트 재생 (힐 이펙트)
            EffectManager.Instance.PlayAnimatedSpriteEffect(healEffectPrefab, rogue.transform.position, false);
        }
    }
}