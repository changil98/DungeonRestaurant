public class SlimeSkillState : IState
{
    private CaptainSlime slime;
    private bool isSkillCompleted = false;

    public SlimeSkillState(CaptainSlime slime)
    {
        this.slime = slime;

    }
    public void OnEnter()
    {
        isSkillCompleted = false;
        slime.UseSkill();
    }


    public void Update()
    {
        if(!isSkillCompleted && slime.isSkillAnimationComplete)
        {
            isSkillCompleted = true;
            slime.isSkillAnimationComplete = false;
        }
    }

    public void FixedUpdate()
    {
    }


    public void OnExit()
    {
        isSkillCompleted = false;
        slime.isSkillAnimationComplete = false;
    }

    public bool IsSkillCompleted()
    {
        return isSkillCompleted;
    }

}
