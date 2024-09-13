using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeIdleState : IState
{
    private CaptainSlime slime;
    private float timer;

    public SlimeIdleState(CaptainSlime slime)
    {
        this.slime = slime;
    }

    public void OnEnter()
    {
        timer = 0f;
        slime.PlayAnimation(AnimationData.IdleHash,true);
    }

    public void Update()
    {
        timer += Time.deltaTime;
    }

    public bool ShouldMove()
    {
        return timer >= slime.idleStateDuration && slime.IsEnemyInRange();
    }

    public void FixedUpdate() { }
    public void OnExit() { }
}
