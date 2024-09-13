using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Knight GreatShield Skill", menuName = "Skills/Knight/GreatShield")]
public class GreatShield : BaseSkill
{
    public float shieldMultiplier = 0.25f;
    public float shieldRadius = 5f; // ��ȣ���� �ο��� ����
    public GameObject shieldEffectPrefab; // ��ȣ�� ȿ�� ������

    public override void ExecuteSkill(BaseCombatAI character)
    {
        if (character is Knight knight)
        {
            float shieldAmount = knight.MaxHP * shieldMultiplier;

            // Knight �ڽſ��� ��ȣ�� �ο� �� ��ų ����Ʈ ���
            ApplyShieldToKnight(knight, shieldAmount);

            // �ֺ� �Ʊ����� ��ȣ�� �ο�
            Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(knight.transform.position, shieldRadius, LayerMask.GetMask("Character"));
            foreach (Collider2D collider in nearbyColliders)
            {
                BaseCombatAI ally = collider.GetComponent<BaseCombatAI>();
                if (ally != null && ally != knight && ally.team == knight.team)
                {
                    ApplyShieldToAlly(ally, shieldAmount);
                }
            }

            // ��ų �ִϸ��̼� ���
            knight.PlayAnimation(AnimationData.Skill_NormalHash);
            SoundManager.Instance.PlaySound("SFX_HolySkill");
        }
    }

    private void ApplyShieldToKnight(Knight knight, float shieldAmount)
    {
        knight.AddShield(shieldAmount);

        // ��ų ����ڿ��� BaseSkill�� skillEffectObject ����Ʈ ���
        if (skillEffectObject != null)
        {
            Vector3 offset = knight.transform.position;
            offset.y += 1f;
            EffectManager.Instance.PlayAnimatedSpriteEffect(skillEffectObject, offset, false);
        }
    }

    private void ApplyShieldToAlly(BaseCombatAI ally, float shieldAmount)
    {
        ally.AddShield(shieldAmount);

        // ������ �޴� �Ʊ����� shieldEffectPrefab ����Ʈ ���
        if (shieldEffectPrefab != null)
        {
            EffectManager.Instance.PlayAnimatedSpriteEffect(shieldEffectPrefab, ally.transform.position, false);
        }
    }
}