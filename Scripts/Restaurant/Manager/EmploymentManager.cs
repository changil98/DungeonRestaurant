using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class EmploymentManager : Singleton<EmploymentManager>
{
    [Title("CharacterEmployment")]
    public List<CharacterData> employmentList = new List<CharacterData>();

    [Title("CheckEmployment")]
    public List<bool> isEmploymentAvailability = new List<bool>();

    [ShowInInspector] private List<CharacterInfo> commonCharacterList = new List<CharacterInfo>();
    [ShowInInspector] private List<CharacterInfo> rareCharacterList = new List<CharacterInfo>();
    [ShowInInspector] private List<CharacterInfo> heroCharacterList = new List<CharacterInfo>();

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        InitCharacterInfoList();
        GenerateEmploymentList();
    }

    private void InitCharacterInfoList()
    {
        commonCharacterList = DataManager.Instance.InfoDict.GetListByRank(eCharacterRank.Common);
        rareCharacterList = DataManager.Instance.InfoDict.GetListByRank(eCharacterRank.Rare);
        heroCharacterList = DataManager.Instance.InfoDict.GetListByRank(eCharacterRank.Hero);
    }

    public void GenerateEmploymentList()
    {
        CharacterNameList nameList = DataManager.Instance.NameList;
        employmentList.Clear();
        isEmploymentAvailability.Clear();
        for (int i = 0; i < GameManager.MAX_PARTY_NUMBER; i++)
        {
            int randomLevel = Random.Range(1, DataManager.Instance.userInfo.UserLevel + 1);
            CharacterInfo randomCharacter = CharacterRank();
            string characterName = DataManager.Instance.NameList.GetRandomName(randomCharacter.Rank);
            BaseSkill skill = AssignRandomSkill(randomCharacter.CharacterClass);
            CharacterPrefab skin = SetSkin(randomCharacter.CharacterClass);
            CharacterData character = new CharacterData(randomCharacter, randomLevel, characterName, skill, skin);

            employmentList.Add(character);
            isEmploymentAvailability.Add(true);
        }
    }

    public void ReRoll()
    {
        GenerateEmploymentList();
    }

    private BaseSkill AssignRandomSkill(eCharacterClass characterClass)
    {
        BaseSkill randomSkill = SkillManager.Instance.GetRandomSkill(characterClass);
        return randomSkill;
    }

    private CharacterPrefab SetSkin(eCharacterClass characterClass)
    {
        CharacterPrefabList[] skinList = DataManager.Instance.PrefabList;
        for (int i = 0; i < skinList.Length; i++)
        {
            if (skinList[i].characterClass == characterClass)
            {
                int randomIdx = Random.Range(0, skinList[i].prefabList.Count);
                return skinList[i].prefabList[randomIdx];
            }
        }
        return null;
    }

    private CharacterInfo CharacterRank()
    {
        float randomIdx = Random.Range(0, 1f);
        if (randomIdx < DataManager.Instance.UpgradeLevel.commonPercent)
        {
            int idx = Random.Range(0, commonCharacterList.Count);
            return commonCharacterList[idx];
        }
        else if (randomIdx < DataManager.Instance.UpgradeLevel.commonPercent + DataManager.Instance.UpgradeLevel.rarePercent)
        {
            int idx = Random.Range(0, rareCharacterList.Count);
            return rareCharacterList[idx];
        }
        else
        {
            int idx = Random.Range(0, heroCharacterList.Count);
            return heroCharacterList[idx];
        }
    }
}