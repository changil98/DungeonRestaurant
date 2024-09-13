using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "New Priest Cure Skill", menuName = "Skills/Priest/Cure")]
public class Cure : BaseSkill
{
    public float healMultiplier = 1.5f;
    public float healRadius = 10f; // �� ����
    public GameObject healEffectPrefab; // �� ����Ʈ ������

    public override void ExecuteSkill(BaseCombatAI character)
    {
        if (character is Priest priest)
        {
            // ���� ü�� ������ ���� �Ʊ� ã��
            BaseCombatAI targetToHeal = FindLowestHealthRatioAlly(priest);

            // �ֺ��� �Ʊ��� ������ �ڽ��� ������� ����
            if (targetToHeal == null)
            {
                targetToHeal = priest;
            }

            if (targetToHeal != null)
            {
                // ��ų �ִϸ��̼� ���
                priest.PlayAnimation(AnimationData.Skill_MagicHash);
                SoundManager.Instance.PlaySound("SFX_HolySkill");
                // ���� ���
                float healAmount = priest.AttackDamage * healMultiplier;

                // ü�� ȸ��
                targetToHeal.Heal(healAmount);
                SoundManager.Instance.PlaySound("SFX_Heal");
                // ��ų ����Ʈ ���
                PlaySkillEffect(priest);

                // �� ����Ʈ ���
                PlayHealEffect(targetToHeal);                
            }           
        }
    }

    private BaseCombatAI FindLowestHealthRatioAlly(Priest priest)
    {
        return Physics2D.OverlapCircleAll(priest.transform.position, healRadius, LayerMask.GetMask("Character"))
            .Select(collider => collider.GetComponent<BaseCombatAI>())
            .Where(ally => ally != null && ally.team == priest.team && ally != priest)
            .OrderBy(ally => ally.CurrentHP / ally.MaxHP)
            .FirstOrDefault();
    }

    private void PlaySkillEffect(Priest priest)
    {
        if (skillEffectObject != null)
        {
            Vector3 effectPosition = priest.transform.position;
            effectPosition.y += 1f; // ����Ʈ�� ĳ���� ���� ǥ��
            EffectManager.Instance.PlayAnimatedSpriteEffect(skillEffectObject, effectPosition, false);
        }
    }

    private void PlayHealEffect(BaseCombatAI target)
    {
        if (healEffectPrefab != null)
        {
            Vector3 effectPosition = target.transform.position;
            effectPosition.y += 1f; // ����Ʈ�� ĳ���� ���� ǥ��
            EffectManager.Instance.PlayAnimatedSpriteEffect(healEffectPrefab, effectPosition, false);
        }
    }
}
