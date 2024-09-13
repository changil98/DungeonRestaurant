using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

[CreateAssetMenu(fileName = "DivineSmite", menuName = "Skills/Paladin/DivineSmite")]
public class DivineSmite : BaseSkill
{
    [SerializeField] private float damageMultiplier = 1.5f;
    [SerializeField] private float stunDuration = 1f;
    [SerializeField] private float shieldMultiplier = 0.2f;
    private float skillAnimationLength;
    private bool isFirstUse = true;

    public override void ExecuteSkill(BaseCombatAI character)
    {
        if (character is Paladin paladin)
        {
            paladin.StartCoroutine(PerformDivinSmite(paladin));
        }
    }

    private IEnumerator PerformDivinSmite(Paladin paladin)
    {
        Animator animator = paladin.GetComponentInChildren<Animator>();
        skillAnimationLength = AnimationData.GetAnimationLength(animator, AnimationData.Skill_NormalHash);
        paladin.PlayAnimation(AnimationData.Skill_NormalHash);
        SoundManager.Instance.PlaySound("SFX_DivineSmite");
        Vector3 effectPos = paladin.cachedNearestTarget.transform.position;
        effectPos.y += 1f;
        

        yield return new WaitForSeconds(skillAnimationLength * 0.58f);
        EffectManager.Instance.PlayAnimatedSpriteEffect(skillEffectObject, effectPos);

        // 대상에게 데미지를 입히고 기절시킴
        if (paladin.cachedNearestTarget != null)
        {
            float damage = paladin.AttackDamage * damageMultiplier;
            paladin.cachedNearestTarget.TakeDamage(damage, DamageType.Physical, AttackType.Blunt, false);
            paladin.cachedNearestTarget.ApplyStun(stunDuration);
        }

        yield return new WaitForSeconds(skillAnimationLength * 0.42f);

        // 보호막 부여
        float shieldAmount = paladin.MaxHP * shieldMultiplier;

        if (isFirstUse || paladin.ShieldAmount == 0)
        {
            // 첫 사용이거나 팔라딘 자신에게 보호막이 없는 경우
            paladin.AddShield(shieldAmount);
            isFirstUse = false;
        }
        else
        {
            // 팔라딘에게 보호막이 있는 경우, 보호막이 없는 가장 가까운 아군에게 부여
            List<PlayerCombatAI> allies = FindAlliesWithoutShield(paladin);
            PlayerCombatAI targetAlly = FindNearestAlly(paladin, allies);

            if (targetAlly != null)
            {
                targetAlly.AddShield(shieldAmount);
            }
            else
            {
                // 모든 아군에게 보호막이 있다면 자신에게 부여 (보호막 갱신)
                paladin.AddShield(shieldAmount);
            }
        }
    }

    private List<PlayerCombatAI> FindAlliesWithoutShield(Paladin paladin)
    {
        return Physics2D.OverlapCircleAll(paladin.transform.position, 50f, LayerMask.GetMask("Character"))
            .Select(collider => collider.GetComponent<PlayerCombatAI>())
            .Where(ally => ally != null && ally.team == paladin.team && ally.ShieldAmount == 0 && ally != paladin)
            .ToList();
    }

    private PlayerCombatAI FindNearestAlly(Paladin paladin, List<PlayerCombatAI> allies)
    {
        return allies.OrderBy(ally => Vector2.Distance(paladin.transform.position, ally.transform.position))
            .FirstOrDefault();
    }
}
