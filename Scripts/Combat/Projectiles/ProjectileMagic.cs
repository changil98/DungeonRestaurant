using UnityEngine;

public class ProjectileMagic : MonoBehaviour
{
    private BaseCombatAI owner;
    private Transform target;
    private Vector3 direction;
    private float damage;
    private float speed = 10f;
    private bool isSkillProjectile = false;
    private bool isCriticalHit = false;
    private GameObject criticalHitEffect;
    public GameObject skillHitEffect;
    protected float lifetime = 0f;
    protected float maxLifetime = 2f;

    public void Initialize(Transform target, float damage, BaseCombatAI owner, bool isCriticalHit = false, GameObject criticalHitEffect = null)
    {
        this.target = target;
        this.damage = damage;
        this.owner = owner;
        this.isCriticalHit = isCriticalHit;
        this.criticalHitEffect = criticalHitEffect;
        this.isSkillProjectile = false;
    }

    public void InitializeSkillProjectile(Vector3 direction, float damage, BaseCombatAI owner)
    {
        this.direction = direction.normalized;
        this.damage = damage;
        this.isSkillProjectile = true;
        this.speed = 15f;
        this.owner = owner;
    }

    private void Update()
    {
        lifetime += Time.deltaTime;
        if (lifetime >= maxLifetime)
        {
            Destroy(gameObject);
            return;
        }

        if (isSkillProjectile)
        {
            transform.Translate(direction * speed * Time.deltaTime);
            if (!GetComponent<Renderer>().isVisible)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            if (!target.gameObject.activeSelf)
            {
                Destroy(gameObject);
                return;
            }
            direction = (target.position - transform.position).normalized;
            transform.Translate(direction * speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        BaseCombatAI targetAI = collision.GetComponent<BaseCombatAI>();
        if (targetAI != null && targetAI != owner && targetAI.team != owner.team)
        {
            if (isSkillProjectile)
            {
                targetAI.TakeDamage(damage, DamageType.Magical, AttackType.Blunt, false);
                if (skillHitEffect != null)
                {
                    EffectManager.Instance.PlayAnimatedSpriteEffect(skillHitEffect, targetAI.transform.position, false);
                }
            }
            else
            {
                targetAI.TakeDamage(damage, DamageType.Magical, AttackType.Blunt, isCriticalHit);
                if (isCriticalHit && criticalHitEffect != null)
                {
                    EffectManager.Instance.PlayAnimatedSpriteEffect(criticalHitEffect, targetAI.transform.position, false);
                }
            }

            Destroy(gameObject);
        }
    }
}