using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using System;


public enum Team { Ally, Enemy }

public abstract class BaseCombatAI : MonoBehaviour
{
    //가장 가까운 적을 찾기
    //공격 사거리 안에 적을 감지하기
    //일반 공격은 원거리, 근거리 : 캐릭터마다 공격방식이 다르니까 따로 구현

    //나중에 따로 데이터로 받을 예정
    [Title("Stat")]
    private float currentHP = 1;
    [ShowInInspector]
    public float CurrentHP
    {
        get => currentHP;
        set
        {
            currentHP = value;
            OnHPChange?.Invoke();
        }
    }
    [ShowInInspector] public float MaxHP { get; set; }
    private float mana;
    [ShowInInspector] public float Mana
    {
        get => mana;
        set
        {
            mana = value;
            OnManaChange?.Invoke();
        }
    }
    [ShowInInspector] public float MaxMana { get; set; }
    [ShowInInspector] public float AttackDamage { get; set; }
    [ShowInInspector] public float Def { get; set; }
    protected float originalDef;
    [ShowInInspector] public float MovementSpeed { get; set; }
    [ShowInInspector] public float AttackSpeed { get; set; }
    [ShowInInspector] public float AttackRange { get; set; }
    public float BaseAttackInterval { get; set; }
    public float AttackSpeedIncrease { get; set; }
    public float AttackSpeedDecrease { get; set; }
    [ShowInInspector] public float Resistance { get; set; }
    [ShowInInspector] public float CriticalPersent { get; set; }
    [ShowInInspector] public float CriticalDamage { get; set; }
    public BaseSkill currentSkill;

    public delegate void OnDataChange();
    public event OnDataChange OnHPChange;
    public event OnDataChange OnManaChange;
    public event OnDataChange OnShieldChange;

    [Title("Collider Objects")]
    [SerializeField] private GameObject hitBoxObject;

    [Title("FootStep Effect")]
    public GameObject footStepEffect;
    [Title("Stun Effect")]
    public ParticleSystem stunEffect;


    [Title("Character UI References")]
    public Image HPBar;
    public Image MPBar;
    public Image ShieldBar;
    public float shieldAmount;
    public float ShieldAmount
    {
        get => shieldAmount;
        set
        {
            shieldAmount = value;
            OnShieldChange?.Invoke();
        }
    }

    protected StateMachine stateMachine;
    protected Animator animator;
    [EnumToggleButtons]
    public Team team;
    public BaseCombatAI cachedNearestTarget;
    private float lastTargetUpdateTime;
    private const float TARGET_UPDATE_INTERVAL = 0.2f;
    protected const int MAX_ENEMIES = 12;
    protected Collider2D[] colliderResults;
    SpriteRenderer spriteRenderer;
    protected CapsuleCollider2D myCollider;
    protected SortingGroup characterSortingGroup;
    protected Vector3 lastPosition;
    public bool IsFacingRight { get; set; }
    public bool initialFacingRight = true;
    protected bool lastFacingRight;
    public bool IsStunned { get; private set; }
    private float stunDuration;
    private float currentDefenseReduction = 0f;
    private Coroutine poisonCoroutine;
    protected bool isCombatStarted = false;
    protected bool isUntargetable = false;

    protected virtual void Awake()
    {
        stateMachine = new StateMachine();
        animator = GetComponentInChildren<Animator>();
        myCollider = GetComponent<CapsuleCollider2D>();
        InitializeStates();
        InitializeCharacterSortingGroup();
        SetFacingDirection();
        UpdateFacingDirection();

        OnHPChange += UpdateHPBar;
        OnManaChange += UpdateMPBar;
        OnShieldChange += UpdateShieldBar;

        colliderResults = new Collider2D[MAX_ENEMIES];
    }



    protected virtual void Update()
    {
        stateMachine.Update();
        if (!isCombatStarted && GameManager.Instance.isCombatStart)
        {
            OnCombatStart();
            isCombatStarted = true;
        }

        if (GameManager.Instance.isCombatStart)
        {
            UpdateNearestTarget();
        }
        UpdateStunState();
        UpdateFacingDirection();

    }

