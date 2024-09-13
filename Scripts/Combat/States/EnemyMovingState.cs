using UnityEngine;

public class EnemyMovingState : IState
{
    private BaseCombatAI owner;
    private Transform targetTransform;
    private float footstepTimer = 0f;
    private float footstepInterval = 0.3f;
    private bool isRunAnimationPlaying = false;
    public EnemyMovingState(BaseCombatAI owner)
    {
        this.owner = owner;
    }
    public void OnEnter()
    {
        if (!isRunAnimationPlaying)
        {
            owner.PlayAnimation(AnimationData.RunHash);
        }
        UpdateTarget();
    }
    public void Update()
    {
        if (owner.CheckTarget())
        {
            Vector2 direction = (owner.cachedNearestTarget.transform.position - owner.transform.position).normalized;
            owner.transform.Translate(direction * owner.MovementSpeed * Time.deltaTime);

            // ���ڱ� ����Ʈ ���� Ÿ�̸� ������Ʈ
            footstepTimer += Time.deltaTime;
            if (footstepTimer >= footstepInterval)
            {
                CreateFootstepEffect();
                footstepTimer = 0f; // Ÿ�̸� ����
            }
        }
        else if (owner.cachedNearestTarget = null)
        {
            owner.UpdateNearestTarget();
        }
    }
    private void CreateFootstepEffect()
    {
        if (owner.footStepEffect != null)
        {
            Vector3 effectPosition = owner.transform.position;

            EffectManager.Instance.PlayAnimatedSpriteEffect(owner.footStepEffect, effectPosition, false);
        }
    }
    private void UpdateTarget()
    {
        BaseCombatAI target = owner.cachedNearestTarget;
        targetTransform = target?.transform;
    }
    public void OnExit()
    {

    }
    public void FixedUpdate()
    {

    }
}


