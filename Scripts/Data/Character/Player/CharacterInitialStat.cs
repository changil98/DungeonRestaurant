using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

[Serializable]
public class CharacterInitialStat : StatBase
{
    [PropertyOrder(-1)] public string Rcode { get; set; }
    [PropertyOrder(-1)] public eCharacterClass CharacterClass { get; set; }
    CharacterInitialStat()
    {
        HP = 500;
        ATK = 50;
        DEF = 25;
        Resistance = 25;
        AttackSpeed = 0.75f;
        Range = 1f;
        CriticalDamage = 100;
        MoveSpeed = 1f;
    }
}
