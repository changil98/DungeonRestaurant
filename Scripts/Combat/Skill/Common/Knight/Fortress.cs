using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "New Knight Fortress Skill", menuName = "Skills/Knight/Fortress")]
public class Fortress : BaseSkill
{
    public float tauntDuration = 1f;
    public float skillDuration = 3f;
    public float damageReductionPercentage = 50f;
    public float tauntRadius = 5f; // ���� ����

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
        // ��ų �ִϸ��̼� ���
        knight.PlayAnimation(AnimationData.Skill_NormalHash);
        SoundManager.Instance.PlaySound("SFX_HolySkill");
        EffectManager.Instance.PlayAnimatedSpriteEffect(skillEffectObject,offset,false);

        // �ֺ� �� ����
        Collider2D[] enemies = Physics2D.OverlapCircleAll(knight.transform.position, tauntRadius, LayerMask.GetMask("Character"));
        foreach (Collider2D enemyCollider in enemies)
        {
            BaseCombatAI enemy = enemyCollider.GetComponent<BaseCombatAI>();
            if (enemy != null && enemy.team != knight.team)
            {
                enemy.SetForcedTarget(knight, tauntDuration);
            }
        }

        // ������ ���� �� ���� �ߴ� ���� ����
        knight.SetDamageReduction(damageReductionPercentage);
        knight.SetAttackSuspended(true);

        // ��ų ���� �ð� ���
        yield return new WaitForSeconds(skillDuration);

        // ���� ����
        knight.SetDamageReduction(0);
        knight.SetAttackSuspended(false);

    }
}