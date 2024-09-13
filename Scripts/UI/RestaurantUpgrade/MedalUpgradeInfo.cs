using System.Collections.Generic;
using UnityEngine;

public enum MedalUpgradeType
{
    entry,
    employPrice,
    characterLvUpPrice,
    rarePercent,
    heroPercent,
    maxCharacterListCount
}

[CreateAssetMenu(fileName = "MedalUpgradeInfo", menuName = "Scriptable Object/MedalUpgradeInfo", order = int.MaxValue)]
public class MedalUpgradeInfo : ScriptableObject
{
    public string medalUpgradeName;
    public int maxLevel;
    public List<int> needMedal;
}
