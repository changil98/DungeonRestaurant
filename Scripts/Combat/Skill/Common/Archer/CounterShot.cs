using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Archer CounterShot Skill", menuName = "Skills/Archer/CounterShot")]
public class CounterShot : BaseSkill
{
    public float skillDamageMultiplier = 1.5f;
    public float retreatDistance = 3f;
    public float detectionRadius = 5f;
    public GameObject hitEffect;

    public override void ExecuteSkill(BaseCombatAI character)
    {
        if (character is Archer archer)
        {
            archer.PlayAnimation(AnimationData.Skill_BowHash);
            SoundManager.Instance.PlaySound("SFX_RangeAttack");
            EffectManager.Instance.PlayAnimatedSpriteEffect(skillEffectObject, archer.transform.position, false);

            BaseCombatAI currentTarget = archer.cachedNearestTarget;
            BaseCombatAI secondTarget = FindSecondNearestEnemy(archer);

            Vector3 retreatPosition = CalculateRetreatPosition(archer);

            archer.StartCoroutine(PerformCounterShot(archer, currentTarget, secondTarget, retreatPosition));
        }
    }

    private IEnumerator PerformCounterShot(Archer archer, BaseCombatAI currentTarget, BaseCombatAI secondTarget, Vector3 initialRetreatPosition)
    {
        float retreatSpeed = 5f;
        Camera mainCamera = Camera.main;
        float elapsedTime = 0f;
        float maxRetreatTime = 1f;

        Vector3 startPosition = archer.transform.position;
        Vector3 currentRetreatPosition = initialRetreatPosition;

        while (elapsedTime < maxRetreatTime)
        {
            Vector3 newPosition = Vector3.MoveTowards(archer.transform.position, currentRetreatPosition, retreatSpeed * Time.deltaTime);
            Vector3 clampedPosition = ClampPositionToCameraBounds(newPosition, mainCamera);

            if (newPosition != clampedPosition)
            {
                // 카메라 경계에 도달했을 때 새로운 후퇴 방향 계산
                currentRetreatPosition = CalculateAlternativeRetreatPosition(archer, startPosition, currentRetreatPosition, mainCamera);
                newPosition = Vector3.MoveTowards(archer.transform.position, currentRetreatPosition, retreatSpeed * Time.deltaTime);
                clampedPosition = ClampPositionToCameraBounds(newPosition, mainCamera);
            }

            archer.transform.position = clampedPosition;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (currentTarget != null)
        {
            currentTarget.TakeDamage(archer.AttackDamage * skillDamageMultiplier,DamageType.Physical,AttackType.Blunt, false);
            EffectManager.Instance.PlayAnimatedSpriteEffect(hitEffect, currentTarget.transform.position, false);
        }

        if (secondTarget != null)
        {
            secondTarget.TakeDamage(archer.AttackDamage * skillDamageMultiplier, DamageType.Physical, AttackType.Blunt, false);
            EffectManager.Instance.PlayAnimatedSpriteEffect(hitEffect, secondTarget.transform.position, false);
        }
    }

    private Vector3 CalculateAlternativeRetreatPosition(Archer archer, Vector3 startPosition, Vector3 currentRetreatPosition, Camera camera)
    {
        Vector2 cameraMin = camera.ViewportToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));
        Vector2 cameraMax = camera.ViewportToWorldPoint(new Vector3(1, 1, camera.nearClipPlane));
        float buffer = 1.5f;

        Vector2 retreatDirection = (currentRetreatPosition - startPosition).normalized;
        Vector2 alternativeDirection = retreatDirection;

        // X 축 방향 조정
        if (archer.transform.position.x <= cameraMin.x + buffer || archer.transform.position.x >= cameraMax.x - buffer)
        {
            alternativeDirection.x = -retreatDirection.x;
        }

        // Y 축 방향 조정
        if (archer.transform.position.y <= cameraMin.y + buffer || archer.transform.position.y >= cameraMax.y - buffer)
        {
            alternativeDirection.y = -retreatDirection.y;
        }

        return archer.transform.position + (Vector3)alternativeDirection * retreatDistance;
    }

    private BaseCombatAI FindSecondNearestEnemy(Archer archer)
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(archer.transform.position, 50f, LayerMask.GetMask("Character"));
        float nearestDistance = float.MaxValue;
        BaseCombatAI secondNearestEnemy = null;

        foreach (Collider2D enemyCollider in enemies)
        {
            BaseCombatAI enemy = enemyCollider.GetComponent<BaseCombatAI>();
            if (enemy != null && enemy != archer.cachedNearestTarget && enemy.team != archer.team)
            {
                float distance = Vector2.Distance(archer.transform.position, enemy.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    secondNearestEnemy = enemy;
                }
            }
        }

        return secondNearestEnemy;
    }

    private Vector3 CalculateRetreatPosition(Archer archer)
    {
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(archer.transform.position, detectionRadius, LayerMask.GetMask("Character"));
        if (nearbyEnemies.Length > 0)
        {
            Vector3 averageEnemyPosition = Vector3.zero;
            int enemyCount = 0;

            foreach (Collider2D enemyCollider in nearbyEnemies)
            {
                BaseCombatAI enemy = enemyCollider.GetComponent<BaseCombatAI>();
                if (enemy != null && enemy.team != archer.team)
                {
                    averageEnemyPosition += enemy.transform.position;
                    enemyCount++;
                }
            }

            if (enemyCount > 0)
            {
                averageEnemyPosition /= enemyCount;
                Vector3 retreatDirection = (archer.transform.position - averageEnemyPosition).normalized;
                return archer.transform.position + retreatDirection * retreatDistance;
            }
        }

        return archer.transform.position;
    }

    private Vector2 ClampPositionToCameraBounds(Vector2 position, Camera camera)
    {
        if (camera == null) return position;

        Vector2 cameraMin = camera.ViewportToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));
        Vector2 cameraMax = camera.ViewportToWorldPoint(new Vector3(1, 1, camera.nearClipPlane));

        float buffer = 1.5f; // 캐릭터의 크기를 고려한 여유 공간
        position.x = Mathf.Clamp(position.x, cameraMin.x + buffer, cameraMax.x - buffer);
        position.y = Mathf.Clamp(position.y, cameraMin.y + buffer, cameraMax.y - buffer);

        return position;
    }
}