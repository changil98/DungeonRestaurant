using UnityEngine;

[CreateAssetMenu(fileName = "New Knight Resist Skill", menuName = "Skills/Knight/Resist")]
public class Resist : BaseSkill
{
    public float healMultiplier = 0.4f;
    public GameObject healEffectPrefab; // 힐 효과 프리팹

    public override void ExecuteSkill(BaseCombatAI character)
    {
        if (character is Knight knight)
        {
            // 잃은 체력 계산
            float lostHealth = knight.MaxHP - knight.CurrentHP;

            // 회복량 계산
            float healAmount = lostHealth * healMultiplier;

            // 체력 회복
            knight.Heal(healAmount);

            // 스킬 애니메이션 재생
            knight.PlayAnimation(AnimationData.Skill_NormalHash);
            SoundManager.Instance.PlaySound("SFX_HolySkill");
            // 스킬 이펙트 재생
            PlaySkillEffect(knight);

            // 힐 이펙트 재생
            PlayHealEffect(knight);
        }
    }

    private void PlaySkillEffect(Knight knight)
    {
        if (skillEffectObject != null)
        {
            Vector3 effectPosition = knight.transform.position;
            effectPosition.y += 2f; // 이펙트를 캐릭터 위에 표시
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