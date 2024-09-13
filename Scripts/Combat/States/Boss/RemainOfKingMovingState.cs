using UnityEngine;

public class RemainOfKingMovingState : IState
{
    private Remain_of_the_king remainOfKing;
    private Transform targetTransform;
    private float footstepTimer = 0f;
    private float footstepInterval = 0.3f;
    private bool isRunAnimationPlaying = false;

    public RemainOfKingMovingState(Remain_of_the_king character)
    {
        this.remainOfKing = character;
    }



    public void OnEnter()
    {
        if (!isRunAnimationPlaying)
        {
            remainOfKing.PlayAnimation(AnimationData.RunHash);
        }
        UpdateTarget();
    }

  
    public void Update()
    {
        if (!remainOfKing.HasEnemyEnteredRange())
        {
            if (remainOfKing.CheckTarget())
            {
                Vector2 direction = (remainOfKing.cachedNearestTarget.transform.position - remainOfKing.transform.position).normalized;
                remainOfKing.transform.Translate(direction * remainOfKing.MovementSpeed * Time.deltaTime);

                
                footstepTimer += Time.deltaTime;
                if (footstepTimer >= footstepInterval)
                {
                    CreateFootstepEffect();
                    footstepTimer = 0f; 
                }
            }
            else if (remainOfKing.cachedNearestTarget == null)
            {
                remainOfKing.UpdateNearestTarget();
            }
        }
        else
        {
            
            isRunAnimationPlaying = false;
            remainOfKing.PlayAnimation(AnimationData.IdleHash);
        }
    }

    private void CreateFootstepEffect()
    {
        if (remainOfKing.footStepEffect != null)
        {
            Vector3 effectPosition = remainOfKing.transform.position;
            EffectManager.Instance.PlayAnimatedSpriteEffect(remainOfKing.footStepEffect, effectPosition, false);
        }
    }

    private void UpdateTarget()
    {
        BaseCombatAI target = remainOfKing.cachedNearestTarget;
        targetTransform = target?.transform;
    }

    public void FixedUpdate()
    {

    }

    public void OnExit()
    {

    }

}


