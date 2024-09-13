using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieState : IState
{
    private BaseCombatAI character;

    public DieState(BaseCombatAI character)
    {
        this.character = character;
    }

    public void OnEnter()
    {
        if(character.team == Team.Ally)
        {
            character.PlayAnimation(AnimationData.DeadHash);
        } 
        //character.Die();
    }

    public void OnExit() { }
    public void Update() { }
    public void FixedUpdate() { }
}
