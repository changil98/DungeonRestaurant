using UnityEngine;
//기존에 사용하는 방식
public class AttackingState : IState
{
    private BaseCombatAI character;
    private float firstAttackTimer;
    private float attackTimer;
    private float attackInterval;
    private BaseCombatAI initialTarget;

    public AttackingState(BaseCombatAI character)
    {
        this.character = character;
        firstAttackTimer = 99;
    }

    public void OnEnter()
    {
        UpdateAttackInterval();
        initialTarget = character.cachedNearestTarget;

        if (firstAttackTimer >= attackInterval)
        {
            PerformAttack();
        }
        else
        {
            character.PlayAnimation(AnimationData.IdleHash);
        }
        attackTimer = 0;
    }

    public void OnExit()
    {
        if (firstAttackTimer < attackInterval)
        {
            firstAttackTimer += Time.deltaTime;
        }
    }

    public void Update()
    {
        if (character.CheckTarget())
        {
            if (character.IsTargetInAttackRange())
            {
                attackTimer += Time.deltaTime;
                firstAttackTimer += Time.deltaTime;

                if (attackTimer >= attackInterval)
                {
                    PerformAttack();
                }
            }
            else if (character.cachedNearestTarget == initialTarget)
            {
                // 초기 타겟이 범위를 벗어났을 때만 새 타겟을 찾음
                FindNewTargetInRange();
            }
        }
        else
        {
            character.UpdateNearestTarget();
        }
    }

    public void FixedUpdate() { }

    private void UpdateAttackInterval()
    {
        float newInterval = 1f / character.CalculateFinalAttackSpeed();
        if (!Mathf.Approximately(newInterval, attackInterval))
        {
            attackInterval = newInterval;
        }
    }

    private void FindNewTargetInRange()
    {
        BaseCombatAI newTarget = character.FindNearestTargetInRange();
        if (newTarget != null)
        {
            character.cachedNearestTarget = newTarget;
            initialTarget = newTarget;
            firstAttackTimer = attackInterval; // 새 타겟으로 바뀌면 즉시 공격 가능하도록 설정
        }
        else
        {
            character.cachedNearestTarget = null;
            initialTarget = null;
            character.PlayAnimation(AnimationData.IdleHash, false);
            Debug.Log("2");
            character.UpdateNearestTarget();
        }
    }

    private void PerformAttack()
    {
        UpdateAttackInterval();
        character.Attack();
        attackTimer = 0;
        firstAttackTimer = 0;
    }
}