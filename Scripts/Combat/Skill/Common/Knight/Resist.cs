using UnityEngine;

[CreateAssetMenu(fileName = "New Knight Resist Skill", menuName = "Skills/Knight/Resist")]
public class Resist : BaseSkill
{
    public float healMultiplier = 0.4f;
    public GameObject healEffectPrefab; // �� ȿ�� ������

    public override void ExecuteSkill(BaseCombatAI character)
    {
        if (character is Knight knight)
        {
            // ���� ü�� ���
            float lostHealth = knight.MaxHP - knight.CurrentHP;

            // ȸ���� ���
            float healAmount = lostHealth * healMultiplier;

            // ü�� ȸ��
            knight.Heal(healAmount);

            // ��ų �ִϸ��̼� ���
            knight.PlayAnimation(AnimationData.Skill_NormalHash);
            SoundManager.Instance.PlaySound("SFX_HolySkill");
            // ��ų ����Ʈ ���
            PlaySkillEffect(knight);

            // �� ����Ʈ ���
            PlayHealEffect(knight);
        }
    }

    private void PlaySkillEffect(Knight knight)
    {
        if (skillEffectObject != null)
        {
            Vector3 effectPosition = knight.transform.position;
            effectPosition.y += 2f; // ����Ʈ�� ĳ���� ���� ǥ��
            EffectManager.Instance.PlayAnimatedSpriteEffect(skillEffectObject, effectPosition, false);
        }
    }

    private void PlayHealEffect(Knight knight)
    {
        if (healEffectPrefab != null)
        {
            EffectManager.Instance.PlayAnimatedSpriteEffect(healEffectPrefab, knight.transform.position, false);
        }
    }
}