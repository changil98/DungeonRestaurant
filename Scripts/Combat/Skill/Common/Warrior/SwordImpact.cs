using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "New Warrior SwordImpact Skill", menuName = "Skills/Warrior/SwordImpact")]
public class SwordImpact : BaseSkill
{
    public float damageMultiplier = 1.2f;
    public float shieldMultiplier = 0.3f;
    public float knockbackForce = 5f;
    public float areaOfEffect = 5f;
    public float maxKnockbackDistance = 3f;

    public override void ExecuteSkill(BaseCombatAI character)
    {
        if (character is Warrior warrior)
        {
            warrior.PlayAnimation(AnimationData.Skill_NormalHash);
            SoundManager.Instance.PlaySound("SFX_WarriorSkill3");
            Vector3 targetPos = warrior.cachedNearestTarget.transform.position;
            targetPos.y += 0.8f;
            EffectManager.Instance.PlayAnimatedSpriteEffect(skillEffectObject, targetPos, false);

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(warrior.transform.position, areaOfEffect, LayerMask.GetMask("Character"));
            foreach (Collider2D enemyCollider in hitEnemies)
            {
                BaseCombatAI enemy = enemyCollider.GetComponent<BaseCombatAI>();
                if (enemy != null && enemy.team != warrior.team)
                {
                    float damage = warrior.AttackDamage * damageMultiplier;
                    enemy.TakeDamage(damage, DamageType.Physical, AttackType.Slash, false);
                    enemy.ApplyStun(1f);
                    Vector2 knockbackDirection = (enemy.transform.position - warrior.transform.position).normalized;
                    warrior.StartCoroutine(ApplyKnockback(enemy, knockbackDirection));
                }
            }

            float shieldAmount = warrior.AttackDamage * shieldMultiplier;
            warrior.ShieldAmount += shieldAmount;
        }
    }

    private IEnumerator ApplyKnockback(BaseCombatAI enemy, Vector2 knockbackDirection)
    {
        float distanceMoved = 0f;
        Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
        Camera mainCamera = Camera.main;

        while (distanceMoved < maxKnockbackDistance)
        {
            Vector2 oldPosition = enemy.transform.position;
            enemyRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            yield return new WaitForFixedUpdate();

            Vector2 newPosition = ClampPositionToCameraBounds(enemy.transform.position, mainCamera);
            enemy.transform.position = newPosition;

            distanceMoved += Vector2.Distance(oldPosition, newPosition);
            if (distanceMoved >= maxKnockbackDistance || oldPosition == newPosition)
            {
                enemyRb.velocity = Vector2.zero;
                break;
            }
        }
    }

    private Vector2 ClampPositionToCameraBounds(Vector2 position, Camera camera)
    {
        if (camera == null) return position;

        Vector2 cameraMin = camera.ViewportToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));
        Vector2 cameraMax = camera.ViewportToWorldPoint(new Vector3(1, 1, camera.nearClipPlane));

        // 캐릭터의 크기를 고려한 여유 공간 (필요에 따라 조정)
        float buffer = 1.5f;

        position.x = Mathf.Clamp(position.x, cameraMin.x + buffer, cameraMax.x - buffer);
        position.y = Mathf.Clamp(position.y, cameraMin.y + buffer, cameraMax.y - buffer);

        return position;
    }
}