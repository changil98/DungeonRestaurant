using Sirenix.OdinInspector;
using System;

[Serializable]
public class CharacterInfo
{
    [ShowInInspector] public string Rcode { get; set; }
    [ShowInInspector] public eCharacterClass CharacterClass { get; set; }
    [ShowInInspector] public string ClassString { get; set; }
    [ShowInInspector] public eCharacterRank Rank { get; set; }
    [ShowInInspector] public string Description { get; set; }

    [Title("Level Up Increase Stat")]
    [ShowInInspector] public int MinHP { get; set; }
    [ShowInInspector] public int MaxHP { get; set; }
    [ShowInInspector] public int MinATK { get; set; }
    [ShowInInspector] public int MaxATK { get; set; }
    [ShowInInspector] public int MinDEF { get; set; }
    [ShowInInspector] public int MaxDEF { get; set; }
    [ShowInInspector] public int MinResistance { get; set; }
    [ShowInInspector] public int MaxResistance { get; set; }
    [ShowInInspector] public float MinAttackSpeed { get; set; }
    [ShowInInspector] public float MaxAttackSpeed { get; set; }
}
