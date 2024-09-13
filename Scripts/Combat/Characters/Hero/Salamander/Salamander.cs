using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Salamander : PlayerCombatAI
{
    public GameObject projectilePrefab;
    public Transform pivotTransform;
    public bool isSkillAnimationComplete = false;

    private AnimationEventDispatcher animEventDispatcher;
    private Coroutine attackCoroutine;
    private float baseAttackAnimationLength;

    protected override void Awake()
    {
        base.Awake();
        animEventDispatcher = GetComponentInChildren<AnimationEventDispatcher>();
        if (animEventDispatcher != null)
        {
            animEventDispatcher.OnAnimationComplete.AddListener(OnAnimationComplete);
        }
        baseAttackAnimationLength = AnimationData.GetAnimationLength(animator, AnimationData.Attack_MagicHash);
    }

    public override void OnCombatStart()
    {
        base.OnCombatStart();
    }

    protected override void InitializeStates()
    {
        var idleState = new IdleState(this);
        var moveState = new MovingState(this);
        var attackState = new AttackingState(this);
        var skillState = new SalamanderSkillState(this);
        var stunState = new StunState(this);
        var dieState = new DieState(this);

        stateMachine.AddTransition(idleState, moveState, new FuncPredicate(() => GameManager.Instance.isCombatStart));
        stateMachine.AddTransition(moveState, attackState, new FuncPredicate(() =>
        IsTargetInAttackRange() && CheckTarget()));

        stateMachine.AddTransition(attackState, moveState,
            new FuncPredicate(() => !IsAnyTargetInAttackRange() || !IsTargetInAttackRange()));

        stateMachine.AddTransition(attackState, skillState,
             new FuncPredicate(() => Mana >= currentSkill.manaCost));
        stateMachine.AddTransition(skillState, attackState,
           new FuncPredicate(() => skillState.IsSkillCompleted() && IsTargetInAttackRange() && CheckTarget()));
        stateMachine.AddTransition(skillState, moveState,
            new FuncPredicate(() => skillState.IsSkillCompleted() && (!IsTargetInAttackRange() || !CheckTarget())));

        stateMachine.AddAnyTransition(dieState, new FuncPredicate(() => CurrentHP <= 0));
        stateMachine.AddTransition(dieState, moveState, new FuncPredicate(() => CurrentHP > 0));

        stateMachine.SetState(idleState);
        stateMachine.AddAnyTransition(idleState, new FuncPredicate(() => !GameManager.Instance.isCombatStart));
    }

    public override void UseSkill()
    {
        currentSkill.ExecuteSkill(this);
        Mana -= currentSkill.manaCost;
    }

    public override void Attack()
    {
        if (CheckTarget())
        {
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
            }
            attackCoroutine = StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        float attackSpeed = CalculateFinalAttackSpeed();
        float animationDuration = baseAttackAnimationLength / attackSpeed;

        // 애니메이션 속도 조절
        animator.SetFloat("AttackSpeed", attackSpeed);
        PlayAnimation(AnimationData.Attack_MagicHash, true);


        yield return new WaitForSeconds(animationDuration * 0.6f);

        if (CheckTarget())
        {
            LaunchProjectile();
            GainMana(10);
        }


        yield return new WaitForSeconds(animationDuration * 0.4f);


    }

    private void LaunchProjectile()
    {
        (bool isCritical, float damageMultiplier) = CalculateCriticalHit();
        float damage = AttackDamage * damageMultiplier;

        GameObject projectile = Instantiate(projectilePrefab, pivotTransform.position, Quaternion.identity);
        Projectile projectileComponent = projectile.GetComponent<Projectile>();
        if (projectileComponent != null)
        {
            projectileComponent.Initialize(cachedNearestTarget.transform, damage, this, isCritical);
        }
    }


    private void OnAnimationComplete(string animationName)
    {
        if (animationName == "5_Skill_Magic")
        {
            isSkillAnimationComplete = true;
        }
    }
}
