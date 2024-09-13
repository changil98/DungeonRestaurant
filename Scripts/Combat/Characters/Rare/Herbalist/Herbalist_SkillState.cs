public class Herbalist_SkillState : IState
{
    private Herbalist character;
    private bool isSkillExecuted = false;

    public Herbalist_SkillState(Herbalist character)
    {
        this.character = character;
    }

    public void OnEnter()
    {
        isSkillExecuted = true;
        character.UseSkill();
    }

    public void Update()
    {
        if (!isSkillExecuted && character.isSkillAnimationComplete)
        {
            isSkillExecuted = true;
            character.isSkillAnimationComplete = false;
        }
    }

    public void FixedUpdate() { }

    public void OnExit()
    {
        isSkillExecuted = false;
        character.isSkillAnimationComplete = false;
    }

    public bool IsSkillCompleted()
    {
        return isSkillExecuted;
    }
}
