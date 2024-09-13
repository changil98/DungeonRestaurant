using UnityEngine;

public class RangerProjectile : Projectile
{
    public bool isLockOnProjectile = false;
    public BaseCombatAI lockOnTarget;
    [SerializeField] GameObject LockOnVFXonHit;

    public void InitializeLockOnProjectile(BaseCombatAI target, float damage, BaseCombatAI owner)
    {
        this.lockOnTarget = target;
        this.damage = damage;
        this.owner = owner;
        this.isLockOnProjectile = true;
        UpdateDirection();
    }

    protected override void Update()
    {
        if (!lockOnTarget.gameObject.activeSelf)
        {
            Destroy(gameObject);
        }
        lifetime += Time.deltaTime;
        if (lifetime >= maxLifetime)
        {
            Destroy(gameObject);
            return;
        }

        if (isLockOnProjectile)
        {
            if (lockOnTarget == null)
            {
                Destroy(gameObject);
                return;
            }
            direction = (lockOnTarget.transform.position - transform.position).normalized;
            transform.Translate(direction * speed * Time.deltaTime);
        }
        else
        {
            base.Update();
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        BaseCombatAI targetAI = collision.GetComponent<BaseCombatAI>();
        if (targetAI != null && targetAI != owner && targetAI.team != owner.team)
        {
            if (isLockOnProjectile && targetAI == lockOnTarget)
            {
                EffectManager.Instance.PlayAnimatedSpriteEffect(LockOnVFXonHit, targetAI.transform.position);
            }
            else if (!isLockOnProjectile && targetAI == target.GetComponent<BaseCombatAI>())
            {
                if (isCriticalHit && criticalHitEffect != null)
                {
                    EffectManager.Instance.PlayAnimatedSpriteEffect(criticalHitEffect, targetAI.transform.position, false);
                }
            }

            targetAI.TakeDamage(damage, DamageType.Physical, AttackType.Blunt, isCriticalHit);
            Destroy(gameObject);
        }
    }
}