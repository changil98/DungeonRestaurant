public class RogueSkillState : IState
{
    private Rogue rogue;
    private bool isSkillExecuted = false;

    public RogueSkillState(Rogue rogue)
    {
        this.rogue = rogue;
    }
    
    public void OnEnter()
    {
        isSkillExecuted = false;
        rogue.UseSkill();
    }

    public void Update()
    {
        if (!isSkillExecuted && rogue.isSkillAnimationComplete)
        {
            isSkillExecuted = true;
            rogue.isSkillAnimationComplete = false;
        }
    }

    public void FixedUpdate() { }

    public void OnExit()
    {
        isSkillExecuted = false;
        rogue.isSkillAnimationComplete = false;
    }

    public bool IsSkillCompleted()
    {
        return isSkillExecuted;
    }
}
