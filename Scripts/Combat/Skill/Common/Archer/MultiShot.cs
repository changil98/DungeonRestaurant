using UnityEngine;

[CreateAssetMenu(fileName = "New Archer MultiShot Skill", menuName = "Skills/Archer/MultiShot")]
public class MultiShot : BaseSkill
{
    public float damageMultiplier;
    public int numberOfShots = 5;
    public float skillSpreadAngle = 45f;

    public override void ExecuteSkill(BaseCombatAI character)
    {
        if (character is Archer archer && archer.cachedNearestTarget != null)
        {
            archer.PlayAnimation(AnimationData.Skill_BowHash);
            SoundManager.Instance.PlaySound("SFX_RangeAttack");
            EffectManager.Instance.PlayAnimatedSpriteEffect(skillEffectObject, archer.pivotTransform.position, false);

            Vector3 targetDirection = (archer.cachedNearestTarget.transform.position - archer.transform.position).normalized;
            float baseAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;

            float angleStep = skillSpreadAngle / (numberOfShots - 1);
            float startAngle = baseAngle - skillSpreadAngle / 2;

            for (int i = 0; i < numberOfShots; i++)
            {
                float angle = startAngle + (angleStep * i);
                Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector3.right;

                GameObject projectile = Instantiate(archer.skillProjectilePrefab, archer.pivotTransform.position, Quaternion.identity);
                ArcherProjectile projectileComponent = projectile.GetComponent<ArcherProjectile>();

                if (projectileComponent != null)
                {
                    projectileComponent.InitializeSkillProjectile(direction, archer.AttackDamage * damageMultiplier, archer);
                }
            }
        }
    }
}
