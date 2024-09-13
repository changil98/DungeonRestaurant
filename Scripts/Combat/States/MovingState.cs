using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MovingState : IState
{

    private BaseCombatAI owner;
    private Transform targetTransform;
    private float footstepTimer = 0f;
    private float footstepInterval = 0.3f;
    private bool isRunAnimationPlaying = false;
    private const float AVOIDANCE_RADIUS = 1f;
    private const int DIRECTION_COUNT = 8;
    private const int MAX_COLLIDERS = 20;
    private const float BLOCKED_THRESHOLD = 0.3f; // 경로가 막혔다고 판단하는 임계값

    private static ColliderPoolTest colliderPool = new ColliderPoolTest(MAX_COLLIDERS);
    private List<Collider2D> nearbyAllies = new List<Collider2D>(MAX_COLLIDERS);
    private float lastCollisionCheckTime = 0f;
    private const float COLLISION_CHECK_INTERVAL = 0.1f;

    public MovingState(BaseCombatAI owner)
    {
        this.owner = owner;
    }

    public void OnEnter()
    {
        if (!isRunAnimationPlaying)
        {
            owner.PlayAnimation(AnimationData.RunHash);
        }
        UpdateTarget();
    }

    public void Update()
    {
        if (owner.CheckTarget())
        {
            Vector2 targetDirection = GetTargetDirection();

            if (Time.time - lastCollisionCheckTime >= COLLISION_CHECK_INTERVAL)
            {
                UpdateNearbyAllies(); //근처의 아군 감지
                lastCollisionCheckTime = Time.time;
            }

            Vector2 moveDirection = DetermineMovementDirection(targetDirection); //방향 계산
            MoveInDirection(moveDirection); //이동

            UpdateFootstepEffect();
        }
        else
        {
            owner.UpdateNearestTarget();
        }
    }

    private Vector2 GetTargetDirection()
    {
        return (owner.cachedNearestTarget.transform.position - owner.transform.position).normalized;
    }

    private void UpdateNearbyAllies()
    {
        nearbyAllies.Clear();
        int hitCount = Physics2D.OverlapCircleNonAlloc(owner.transform.position, AVOIDANCE_RADIUS, colliderPool.GetColliders());

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D collider = colliderPool.GetColliders()[i];
            BaseCombatAI otherAI = collider.GetComponent<BaseCombatAI>();
            if (otherAI != null && otherAI != owner && otherAI.team == owner.team)
            {
                nearbyAllies.Add(collider);
            }
        }
    }

    private Vector2 DetermineMovementDirection(Vector2 targetDirection)
    {
        float obstacleScore = EvaluateObstacles(targetDirection);

        if (obstacleScore < BLOCKED_THRESHOLD)
        {
            // 경로가 많이 막혀있으면 그대로 통과
            return targetDirection;
        }
        else
        {
            // 약간의 장애물만 있으면 회피 시도
            return FindBestAvoidanceDirection(targetDirection);
        }
    }

    private float EvaluateObstacles(Vector2 direction)
    {
        float score = 1f;
        Vector2 futurePosition = (Vector2)owner.transform.position + direction * AVOIDANCE_RADIUS;

        foreach (Collider2D ally in nearbyAllies)
        {
            float distance = Vector2.Distance(futurePosition, ally.transform.position);
            if (distance < AVOIDANCE_RADIUS)
            {
                score -= (AVOIDANCE_RADIUS - distance) / AVOIDANCE_RADIUS;
            }
        }

        return score;
    }

    private Vector2 FindBestAvoidanceDirection(Vector2 targetDirection)
    {
        Vector2 bestDirection = targetDirection;
        float bestScore = EvaluateObstacles(targetDirection);

        for (int i = 0; i < DIRECTION_COUNT; i++)
        {
            float angle = i * (360f / DIRECTION_COUNT);
            Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.right;

            float score = EvaluateObstacles(direction);
            if (score > bestScore)
            {
                bestScore = score;
                bestDirection = direction;
            }
        }

        return Vector2.Lerp(targetDirection, bestDirection, 0.5f).normalized;
    }

    private void MoveInDirection(Vector2 direction)
    {
        owner.transform.Translate(direction * owner.MovementSpeed * Time.deltaTime);
    }

    private void UpdateFootstepEffect()
    {
        footstepTimer += Time.deltaTime;
        if (footstepTimer >= footstepInterval)
        {
            CreateFootstepEffect();
            footstepTimer = 0f;
        }
    }

    private void CreateFootstepEffect()
    {
        if (owner.footStepEffect != null)
        {
            Vector3 effectPosition = owner.transform.position;
            EffectManager.Instance.PlayAnimatedSpriteEffect(owner.footStepEffect, effectPosition, false);
        }
    }

    private void UpdateTarget()
    {
        BaseCombatAI target = owner.cachedNearestTarget;
        targetTransform = target?.transform;
    }

    public void OnExit()
    {
        nearbyAllies.Clear();
    }

    public void FixedUpdate()
    {
    }
}
public class ColliderPoolTest
{
    private Collider2D[] colliders;

    public ColliderPoolTest(int size)
    {
        colliders = new Collider2D[size];
    }

    public Collider2D[] GetColliders()
    {
        return colliders;
    }
}