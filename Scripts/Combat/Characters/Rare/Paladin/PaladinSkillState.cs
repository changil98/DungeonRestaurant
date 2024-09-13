public class PaladinSkillState : IState
{
    private Paladin character;
    private bool isSkillExecuted = false;

    public PaladinSkillState(Paladin character)
    {
        this.character = character;
    }

    public void OnEnter()
    {
        isSkillExecuted = false;
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
