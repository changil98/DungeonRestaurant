using UnityEngine;

[CreateAssetMenu(fileName = "New Warrior Bash Skill", menuName = "Skills/Warrior/Bash")]
public class Bash : BaseSkill
{
    public float damageMultiplier;
    public float stunDuration;
    public float areaOfEffect = 5f;

    public override void ExecuteSkill(BaseCombatAI character)
    {
        if (character is Warrior warrior)
        {
            warrior.PlayAnimation(AnimationData.Skill_NormalHash);
            SoundManager.Instance.PlaySound("SFX_WarriorSkill1,2");
            Vector3 targetPos = warrior.cachedNearestTarget.transform.position;
            targetPos.y += 0.8f;

            EffectManager.Instance.PlayAnimatedSpriteEffect
                (skillEffectObject, targetPos, false);

            // 광역 대미지 및 기절 적용
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(warrior.transform.position, areaOfEffect, LayerMask.GetMask("Character"));

            foreach (Collider2D enemyCollider in hitEnemies)
            {
                BaseCombatAI enemy = enemyCollider.GetComponent<BaseCombatAI>();
                if (enemy != null && enemy.team != warrior.team)
                {
                    enemy.ApplyStun(stunDuration);
                    float damage = warrior.AttackDamage * damageMultiplier;
                    enemy.TakeDamage(damage, DamageType.Physical, AttackType.Slash, false);
                }
            }
        }      
    }
}
