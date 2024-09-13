using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "New Knight Fortress Skill", menuName = "Skills/Knight/Fortress")]
public class Fortress : BaseSkill
{
    public float tauntDuration = 1f;
    public float skillDuration = 3f;
    public float damageReductionPercentage = 50f;
    public float tauntRadius = 5f; // 도발 범위

    public override void ExecuteSkill(BaseCombatAI character)
    {
        if (character is Knight knight)
        {
            knight.StartCoroutine(PerformFortress(knight));
        }
    }

    private IEnumerator PerformFortress(Knight knight)
    {
        Vector3 offset = knight.transform.position;
        offset.y += 2.3f;
        // 스킬 애니메이션 재생
        knight.PlayAnimation(AnimationData.Skill_NormalHash);
        SoundManager.Instance.PlaySound("SFX_HolySkill");
        EffectManager.Instance.PlayAnimatedSpriteEffect(skillEffectObject,offset,false);

        // 주변 적 도발
        Collider2D[] enemies = Physics2D.OverlapCircleAll(knight.transform.position, tauntRadius, LayerMask.GetMask("Character"));
        foreach (Collider2D enemyCollider in enemies)
        {
            BaseCombatAI enemy = enemyCollider.GetComponent<BaseCombatAI>();
            if (enemy != null && enemy.team != knight.team)
            {
                enemy.SetForcedTarget(knight, tauntDuration);
            }
        }

        // 데미지 감소 및 공격 중단 상태 설정
        knight.SetDamageReduction(damageReductionPercentage);
        knight.SetAttackSuspended(true);

        // 스킬 지속 시간 대기
        yield return new WaitForSeconds(skillDuration);

        // 상태 복구
        knight.SetDamageReduction(0);
        knight.SetAttackSuspended(false);

    }
}