using System.Collections;
using UnityEngine;

public class RangedAttackingState : IState
{
    private BaseCombatAI character;
    private float firstAttackTimer;
    private float attackTimer;
    private float attackInterval;
    

    public RangedAttackingState(BaseCombatAI character)
    {
        this.character = character;
    }

    public void OnEnter()
    {
        if (firstAttackTimer >= attackInterval)
        {
            character.Attack();
            firstAttackTimer = 0;
        }
        else
        {
            firstAttackTimer += Time.deltaTime;
            character.PlayAnimation(AnimationData.IdleHash);
        }

        UpdateAttackInterval();    
        attackTimer = 0;     
    }

    public void OnExit() 
    {
        if (firstAttackTimer < attackInterval)
        {
            firstAttackTimer += Time.deltaTime;
        }
    }

    public void Update() 
    {
        if (character.CheckTarget())
        {
            attackTimer += Time.deltaTime;

            if (attackTimer >= attackInterval)
            {
                UpdateAttackInterval();
                character.Attack();
                attackTimer = 0; // 타이머를 0으로 리셋
            }
        }
        else
        {
            character.PlayAnimation(AnimationData.IdleHash,false);
        }
    }

    public void FixedUpdate()
    {
        
    }

    private void UpdateAttackInterval()
    {
        float newInterval = 1f / character.CalculateFinalAttackSpeed();
        if (!Mathf.Approximately(newInterval, attackInterval))
        {
            attackInterval = newInterval;
        }
    }
}