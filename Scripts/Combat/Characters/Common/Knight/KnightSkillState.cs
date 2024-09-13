public class KnightSkillState : IState
{
    private Knight knight;
    private bool isSkillExecuted = false;

    public KnightSkillState(Knight knight)
    {
        this.knight = knight;
    }

    public void OnEnter()
    {
        isSkillExecuted = false;
        knight.UseSkill();
    }

    public void Update()
    {
        if (!isSkillExecuted && knight.isSkillAnimationComplete)
        {
            isSkillExecuted = true;
            knight.isSkillAnimationComplete = false;
        }
    }

    public void FixedUpdate() { }

    public void OnExit()
    {
        isSkillExecuted = false;
        knight.isSkillAnimationComplete = false;
    }

    public bool IsSkillCompleted()
    {
        return isSkillExecuted;
    }
}
