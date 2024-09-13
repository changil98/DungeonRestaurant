using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "New Rogue Acceleration Skill", menuName = "Skills/Rogue/Acceleration")]
public class Acceleration : BaseSkill
{
    public float duration = 5f; // 스킬 지속 시간
    public float dodgeChance = 0.5f; // 데미지 무시 확률 (50%)

    public override void ExecuteSkill(BaseCombatAI character)
    {
        if (character is Rogue rogue)
        {
            rogue.StartCoroutine(PerformAcceleration(rogue));
        }
    }

    private IEnumerator PerformAcceleration(Rogue rogue)
    {
        // 스킬 시작 이펙트
        PlayAccelerationEffect(rogue);
        SoundManager.Instance.PlaySound("SFX_RogueBuff");
        // 스킬 애니메이션 시작
        rogue.PlayAnimation(AnimationData.Skill_NormalHash);

        // 데미지 무시 효과 활성화
        rogue.SetAccelerationActive(true);

        // 지속 시간 동안 대기
        yield return new WaitForSeconds(duration);

        // 데미지 무시 효과 비활성화
        rogue.SetAccelerationActive(false);
    }

    private void PlayAccelerationEffect(Rogue rogue)
    {
        if (skillEffectObject != null)
        {
            Vector3 effectPos = rogue.transform.position;
            Vector3 offset = effectPos - rogue.transform.position;
            offset.y += 0.5f;
            EffectManager.Instance.PlayFollowingAnimatedSpriteEffect(skillEffectObject, rogue.transform,offset , false);
        }
    }
}