    protected virtual void InitializeCharacterStat() { }

    protected virtual void ApplyCharacterStat() { }

    public virtual void SetCharacterInfo()
    {
        InitializeCharacterStat();
        ApplyCharacterStat();
        if (team == Team.Ally)
        {
            GameManager.Instance.combatController.PlayerDataLoadComplete();
        }
    }

    public void InitializedTarget()
    {
        UpdateNearestTarget();
    }

    protected virtual void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    protected abstract void InitializeStates();

    public abstract void Attack();

    protected virtual void ActivatePassiveSkill() { }

    public virtual void OnCombatStart()
    {
        ActivatePassiveSkill();
    }

    public virtual void UseSkill()
    {
       
    }

    public virtual float CalculateFinalAttackSpeed()
    {
        float baseAttackSpeed = 1f / BaseAttackInterval;
        return baseAttackSpeed * (1 + AttackSpeedIncrease) * (1 - AttackSpeedDecrease);
    }

    public void PlayAnimation(int animationHash, bool shouldRestart = false)
    {
        if (animator != null)
        {
            if (shouldRestart)
            {
                animator.Play(animationHash, 0, 0f);
            }
            else
            {
                animator.CrossFade(animationHash, AnimationData.crossFadeDuration);
            }
        }
    }

    public virtual void OnTargetDeath(BaseCombatAI deadTarget)
    {
        if (cachedNearestTarget == null)
        {
            UpdateNearestTarget();
        }
    }
    public virtual void UpdateNearestTarget()
    {
        if (Time.time - lastTargetUpdateTime > TARGET_UPDATE_INTERVAL)
        {
            int enemyCount = Physics2D.OverlapCircleNonAlloc(transform.position, 50f, colliderResults, LayerMask.GetMask("Character"));

            float nearestSqrDistance = float.MaxValue;
            BaseCombatAI newNearestTarget = null;

            for (int i = 0; i < enemyCount; i++)
            {
                BaseCombatAI potentialEnemy = colliderResults[i].GetComponent<BaseCombatAI>();
                if (potentialEnemy != null && potentialEnemy != this && potentialEnemy.team != this.team && !potentialEnemy.isUntargetable)
                {
                    float sqrDistance = (transform.position - potentialEnemy.transform.position).sqrMagnitude;
                    if (sqrDistance < nearestSqrDistance)
                    {
                        nearestSqrDistance = sqrDistance;
                        newNearestTarget = potentialEnemy;
                    }
                }
            }
            if (newNearestTarget != null && (cachedNearestTarget == null || !cachedNearestTarget.CheckAlive()))
            {
                cachedNearestTarget = newNearestTarget;
            }

            lastTargetUpdateTime = Time.time;
        }
    }


    public virtual BaseCombatAI FindNearestTargetInRange()
    {
        int enemyCount = Physics2D.OverlapCircleNonAlloc(transform.position, AttackRange, colliderResults, LayerMask.GetMask("Character"));
        float nearestSqrDistance = float.MaxValue;
        BaseCombatAI nearestTarget = null;
        float sqrAttackRange = AttackRange * AttackRange;

        for (int i = 0; i < enemyCount; i++)
        {
            BaseCombatAI potentialEnemy = colliderResults[i].GetComponent<BaseCombatAI>();
            if (potentialEnemy != null && potentialEnemy != this && potentialEnemy.team != this.team 
                && potentialEnemy.CheckAlive() && !potentialEnemy.isUntargetable)
            {
                float sqrDistance = (transform.position - potentialEnemy.transform.position).sqrMagnitude;
                if (sqrDistance < nearestSqrDistance && sqrDistance <= sqrAttackRange)
                {
                    nearestSqrDistance = sqrDistance;
                    nearestTarget = potentialEnemy;
                }
            }
        }

        return nearestTarget;
    }

