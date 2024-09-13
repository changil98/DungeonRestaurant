using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "New Rogue Acceleration Skill", menuName = "Skills/Rogue/Acceleration")]
public class Acceleration : BaseSkill
{
    public float duration = 5f; // ��ų ���� �ð�
    public float dodgeChance = 0.5f; // ������ ���� Ȯ�� (50%)

    public override void ExecuteSkill(BaseCombatAI character)
    {
        if (character is Rogue rogue)
        {
            rogue.StartCoroutine(PerformAcceleration(rogue));
        }
    }

    private IEnumerator PerformAcceleration(Rogue rogue)
    {
        // ��ų ���� ����Ʈ
        PlayAccelerationEffect(rogue);
        SoundManager.Instance.PlaySound("SFX_RogueBuff");
        // ��ų �ִϸ��̼� ����
        rogue.PlayAnimation(AnimationData.Skill_NormalHash);

        // ������ ���� ȿ�� Ȱ��ȭ
        rogue.SetAccelerationActive(true);

        // ���� �ð� ���� ���
        yield return new WaitForSeconds(duration);

        // ������ ���� ȿ�� ��Ȱ��ȭ
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