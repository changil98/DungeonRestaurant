using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    private BaseCombatAI character;

    public IdleState(BaseCombatAI character)
    {
        this.character = character;
    }

    public void OnEnter()
    {
       character.PlayAnimation(AnimationData.IdleHash);       
    }

    public void OnExit()
    {
        
       
    }

    public void Update()
    {
       
    }

    public void FixedUpdate()
    {
        
    }
}