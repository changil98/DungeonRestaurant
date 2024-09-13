public class BerserkerSkillState : IState
{
    private Berserker berserker;
    private bool isSkillExecuted = false;

    public BerserkerSkillState(Berserker berserker)
    {
        this.berserker = berserker;
    }

    public void OnEnter()
    {
        isSkillExecuted = false;
        berserker.UseSkill();

    }

    public void Update()
    {
        if (!isSkillExecuted && berserker.isSkillAnimationComplete)
        {
            isSkillExecuted = true;
            berserker.isSkillAnimationComplete = false;
        }
    }

    public void FixedUpdate() { }

    public void OnExit()
    {
        isSkillExecuted = false;
        berserker.isSkillAnimationComplete = false;
    }

    public bool IsSkillCompleted()
    {
        return isSkillExecuted;
    }
}
