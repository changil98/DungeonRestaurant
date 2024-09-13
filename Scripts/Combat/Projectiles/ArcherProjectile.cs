using UnityEngine;

public class ArcherProjectile : Projectile
{
    public GameObject skillHitEffect;
    private bool isSkillProjectile = false;

    public void InitializeSkillProjectile(Vector3 direction, float damage, BaseCombatAI owner)
    {
        this.direction = direction.normalized;
        this.damage = damage;
        this.isSkillProjectile = true;
        this.speed = 15f;
        this.owner = owner;
        UpdateDirection();
    }

    protected override void Update()
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
            base.Update();
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        BaseCombatAI targetAI = collision.GetComponent<BaseCombatAI>();
        if (targetAI != null && targetAI != owner && targetAI.team != owner.team)
        {
            // ������ ����
            targetAI.TakeDamage(damage, DamageType.Physical, AttackType.Blunt, isCriticalHit);

            // ����Ʈ ���
            GameObject effectToPlay = isSkillProjectile ? skillHitEffect : (isCriticalHit ? criticalHitEffect : null);
            if (effectToPlay != null)
            {
                EffectManager.Instance.PlayAnimatedSpriteEffect(effectToPlay, targetAI.transform.position, false);
            }

            // ����ü �ı�
            Destroy(gameObject);
        }
    }
}
