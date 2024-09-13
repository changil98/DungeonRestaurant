using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "New Priest Cure Skill", menuName = "Skills/Priest/Cure")]
public class Cure : BaseSkill
{
    public float healMultiplier = 1.5f;
    public float healRadius = 10f; // 힐 범위
    public GameObject healEffectPrefab; // 힐 이펙트 프리팹

    public override void ExecuteSkill(BaseCombatAI character)
    {
        if (character is Priest priest)
        {
            // 가장 체력 비율이 낮은 아군 찾기
            BaseCombatAI targetToHeal = FindLowestHealthRatioAlly(priest);

            // 주변에 아군이 없으면 자신을 대상으로 설정
            if (targetToHeal == null)
            {
                targetToHeal = priest;
            }

            if (targetToHeal != null)
            {
                // 스킬 애니메이션 재생
                priest.PlayAnimation(AnimationData.Skill_MagicHash);
                SoundManager.Instance.PlaySound("SFX_HolySkill");
                // 힐량 계산
                float healAmount = priest.AttackDamage * healMultiplier;

                // 체력 회복
                targetToHeal.Heal(healAmount);
                SoundManager.Instance.PlaySound("SFX_Heal");
                // 스킬 이펙트 재생
                PlaySkillEffect(priest);

                // 힐 이펙트 재생
                PlayHealEffect(targetToHeal);                
            }           
        }
    }

    private BaseCombatAI FindLowestHealthRatioAlly(Priest priest)
    {
        return Physics2D.OverlapCircleAll(priest.transform.position, healRadius, LayerMask.GetMask("Character"))
            .Select(collider => collider.GetComponent<BaseCombatAI>())
            .Where(ally => ally != null && ally.team == priest.team && ally != priest)
            .OrderBy(ally => ally.CurrentHP / ally.MaxHP)
            .FirstOrDefault();
    }

    private void PlaySkillEffect(Priest priest)
    {
        if (skillEffectObject != null)
        {
            Vector3 effectPosition = priest.transform.position;
            effectPosition.y += 1f; // 이펙트를 캐릭터 위에 표시
            EffectManager.Instance.PlayAnimatedSpriteEffect(skillEffectObject, effectPosition, false);
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
