using UnityEngine;

public class UpgradeManager : Singleton<UpgradeManager>
{
    public void StatUpgradeAllCharacters(UpgradeType upgradeType)
    {
        foreach (var character in DataManager.Instance.characterList)
        {
            if (upgradeType == UpgradeType.Atk || upgradeType == UpgradeType.Def || upgradeType == UpgradeType.Hp || upgradeType == UpgradeType.Resist || upgradeType == UpgradeType.CriticalDmg)
                RemoveLastIncrease(character.Stat, upgradeType);
            else
                RemoveLastIncreaseFloat(character.Stat, upgradeType);

            UpgradeStatType(character.Stat, upgradeType);
        }
    }

    public void StatUpgradeEmployCharacters(UpgradeType upgradeType)
    {
        foreach (var employCharacter in EmploymentManager.Instance.employmentList)
        {
            if (upgradeType == UpgradeType.Atk || upgradeType == UpgradeType.Def || upgradeType == UpgradeType.Hp || upgradeType == UpgradeType.Resist || upgradeType == UpgradeType.CriticalDmg)
                RemoveLastIncrease(employCharacter.Stat, upgradeType);
            else
                RemoveLastIncreaseFloat(employCharacter.Stat, upgradeType);

            UpgradeStatType(employCharacter.Stat, upgradeType);
        }
    }

    public void UpgradeStatType(CharacterStat stat, UpgradeType upgradeType)
    {
        if (upgradeType == UpgradeType.Atk || upgradeType == UpgradeType.Def) // 공격력, 방어력
        {
            float increasePercentage = stat.GetLevel(upgradeType) * 0.05f;
            int increaseAmount = Mathf.RoundToInt(stat.GetIntStat(upgradeType) * increasePercentage);
            stat.IncreaseStat(upgradeType, increaseAmount);

            SaveValue(stat, upgradeType, increaseAmount);
        }
        else if (upgradeType == UpgradeType.Hp) // 체력
        {
            float increasePercentage = stat.GetLevel(upgradeType) * 0.05f;
            if (stat.GetLevel(upgradeType) == 9)
            {
                increasePercentage = (stat.GetLevel(upgradeType) + 1) * 0.05f;
            }
            int increaseAmount = Mathf.RoundToInt(stat.GetIntStat(upgradeType) * increasePercentage);
            stat.IncreaseStat(upgradeType, increaseAmount);

            SaveValue(stat, upgradeType, increaseAmount);
        }
        else if (upgradeType == UpgradeType.Resist) // 저항
        {
            int increaseAmount = stat.GetLevel(upgradeType) * 10;
            stat.IncreaseStat(upgradeType, increaseAmount);

            SaveValue(stat, upgradeType, increaseAmount);
        }
        else if (upgradeType == UpgradeType.CriticalDmg) // 크리 데미지
        {
            int increaseAmount = stat.GetLevel(upgradeType) * 5;
            if (stat.GetLevel(upgradeType) == 9)
            {
                increaseAmount += 5;
            }
            stat.IncreaseStat(upgradeType, increaseAmount);

            SaveValue(stat, upgradeType, increaseAmount);
        }
        else if (upgradeType == UpgradeType.CriticalPercent) // 크리 확률
        {
            float increaseAmount = stat.GetLevel(upgradeType) * 0.05f;
            stat.IncreaseStat(upgradeType, increaseAmount);

            SaveValue(stat, upgradeType, increaseAmount);
        }
        else
        {
            float increasePercentage = stat.GetLevel(upgradeType) * 0.05f;
            if (stat.GetLevel(upgradeType) == 9)
            {
                increasePercentage = (stat.GetLevel(upgradeType) + 1) * 0.05f;
            }
            float increaseAmount = Mathf.Round(stat.GetFloatStat(upgradeType) * increasePercentage * 100f) / 100f; // 공격 속도 소수점 두자리로 증가
            stat.IncreaseStat(upgradeType, increaseAmount);

            SaveValue(stat, upgradeType, increaseAmount);
        }
    }

    public void SaveValue(CharacterStat stat, UpgradeType upgradeType, int increaseAmount) // 증가한 값을 저장 (int)
    {
        //var stat = character.Stat;

        switch (upgradeType)
        {
            case UpgradeType.Atk:
                stat.increaseAtk += increaseAmount;
                break;
            case UpgradeType.Def:
                stat.increaseDef += increaseAmount;
                break;
            case UpgradeType.Resist:
                stat.increaseResist += increaseAmount;
                break;
            case UpgradeType.Hp:
                stat.increaseHp += increaseAmount;
                break;
            case UpgradeType.CriticalDmg:
                stat.increaseCriticalDmg += increaseAmount;
                break;
        }
    }

    public void SaveValue(CharacterStat stat, UpgradeType upgradeType, float increaseAmount) // 증가한 값을 저장 (float)
    {
        switch (upgradeType)
        {
            case UpgradeType.AttackSpeed:
                stat.increaseAttackSpeed += increaseAmount;
                break;
            case UpgradeType.CriticalPercent:
                stat.increaseCriticalPercent += increaseAmount;
                break;
        }
    }

