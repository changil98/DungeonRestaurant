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
        //AddTransition = A ���¿��� B ���·� ��ȯ
        //AddAnyTransition = ���� � �����̵� ������ ������ �� �ش� ���·� ��ȯ
        stateMachine.AddTransition(idleState, moveState, new FuncPredicate(() => GameManager.Instance.isCombatStart || CheckTarget()));

        stateMachine.AddTransition(moveState, attackState, new FuncPredicate(() => IsTargetInAttackRange() && CheckTarget()));

        //���� �׾ �������� �ڵ����� ���� ã�� �̵��� ����
        stateMachine.AddTransition(attackState, moveState,
             new FuncPredicate(() => !IsAnyTargetInAttackRange() || !IsTargetInAttackRange()));


        //��ų ��� ���� ��ȯ
        stateMachine.AddTransition(attackState, skillState,
            new FuncPredicate(() => Mana >= currentSkill.manaCost));

        stateMachine.AddTransition(skillState, attackState,
            new FuncPredicate(() => skillState.IsSkillCompleted() && IsTargetInAttackRange() && CheckTarget()));
        stateMachine.AddTransition(skillState, moveState,
            new FuncPredicate(() => skillState.IsSkillCompleted() && (!IsTargetInAttackRange() || !CheckTarget())));

        stateMachine.AddAnyTransition(dieState, new FuncPredicate(() => CurrentHP <= 0));
        stateMachine.AddTransition(dieState, moveState, new FuncPredicate(() => CurrentHP > 0));

        //���� �ɷ��� �� ������ȯ
        stateMachine.AddTransition(stunState, attackState,
            new FuncPredicate(() => !IsStunned && IsTargetInAttackRange() && CheckTarget()));
        stateMachine.AddTransition(stunState, moveState,
            new FuncPredicate(() => !IsStunned && (!IsTargetInAttackRange() || !CheckTarget())));

        //���� ����
        stateMachine.AddAnyTransition(stunState, new FuncPredicate(() => IsStunned));
        //�⺻����
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

        // �ִϸ��̼� �ӵ� ����
        animator.SetFloat("AttackSpeed", attackSpeed);
        PlayAnimation(AnimationData.Attack_NormalHash, true);

        // 60% �������� ���
        yield return new WaitForSeconds(animationDuration * 0.52f);

        if (CheckTarget())
        {
            ApplyDamage();
            GainMana(10);
        }

        // ������ �ִϸ��̼� �Ϸ� ���
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
