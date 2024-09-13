using UnityEngine;

public static class AnimationData
{
    //SPUM
    public static readonly int IdleHash = Animator.StringToHash("Idle");
    public static readonly int RunHash = Animator.StringToHash("Run");
    public static readonly int Attack_BowHash = Animator.StringToHash("2_Attack_Bow");
    public static readonly int Attack_MagicHash = Animator.StringToHash("2_Attack_Magic");
    public static readonly int Attack_NormalHash = Animator.StringToHash("Attack");
    public static readonly int Debuff_StunHash = Animator.StringToHash("3_Debuff_Stun");
    public static readonly int DeadHash = Animator.StringToHash("4_Death");
    public static readonly int Skill_BowHash = Animator.StringToHash("5_Skill_Bow");
    public static readonly int Skill_MagicHash = Animator.StringToHash("5_Skill_Magic");
    public static readonly int Skill_NormalHash = Animator.StringToHash("5_Skill_Normal");

    public const float crossFadeDuration = 0.1f;

    public static float GetAnimationLength(Animator animator, int animationHash)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if(stateInfo.shortNameHash == animationHash)
        {
            return stateInfo.length;
        }
        else
        {
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            foreach(AnimationClip clip in clips)
            {
                if(Animator.StringToHash(clip.name) == animationHash)
                {
                    return clip.length;
                }
            }
        }

        return 1f;
    }
}
