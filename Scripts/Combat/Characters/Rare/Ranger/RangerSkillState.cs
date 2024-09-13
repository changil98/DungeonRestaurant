public class RangerSkillState : IState
{
    private Ranger ranger;
    private bool isSkillExecuted = false;
    private bool hasCheckedTargetAfterSkill = false;

    public RangerSkillState(Ranger ranger)
    {
        this.ranger = ranger;
    }

    public void OnEnter()
    {
        isSkillExecuted = false;
        hasCheckedTargetAfterSkill = false;
        ranger.UseSkill();
    }

    public void Update()
    {
        if (!isSkillExecuted && ranger.isSKillAnimationComplete)
        {
            isSkillExecuted = true;
            ranger.isSKillAnimationComplete = false;
        }

        if (isSkillExecuted && !hasCheckedTargetAfterSkill)
        {
            ranger.UpdateTargetIfOutOfRange();
            hasCheckedTargetAfterSkill = true;
        }
    }

    public void FixedUpdate()
    {
        // 필요한 경우 여기에 FixedUpdate 로직 추가
    }

    public void OnExit()
    {
        isSkillExecuted = false;
        hasCheckedTargetAfterSkill = false;
        ranger.isSKillAnimationComplete = false;
    }

    public bool IsSkillCompleted()
    {
        return isSkillExecuted && hasCheckedTargetAfterSkill;
    }
}
