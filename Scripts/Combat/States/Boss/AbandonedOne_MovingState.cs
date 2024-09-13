using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbandonedOne_MovingState : IState
{
    private AbandonedOne abandonedOne;
    private Transform targetTransform;
    private float footstepTimer = 0f;
    private float footstepInterval = 0.3f;
    private bool isRunAnimationPlaying = false;

    public AbandonedOne_MovingState(AbandonedOne character) 
    { 
        abandonedOne = character;
    }

  

    public void OnEnter()
    {
        if (!isRunAnimationPlaying)
        {
            abandonedOne.PlayAnimation(AnimationData.RunHash);
        }
        UpdateTarget();
    }

  
    public void Update()
    {
        if (!abandonedOne.HasEnemyEnteredRange())
        {
            if (abandonedOne.CheckTarget())
            {
                Vector2 direction = (abandonedOne.cachedNearestTarget.transform.position - abandonedOne.transform.position).normalized;
                abandonedOne.transform.Translate(direction * abandonedOne.MovementSpeed * Time.deltaTime);

                // Update footstep effect timer
                footstepTimer += Time.deltaTime;
                if (footstepTimer >= footstepInterval)
                {
                    CreateFootstepEffect();
                    footstepTimer = 0f; // Reset timer
                }
            }
            else if (abandonedOne.cachedNearestTarget == null)
            {
                abandonedOne.UpdateNearestTarget();
            }
        }
        else
        {
            isRunAnimationPlaying = false;
            abandonedOne.PlayAnimation(AnimationData.IdleHash);
        }
    }

    private void CreateFootstepEffect()
    {
        if (abandonedOne.footStepEffect != null)
        {
            Vector3 effectPosition = abandonedOne.transform.position;
            EffectManager.Instance.PlayAnimatedSpriteEffect(abandonedOne.footStepEffect, effectPosition, false);
        }
    }

    private void UpdateTarget()
    {
        BaseCombatAI target = abandonedOne.cachedNearestTarget;
        targetTransform = target?.transform;
    }

    public void FixedUpdate()
    {

    }

    public void OnExit()
    {

    }

}
