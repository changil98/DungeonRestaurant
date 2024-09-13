using System;

[Serializable]
public class InitialStatDict : DataDictionary<eCharacterClass, CharacterInitialStat>
{
    public InitialStatDict(DataSheetController controller) : base(controller, data => data.CharacterClass) { }
}