    public bool IsAnyTargetInAttackRange()
    {
        int enemyCount = Physics2D.OverlapCircleNonAlloc(transform.position, AttackRange, colliderResults, LayerMask.GetMask("Character"));
        for (int i = 0; i < enemyCount; i++)
        {
            BaseCombatAI potentialEnemy = colliderResults[i].GetComponent<BaseCombatAI>();
            if (potentialEnemy != null && potentialEnemy != this && potentialEnemy.team != this.team 
                && potentialEnemy.CheckAlive() &&!potentialEnemy.isUntargetable)
            {
                return true;
            }
        }
        return false;
    }


    public bool IsTargetInAttackRange()
    {
        if (cachedNearestTarget == null || cachedNearestTarget.isUntargetable) return false;
        return (transform.position - cachedNearestTarget.transform.position).sqrMagnitude <= (AttackRange * AttackRange);
    }

    public bool CheckAlive()
    {
        return CurrentHP > 0 && gameObject.activeSelf;
    }

    public bool CheckTarget()
    {
        return cachedNearestTarget != null && cachedNearestTarget.CheckAlive() && !cachedNearestTarget.isUntargetable;
    }

    public void ChangeState(IState newState)
    {
        stateMachine.SetState(newState);
    }

    protected virtual void UpdateHPBar()
    {
        if (HPBar != null)
        {
            HPBar.fillAmount = CurrentHP / MaxHP;
        }
    }

    public virtual void UpdateShieldBar()
    {
        if (ShieldBar != null)
        {
            ShieldBar.fillAmount = ShieldAmount / MaxHP;
        }
    }

    protected (bool isCritical, float damageMultiplier) CalculateCriticalHit()
    {
        bool isCritical = UnityEngine.Random.value < CriticalPersent;
        float damageMultiplier = isCritical ? (100f + CriticalDamage) / 100f : 1f;
        return (isCritical, damageMultiplier);
    }

    public virtual void TakeDamage(float damage, DamageType damageType, AttackType attackType,bool isCritical = false)
    {
        if (CurrentHP == 0) return;

        float actualDamage;
        if (damageType == DamageType.Magical)
        {
            // 마법 피해 계산
            actualDamage = damage / (1 + Resistance * 0.01f);
        }
        else
        {
            // 물리 피해 계산
            actualDamage = damage / (1 + Def * 0.01f);
        }

        Color textColor = Color.white;
        string damageText = actualDamage.ToString("F0");

        // 보호막은 방어력의 영향을 받지 않음
        if (ShieldAmount > 0)
        {
            textColor = Color.cyan;
            if (damage <= ShieldAmount)
            {
                ShieldAmount -= damage;
                return;
            }
            else
            {
                damage -= ShieldAmount;
                ShieldAmount = 0;
            }
        }

        if (isCritical)
        {
            textColor = Color.red;
            damageText += "!";
        }

        // HP 감소
        if (CurrentHP > 0)
        {
            CurrentHP = Mathf.Max(0, CurrentHP - actualDamage);
        }

        // 플로팅 텍스트 표시
        if (FloatingTextManager.Instance != null && FloatingTextManager.Instance.isFloatingTextEnabled == true)
        {
            FloatingTextManager.Instance.ShowFloatingText(damageText, transform.position, textColor);
        }

        // 공격 타입에 따른 사운드 재생
        if (SoundManager.Instance != null)
        {
            string soundEffect = (attackType == AttackType.Blunt) ? "SFX_Hit01" : "SFX_Hit02";
            SoundManager.Instance.PlaySound(soundEffect);
        }
        // 사망 처리
        if (CurrentHP == 0)
        {
            Die();
        }
    }

    protected virtual void SetFacingDirection()
    {
        if (team == Team.Ally)
        {
            initialFacingRight = true;
        }
        else if (team == Team.Enemy)
        {
            initialFacingRight = true;
        }
        IsFacingRight = initialFacingRight;
    }

    protected virtual void InitializeCharacterSortingGroup()
    {
        characterSortingGroup = GetComponentInChildren<SortingGroup>();
    }

    protected virtual void UpdateFacingDirection() { }

