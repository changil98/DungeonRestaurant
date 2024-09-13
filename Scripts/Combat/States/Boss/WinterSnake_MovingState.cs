using UnityEngine;

public class WinterSnake_MovingState : IState
{
    private WinterSnake winterSnake;
    private Transform targetTransform;
    private float footstepTimer = 0f;
    private float footstepInterval = 0.3f;
    private bool isRunAnimationPlaying = false;

    public WinterSnake_MovingState(WinterSnake winterSnake)
    {
        this.winterSnake = winterSnake;
    }

    public void OnEnter()
    {
        if (!isRunAnimationPlaying)
        {
            winterSnake.PlayAnimation(AnimationData.RunHash);
        }
        UpdateTarget();
    }


    public void Update()
    {
        if (!winterSnake.HasEnemyEnteredRange())
        {
            if (winterSnake.CheckTarget())
            {
                Vector2 direction = (winterSnake.cachedNearestTarget.transform.position - winterSnake.transform.position).normalized;
                winterSnake.transform.Translate(direction * winterSnake.MovementSpeed * Time.deltaTime);


                footstepTimer += Time.deltaTime;
                if (footstepTimer >= footstepInterval)
                {
                    CreateFootstepEffect();
                    footstepTimer = 0f;
                }
            }
            else if (winterSnake.cachedNearestTarget == null)
            {
                winterSnake.UpdateNearestTarget();
            }
        }
        else
        {

            isRunAnimationPlaying = false;
            winterSnake.PlayAnimation(AnimationData.IdleHash);
        }
    }

    private void CreateFootstepEffect()
    {
        if (winterSnake.footStepEffect != null)
        {
            Vector3 effectPosition = winterSnake.transform.position;
            EffectManager.Instance.PlayAnimatedSpriteEffect(winterSnake.footStepEffect, effectPosition, false);
        }
    }

    private void UpdateTarget()
    {
        BaseCombatAI target = winterSnake.cachedNearestTarget;
        targetTransform = target?.transform;
    }

    public void FixedUpdate()
    {

    }

    public void OnExit()
    {

    }

}
