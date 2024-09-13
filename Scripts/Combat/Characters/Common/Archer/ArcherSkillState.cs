public class ArcherSkillState : IState
{
    private Archer archer;
    private bool isSkillExecuted = false;
    private bool hasCheckedTargetAfterSkill = false;

    public ArcherSkillState(Archer archer)
    {
        this.archer = archer;
    }

    public void OnEnter()
    {
        isSkillExecuted = false;
        hasCheckedTargetAfterSkill = false;
        archer.UseSkill();
    }

    public void Update()
    {
        if (!isSkillExecuted && archer.isSKillAnimationComplete)
        {
            isSkillExecuted = true;
            archer.isSKillAnimationComplete = false;
        }

        if (isSkillExecuted && !hasCheckedTargetAfterSkill)
        {
            archer.UpdateTargetIfOutOfRange();
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
        archer.isSKillAnimationComplete = false;
    }

    public bool IsSkillCompleted()
    {
        return isSkillExecuted && hasCheckedTargetAfterSkill;
    }
}