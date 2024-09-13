using UnityEngine;
//������ ����ϴ� ���
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
                // �ʱ� Ÿ���� ������ ����� ���� �� Ÿ���� ã��
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
            firstAttackTimer = attackInterval; // �� Ÿ������ �ٲ�� ��� ���� �����ϵ��� ����
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