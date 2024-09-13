using UnityEngine;

public class SlimeRetreatState : IState
{
    private CaptainSlime slime;
    private Vector2 retreatDirection;
    private float retreatMultiplier = 1.5f;

    public SlimeRetreatState(CaptainSlime owner)
    {
        this.slime = owner;
    }

    public void OnEnter()
    {
        slime.PlayAnimation(AnimationData.RunHash);
    }

    public void Update()
    {
        if (slime.CheckTarget())
        {
            if (IsWithinEnemyAttackRange())
            {
                Vector2 newPosition = CalculateRetreatPosition();
                slime.transform.position = Vector2.MoveTowards(slime.transform.position, newPosition, slime.MovementSpeed * Time.deltaTime);
            }
        }
    }

    private bool IsWithinEnemyAttackRange()
    {
        if (slime.cachedNearestTarget == null) return false;

        float distanceToEnemy = Vector2.Distance(slime.transform.position, slime.cachedNearestTarget.transform.position);
        return distanceToEnemy <= slime.cachedNearestTarget.AttackRange;
    }

    private Vector2 CalculateRetreatPosition()
    {
        if (slime.cachedNearestTarget == null) return slime.transform.position;

        Vector2 directionToTarget = (slime.cachedNearestTarget.transform.position - slime.transform.position).normalized;
        float retreatDistance = slime.cachedNearestTarget.AttackRange * retreatMultiplier;

        return (Vector2)slime.transform.position - directionToTarget * retreatDistance;
    }
  
    public void FixedUpdate() { }
    public void OnExit()
    {
        
    }
}

