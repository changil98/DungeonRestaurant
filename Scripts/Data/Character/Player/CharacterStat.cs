using Sirenix.OdinInspector;
using UnityEngine;

public enum UpgradeType
{
    Atk,
    Def,
    Resist,
    Hp,
    AttackSpeed,
    CriticalPercent,
    CriticalDmg
}

[System.Serializable]
public class CharacterStat : StatBase
{
    [Title("Increase Stat Save")]
    public int increaseHp;
    public int increaseAtk;
    public int increaseDef;
    public int increaseResist;
    public float increaseAttackSpeed;
    public float increaseCriticalPercent;
    public int increaseCriticalDmg;

    public CharacterInfo CharacterInfo { get; set; }

    public CharacterStat(CharacterInfo info, int level)
    {
        CharacterInfo = info;
        InitializeStat(info, level);
    }

    public CharacterStat(CharacterStat other)
    {
        hp = other.hp;
        def = other.def;
        atk = other.atk;
        resistance = other.resistance;
        criticalPercent = other.criticalPercent;
        mp = other.mp;
        range = other.range;
        moveSpeed = other.moveSpeed;
        attackSpeed = other.attackSpeed;
        criticalDamage = other.criticalDamage;
    }

    public CharacterStat Clone()
    {
        return new CharacterStat(this);
    }


    private void InitializeStat(CharacterInfo info, int level)
    {
        CharacterInitialStat stat = DataManager.Instance.InitStatDict.GetData(info.CharacterClass);

        hp = stat.HP;
        def = stat.DEF;
        atk = stat.ATK;
        resistance = stat.Resistance;
        criticalPercent = stat.CriticalPercent;
        mp = stat.MP;
        range = stat.Range;
        moveSpeed = stat.MoveSpeed;
        attackSpeed = stat.AttackSpeed;
        criticalDamage = stat.CriticalDamage;

        IncreaseStatOnLevelUp(level);
    }

    public void IncreaseStatOnLevelUp()
    {
        IncreaseStat(ref hp, Random.Range(CharacterInfo.MinHP, CharacterInfo.MaxHP + 1));
        IncreaseStat(ref atk, Random.Range(CharacterInfo.MinATK, CharacterInfo.MaxATK + 1));
        IncreaseStat(ref def, Random.Range(CharacterInfo.MinDEF, CharacterInfo.MaxDEF + 1));
        IncreaseStat(ref resistance, Random.Range(CharacterInfo.MinResistance, CharacterInfo.MaxResistance + 1));
        IncreaseStat(ref attackSpeed, Random.Range(CharacterInfo.MinAttackSpeed, CharacterInfo.MaxAttackSpeed));
    }

    private void IncreaseStatOnLevelUp(int level)
    {
        IncreaseStat(ref hp, Random.Range(CharacterInfo.MinHP * level, CharacterInfo.MaxHP * level + 1));
        IncreaseStat(ref atk, Random.Range(CharacterInfo.MinATK * level, CharacterInfo.MaxATK * level + 1));
        IncreaseStat(ref def, Random.Range(CharacterInfo.MinDEF * level, CharacterInfo.MaxDEF * level + 1));
        IncreaseStat(ref resistance, Random.Range(CharacterInfo.MinResistance * level, CharacterInfo.MaxResistance * level + 1));
        IncreaseStat(ref attackSpeed, Random.Range(CharacterInfo.MinAttackSpeed * level, CharacterInfo.MaxAttackSpeed * level));
    }

    public int GetLevel(UpgradeType upgradeType)
    {
        return upgradeType switch
        {
            UpgradeType.Atk => DataManager.Instance.UpgradeLevel.atkLevel,
            UpgradeType.Def => DataManager.Instance.UpgradeLevel.defLevel,
            UpgradeType.Resist => DataManager.Instance.UpgradeLevel.resistLevel,
            UpgradeType.Hp => DataManager.Instance.UpgradeLevel.hpLevel,
            UpgradeType.AttackSpeed => DataManager.Instance.UpgradeLevel.AttackSpeedLevel,
            UpgradeType.CriticalPercent => DataManager.Instance.UpgradeLevel.criticalPercentLevel,
            UpgradeType.CriticalDmg => DataManager.Instance.UpgradeLevel.criticalDmgLevel,
            _ => 0,
        };
    }

    public int GetIntStat(UpgradeType upgradeType)
    {
        return upgradeType switch
        {
            UpgradeType.Atk => atk,
            UpgradeType.Def => def,
            UpgradeType.Resist => resistance,
            UpgradeType.Hp => hp,
            UpgradeType.CriticalDmg => (int)criticalDamage,
            _ => 0,
        };
    }

    public float GetFloatStat(UpgradeType upgradeType)
    {
        return upgradeType switch
        {
            UpgradeType.AttackSpeed => attackSpeed,
            UpgradeType.CriticalPercent => criticalPercent,
            _ => 0,
        };
    }

    public void IncreaseStat(UpgradeType upgradeType, int amount)
    {
        switch (upgradeType)
        {
            case UpgradeType.Atk:
                atk += amount;
                break;
            case UpgradeType.Def:
                def += amount;
                break;
            case UpgradeType.Resist:
                resistance += amount;
                break;
            case UpgradeType.Hp:
                hp += amount;
                break;
            case UpgradeType.CriticalDmg:
                criticalDamage += amount;
                break;
        }
    }

    public void IncreaseStat(UpgradeType upgradeType, float amount)
    {
        switch (upgradeType)
        {
            case UpgradeType.AttackSpeed:
                attackSpeed += amount;
                break;
            case UpgradeType.CriticalPercent:
                criticalPercent += amount;
                break;
        }
    }

    private void IncreaseStat(ref int stat, int amount)
    {
        stat += amount;
    }
    private void IncreaseStat(ref float stat, float amount)
    {
        stat += amount;
    }
}