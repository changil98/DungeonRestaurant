using System.Collections;
using UnityEngine;

public class Rogue : PlayerCombatAI
{
    public Transform ActiveSkillTransform;
    private AnimationEventDispatcher animEventDispatcher;
    public bool isSkillAnimationComplete = false;

    private bool isAccelerationActive = false;
    private float DodgeChange = 0.5f;

    [SerializeField] private float untargetableAlpha = 0.5f; // 반투명 상태의 알파값
    private SpriteRenderer[] spriteRenderers;

    private bool useFirstTime = false;
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
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        baseAttackAnimationLength = AnimationData.GetAnimationLength(animator, AnimationData.Attack_NormalHash);
    }


    //랜덤 스킬 배정
    public override void OnCombatStart() //나중에 모험가 모집 버튼이나 그런 걸 눌렀을 때 등록하도록
    {
        base.OnCombatStart();
        if(currentSkill is Sneak && !useFirstTime)
        {
            GainMana(90);
            useFirstTime = true;
        }
    }

    protected override void InitializeStates()
    {
        var idleState = new IdleState(this);
        var moveState = new MovingState(this);
        var attackState = new AttackingState(this);
        var skillState = new RogueSkillState(this);
        var stunState = new StunState(this);
        var dieState = new DieState(this);
        //AddTransition = A 상태에서 B 상태로 전환
        //AddAnyTransition = 지금 어떤 상태이든 조건을 만족할 시 해당 상태로 전환
        stateMachine.AddTransition(idleState, moveState, new FuncPredicate(() => GameManager.Instance.isCombatStart));

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

    public void BecomeUntargetable(float duration)
    {
        StartCoroutine(UntargetableCoroutine(duration));
    }

    private IEnumerator UntargetableCoroutine(float duration)
    {
        SetAlpha(untargetableAlpha);
        isUntargetable = true;    
        yield return new WaitForSeconds(duration);
        isUntargetable = false;  
        SetAlpha(1f);
    }

    public override void Heal(float amount)
    {
        base.Heal(amount);
    }

    public void SetAccelerationActive(bool active)
    {
        isAccelerationActive = active;
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
        cachedNearestTarget.TakeDamage(damage, DamageType.Physical, AttackType.Slash,isCritical);
    }


    public override void TakeDamage(float damage, DamageType damageType,AttackType attackType, bool isCritical)
    {

        if(isAccelerationActive && Random.value < DodgeChange && FloatingTextManager.Instance.isFloatingTextEnabled == true)
        {
            FloatingTextManager.Instance.ShowFloatingText("회피함!", transform.position, Color.yellow);
            return; //회피 시 대미지 무시
        }

        base.TakeDamage(damage, damageType, attackType, isCritical);
    }

    private void OnAnimationComplete(string animationName)
    {
        if (animationName == "5_Skill_Normal")
        {
            isSkillAnimationComplete = true;
        }
    }

    private void SetAlpha(float alpha)
    {
        foreach (SpriteRenderer renderer in spriteRenderers)
        {
            Color color = renderer.color;
            color.a = alpha;
            renderer.color = color;
        }
    }
}
