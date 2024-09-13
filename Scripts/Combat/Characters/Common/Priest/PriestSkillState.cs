using UnityEngine;

public class PriestSkillState : IState
{
    private Priest priest;
    private bool isSkillExecuted = false;

    public PriestSkillState(Priest priest)
    {
        this.priest = priest;
    }

    public void OnEnter()
    {
        isSkillExecuted = false;
        priest.UseSkill();
    }

    public void Update()
    {
        if (!isSkillExecuted && priest.isSkillAnimationComplete)
        {
            isSkillExecuted = true;
            priest.isSkillAnimationComplete = false;
        }
    }

    public void FixedUpdate() { }

    public void OnExit()
    {
        isSkillExecuted = false;
        priest.isSkillAnimationComplete = false;
    }

    public bool IsSkillCompleted()
    {
        return isSkillExecuted;
    }
}
