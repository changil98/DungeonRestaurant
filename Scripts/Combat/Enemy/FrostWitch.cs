using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostWitch : EnemyCombatAI
{
    [Title("References")]
    public GameObject projectilePrefab;

    [Title("Frost Curse Settings")]
    [SerializeField] private float curseCooldown = 10f;
    [SerializeField] private float curseDuration = 10f;
    [SerializeField] private float attackSpeedReductionAmount = 0.5f;
    [SerializeField] private GameObject skillEffect;

    private BaseCombatAI forcedTarget;
    private float forcedTargetEndTime;
    private bool hasEnemyEnteredRange = false;
    private Coroutine frostCurseCoroutine;
    private Dictionary<PlayerCombatAI, Coroutine> activeEffects = new Dictionary<PlayerCombatAI, Coroutine>();


    protected override void Awake()
    {
        base.Awake();
        frostCurseCoroutine = StartCoroutine(FrostCurseRoutine());
    }

    protected override void InitializeStates()
    {
        var idleState = new IdleState(this);
        var moveState = new FrostWitch_MovingState(this);
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

    private IEnumerator FrostCurseRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(curseCooldown);
            ApplyFrostCurse();
        }
    }

    private void ApplyFrostCurse()
    {
        PlayerCombatAI[] alliedCharacters = FindObjectsOfType<PlayerCombatAI>();
        if (alliedCharacters.Length > 0)
        {
            int randomIndex = Random.Range(0, alliedCharacters.Length);
            PlayerCombatAI target = alliedCharacters[randomIndex];

            if (activeEffects.TryGetValue(target, out Coroutine existingCoroutine))
            {
                if (existingCoroutine != null)
                {
                    StopCoroutine(existingCoroutine);
                }
                activeEffects.Remove(target);
            }

            Coroutine newCoroutine = StartCoroutine(ApplyAttackSpeedReduction(target));
            if (newCoroutine != null)
            {
                activeEffects[target] = newCoroutine;
            }
        }
    }

    private IEnumerator ApplyAttackSpeedReduction(PlayerCombatAI target)
    {
        if (target == null)
        {
            yield break;
        }

        Vector3 effectPos = target.transform.position;
        Vector3 offset = effectPos - target.transform.position;
        offset.y += 2f;

        EffectManager.Instance.PlayFollowingAnimatedSpriteEffect(skillEffect, target.transform, offset, false);

        float originalAttackSpeed = target.AttackSpeed;
        float reducedAttackSpeed = originalAttackSpeed * (1 - attackSpeedReductionAmount);

        target.AttackSpeed = reducedAttackSpeed;
        target.BaseAttackInterval = 1f / reducedAttackSpeed;

       

        yield return new WaitForSeconds(curseDuration);

        if (target != null)
        {
            target.AttackSpeed = originalAttackSpeed;
            target.BaseAttackInterval = 1f / originalAttackSpeed;
            
        }

        activeEffects.Remove(target);
    }



    public override void Die()
    {
        // 기존의 Die 로직 실행
        base.Die();

        // FrostCurse 코루틴 정지
        if (frostCurseCoroutine != null)
        {
            StopCoroutine(frostCurseCoroutine);
            frostCurseCoroutine = null;
        }

        // 모든 활성 효과 제거
        RemoveAllActiveEffects();

        // 추가적인 정리 작업이 필요하다면 여기에 구현
    }



    private void RemoveAllActiveEffects()
    {
        foreach (var effect in activeEffects)
        {
            if (effect.Value != null)
            {
                StopCoroutine(effect.Value);
            }
            if (effect.Key != null)
            {
                effect.Key.AttackSpeed = 1f / effect.Key.BaseAttackInterval;
            }
        }
        activeEffects.Clear();
    }


    public bool HasEnemyEnteredRange()
    {
        return hasEnemyEnteredRange;
    }
}
