using System.Collections;
using UnityEngine;

public class SwordMaster : PlayerCombatAI
{
    private AnimationEventDispatcher animEventDispatcher;
    public bool isSkillAnimationComplete = false;
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
        baseAttackAnimationLength = AnimationData.GetAnimationLength(animator, AnimationData.Attack_NormalHash);
    }

    protected override void InitializeStates()
    {
        var idleState = new IdleState(this);
        var moveState = new MovingState(this);
        var attackState = new AttackingState(this);
        var skillState = new SwordMasterSkillState(this);
        var stunState = new StunState(this);
        var dieState = new DieState(this);
        //AddTransition = A 상태에서 B 상태로 전환
        //AddAnyTransition = 지금 어떤 상태이든 조건을 만족할 시 해당 상태로 전환
        stateMachine.AddTransition(idleState, moveState, new FuncPredicate(() => GameManager.Instance.isCombatStart || CheckTarget()));

        stateMachine.AddTransition(moveState, attackState, new FuncPredicate(() => IsTargetInAttackRange() && CheckTarget()));

        //적이 죽어서 없어지면 자동으로 적을 찾아 이동해 공격
        stateMachine.AddTransition(attackState, moveState,
             new FuncPredicate(() => !IsAnyTargetInAttackRange() || !IsTargetInAttackRange()));


        //스킬 사용 상태 전환
        stateMachine.AddTransition(attackState, skillState,
            new FuncPredicate(() => Mana >= currentSkill.manaCost));

        stateMachine.AddTransition(skillState, attackState,
            new FuncPredicate(() => skillState.IsSkillCompleted() && IsTargetInAttackRange() && CheckTarget()));
        stateMachine.AddTransition(skillState, moveState,
            new FuncPredicate(() => skillState.IsSkillCompleted() && (!IsTargetInAttackRange() || !CheckTarget())));

        stateMachine.AddAnyTransition(dieState, new FuncPredicate(() => CurrentHP <= 0));
        stateMachine.AddTransition(dieState, moveState, new FuncPredicate(() => CurrentHP > 0));

        //스턴 걸렸을 때 상태전환
        stateMachine.AddTransition(stunState, attackState,
            new FuncPredicate(() => !IsStunned && IsTargetInAttackRange() && CheckTarget()));
        stateMachine.AddTransition(stunState, moveState,
            new FuncPredicate(() => !IsStunned && (!IsTargetInAttackRange() || !CheckTarget())));

        //스턴 상태
        stateMachine.AddAnyTransition(stunState, new FuncPredicate(() => IsStunned));
        //기본상태
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
        PlayAnimation(AnimationData.Attack_NormalHash, true);

        // 60% 지점까지 대기
        yield return new WaitForSeconds(animationDuration * 0.52f);

        if (CheckTarget())
        {
            ApplyDamage();
            GainMana(10);
        }

        // 나머지 애니메이션 완료 대기
        yield return new WaitForSeconds(animationDuration * 0.48f);
    }

    private void ApplyDamage()
    {
        (bool isCritical, float damageMultiplier) = CalculateCriticalHit();
        float damage = AttackDamage * damageMultiplier;
        cachedNearestTarget.TakeDamage(damage, DamageType.Physical, AttackType.Slash, isCritical);
    }

    private void OnAnimationComplete(string animationName)
    {
        if (animationName == "5_Skill_Normal")
        {
            isSkillAnimationComplete = true;
        }
    }
}
