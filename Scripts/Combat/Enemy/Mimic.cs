using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class Mimic : EnemyCombatAI
{
    [Title("References")]
    public GameObject explodeEffect;
    [Title("Skill Balancing")]
    public float explosionRadius = 3f;
    public float knockbackForce = 5f;
    public float maxKnockbackDistance = 3f;


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
        // 폭발 효과 재생
        if (explodeEffect != null)
        {
            EffectManager.Instance.PlayAnimatedSpriteEffect(explodeEffect, transform.position);
        }

        if (Random.value < 0.5f)
        {
            HealNearbyUnits();
        }
        else
        {
            DamageAndKnockbackNearbyUnits();
        }

        // 기본 Die 메서드 호출

        base.Die();
    }

    private void HealNearbyUnits()
    {
        Collider2D[] nearbyUnits = Physics2D.OverlapCircleAll(transform.position, explosionRadius, LayerMask.GetMask("Character"));
        foreach (Collider2D unitCollider in nearbyUnits)
        {
            BaseCombatAI unit = unitCollider.GetComponent<BaseCombatAI>();
            if (unit != null)
            {
                float healAmount = unit.MaxHP * 0.5f;
                unit.Heal(healAmount);
            }
        }
    }

    private void DamageAndKnockbackNearbyUnits()
    {
        Collider2D[] nearbyUnits = Physics2D.OverlapCircleAll(transform.position, explosionRadius, LayerMask.GetMask("Character"));
        foreach (Collider2D unitCollider in nearbyUnits)
        {
            BaseCombatAI unit = unitCollider.GetComponent<BaseCombatAI>();
            if (unit != null && unit != this)
            {
                float damage = AttackDamage * 3;
                unit.TakeDamage(damage, DamageType.Physical, AttackType.Blunt, false);

                Vector2 knockbackDirection = (unit.transform.position - transform.position).normalized;
                StartCoroutine(ApplyKnockback(unit, knockbackDirection));
            }
        }
    }

    private IEnumerator ApplyKnockback(BaseCombatAI unit, Vector2 knockbackDirection)
    {
        float distanceMoved = 0f;
        Rigidbody2D unitRb = unit.GetComponent<Rigidbody2D>();
        Camera mainCamera = Camera.main;

        while (distanceMoved < maxKnockbackDistance)
        {
            Vector2 oldPosition = unit.transform.position;
            unitRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            yield return new WaitForFixedUpdate();

            Vector2 newPosition = ClampPositionToCameraBounds(unit.transform.position, mainCamera);
            unit.transform.position = newPosition;
            distanceMoved += Vector2.Distance(oldPosition, newPosition);

            if (distanceMoved >= maxKnockbackDistance || oldPosition == newPosition)
            {
                unitRb.velocity = Vector2.zero;
                break;
            }
        }
    }

    private Vector2 ClampPositionToCameraBounds(Vector2 position, Camera camera)
    {
        if (camera == null) return position;

        Vector2 cameraMin = camera.ViewportToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));
        Vector2 cameraMax = camera.ViewportToWorldPoint(new Vector3(1, 1, camera.nearClipPlane));


        float buffer = 1.5f;
        position.x = Mathf.Clamp(position.x, cameraMin.x + buffer, cameraMax.x - buffer);
        position.y = Mathf.Clamp(position.y, cameraMin.y + buffer, cameraMax.y - buffer);

        return position;
    }
}
