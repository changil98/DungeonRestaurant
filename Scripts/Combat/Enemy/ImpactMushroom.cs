using Sirenix.OdinInspector;
using UnityEngine;

public class ImpactMushroom : EnemyCombatAI
{
    [Title("References")]
    public GameObject projectilePrefab;
    public GameObject explodeEffect;
    [Title("Skill Balancing")]
    public float explosionRadius = 3f;
    public float stunDurationOnDie = 1f;

    private BaseCombatAI forcedTarget;
    private float forcedTargetEndTime;
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void InitializeStates()
    {
        var idleState = new IdleState(this);
        var moveState = new EnemyMovingState(this);
        var attackState = new EnemyAttackingState(this);
        var stunState = new StunState(this);
        var dieState = new DieState(this);

        //전투 시작 시 상태 전환
        stateMachine.AddTransition(idleState, moveState, new FuncPredicate(() => GameManager.Instance.isCombatStart));
        //사거리 내 적이 없다면 가까운 적을 추적
        stateMachine.AddTransition(moveState, attackState, new FuncPredicate(() => IsTargetInAttackRange() && CheckTarget()));
        stateMachine.AddTransition(attackState, moveState,
        new FuncPredicate(() => !IsTargetInAttackRange() || !CheckTarget()));

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
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Projectile projectileComponent = projectile.GetComponent<Projectile>();
            if (projectileComponent != null)
            {
                projectileComponent.Initialize(cachedNearestTarget.transform, AttackDamage, this);
            }
        }

    }

    public override void Die()
    {
        // 폭발 효과 재생
        if (explodeEffect != null)
        {
            EffectManager.Instance.PlayAnimatedSpriteEffect(explodeEffect, transform.position);
        }

        // 주변 적에게 기절 효과 적용
        ApplyStunToNearbyEnemies();

        // 기본 Die 메서드 호출

        base.Die();
    }

    private void ApplyStunToNearbyEnemies()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, LayerMask.GetMask("Character"));

        foreach (var hitCollider in hitColliders)
        {
            BaseCombatAI enemyAI = hitCollider.GetComponent<BaseCombatAI>();
            if (enemyAI != null && enemyAI != this && enemyAI.team != this.team)
            {
                enemyAI.ApplyStun(stunDurationOnDie);
            }
        }
    }


}
