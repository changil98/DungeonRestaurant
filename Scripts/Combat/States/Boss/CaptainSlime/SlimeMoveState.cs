using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeMoveState : IState
{
    private CaptainSlime slime;
    private float timer;
    private Vector2 targetPosition;

    public SlimeMoveState(CaptainSlime slime)
    {
        this.slime = slime;
    }

    public void OnEnter()
    {
        timer = 0f;
        targetPosition = slime.GetNextMovePosition(slime.MovementSpeed * slime.moveStateDuration);
        slime.PlayAnimation(AnimationData.RunHash);
    }

    public void Update()
    {
        timer += Time.deltaTime;
        slime.transform.position = Vector2.MoveTowards(slime.transform.position, targetPosition, slime.MovementSpeed * Time.deltaTime);
    }

    public bool IsFinished()
    {
        return timer >= slime.moveStateDuration || Vector2.Distance(slime.transform.position, targetPosition) < 0.1f;
    }

    public void FixedUpdate() { }
    public void OnExit() { }
}