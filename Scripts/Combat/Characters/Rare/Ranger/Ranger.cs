using UnityEngine;
using System.Collections;

public class Ranger : PlayerCombatAI
{
    [SerializeField] private int maxLockOnArrowCount = 5;
    private int currentLockOnArrowCount = 1;
    [SerializeField] public GameObject projectilePrefab;
    [SerializeField] public GameObject lockOnProjectilePrefab;
    [SerializeField] public Transform pivotTransform;
    public bool isSKillAnimationComplete = false;
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
        baseAttackAnimationLength = AnimationData.GetAnimationLength(animator, AnimationData.Attack_BowHash);
    }

    public override void OnCombatStart()
    {
        base.OnCombatStart();
        if(currentSkill is LockOn)
        {
            ResetLockOnArrowCount();
        }     
    }

    protected override void InitializeStates()
    {
        var idleState = new IdleState(this);
        var moveState = new MovingState(this);
        var attackState = new AttackingState(this);
        var skillState = new RangerSkillState(this);
        var stunState = new StunState(this);
        var dieState = new DieState(this);

        stateMachine.AddTransition(idleState, moveState, new FuncPredicate(() => GameManager.Instance.isCombatStart || CheckTarget()));
        stateMachine.AddTransition(moveState, attackState, new FuncPredicate(() => IsTargetInAttackRange() && CheckTarget()));

        stateMachine.AddTransition(attackState, moveState,
                    new FuncPredicate(() => !IsAnyTargetInAttackRange() || !IsTargetInAttackRange()));
        stateMachine.AddTransition(attackState, skillState,
             new FuncPredicate(() => Mana >= currentSkill.manaCost));
        stateMachine.AddTransition(skillState, attackState,
           new FuncPredicate(() => skillState.IsSkillCompleted() && IsTargetInAttackRange() && CheckTarget()));
        stateMachine.AddTransition(skillState, moveState,
            new FuncPredicate(() => skillState.IsSkillCompleted() && (!IsTargetInAttackRange() || !CheckTarget())));

        //스턴 걸렸을 때 상태전환
        stateMachine.AddTransition(stunState, attackState,
            new FuncPredicate(() => !IsStunned && IsTargetInAttackRange() && CheckTarget()));
        stateMachine.AddTransition(stunState, moveState,
            new FuncPredicate(() => !IsStunned && (!IsTargetInAttackRange() || !CheckTarget())));


        stateMachine.AddAnyTransition(dieState, new FuncPredicate(() => CurrentHP <= 0));
        stateMachine.SetState(idleState);
        stateMachine.AddAnyTransition(idleState, new FuncPredicate(() => !GameManager.Instance.isCombatStart));
    }

    public override void UseSkill()
    {
        currentSkill.ExecuteSkill(this);
        Mana -= currentSkill.manaCost;
        UpdateMPBar();
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
        PlayAnimation(AnimationData.Attack_BowHash, true);


        yield return new WaitForSeconds(animationDuration * 0.7f);

        if (CheckTarget())
        {
            SoundManager.Instance.PlaySound("SFX_RangeAttack");
            LaunchProjectile();
            GainMana(10);
        }


        yield return new WaitForSeconds(animationDuration * 0.3f);

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

    public void UpdateTargetIfOutOfRange()
    {
        if (cachedNearestTarget == null || !IsTargetInAttackRange())
        {
            BaseCombatAI newTarget = FindNearestTargetInRange();
            if (newTarget != null)
            {
                cachedNearestTarget = newTarget;
            }
        }
    }

    private void OnAnimationComplete(string animationName)
    {
        if (animationName == "5_Skill_Bow")
        {
            isSKillAnimationComplete = true;
        }
    }

    public int GetLockOnArrowCount()
    {
        return currentLockOnArrowCount;
    }

    public void IncreaseLockOnArrowCount()
    {
        if (currentLockOnArrowCount < maxLockOnArrowCount)
        {
            currentLockOnArrowCount++;
        }
    }

    // 필요한 경우 화살 개수를 리셋하는 메서드
    public void ResetLockOnArrowCount()
    {
        currentLockOnArrowCount = 1;
    }
}
