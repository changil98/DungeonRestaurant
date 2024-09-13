using UnityEngine;

[CreateAssetMenu(fileName = "New Rogue LifeSteal Skill", menuName = "Skills/Rogue/LifeSteal")]
public class LifeSteal : BaseSkill
{
    public float skillDamageMultiplier = 1.5f;
    public GameObject lifeStealEffectPrefab;
    public GameObject healEffectPrefab;

    public override void ExecuteSkill(BaseCombatAI character)
    {
        if (character is Rogue rogue)
        {
            PerformLifeSteal(rogue);
        }
    }

    private void PerformLifeSteal(Rogue rogue)
    {
        if (rogue.cachedNearestTarget != null)
        {
            rogue.PlayAnimation(AnimationData.Skill_NormalHash);
            SoundManager.Instance.PlaySound("SFX_DaggerSwing");
            // ������ ���
            float damage = rogue.AttackDamage * skillDamageMultiplier;

            // ��󿡰� ������ ����
            rogue.cachedNearestTarget.TakeDamage(damage, DamageType.Physical, AttackType.Slash, false);

            // ������ ��� (�������� ����)
            float healAmount = damage * 0.5f;

            // �α׿��� ü�� ȸ��
            rogue.Heal(healAmount);

            // ����Ʈ ���
            PlayLifeStealEffect(rogue, rogue.cachedNearestTarget.transform.position);

            
        }
              
    }

    private void PlayLifeStealEffect(Rogue rogue, Vector3 targetPosition)
    {
        if (lifeStealEffectPrefab != null)
        {
            // ��� ��ġ�� ����Ʈ ���
            EffectManager.Instance.PlayAnimatedSpriteEffect(lifeStealEffectPrefab, targetPosition, false);

            Vector3 offset = rogue.transform.position;
            offset.y += 2f;

            // �α� ��ġ�� ����Ʈ ��� (�� ����Ʈ)
            EffectManager.Instance.PlayAnimatedSpriteEffect(healEffectPrefab, rogue.transform.position, false);
        }
    }
}