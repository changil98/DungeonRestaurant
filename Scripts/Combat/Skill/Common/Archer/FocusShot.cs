using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "New Archer FocusShot Skill", menuName = "Skills/Archer/FocusShot")]
public class FocusShot : BaseSkill
{
    public float skillDamageMultiplier = 2f;
    public float focusDuration = 1f;
    public float attackRange = 10f;
    public GameObject hitEffect;

    public override void ExecuteSkill(BaseCombatAI character)
    {
        if (character is Archer archer)
        {
            archer.StartCoroutine(PerformFocusShot(archer));
        }
    }

    private IEnumerator PerformFocusShot(Archer archer)
    {
        BaseCombatAI target = FindLowestHealthEnemy(archer);
        if (target != null)
        {
            archer.PlayAnimation(AnimationData.Skill_BowHash);
            SoundManager.Instance.PlaySound("SFX_RangeAttack");
            Vector3 effectPos = archer.pivotTransform.position;
            // 회전된 이펙트 생성 및 실행
            GameObject rotatedEffect = CreateRotatedEffect(skillEffectObject, archer.transform.position, target.transform.position);
            rotatedEffect.transform.position = effectPos;
            rotatedEffect.SetActive(true);

            float damage = archer.AttackDamage * skillDamageMultiplier;
            target.TakeDamage(damage, DamageType.Physical, AttackType.Blunt, false);

            EffectManager.Instance.PlayAnimatedSpriteEffect(hitEffect, target.transform.position, false);

            // 애니메이션 컴포넌트 가져오기
            Animator animator = rotatedEffect.GetComponent<Animator>();
            if (animator != null)
            {
                // 애니메이션 길이 가져오기
                AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
                if (clipInfo.Length > 0)
                {
                    float animationLength = clipInfo[0].clip.length;
                    yield return new WaitForSeconds(Mathf.Max(focusDuration, animationLength));
                }
                else
                {
                    yield return new WaitForSeconds(focusDuration);
                }
            }
            else
            {
                yield return new WaitForSeconds(focusDuration);
            }

            


            // 애니메이션이 완료된 후 이펙트 제거
            Destroy(rotatedEffect);
        }
    }

    private GameObject CreateRotatedEffect(GameObject originalPrefab, Vector3 archerPosition, Vector3 targetPosition)
    {
        GameObject rotatedEffect = Instantiate(originalPrefab);
        rotatedEffect.SetActive(false);

        Vector3 direction = targetPosition - archerPosition;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        
        angle -= 90f;

        
        SpriteRenderer spriteRenderer = rotatedEffect.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            
            if (direction.x < 0)
            {
                spriteRenderer.flipY = true;
                
                angle += 180f;
            }
            else
            {
                spriteRenderer.flipY = false;
            }
        }

        rotatedEffect.transform.rotation = Quaternion.Euler(0, 0, angle);

        return rotatedEffect;
    }


    private BaseCombatAI FindLowestHealthEnemy(Archer archer)
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(archer.transform.position, attackRange, LayerMask.GetMask("Character"));
        BaseCombatAI lowestHealthEnemy = null;
        float lowestHealth = float.MaxValue;
        foreach (Collider2D enemyCollider in enemies)
        {
            BaseCombatAI enemy = enemyCollider.GetComponent<BaseCombatAI>();
            if (enemy != null && enemy.team != archer.team)
            {
                if (enemy.CurrentHP < lowestHealth)
                {
                    lowestHealth = enemy.CurrentHP;
                    lowestHealthEnemy = enemy;
                }
            }
        }
        return lowestHealthEnemy;
    }
}