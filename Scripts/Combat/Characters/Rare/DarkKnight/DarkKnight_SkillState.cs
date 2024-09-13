public class DarkKnight_SkillState : IState
{
    private DarkKnight dk;
    private bool isSkillExecuted = false;

    public DarkKnight_SkillState(DarkKnight dk)
    {
        this.dk = dk;
    }

    public void OnEnter()
    {
        isSkillExecuted = false;
        dk.UseSkill();

    }

    public void Update()
    {
        if (!isSkillExecuted && dk.isSkillAnimationComplete)
        {
            isSkillExecuted = true;
            dk.isSkillAnimationComplete = false;
        }
    }

    public void FixedUpdate() { }

    public void OnExit()
    {
        isSkillExecuted = false;
        dk.isSkillAnimationComplete = false;
    }

    public bool IsSkillCompleted()
    {
        return isSkillExecuted;
    }
}