    protected virtual void Flip()
    {
        IsFacingRight = !IsFacingRight;
        if (characterSortingGroup != null)
        {
            Vector3 localScale = characterSortingGroup.transform.localScale;
            localScale.x = IsFacingRight ? 1 : -1;
            characterSortingGroup.transform.localScale = localScale;
        }

    }

    public void GainMana(float amount)
    {
        Mana = Mathf.Min(Mana + amount, MaxMana);
        
    }

    public virtual void UpdateMPBar()
    {
        if (MPBar != null)
        {
            MPBar.fillAmount = Mana / MaxMana;
        }
    }

    public virtual void Heal(float amount)
    {
        CurrentHP = Mathf.Min(CurrentHP + amount, MaxHP);
        string healText = $"{amount:F0}";
        if (amount > 0 && FloatingTextManager.Instance.isFloatingTextEnabled == true)
        {
            FloatingTextManager.Instance.ShowFloatingText(healText, transform.position, Color.green);
        }    
    }

    public virtual void AddShield(float amount)
    {
        ShieldAmount += amount;
    }

    public virtual void Die()
    {
        GameManager.Instance.combatController.CharacterDie(team);
        myCollider.enabled = false;
        StartCoroutine(DeactiveAfterDelay(1f));
    }

    private IEnumerator DeactiveAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
        //Destroy(gameObject);
    }

    public virtual void Resurrection(int HPPercentage)
    {
        if (CheckAlive() == true)
            return;

        if (HPPercentage < 0)
            return;

        GameManager.Instance.combatController.CharacterResurrect(team);
        CurrentHP = MaxHP * (HPPercentage / 100);
        myCollider.enabled = true;
        gameObject.SetActive(true);
    }

    public virtual void SetForcedTarget(BaseCombatAI target, float duration)
    {
        StartCoroutine(ForcedTargetCoroutine(target, duration));
    }

    protected IEnumerator ForcedTargetCoroutine(BaseCombatAI target, float duration)
    {
        BaseCombatAI originalTarget = cachedNearestTarget;
        cachedNearestTarget = target;
        yield return new WaitForSeconds(duration);
        cachedNearestTarget = originalTarget;
    }

    private void UpdateStunState()
    {
        if (IsStunned)
        {
            stunDuration -= Time.deltaTime;
            if (stunDuration <= 0)
            {
                IsStunned = false;
            }
        }
    }

    public virtual void ApplyStun(float duration)
    {
        IsStunned = true;
        stunDuration = duration;
    }

    public virtual void ApplyDefenseReduction(float reductionPercentage)
    {
        float reductionAmount = Def * reductionPercentage;
        Def = reductionAmount;
    }

    public virtual void RestoreDefense()
    {
        Def = originalDef;
    }

    public void ApplyPoisonEffect(Color poisonColor, float duration, float damagePerSecond)
    {
        if (poisonCoroutine != null)
        {
            StopCoroutine(poisonCoroutine);
        }
        poisonCoroutine = StartCoroutine(PoisonEffectCoroutine(poisonColor, duration, damagePerSecond));
    }

    private IEnumerator PoisonEffectCoroutine(Color poisonColor, float duration, float damagePerSecond)
    {
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        List<SpriteRenderer> affectedSprites = new List<SpriteRenderer>();
        List<Color> originalColors = new List<Color>();

        foreach (var renderer in spriteRenderers)
        {
            if (!renderer.CompareTag("IgnoreColorChange"))
            {
                affectedSprites.Add(renderer);
                originalColors.Add(renderer.color);
                renderer.color = poisonColor;
            }
        }

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            yield return new WaitForSeconds(1f);
            TakeDamage(damagePerSecond,DamageType.Magical,AttackType.Blunt,false);
            elapsedTime += 1f;
        }

        // 색상 복원
        for (int i = 0; i < affectedSprites.Count; i++)
        {
            affectedSprites[i].color = originalColors[i];
        }

        poisonCoroutine = null;
    }


    public Animator GetAnimator() { return animator; }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        Vector2 startPosition = new Vector2(transform.position.x, transform.position.y + 0.5f);
        Gizmos.DrawWireSphere(startPosition, AttackRange);
    }
}