    public void RemoveLastIncrease(CharacterStat stat, UpgradeType upgradeType) // 최근 증가한 값을 빼주는 것 (int)
    {
        //var stat = character.Stat;

        switch (upgradeType)
        {
            case UpgradeType.Atk:
                stat.IncreaseStat(upgradeType, -stat.increaseAtk);
                stat.increaseAtk = 0;
                break;
            case UpgradeType.Def:
                stat.IncreaseStat(upgradeType, -stat.increaseDef);
                stat.increaseDef = 0;
                break;
            case UpgradeType.Resist:
                stat.IncreaseStat(upgradeType, -stat.increaseResist);
                stat.increaseResist = 0;
                break;
            case UpgradeType.Hp:
                stat.IncreaseStat(upgradeType, -stat.increaseHp);
                stat.increaseHp = 0;
                break;
            case UpgradeType.CriticalDmg:
                stat.IncreaseStat(upgradeType, -stat.increaseCriticalDmg);
                stat.increaseCriticalDmg = 0;
                break;
        }
    }

    public void RemoveLastIncreaseFloat(CharacterStat stat, UpgradeType upgradeType) // 최근 증가한 값을 빼주는 것 (float)
    {
        //var stat = character.Stat;

        switch (upgradeType)
        {
            case UpgradeType.AttackSpeed:
                stat.IncreaseStat(upgradeType, -stat.increaseAttackSpeed);
                stat.increaseAttackSpeed = 0;
                break;
            case UpgradeType.CriticalPercent:
                stat.IncreaseStat(upgradeType, -stat.increaseCriticalPercent);
                stat.increaseCriticalPercent = 0;
                break;
        }
    }

    public void RemoveLastIncreaseAll(CharacterStat stat)
    {
        RemoveLastIncrease(stat, UpgradeType.Atk);
        RemoveLastIncrease(stat, UpgradeType.Def);
        RemoveLastIncrease(stat, UpgradeType.Resist);
        RemoveLastIncrease(stat, UpgradeType.Hp);
        RemoveLastIncreaseFloat(stat, UpgradeType.AttackSpeed);
        RemoveLastIncrease(stat, UpgradeType.CriticalDmg);
        RemoveLastIncreaseFloat(stat, UpgradeType.CriticalPercent);
    }

    public void StatUpgradeAll(CharacterStat stat)
    {
        UpgradeStatType(stat, UpgradeType.Atk);
        UpgradeStatType(stat, UpgradeType.Def);
        UpgradeStatType(stat, UpgradeType.Resist);
        UpgradeStatType(stat, UpgradeType.Hp);
        UpgradeStatType(stat, UpgradeType.AttackSpeed);
        UpgradeStatType(stat, UpgradeType.CriticalPercent);
        UpgradeStatType(stat, UpgradeType.CriticalDmg);
    }

    public void IncreaseStatLevel(UpgradeType upgradeType)
    {
        switch (upgradeType)
        {
            case UpgradeType.Atk:
                DataManager.Instance.UpgradeLevel.atkLevel++;
                break;
            case UpgradeType.Def:
                DataManager.Instance.UpgradeLevel.defLevel++;
                break;
            case UpgradeType.Resist:
                DataManager.Instance.UpgradeLevel.resistLevel++;
                break;
            case UpgradeType.Hp:
                DataManager.Instance.UpgradeLevel.hpLevel++;
                break;
            case UpgradeType.AttackSpeed:
                DataManager.Instance.UpgradeLevel.AttackSpeedLevel++;
                break;
            case UpgradeType.CriticalPercent:
                DataManager.Instance.UpgradeLevel.criticalPercentLevel++;
                break;
            case UpgradeType.CriticalDmg:
                DataManager.Instance.UpgradeLevel.criticalDmgLevel++;
                break;
            default: break;
        }
    }

    public void IncreaseMedalLevel(MedalUpgradeType upgradeType)
    {
        switch (upgradeType)
        {
            case MedalUpgradeType.entry:
                DataManager.Instance.UpgradeLevel.entryLevel++;
                DataManager.Instance.userInfo.MaxPartyNumber++;
                break;
            case MedalUpgradeType.employPrice:
                DataManager.Instance.UpgradeLevel.employPriceLevel++; // CharacterEmployment.cs에서 값 변환
                break;
            case MedalUpgradeType.characterLvUpPrice:
                DataManager.Instance.UpgradeLevel.characterLvUpPriceLevel++; // LevelUp.cs에서 값 변환
                break;
            case MedalUpgradeType.rarePercent:
                DataManager.Instance.UpgradeLevel.rarePercentLevel++;
                DataManager.Instance.UpgradeLevel.UpdateRarePercent();
                break;
            case MedalUpgradeType.heroPercent:
                DataManager.Instance.UpgradeLevel.heroPercentLevel++;
                DataManager.Instance.UpgradeLevel.UpdateHeroPercent();
                break;
            case MedalUpgradeType.maxCharacterListCount:
                DataManager.Instance.UpgradeLevel.maxCharacterListCountLevel++;
                IncreaseMaxCharacterListCount();
                break;
            default: break;
        }
    }

    public float GetEmployPriceDiscount()
    {
        return 0.05f * DataManager.Instance.UpgradeLevel.employPriceLevel;
    }

    public float GetCharacterLevelUpPriceDiscount()
    {
        return 0.05f * DataManager.Instance.UpgradeLevel.characterLvUpPriceLevel;
    }

    public void IncreaseMaxCharacterListCount()
    {
        UserInfo.userInfo.maxCharacterListCount
            = 10 + 5 * DataManager.Instance.UpgradeLevel.maxCharacterListCountLevel;
    }
}
