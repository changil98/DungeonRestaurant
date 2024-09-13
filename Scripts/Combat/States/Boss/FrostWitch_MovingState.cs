using UnityEngine;

public class FrostWitch_MovingState : IState
{
    private FrostWitch frostWitch;
    private Transform targetTransform;
    private float footstepTimer = 0f;
    private float footstepInterval = 0.3f;
    private bool isRunAnimationPlaying = false;

    public FrostWitch_MovingState(FrostWitch frostWitch)
    {
        this.frostWitch = frostWitch;
    }

    public void OnEnter()
    {
        if (!isRunAnimationPlaying)
        {
            frostWitch.PlayAnimation(AnimationData.RunHash);
        }
        UpdateTarget();
    }


    public void Update()
    {
        if (!frostWitch.HasEnemyEnteredRange())
        {
            if (frostWitch.CheckTarget())
            {
                Vector2 direction = (frostWitch.cachedNearestTarget.transform.position - frostWitch.transform.position).normalized;
                frostWitch.transform.Translate(direction * frostWitch.MovementSpeed * Time.deltaTime);


                footstepTimer += Time.deltaTime;
                if (footstepTimer >= footstepInterval)
                {
                    CreateFootstepEffect();
                    footstepTimer = 0f;
                }
            }
            else if (frostWitch.cachedNearestTarget == null)
            {
                frostWitch.UpdateNearestTarget();
            }
        }
        else
        {

            isRunAnimationPlaying = false;
            frostWitch.PlayAnimation(AnimationData.IdleHash);
        }
    }

    private void CreateFootstepEffect()
    {
        if (frostWitch.footStepEffect != null)
        {
            Vector3 effectPosition = frostWitch.transform.position;
            EffectManager.Instance.PlayAnimatedSpriteEffect(frostWitch.footStepEffect, effectPosition, false);
        }
    }

    private void UpdateTarget()
    {
        BaseCombatAI target = frostWitch.cachedNearestTarget;
        targetTransform = target?.transform;
    }

    public void FixedUpdate()
    {

    }

    public void OnExit()
    {

    }

}
