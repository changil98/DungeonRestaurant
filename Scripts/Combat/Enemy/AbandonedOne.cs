using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbandonedOne : EnemyCombatAI
{
    [Title("References")]
    public GameObject projectilePrefab;

    [Title("Skill Balancing")]
    public float areaEffectRadius = 3f;
    public float areaEffectDamageMultiplier = 0.1f;
    public float areaEffectInterval = 1f;

    private BaseCombatAI forcedTarget;
    private float forcedTargetEndTime;
    private bool hasEnemyEnteredRange = false;
    private Coroutine areaEffectCoroutine;


    protected override void Awake()
    {
        base.Awake();
        areaEffectCoroutine = StartCoroutine(ApplyAreaEffect());

    }
    protected override void InitializeStates()
    {
        var idleState = new IdleState(this);
        var moveState = new AbandonedOne_MovingState(this);
        var attackState = new EnemyAttackingState(this);
        var stunState = new StunState(this);
        var dieState = new DieState(this);

        
        stateMachine.AddTransition(idleState, moveState, new FuncPredicate(() => GameManager.Instance.isCombatStart));

        
        stateMachine.AddTransition(moveState, attackState, new FuncPredicate(() =>
        {
            if (IsTargetInAttackRange() && CheckTarget())
            {
                hasEnemyEnteredRange = true;
                return true;
            }
            return false;
        }));

        
        stateMachine.AddTransition(attackState, moveState,
            new FuncPredicate(() => !IsTargetInAttackRange() || !CheckTarget()));

        
        stateMachine.AddAnyTransition(dieState, new FuncPredicate(() => CurrentHP <= 0));

        
        stateMachine.AddTransition(stunState, attackState,
            new FuncPredicate(() => !IsStunned && IsTargetInAttackRange() && CheckTarget()));
        stateMachine.AddTransition(stunState, moveState,
            new FuncPredicate(() => !IsStunned && (!IsTargetInAttackRange() || !CheckTarget())));
        stateMachine.AddAnyTransition(stunState, new FuncPredicate(() => IsStunned));

        
        stateMachine.SetState(idleState);
        stateMachine.AddAnyTransition(idleState, new FuncPredicate(() => !GameManager.Instance.isCombatStart));
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
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Projectile projectileComponent = projectile.GetComponent<Projectile>();
            if (projectileComponent != null)
            {
                projectileComponent.Initialize(cachedNearestTarget.transform, AttackDamage, this);
            }
        }
    }

    private IEnumerator ApplyAreaEffect()
    {
        while (true)
        {
            yield return new WaitForSeconds(areaEffectInterval);

            Collider2D[] nearbyUnits = Physics2D.OverlapCircleAll(transform.position, areaEffectRadius, LayerMask.GetMask("Character"));
            foreach (Collider2D unitCollider in nearbyUnits)
            {
                BaseCombatAI unit = unitCollider.GetComponent<BaseCombatAI>();
                if (unit != null && unit.team != this.team)
                {
                    float damage = AttackDamage * areaEffectDamageMultiplier;
                    unit.TakeDamage(damage, DamageType.Magical, AttackType.Blunt, false);
                }
            }
        }
    }

    public override void Die()
    {
        if (areaEffectCoroutine != null)
        {
            StopCoroutine(areaEffectCoroutine);
        }
        base.Die();
    }

    public bool HasEnemyEnteredRange()
    {
        return hasEnemyEnteredRange;
    }


}
