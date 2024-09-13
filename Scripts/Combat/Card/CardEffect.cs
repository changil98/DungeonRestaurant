using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CardEffect
{
    public static void Buff_TeamHeal(List<PlayerCombatAI> character, float value)
    {
        for (int i = 0; i < character.Count; i++)
        {
            character[i].Heal(character[i].MaxHP * (value / 100));
        }
    }

    public static void Buff_SoloHeal(PlayerCombatAI character, float value)
    {
        character.Heal(character.MaxHP * (value / 100));
    }

    public static void Buff_TeamMana(List<PlayerCombatAI> character, float value)
    {
        for (int i = 0; i < character.Count; i++)
        {
            character[i].GainMana(value);
        }
    }

    public static void Buff_SoloMana(PlayerCombatAI character)
    {
        character.GainMana(character.MaxMana);
    }

    public static void Buff_TeamShield(List<PlayerCombatAI> character, float value)
    {
        for (int i = 0; i < character.Count; i++)
        {
            character[i].AddShield(value);
        }
    }

    public static void Buff_LegendShield(PlayerCombatAI character, float value)
    {
        character.AddShield(value);
    }

    public static void Buff_AttackUp(List<PlayerCombatAI> character, float value)
    {
        for (int i = 0; i < character.Count; i++)
        {
            character[i].AttackDamage *= 1 + (value / 100);
        }
    }

    public static void Buff_CriticalChance(List<PlayerCombatAI> character, float value)
    {
        for (int i = 0; i < character.Count; i++)
        {
            character[i].CriticalPersent *= 1 + (value / 100);
        }
    }
    public static void Buff_CriticalDamage(List<PlayerCombatAI> character, float value)
    {
        for (int i = 0; i < character.Count; i++)
        {
            character[i].CriticalDamage *= 1 + (value / 100);
        }
    }

    public static void Buff_HpUp(List<PlayerCombatAI> character, float value)
    {
        for (int i = 0; i < character.Count; i++)
        {
            float hp = character[i].MaxHP *= 1 + (value / 100);
            character[i].MaxHP *= 1 + (value / 100);
            //character[i].CurrentHP += hp;
        }
    }

    public static void Buff_DefUp(List<PlayerCombatAI> character, float value)
    {
        for (int i = 0; i < character.Count; i++)
        {
            character[i].Def *= 1 + (value / 100);
        }
    }

    public static void Buff_ResistanceUp(List<PlayerCombatAI> character, float value)
    {
        for (int i = 0; i < character.Count; i++)
        {
            character[i].Resistance += value;
        }
    }

    public static void Buff_AttackSpeed(List<PlayerCombatAI> character, float value)
    {
        for (int i = 0; i < character.Count; i++)
        {
            character[i].IncreaseAttackSpeed(value);
        }
    }

    public static void Buff_MoveSpeed(List<PlayerCombatAI> character, float value)
    {
        for (int i = 0; i < character.Count; i++)
        {
            character[i].MovementSpeed += value;
        }
    }

    public static void Buff_Resurrect(List<PlayerCombatAI> character, int value)
    {
        while (true)
        {
            int randomIdx = Random.RandomRange(0, character.Count);
            PlayerCombatAI player = character[randomIdx];
            if (player.CheckAlive() == false)
            {
                player.Resurrection(value);
                break;
            }
        }
    }

    public static void DeBuff_EnemyAttackSpeedDown(List<EnemyCombatAI> enemy, float value)
    {
        for (int i = 0; i < enemy.Count; i++)
        {
            enemy[i].DecreaseAttackSpeed(value);
        }
    }

    public static void DeBuff_EnemyStun(List<EnemyCombatAI> enemy, float value, float cardLevel)
    {
        for (int i = 0; i < enemy.Count; i++)
        {
            float totalStunDuration = value + (0.5f * cardLevel);
            enemy[i].ApplyStun(totalStunDuration);
        }
    }

    public static void DeBuff_EnemyDamage(List<EnemyCombatAI> enemy, float value, float cardLevel)
    {
        for (int i = 0; i < enemy.Count; i++)
        {
            float damage = enemy[i].MaxHP * (value / 100);
            enemy[i].TakeDamage(damage, DamageType.Physical, AttackType.Blunt, false);
        }
    }

    public static void DeBuff_EnemyMoveSpeedDown(List<EnemyCombatAI> enemy, float value, float cardLevel)
    {
        for (int i = 0; i < enemy.Count; i++)
        {
            float additionalDecrease = 1 + (value / 100) * (1 + 0.05f * cardLevel);
            enemy[i].MovementSpeed -= additionalDecrease;
        }
    }

    public static void ChargeHunger(HungerGauge hungerGauge, float value)
    {
        hungerGauge.hungerGauge.fillAmount += value;
    }
}
