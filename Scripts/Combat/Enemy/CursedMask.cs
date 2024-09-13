using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursedMask : EnemyCombatAI
{
    [SerializeField] private float auraRadius = 2.5f; // 오라의 범위
    [SerializeField] private float attackSpeedReduction = 0.15f; // 15% 공격속도 감소


    private BaseCombatAI forcedTarget;
    private float forcedTargetEndTime;

    private HashSet<PlayerCombatAI> affectedAllies = new HashSet<PlayerCombatAI>();
    private static HashSet<PlayerCombatAI> globalAffectedAllies = new HashSet<PlayerCombatAI>();

    protected override void Awake()
    {
        base.Awake();
        StartCoroutine(AuraEffect());
    }

    protected override void InitializeStates()
    {
        var idleState = new IdleState(this);
        var moveState = new EnemyMovingState(this);
        var attackState = new EnemyAttackingState(this);
        var stunState = new StunState(this);
        var dieState = new DieState(this);

        //전투 시작 시 상태 전환
        stateMachine.AddTransition(idleState, moveState, new FuncPredicate(() => GameManager.Instance.isCombatStart || CheckTarget()));
        //사거리 내 적이 없다면 가까운 적을 추적
        stateMachine.AddTransition(moveState, attackState, new FuncPredicate(() => IsTargetInAttackRange() && CheckTarget()));
        stateMachine.AddTransition(attackState, moveState,
        new FuncPredicate(() => !IsAnyTargetInAttackRange() || !IsTargetInAttackRange()));

        stateMachine.AddAnyTransition(dieState, new FuncPredicate(() => CurrentHP <= 0));

        //스턴 상태 후 상태 전환
        stateMachine.AddTransition(stunState, attackState,
            new FuncPredicate(() => !IsStunned && IsTargetInAttackRange() && CheckTarget()));
        stateMachine.AddTransition(stunState, moveState,
            new FuncPredicate(() => !IsStunned && (!IsTargetInAttackRange() || !CheckTarget())));
        //스턴 상태
        stateMachine.AddAnyTransition(stunState, new FuncPredicate(() => IsStunned));

        //기본 상태
        stateMachine.SetState(idleState);
        stateMachine.AddAnyTransition(idleState, new FuncPredicate(() => !GameManager.Instance.isCombatStart));
    }

    public override void SetForcedTarget(BaseCombatAI target, float duration)
    {
        forcedTarget = target;
        forcedTargetEndTime = Time.time + duration;
    }

    // UpdateNearestTarget 메서드 오버라이드
    public override void UpdateNearestTarget()
    {
        if (forcedTarget != null && forcedTarget.gameObject.activeSelf)
        {
            cachedNearestTarget = forcedTarget;
        }
        else
        {
            base.UpdateNearestTarget();
            forcedTarget = null;
        }
    }

    public override void Attack()
    {
        if (CheckTarget())
        {
            PlayAnimation(AnimationData.Attack_NormalHash, true);
            cachedNearestTarget.TakeDamage(AttackDamage, DamageType.Physical, AttackType.Blunt, false);
        }
    }

    public override void Die()
    {
        base.Die();
        RemoveAuraEffectFromAllAffectedAllies();
    }

    private IEnumerator AuraEffect()
    {
        while (true)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, auraRadius, LayerMask.GetMask("Character"));

            foreach (Collider2D collider in colliders)
            {
                PlayerCombatAI ally = collider.GetComponent<PlayerCombatAI>();
                if (ally != null && ally.team == Team.Ally)
                {
                    if (!globalAffectedAllies.Contains(ally))
                    {
                        ally.DecreaseAttackSpeed(attackSpeedReduction);
                        affectedAllies.Add(ally);
                        globalAffectedAllies.Add(ally);
                    }
                }
            }

            
            affectedAllies.RemoveWhere(ally =>
            {
                if (ally == null || Vector2.Distance(transform.position, ally.transform.position) > auraRadius)
                {
                    ally?.IncreaseAttackSpeed(attackSpeedReduction);
                    globalAffectedAllies.Remove(ally);
                    return true;
                }
                return false;
            });

            yield return new WaitForSeconds(0.5f);
        }
    }

    private void RemoveAuraEffectFromAllAffectedAllies()
    {
        foreach (var ally in affectedAllies)
        {
            if (ally != null)
            {
                ally.IncreaseAttackSpeed(attackSpeedReduction);
                globalAffectedAllies.Remove(ally);
            }
        }
        affectedAllies.Clear();
    }



}
