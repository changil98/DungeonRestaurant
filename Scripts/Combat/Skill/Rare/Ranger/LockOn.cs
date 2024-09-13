using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;

[CreateAssetMenu(fileName = "LockOn", menuName = "Skills/Ranger/LockOn")]
public class LockOn : BaseSkill
{
    [SerializeField] private float damageMultiplier = 0.5f;
    [SerializeField] private float totalShootDuration = 0.5f;
    [SerializeField] GameObject LockOnVFX1;
    [SerializeField] GameObject lockOnVFX2;
    private float skillAnimationLength;

    public override void ExecuteSkill(BaseCombatAI character)
    {
        if (character is Ranger ranger)
        {
            ranger.StartCoroutine(ExecuteSkillCoroutine(ranger));
        }
    }

    private IEnumerator ExecuteSkillCoroutine(Ranger ranger)
    {
        Animator animator = ranger.GetComponentInChildren<Animator>();
        ranger.PlayAnimation(AnimationData.Skill_BowHash);
        
        // 애니메이션 길이 구하기
        skillAnimationLength = AnimationData.GetAnimationLength(animator, AnimationData.Skill_BowHash);

        // 70% 지점까지 대기
        yield return new WaitForSeconds(skillAnimationLength * 0.7f);

        // 70% 지점에서 효과 재생 및 스킬 실행
        SoundManager.Instance.PlaySound("SFX_RangeAttack");
        EffectManager.Instance.PlayAnimatedSpriteEffect(lockOnVFX2, ranger.pivotTransform.position, ranger.IsFacingRight);
        ranger.StartCoroutine(PerformLockOn(ranger));

        // 나머지 30% 대기
        yield return new WaitForSeconds(skillAnimationLength * 0.3f);
    }

    private IEnumerator PerformLockOn(Ranger ranger)
    {
        BaseCombatAI target = FindLowestHPTarget(ranger);
        if (target == null) yield break;

        Vector3 effectPos = target.transform.position;
        Vector3 offset = effectPos - target.transform.position;
        offset.y += 2f;
        EffectManager.Instance.PlayFollowingAnimatedSpriteEffect(LockOnVFX1, target.transform, offset, target.IsFacingRight);

        int arrowCount = ranger.GetLockOnArrowCount();
        float delayBetweenShots = totalShootDuration / arrowCount;
        for (int i = 0; i < arrowCount; i++)
        {
            ShootArrow(ranger, target);
            yield return new WaitForSeconds(delayBetweenShots);
        }
        ranger.IncreaseLockOnArrowCount();
    }

    private void ShootArrow(Ranger ranger, BaseCombatAI target)
    {
        float damage = ranger.AttackDamage * damageMultiplier;
        GameObject projectile = Object.Instantiate(ranger.lockOnProjectilePrefab, ranger.pivotTransform.position, Quaternion.identity);
        RangerProjectile projectileComponent = projectile.GetComponent<RangerProjectile>();
        if (projectileComponent != null)
        {
            projectileComponent.InitializeLockOnProjectile(target, damage, ranger);
        }
    }

    private BaseCombatAI FindLowestHPTarget(Ranger ranger)
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(ranger.transform.position, 50f, LayerMask.GetMask("Character"));
        BaseCombatAI lowestHPEnemy = null;
        float lowestHP = float.MaxValue;
        foreach (Collider2D enemyCollider in enemies)
        {
            BaseCombatAI enemy = enemyCollider.GetComponent<BaseCombatAI>();
            if (enemy != null && enemy.team != ranger.team)
            {
                if (enemy.CurrentHP < lowestHP)
                {
                    lowestHP = enemy.CurrentHP;
                    lowestHPEnemy = enemy;
                }
            }
        }
        return lowestHPEnemy;
    }
}