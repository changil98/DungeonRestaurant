public class WarriorSkillState : IState
{
    private Warrior warrior;
    private bool isSkillExecuted = false;
 
    public WarriorSkillState(Warrior warrior)
    {
        this.warrior = warrior;       
    }

    public void OnEnter()
    {
        isSkillExecuted = false;
        warrior.UseSkill();     
    }

    public void Update()
    {
       if(!isSkillExecuted && warrior.isSkillAnimationComplete)
        {
            isSkillExecuted = true;
            warrior.isSkillAnimationComplete = false;
        }
    }

    public void FixedUpdate() { }

    public void OnExit()
    {
        isSkillExecuted = false;
        warrior.isSkillAnimationComplete = false;
    }

    public bool IsSkillCompleted()
    {
        return isSkillExecuted;
    }
}
