using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EntryInfo
{
    [SerializeField] private int characterID;
    public int CharacterID { get => characterID; set { characterID = value; } }

    [SerializeField] private int index = -1;
    public void SetEntry(int id, int index)
    {
        CharacterID = id;
        this.index = index;
    }

    public int GetIndex()
    {
        // Entry is null
        if (index == -1)
            return -1;

        List<CharacterData> list = DataManager.Instance.characterList;

        // ok
        if (index < list.Count && list[index].ID == characterID)
            return index;

        // index is change
        for(int i = 0; i < list.Count; i++)
        {
            if(CharacterID == list[i].ID)
            {
                index = i;
                return i;
            }
        }

        // characterdata is not in list
        Clear();
        return -1;
    }

    public void Clear()
    {
        CharacterID = -1;
        index = -1;
    }
}

[System.Serializable]
public class EntryPreset
{
    [SerializeField] private EntryInfo[] entryList;
    public EntryInfo[] EntryList{ get => entryList; set { entryList = value; } }

    public EntryPreset()
    {
        entryList = new EntryInfo[GameManager.MAX_POSITION];
        for (int i = 0; i < EntryList.Length; i++)
        {
            entryList[i] = new EntryInfo();
        }
    }
}
