using Sirenix.OdinInspector;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


[Serializable]
public class CharacterData
{
    #region Fields
    public string characterName;

    [SerializeField] private int level = 1;
    public int Level
    {
        get { return level; }
        set
        {
            level = value;
            OnLevelUp?.Invoke();
        }
    }
    public delegate void LevelUpEventHandler();
    protected event LevelUpEventHandler OnLevelUp;

    [SerializeField] private CharacterInfo info;
    public CharacterInfo Info { get { return info; } set { info = value; } }

    [SerializeField] private CharacterStat stat = null;
    public CharacterStat Stat { get { return stat; } set { stat = value; } }

    public BaseSkill skill;

    public CharacterPrefab skin;

    [SerializeField, PropertyOrder(-1)] private int id;
    private static System.Random random = new System.Random();

    public int ID { get => id; }

    #endregion

    public CharacterData(CharacterInfo info, int level, string characterName, BaseSkill skill, CharacterPrefab skin)
    {
        this.info = info;
        this.level = level;
        stat = new CharacterStat(info, level);
        this.characterName = characterName;
        this.skill = skill;
        this.skin = skin;
        GenerateHashCode(characterName, info.Rcode, skill.rcode, random.Next());
        OnLevelUp += stat.IncreaseStatOnLevelUp;
    }

    public CharacterData(CharacterDataSerializable data)
    {
        Deserialization(data);
        OnLevelUp += stat.IncreaseStatOnLevelUp;
    }

    public void LevelUp()
    {
        // 레벨업 전에 강화로 인한 스탯 초기화
        UpgradeManager.Instance.RemoveLastIncreaseAll(this.stat);
        if (!DataManager.Instance.userInfo.isUserTutorials && ToturialsManager.Instance.phase == 35)
        {
            ToturialsManager.Instance.isClear[15] = true;
            ToturialsManager.Instance.OnNextPhase();
        }
        Level++;

        // 레벨업 후 다시 강화로 인한 스탯 적용
        UpgradeManager.Instance.StatUpgradeAll(this.stat);
    }

    public void SetLevelUpEvent(LevelUpEventHandler action)
    {
        OnLevelUp += action;
    }

    public void RemoveLevelUpEvent(LevelUpEventHandler action)
    {
        OnLevelUp -= action;
    }



    private void GenerateHashCode(params object[] values)
    {
        int hash = 17;
        foreach (var value in values)
        {
            hash = hash * 31 + (value != null ? value.GetHashCode() : 0);
        }
        id = hash;
    }

    public override bool Equals(object obj)
    {
        var other = obj as CharacterData;
        if (other == null)
            return false;

        return ID == other.ID;
    }

    public override int GetHashCode()
    {
        return ID;
    }

    public CharacterDataSerializable Serialization()
    {
        UpgradeManager.Instance.RemoveLastIncreaseAll(this.stat);
        CharacterStat defaultStat = this.stat.Clone();
        return new CharacterDataSerializable
        {
            characterName = this.characterName,
            level = this.level,
            characterInfoRcode = this.info != null ? this.info.Rcode : null,
            stat = defaultStat,
            skillRcode = this.skill != null ? this.skill.rcode : null,
            skinRcode = this.skin != null ? this.skin.rcode : null,
            id = this.id
        };
    }

    public void Deserialization(CharacterDataSerializable serializable)
    {
        this.characterName = serializable.characterName;
        this.level = serializable.level;
        this.Info = DataManager.Instance.InfoDict.GetData(serializable.characterInfoRcode);
        this.stat = serializable.stat;
        this.stat.CharacterInfo = this.info;
        UpgradeManager.Instance.StatUpgradeAll(this.stat);
        this.skill = SkillManager.Instance.GetSkill(Info.CharacterClass, serializable.skillRcode);
        this.skin = DataManager.Instance.GetPrefab(Info.CharacterClass, serializable.skinRcode);
        this.id = serializable.id;
    }
}

[Serializable]
public class CharacterDataSerializable
{
    public string characterName;
    public int level;
    public string characterInfoRcode;
    public CharacterStat stat;
    public string skillRcode;
    public string skinRcode;
    public int id;
}