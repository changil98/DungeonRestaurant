using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Priest Blessing Skill", menuName = "Skills/Priest/Blessing")]
public class Blessing : BaseSkill
{
    public float healMultiplier = 0.2f;
    public float attackBoostPercentage = 10f;
    public float attackBoostDuration = 3f;
    public GameObject blessingEffectPrefab;

    public override void ExecuteSkill(BaseCombatAI character)
    {
        if (character is Priest priest)
        {
            // 모든 아군 찾기
            var allies = FindAllAllies(priest);

            foreach (var ally in allies)
            {
                // 체력 회복
                float healAmount = priest.AttackDamage * healMultiplier;
                ally.Heal(healAmount);

                // 공격력 증가 버프 적용
                ally.StartCoroutine(ApplyAttackBoost(ally));

                // 버프 이펙트 재생
                PlayBlessingEffect(ally);
            }

            // 스킬 애니메이션 재생
            priest.PlayAnimation(AnimationData.Skill_MagicHash);
            SoundManager.Instance.PlaySound("SFX_HolySkill");
            // 스킬 이펙트 재생
            PlaySkillEffect(priest);
            SoundManager.Instance.PlaySound("SFX_Heal");
        }
    }

    private IEnumerable<BaseCombatAI> FindAllAllies(Priest priest)
    {
        return Physics2D.OverlapCircleAll(priest.transform.position, 50f, LayerMask.GetMask("Character"))
            .Select(collider => collider.GetComponent<BaseCombatAI>())
            .Where(ally => ally != null && ally.team == priest.team);
    }

    private IEnumerator ApplyAttackBoost(BaseCombatAI ally)
    {
        float originalAttack = ally.AttackDamage;
        ally.AttackDamage *= (1 + attackBoostPercentage / 100f);
        
        yield return new WaitForSeconds(attackBoostDuration);

        ally.AttackDamage = originalAttack;      
    }

    private void PlaySkillEffect(Priest priest)
    {
        if (skillEffectObject != null)
        {
            Vector3 effectPos = priest.pivotTransform.position;
            effectPos.y += 2f;
            EffectManager.Instance.PlayAnimatedSpriteEffect(skillEffectObject, effectPos, false);
        }
    }

    private void PlayBlessingEffect(BaseCombatAI target)
    {
        if (blessingEffectPrefab != null)
        {
            Vector3 effectPosition = target.transform.position;
            Vector3 offset = effectPosition - target.transform.position;
            offset.y += 2f;
            EffectManager.Instance.PlayFollowingAnimatedSpriteEffect(blessingEffectPrefab, target.transform,offset, false);
        }
    }
}