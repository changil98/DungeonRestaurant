using System.Collections;
using UnityEngine;
//������ ���� ����� ���� ã�Ƽ� �����ϴ� ���
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
                firstAttackTimer = attackInterval; // �� Ÿ������ �ٲ�� ��� ���� �����ϵ��� ����
            }
        }
        else if (character.cachedNearestTarget != null && !character.IsTargetInAttackRange())
        {
            // ���� ���� Ÿ���� ����, ���� Ÿ���� ���� ������ ����ٸ�
            character.cachedNearestTarget = null;
            currentTarget = null;
        }
    }
}