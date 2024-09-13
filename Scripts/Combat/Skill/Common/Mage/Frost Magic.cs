using UnityEngine;

[CreateAssetMenu(fileName = "New Mage Frost Magic Skill", menuName = "Skills/Mage/Frost Magic")]
public class FrostMagic : BaseSkill
{
    public float damageMultiplier = 2f;
    public float stunDuration = 2f;


    public override void ExecuteSkill(BaseCombatAI character)
    {
        if (character is Mage mage)
        {
            if (mage.cachedNearestTarget != null)
            {
                // 스킬 애니메이션 재생
                mage.PlayAnimation(AnimationData.Skill_MagicHash);
                SoundManager.Instance.PlaySound("SFX_MagicSkill");
                // 대상에게 피해 입히기
                float damage = mage.AttackDamage * damageMultiplier;
                SoundManager.Instance.PlaySound("SFX_FrostMagicHit");
                mage.cachedNearestTarget.TakeDamage(damage,DamageType.Magical,AttackType.Blunt,false);

                // 대상 기절시키기
                if (mage.cachedNearestTarget is BaseCombatAI targetAI)
                {
                    targetAI.ApplyStun(stunDuration);
                }

                // 프로스트 이펙트 재생
                PlayFrostEffect(mage.cachedNearestTarget.transform);             
            }

           
        }
    }

    private void PlayFrostEffect(Transform target)
    {
        if (skillEffectObject != null)
        {
            Vector3 effectPos = target.position;
            Vector3 offset = effectPos - target.position;
            offset.y += 1.2f;
            EffectManager.Instance.PlayFollowingAnimatedSpriteEffect(skillEffectObject,target, offset, false);
        }
    }   
}