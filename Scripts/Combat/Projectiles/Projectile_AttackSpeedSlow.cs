using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Projectile_AttackSpeedSlow : Projectile
{
    public float ATTACK_SPEED_DECREASE = 0.15f;
    public float SLOW_DURATION = 1f;

    private static Dictionary<PlayerCombatAI, float> slowedTargets = new Dictionary<PlayerCombatAI, float>();

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerCombatAI targetAI = collision.GetComponent<PlayerCombatAI>();
        if (targetAI != null && targetAI != owner && targetAI.team != owner.team)
        {
            targetAI.TakeDamage(damage, DamageType.Magical, AttackType.Blunt, isCriticalHit);
            //대상이 슬로우 상태인지 확인
            if (!slowedTargets.ContainsKey(targetAI))
            {
                StartCoroutine(ApplyAttackSpeedSlow(targetAI));
            }
        }
        Destroy(gameObject);
    }

    private IEnumerator ApplyAttackSpeedSlow(PlayerCombatAI target)
    {
        
        slowedTargets[target] = Time.time + SLOW_DURATION;

        
        target.DecreaseAttackSpeed(ATTACK_SPEED_DECREASE);

        
        yield return new WaitForSeconds(SLOW_DURATION);

        
        target.IncreaseAttackSpeed(ATTACK_SPEED_DECREASE);

        
        slowedTargets.Remove(target);
    }

    //타겟이 슬로우 상태인지 확인
    public static bool CanSlowTarget(PlayerCombatAI target)
    {
        if (slowedTargets.TryGetValue(target, out float endTime))
        {
            return Time.time >= endTime;
        }
        return true;
    }
}