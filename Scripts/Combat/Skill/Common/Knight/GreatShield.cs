using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Knight GreatShield Skill", menuName = "Skills/Knight/GreatShield")]
public class GreatShield : BaseSkill
{
    public float shieldMultiplier = 0.25f;
    public float shieldRadius = 5f; // 보호막을 부여할 범위
    public GameObject shieldEffectPrefab; // 보호막 효과 프리팹

    public override void ExecuteSkill(BaseCombatAI character)
    {
        if (character is Knight knight)
        {
            float shieldAmount = knight.MaxHP * shieldMultiplier;

            // Knight 자신에게 보호막 부여 및 스킬 이펙트 재생
            ApplyShieldToKnight(knight, shieldAmount);

            // 주변 아군에게 보호막 부여
            Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(knight.transform.position, shieldRadius, LayerMask.GetMask("Character"));
            foreach (Collider2D collider in nearbyColliders)
            {
                BaseCombatAI ally = collider.GetComponent<BaseCombatAI>();
                if (ally != null && ally != knight && ally.team == knight.team)
                {
                    ApplyShieldToAlly(ally, shieldAmount);
                }
            }

            // 스킬 애니메이션 재생
            knight.PlayAnimation(AnimationData.Skill_NormalHash);
            SoundManager.Instance.PlaySound("SFX_HolySkill");
        }
    }

    private void ApplyShieldToKnight(Knight knight, float shieldAmount)
    {
        knight.AddShield(shieldAmount);

        // 스킬 사용자에게 BaseSkill의 skillEffectObject 이펙트 재생
        if (skillEffectObject != null)
        {
            Vector3 offset = knight.transform.position;
            offset.y += 1f;
            EffectManager.Instance.PlayAnimatedSpriteEffect(skillEffectObject, offset, false);
        }
    }

    private void ApplyShieldToAlly(BaseCombatAI ally, float shieldAmount)
    {
        ally.AddShield(shieldAmount);

        // 영향을 받는 아군에게 shieldEffectPrefab 이펙트 재생
        if (shieldEffectPrefab != null)
        {
            EffectManager.Instance.PlayAnimatedSpriteEffect(shieldEffectPrefab, ally.transform.position, false);
        }
    }
}