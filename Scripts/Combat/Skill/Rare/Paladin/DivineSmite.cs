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

        // ��󿡰� �������� ������ ������Ŵ
        if (paladin.cachedNearestTarget != null)
        {
            float damage = paladin.AttackDamage * damageMultiplier;
            paladin.cachedNearestTarget.TakeDamage(damage, DamageType.Physical, AttackType.Blunt, false);
            paladin.cachedNearestTarget.ApplyStun(stunDuration);
        }

        yield return new WaitForSeconds(skillAnimationLength * 0.42f);

        // ��ȣ�� �ο�
        float shieldAmount = paladin.MaxHP * shieldMultiplier;

        if (isFirstUse || paladin.ShieldAmount == 0)
        {
            // ù ����̰ų� �ȶ�� �ڽſ��� ��ȣ���� ���� ���
            paladin.AddShield(shieldAmount);
            isFirstUse = false;
        }
        else
        {
            // �ȶ�򿡰� ��ȣ���� �ִ� ���, ��ȣ���� ���� ���� ����� �Ʊ����� �ο�
            List<PlayerCombatAI> allies = FindAlliesWithoutShield(paladin);
            PlayerCombatAI targetAlly = FindNearestAlly(paladin, allies);

            if (targetAlly != null)
            {
                targetAlly.AddShield(shieldAmount);
            }
            else
            {
                // ��� �Ʊ����� ��ȣ���� �ִٸ� �ڽſ��� �ο� (��ȣ�� ����)
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
