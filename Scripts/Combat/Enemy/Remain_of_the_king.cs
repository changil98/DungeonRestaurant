using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Remain_of_the_king : EnemyCombatAI
{
    [Title("References")]
    public GameObject projectilePrefab;

    [Title("Skill Balancing")]
    public float summonInterval = 10f;
    public int summonCount = 4;

    private BaseCombatAI forcedTarget;
    private float forcedTargetEndTime;
    private bool hasEnemyEnteredRange = false;
    private Coroutine summonCoroutine;

    protected override void Awake()
    {
        base.Awake();
        summonCoroutine = StartCoroutine(SummonCitizenRoutine());
    }

    protected override void InitializeStates()
    {
        var idleState = new IdleState(this);
        var moveState = new RemainOfKingMovingState(this);
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

    public override void UpdateNearestTarget()
    {
        if (forcedTarget != null && Time.time < forcedTargetEndTime && forcedTarget.gameObject.activeSelf)
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

    private void SummonCitizen()
    {
        EnemyInfo citizenInfo = DataManager.Instance.EnemyInfoDict.GetData("ENE0011");
        EnemySpawnArea frontSpawnArea = CombatController.instance.enemySpawnArea[SpawnArea.Front];

        for (int i = 0; i < summonCount; i++)
        {
            Vector2 spawnPosition = frontSpawnArea.GetRandomPosition();
            GameObject citizenObject = Instantiate(citizenInfo.Prefab.prefab, spawnPosition, Quaternion.identity);
            EnemyCombatAI spawnedCitizen = citizenObject.GetComponent<EnemyCombatAI>();
            spawnedCitizen.SetEnemyData(citizenInfo);
            CombatController.instance.EnemyAliveCount++;
        }
    }

    private IEnumerator SummonCitizenRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(summonInterval);
            SummonCitizen();
        }
    }

    public override void Die()
    {
        if (summonCoroutine != null)
        {
            StopCoroutine(summonCoroutine);
        }
        base.Die();
    }

    public bool HasEnemyEnteredRange()
    {
        return hasEnemyEnteredRange;
    }
}


