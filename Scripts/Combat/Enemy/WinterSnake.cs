using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinterSnake : EnemyCombatAI
{
    [Title("WinterSnake Skill")]
    [SerializeField] private float pushInterval = 10f;
    [SerializeField] private float pushForce = 5f;
    [SerializeField] private float maxPushDistance = 3f;
    [SerializeField] private float speedReductionDuration = 1f;
    [SerializeField] private float speedReductionAmount = 0.5f;
    [SerializeField] private float pushRadius = 5f;
    [SerializeField] private GameObject skillEffect;


    private BaseCombatAI forcedTarget;
    private float forcedTargetEndTime;
    private bool hasEnemyEnteredRange = false;
    private Coroutine pushCoroutine;

    private Dictionary<PlayerCombatAI, Coroutine> activeSpeedReductions = new Dictionary<PlayerCombatAI, Coroutine>();

    protected override void Awake()
    {
        base.Awake();
        pushCoroutine = StartCoroutine(PushRoutine());
    }


    protected override void InitializeStates()
    {
        var idleState = new IdleState(this);
        var moveState = new WinterSnake_MovingState(this);
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

    private IEnumerator PushRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(pushInterval);
            PushAlliesAway();
        }
    }

    private void PushAlliesAway()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, pushRadius, LayerMask.GetMask("Character"));

        foreach (Collider2D collider in colliders)
        {
            PlayerCombatAI ally = collider.GetComponent<PlayerCombatAI>();
            if (ally != null && ally.team == Team.Ally)
            {
                Vector2 pushDirection = (ally.transform.position - transform.position).normalized;
                StartCoroutine(ApplyPushAndSpeedReduction(ally, pushDirection));
            }
        }
    }

    private IEnumerator ApplyPushAndSpeedReduction(PlayerCombatAI ally, Vector2 pushDirection)
    {
        EffectManager.Instance.PlayAnimatedSpriteEffect(skillEffect, transform.position, false);
        float distanceMoved = 0f;
        Vector3 startPosition = ally.transform.position;

        while (distanceMoved < maxPushDistance)
        {
            Vector3 newPosition = ally.transform.position + (Vector3)(pushDirection * pushForce * Time.fixedDeltaTime);
            newPosition = ClampPositionToCameraBounds(newPosition, Camera.main);

            ally.transform.position = newPosition;
            distanceMoved = Vector3.Distance(startPosition, newPosition);

            if (distanceMoved >= maxPushDistance)
            {
                break;
            }

            yield return new WaitForFixedUpdate();
        }

        // 이동 속도 감소 효과 적용
        if (!activeSpeedReductions.ContainsKey(ally))
        {
            float originalSpeed = ally.GetMovementSpeed();
            ally.DecreaseMovementSpeed(originalSpeed * speedReductionAmount);
            activeSpeedReductions[ally] = StartCoroutine(RemoveSpeedReduction(ally, originalSpeed * speedReductionAmount));
        }
    }

    private IEnumerator RemoveSpeedReduction(PlayerCombatAI ally, float amountToRestore)
    {
        yield return new WaitForSeconds(speedReductionDuration);

        if (ally != null && ally.gameObject.activeSelf)
        {
            ally.IncreaseMovementSpeed(amountToRestore);
        }

        activeSpeedReductions.Remove(ally);
    }

    private Vector3 ClampPositionToCameraBounds(Vector3 position, Camera camera)
    {
        Vector3 cameraMin = camera.ViewportToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));
        Vector3 cameraMax = camera.ViewportToWorldPoint(new Vector3(1, 1, camera.nearClipPlane));

        float buffer = 1.5f;
        float clampedX = Mathf.Clamp(position.x, cameraMin.x + buffer, cameraMax.x - buffer);
        float clampedY = Mathf.Clamp(position.y, cameraMin.y + buffer, cameraMax.y - buffer);

        return new Vector3(clampedX, clampedY, position.z);
    }

    public bool HasEnemyEnteredRange()
    {
        return hasEnemyEnteredRange;
    }

    public override void Die()
    {
        base.Die();
        if (pushCoroutine != null)
        {
            StopCoroutine(pushCoroutine);
        }
    }
}
