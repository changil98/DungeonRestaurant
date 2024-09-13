using UnityEngine;
using System.Collections;

public class Archer : PlayerCombatAI
{
    [SerializeField] public GameObject skillProjectilePrefab;
    [SerializeField] public GameObject projectilePrefab;
    [SerializeField] public Transform pivotTransform;
    [SerializeField] private float skillSpreadAngle = 45f;
    
    public bool isSKillAnimationComplete = false;
    
    private AnimationEventDispatcher animEventDispatcher;
    private int attackCount = 0;
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
    }

    protected override void InitializeStates()
    {
        var idleState = new IdleState(this);
        var moveState = new MovingState(this);
        var attackState = new AttackingState(this);
        var skillState = new ArcherSkillState(this);
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
        stateMachine.AddTransition(dieState, moveState, new FuncPredicate(()=> CurrentHP > 0));

        stateMachine.SetState(idleState);
        stateMachine.AddAnyTransition(idleState, new FuncPredicate(() => !GameManager.Instance.isCombatStart));
    }

    protected override void ActivatePassiveSkill()
    {
        attackCount = 0;
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
        if(animationName == "5_Skill_Bow")
        {
            isSKillAnimationComplete = true;
        }
    }
}
