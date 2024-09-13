using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MasterOfFlame", menuName = "Skills/Salamander/MasterOfFlame")]
public class MasterOfFlame : BaseSkill
{
    public float skillDamageMultiplier = 3f;
    public float skillHealMultiplier = 2f;
    public float skillDuration = 2f;
    public float flameLength = 10f;
    public float flameWidth = 1f;
    
    public override void ExecuteSkill(BaseCombatAI character)
    {
        if (character is Salamander salamander)
        {
            salamander.StartCoroutine(PerformMasterOfFlame(salamander));
        }
    }

    private IEnumerator PerformMasterOfFlame(Salamander salamander)
    {
        if (salamander.cachedNearestTarget != null)
        {
            salamander.PlayAnimation(AnimationData.Skill_MagicHash);
            SoundManager.Instance.PlaySound("SFX_MasterOfFlame");
            Vector3 startPos = salamander.pivotTransform.position;
            Vector3 direction = (salamander.cachedNearestTarget.transform.position - startPos).normalized;
            Vector3 endPos = startPos + direction * flameLength;

            // 회전된 이펙트 생성 및 실행
            GameObject rotatedEffect = CreateRotatedEffect(skillEffectObject, startPos, endPos);
            rotatedEffect.transform.position = startPos;
            rotatedEffect.SetActive(true);

            float totalDamage = salamander.AttackDamage * skillDamageMultiplier;
            float totalHeal = salamander.AttackDamage * skillHealMultiplier;
            float damagePerTick = totalDamage / 6f;
            float healPerTick = totalHeal / 6f;

            for (int i = 0; i < 6; i++)
            {
                ApplyEffects(salamander, startPos, endPos, direction, damagePerTick, healPerTick);
                yield return new WaitForSeconds(skillDuration / 6f);
            }

            // 애니메이션 컴포넌트 가져오기
            Animator animator = rotatedEffect.GetComponent<Animator>();
            if (animator != null)
            {
                // 애니메이션 길이 가져오기
                AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
                if (clipInfo.Length > 0)
                {
                    float animationLength = clipInfo[0].clip.length;
                    yield return new WaitForSeconds(Mathf.Max(0, animationLength - skillDuration));
                }
            }

            // 애니메이션이 완료된 후 이펙트 제거
            Destroy(rotatedEffect);
        }
    }

    private void ApplyEffects(Salamander salamander, Vector3 startPos, Vector3 endPos, Vector3 direction, float damagePerTick, float healPerTick)
    {
        Vector2 boxCenter = (startPos + endPos) / 2;
        Vector2 boxSize = new Vector2(flameLength, flameWidth);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Collider2D[] hits = Physics2D.OverlapBoxAll(boxCenter, boxSize, angle, LayerMask.GetMask("Character"));
        Debug.DrawLine(startPos, endPos, Color.red, skillDuration);
        foreach (Collider2D hit in hits)
        {
            BaseCombatAI character = hit.GetComponent<BaseCombatAI>();
            if (character != null)
            {
                if (character.team != salamander.team)
                {
                    // 적에게 데미지 적용
                    character.TakeDamage(damagePerTick, DamageType.Magical, AttackType.Blunt, false);
                }
                else
                {
                    // 아군 회복
                    character.Heal(healPerTick);
                }
            }
        }
    }

    private GameObject CreateRotatedEffect(GameObject originalPrefab, Vector3 startPos, Vector3 endPos)
    {
        GameObject rotatedEffect = Instantiate(originalPrefab);
        rotatedEffect.SetActive(false);
        Vector3 direction = endPos - startPos;
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

            // 원본 스프라이트의 크기를 고려한 스케일 조정
            float originalLength = spriteRenderer.bounds.size.y;
            float desiredLength = direction.magnitude;
            float scaleFactor = desiredLength / originalLength;

            rotatedEffect.transform.localScale = new Vector3(rotatedEffect.transform.localScale.x, scaleFactor, 1);
        }

        rotatedEffect.transform.rotation = Quaternion.Euler(0, 0, angle);
        rotatedEffect.transform.position = startPos;

        return rotatedEffect;
    }


}
