//using UnityEditor.Presets;
using UnityEngine;

public class MageSkillState : IState
{
    private Mage mage;
    private bool isSkillExecuted = false;

    public MageSkillState(Mage mage)
    {
        this.mage = mage;
    }

    public void OnEnter()
    {
        isSkillExecuted = true;
        mage.UseSkill();
    }

    public void Update()
    {
        if (!isSkillExecuted &&mage.isSkillAnimationComplete)
        {
            isSkillExecuted = true;
            mage.isSkillAnimationComplete = false;
        }
    }

    public void FixedUpdate() { }

    public void OnExit()
    {
        isSkillExecuted = false;
        mage.isSkillAnimationComplete = false;
    }

    public bool IsSkillCompleted()
    {
        return isSkillExecuted;
    }
}
