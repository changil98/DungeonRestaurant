using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : Singleton<SkillManager>
{
    [FolderPath(ParentFolder = "Assets/Resources", RequireExistingPath = true)]
    [SerializeField] private string folderPath;

    [ShowInInspector]
    private Dictionary<eCharacterClass, SkillSet> skillSets = new Dictionary<eCharacterClass, SkillSet>();

    protected override void Awake()
    {
        base.Awake();
        LoadSkillSets();
    }

    private void LoadSkillSets()
    {
        SkillSet[] skillList = Resources.LoadAll<SkillSet>(folderPath);
        foreach (SkillSet skillSet in skillList)
        {
            skillSets[skillSet.skillClass] = skillSet;
        }
    }

    public BaseSkill GetRandomSkill(eCharacterClass characterClass)
    {
        if (skillSets.ContainsKey(characterClass))
        {
            return skillSets[characterClass].GetRandomSkill();
        }

        Debug.LogWarning($"해당 직업의 스킬 셋을 찾을 수 없음: {characterClass}");
        return null;
    }

    public BaseSkill GetSkill(eCharacterClass characterClass, string rcode)
    {
        if (skillSets.ContainsKey(characterClass))
        {
            return skillSets[characterClass].GetSkill(rcode);
        }

        Debug.LogWarning($"해당 직업의 스킬 셋을 찾을 수 없음: {characterClass}");
        return null;
    }
}