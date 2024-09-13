using System.Collections;
using UnityEngine;
//무조건 가장 가까운 적을 찾아서 공격하는 방식
public class EnemyAttackingState : IState
{
    private BaseCombatAI character;
    private float firstAttackTimer;
    private float attackTimer;
    private float attackInterval;
    private BaseCombatAI currentTarget;

    public EnemyAttackingState(BaseCombatAI character)
    {
        this.character = character;
        this.currentTarget = character.cachedNearestTarget;
    }

    public void OnEnter()
    {
        UpdateAttackInterval();
        CheckAndUpdateTarget();
        if (firstAttackTimer >= attackInterval)
        {
            character.Attack();
            firstAttackTimer = 0;
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
        CheckAndUpdateTarget();
        if (character.CheckTarget())
        {
            attackTimer += Time.deltaTime;
            firstAttackTimer += Time.deltaTime;
            if (attackTimer >= attackInterval)
            {
                UpdateAttackInterval();
                character.Attack();
                attackTimer = 0;
                firstAttackTimer = 0;
            }
        }
        else
        {
            character.PlayAnimation(AnimationData.IdleHash, false);
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

    private void CheckAndUpdateTarget()
    {
        BaseCombatAI nearestTargetInRange = character.FindNearestTargetInRange();
        if (nearestTargetInRange != null)
        {
            if (nearestTargetInRange != currentTarget)
            {
                currentTarget = nearestTargetInRange;
                character.cachedNearestTarget = currentTarget;
                firstAttackTimer = attackInterval; // 새 타겟으로 바뀌면 즉시 공격 가능하도록 설정
            }
        }
        else if (character.cachedNearestTarget != null && !character.IsTargetInAttackRange())
        {
            // 범위 내에 타겟이 없고, 현재 타겟이 공격 범위를 벗어났다면
            character.cachedNearestTarget = null;
            currentTarget = null;
        }
    }
}