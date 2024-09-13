using UnityEngine;

public class PoisonProjectile : MonoBehaviour
{
    private BaseCombatAI owner;
    private Transform target;
    private float damage;
    private float speed = 10f;
    private Color poisonColor;
    private float poisonDuration;
    private float poisonTickDamageMultiplier;
    protected float lifetime = 0f;
    protected float maxLifetime = 2f;

    public void Initialize(Transform target, float damage, BaseCombatAI owner, Color poisonColor, float poisonDuration, float poisonTickDamageMultiplier)
    {
        this.target = target;
        this.damage = damage;
        this.owner = owner;
        this.poisonColor = poisonColor;
        this.poisonDuration = poisonDuration;
        this.poisonTickDamageMultiplier = poisonTickDamageMultiplier;
    }

    private void Update()
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

        Vector3 direction = (target.position - transform.position).normalized;
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        BaseCombatAI targetAI = collision.GetComponent<BaseCombatAI>();
        if (targetAI != null && targetAI != owner && targetAI.team != owner.team)
        {
            targetAI.TakeDamage(damage, DamageType.Physical, AttackType.Blunt, false);
            targetAI.ApplyPoisonEffect(poisonColor, poisonDuration, owner.AttackDamage * poisonTickDamageMultiplier);
            Destroy(gameObject);
        }
    }
}