using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Mage Pyro Magic Skill", menuName = "Skills/Mage/Pyro Magic")]
public class PyroMagic : BaseSkill
{
    public float damageMultiplier = 5f;
    public float splashDamageRadius = 5f;

    public override void ExecuteSkill(BaseCombatAI character)
    {
        if (character is Mage mage)
        {
            if (mage.cachedNearestTarget != null)
            {
                SoundManager.Instance.PlaySound("SFX_MagicSkill");
                // 스킬 애니메이션 재생
                mage.PlayAnimation(AnimationData.Skill_MagicHash);

                // 주 대상에게 피해 입히기
                float mainDamage = mage.AttackDamage * damageMultiplier;
                mage.cachedNearestTarget.TakeDamage(mainDamage,DamageType.Magical,AttackType.Blunt,false);

                // 주 대상에게 화염 이펙트 적용
                PlayFireEffect(mage.cachedNearestTarget.transform.position);

                // 주변 적에게 피해 입히기
                ApplySplashDamage(mage, mainDamage / 2f);

            }

            
        }
    }

    private void ApplySplashDamage(Mage mage, float splashDamage)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(mage.cachedNearestTarget.transform.position, splashDamageRadius, LayerMask.GetMask("Character"));

        foreach (Collider2D hitCollider in hitColliders)
        {
            BaseCombatAI enemy = hitCollider.GetComponent<BaseCombatAI>();
            if (enemy != null && enemy != mage.cachedNearestTarget && enemy.team != mage.team)
            {
                SoundManager.Instance.PlaySound("SFX_FireMagicHit");
                enemy.TakeDamage(splashDamage, DamageType.Magical, AttackType.Blunt, false);
            }
        }
    }

    private void PlayFireEffect(Vector3 position)
    {
        if (skillEffectObject != null)
        {
            position.y += 1f;
            EffectManager.Instance.PlayAnimatedSpriteEffect(skillEffectObject, position, false);
        }
    }
}