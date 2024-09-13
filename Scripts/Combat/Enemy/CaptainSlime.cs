using UnityEngine;

public class CaptainSlime : EnemyCombatAI
{
    public float manaCost = 100f;
    public float moveStateDuration = 2f;
    public float idleStateDuration = 2f;
    private float manaRegenTimer = 0f;
    public float manaRegenTime = 1f;
    public float manaRegenAmount = 10f;
    private AnimationEventDispatcher animEventDispatcher;
    public bool isSkillAnimationComplete = false;

    protected override void Awake()
    {
        base.Awake();
        animEventDispatcher = GetComponentInChildren<AnimationEventDispatcher>();
        if (animEventDispatcher != null)
        {
            animEventDispatcher.OnAnimationComplete.AddListener(OnAnimationComplete);
        }
    }

    protected override void InitializeStates()
    {
        var idleState = new SlimeIdleState(this);
        var moveState = new SlimeMoveState(this);
        var skillState = new SlimeSkillState(this);
        var stunState = new StunState(this);
        var dieState = new DieState(this);

        stateMachine.AddTransition(idleState, moveState, new FuncPredicate(() => idleState.ShouldMove()));
        stateMachine.AddTransition(moveState, idleState, new FuncPredicate(() => moveState.IsFinished()));
        stateMachine.AddTransition(idleState, skillState
            , new FuncPredicate(() => Mana >= manaCost));
        stateMachine.AddTransition(moveState, skillState,
            new FuncPredicate(() => Mana >= manaCost));
        stateMachine.AddTransition(skillState, idleState, new FuncPredicate(() => skillState.IsSkillCompleted()));
        stateMachine.AddAnyTransition(stunState, new FuncPredicate(() => IsStunned));
        stateMachine.AddTransition(stunState, idleState, new FuncPredicate(() => !IsStunned));
        stateMachine.AddAnyTransition(dieState, new FuncPredicate(() => CurrentHP <= 0));

        stateMachine.SetState(idleState);
        stateMachine.AddAnyTransition(idleState, new FuncPredicate(() => !GameManager.Instance.isCombatStart));
    }

    protected override void Update()
    {
        base.Update();

        if (isCombatStarted)
        {
            RegenerateMana();
        }
    }

    private void RegenerateMana()
    {
        manaRegenTimer += Time.deltaTime;
        if (manaRegenTimer >= manaRegenTime)
        {
            GainMana(manaRegenAmount);
            manaRegenTimer -= manaRegenTime;  // 정확히 1초마다 실행되도록 보장
        }
    }

    public override void Attack()
    {

    }

    public override void UseSkill()
    {
        if (Mana >= manaCost)
        {
            PlayAnimation(AnimationData.Attack_NormalHash);
            SummonSlimeSoldiers();
            Mana = 0;
            isSkillAnimationComplete = false;
        }
    }

    private void SummonSlimeSoldiers()
    {
        EnemyInfo slimeSoliderInfo = DataManager.Instance.EnemyInfoDict.GetData("ENE0010");
        for (int i = 0; i < 3; i++)
        {
            Vector2 spawnPosition = (Vector2)transform.position + Random.insideUnitCircle * 2f;
            GameObject soldierObject = Instantiate(slimeSoliderInfo.Prefab.prefab, spawnPosition, Quaternion.identity);
            EnemyCombatAI SpawnedSlime = soldierObject.GetComponent<EnemyCombatAI>();
            SpawnedSlime.SetEnemyData(slimeSoliderInfo);

            GameManager.Instance.combatController.EnemyAliveCount++;
        }
    }


    private void OnAnimationComplete(string animationName)
    {
        if (animationName == "Attack")
        {
            isSkillAnimationComplete = true;
        }
    }

    public bool IsEnemyInRange()
    {
        return cachedNearestTarget != null &&
               Vector2.Distance(transform.position, cachedNearestTarget.transform.position) <= AttackRange;
    }

    private Vector2 GetRandomDirection()
    {
        return Random.insideUnitCircle.normalized;
    }

    private Vector2 ClampPositionToCameraBounds(Vector2 position)
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null) return position;

        Vector2 cameraMin = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector2 cameraMax = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));

        // 캐릭터의 크기를 고려한 여유 공간 (필요에 따라 조정)
        float buffer = 3f;

        position.x = Mathf.Clamp(position.x, cameraMin.x + buffer, cameraMax.x - buffer);
        position.y = Mathf.Clamp(position.y, cameraMin.y + buffer, cameraMax.y - buffer);

        return position;
    }

    public Vector2 GetNextMovePosition(float moveDistance)
    {
        Vector2 moveDirection = GetRandomDirection();
        Vector2 nextPosition = (Vector2)transform.position + moveDirection * moveDistance;
        return ClampPositionToCameraBounds(nextPosition);
    }


}
