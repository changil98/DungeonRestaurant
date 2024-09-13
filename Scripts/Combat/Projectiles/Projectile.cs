using UnityEngine;

public class Projectile : MonoBehaviour
{
    protected BaseCombatAI owner;
    protected Transform target;
    protected Vector3 direction;
    protected float damage;
    protected float speed = 10f;
    protected bool isCriticalHit = false;
    protected GameObject criticalHitEffect;
    protected SpriteRenderer spriteRenderer;
    protected float lifetime = 0f;
    protected float maxLifetime = 2f;

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    public virtual void Initialize(Transform target, float damage, BaseCombatAI owner, bool isCriticalHit = false, GameObject criticalHitEffect = null)
    {
        this.target = target;
        this.damage = damage;
        this.owner = owner;
        this.isCriticalHit = isCriticalHit;
        this.criticalHitEffect = criticalHitEffect;
        UpdateDirection();
        lifetime = 0f;
    }

    protected virtual void Update()
    {
        lifetime += Time.deltaTime;
        if (lifetime >= maxLifetime)
        {
            Destroy(gameObject);
            return;
        }

        if (!target.gameObject.activeSelf)
        {
            Destroy(gameObject);
            return;
        }

        UpdateDirection();
        transform.Translate(direction * speed * Time.deltaTime);

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget > 50f)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void UpdateDirection()
    {
        if (target != null)
        {
            direction = (target.position - transform.position).normalized;
        }
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = direction.x < 0;
            spriteRenderer.flipY = direction.y > 0;
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        BaseCombatAI targetAI = collision.GetComponent<BaseCombatAI>();
        if (targetAI != null && targetAI != owner && targetAI.team != owner.team)
        {
            targetAI.TakeDamage(damage, DamageType.Physical, AttackType.Blunt, isCriticalHit);
            if (isCriticalHit && criticalHitEffect != null)
            {
                EffectManager.Instance.PlayAnimatedSpriteEffect(criticalHitEffect, targetAI.transform.position, false);
            }
            Destroy(gameObject);
        }
    }
}