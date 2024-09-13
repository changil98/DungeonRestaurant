using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Mage Venomous Magic Skill", menuName = "Skills/Mage/Venomous Magic")]
public class VenomousMagic : BaseSkill
{
    public float poisonDuration = 5f;
    public float poisonTickDamageMultiplier = 0.2f;
    public float poisonRadius = 3f;
    public Color poisonColor = Color.green;

    public override void ExecuteSkill(BaseCombatAI character)
    {
        if (character is Mage mage)
        {
            Collider2D[] colliderResults = new Collider2D[10];
            int hitCount = Physics2D.OverlapCircleNonAlloc(mage.transform.position, 50f, colliderResults, LayerMask.GetMask("Character"));

            var enemies = colliderResults.Take(hitCount)
                .Select(c => c.GetComponent<BaseCombatAI>())
                .Where(e => e != null && e.team != mage.team)
                .ToList();

            if (enemies.Count > 0)
            {
                BaseCombatAI targetEnemy = enemies[Random.Range(0, enemies.Count)];
                ApplyVenomEffect(mage, targetEnemy);

                // 스킬 애니메이션 재생
                mage.PlayAnimation(AnimationData.Skill_MagicHash);
                SoundManager.Instance.PlaySound("SFX_MagicSkill");
            }

        }
    }

    private void ApplyVenomEffect(Mage mage, BaseCombatAI targetEnemy)
    {
        // 메인 타겟에 독 효과 적용
        StartPoisonEffect(mage, targetEnemy);

        // 주변 적들에게도 독 효과 적용
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(targetEnemy.transform.position, poisonRadius, LayerMask.GetMask("Character"));
        foreach (Collider2D enemyCollider in nearbyEnemies)
        {
            BaseCombatAI nearbyEnemy = enemyCollider.GetComponent<BaseCombatAI>();
            if (nearbyEnemy != null && nearbyEnemy != targetEnemy && nearbyEnemy.team != mage.team)
            {
                StartPoisonEffect(mage, nearbyEnemy);
            }
        }

        // 메인 타겟에 Venom 이펙트 재생
        PlayVenomEffect(targetEnemy.transform.position);
    }

    private void StartPoisonEffect(Mage mage, BaseCombatAI enemy)
    {
        enemy.StartCoroutine(PoisonCoroutine(mage, enemy));
    }

    private IEnumerator PoisonCoroutine(Mage mage, BaseCombatAI enemy)
    {
        SpriteRenderer[] spriteRenderers = enemy.GetComponentsInChildren<SpriteRenderer>(true);
        List<SpriteRenderer> affectedSprites = new List<SpriteRenderer>();
        List<Color> originalColors = new List<Color>();

        foreach (var renderer in spriteRenderers)
        {
            // 특정 조건에 따라 색상 변경 대상 선택
            if (!renderer.CompareTag("IgnoreColorChange"))
            {
                affectedSprites.Add(renderer);
                originalColors.Add(renderer.color);
                renderer.color = poisonColor;
            }
        }

        float elapsedTime = 0f;
        while (elapsedTime < poisonDuration)
        {
            yield return new WaitForSeconds(1f);
            float poisonDamage = mage.AttackDamage * poisonTickDamageMultiplier;
            enemy.TakeDamage(poisonDamage,DamageType.Magical, AttackType.Blunt, false);
            elapsedTime += 1f;
        }

        // 색상 복원
        for (int i = 0; i < affectedSprites.Count; i++)
        {
            affectedSprites[i].color = originalColors[i];
        }
    }

    private void PlayVenomEffect(Vector3 position)
    {
        if (skillEffectObject != null)
        {
            position.y += 1f;
            EffectManager.Instance.PlayAnimatedSpriteEffect(skillEffectObject, position, false);
        }
    }
}