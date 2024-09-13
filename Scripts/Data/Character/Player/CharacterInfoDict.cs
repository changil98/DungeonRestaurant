using System;
using System.Collections.Generic;

[Serializable]
public class CharacterInfoDict : DataDictionary<string, CharacterInfo>
{
    public CharacterInfoDict(DataSheetController controller) : base(controller, data => data.Rcode) { }

    public List<CharacterInfo> GetListByRank(eCharacterRank rank)
    {
        List<CharacterInfo> list = new List<CharacterInfo>();
        foreach (CharacterInfo item in dataDict.Values)
        {
            if (item.Rank == rank) list.Add(item);
        }
        return list;
    }
}