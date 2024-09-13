using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MagicEater", menuName = "Skills/RuneBlade/MagicEater")]
public class MagicEater : BaseSkill
{
    [SerializeField] private float damageMultiplier = 2f;
    [SerializeField] private float resistanceIncrease = 30f;
    [SerializeField] private float passiveManaGain = 20f;
    [SerializeField] private float resistanceDuration = 5f;
    [SerializeField] private GameObject ResistanceUPVFX;
    private bool isResistanceIncreased = false;

    public override void ExecuteSkill(BaseCombatAI character)
    {
        if (character is RuneBlade runeBlade)
        {
            runeBlade.PlayAnimation(AnimationData.Skill_NormalHash);
            SoundManager.Instance.PlaySound("SFX_DaggerSwing");
            runeBlade.StartCoroutine(ApplySkillEffects(runeBlade));
        }
    }

    private IEnumerator ApplySkillEffects(RuneBlade runeBlade)
    {
        if (runeBlade.cachedNearestTarget != null && skillEffectObject != null)
        {
            float totalDamage = runeBlade.AttackDamage * damageMultiplier;
            float damagePerHit = totalDamage / 2f;

            Vector3 effectPos = runeBlade.cachedNearestTarget.transform.position;
            Vector3 offset = runeBlade.cachedNearestTarget.transform.position - effectPos;
            GameObject effectInstance = EffectManager.Instance.PlayFollowingAnimatedSpriteEffect(
                skillEffectObject, runeBlade.cachedNearestTarget.transform, offset, runeBlade.IsFacingRight);

            Animator effectAnimator = effectInstance.GetComponent<Animator>();
            if (effectAnimator != null)
            {
                float animationLength = GetAnimationLength(effectAnimator);

                // 첫 번째 공격 (10% 지점)
                yield return new WaitForSeconds(animationLength * 0.1f);
                SoundManager.Instance.PlaySound("SFX_SkillSlash");
                runeBlade.cachedNearestTarget.TakeDamage(damagePerHit, DamageType.Magical, AttackType.Slash, false);

                // 두 번째 공격 (60% 지점)
                yield return new WaitForSeconds(animationLength * 0.5f);
                SoundManager.Instance.PlaySound("SFX_SkillSlash");
                runeBlade.cachedNearestTarget.TakeDamage(damagePerHit, DamageType.Magical, AttackType.Slash, false);

                // 애니메이션이 완전히 끝날 때까지 대기
                yield return new WaitForSeconds(animationLength * 0.4f);
            }
        }

        // 저항력 증가 효과 적용
        runeBlade.StartCoroutine(IncreaseResistanceTemporarily(runeBlade));
    }

    private IEnumerator IncreaseResistanceTemporarily(RuneBlade runeBlade)
    {
        if (isResistanceIncreased)
        {
            yield break;
        }

        Vector3 effectPos = runeBlade.transform.position;
        Vector3 offset = runeBlade.transform.position - effectPos;
        EffectManager.Instance.PlayFollowingAnimatedSpriteEffect(ResistanceUPVFX, runeBlade.transform, offset, false);

        float originalResistance = runeBlade.Resistance;
        runeBlade.Resistance += resistanceIncrease;
        isResistanceIncreased = true;

        yield return new WaitForSeconds(resistanceDuration);

        runeBlade.Resistance = originalResistance;
        isResistanceIncreased = false;
    }

    public void OnMagicDamageReceived(RuneBlade runeBlade)
    {
        runeBlade.GainMana(passiveManaGain);
    }

    private float GetAnimationLength(Animator animator)
    {
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        if (clipInfo.Length > 0)
        {
            return clipInfo[0].clip.length;
        }
        return 1f; // 기본값 반환
    }
}
