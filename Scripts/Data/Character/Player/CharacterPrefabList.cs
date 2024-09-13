using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "characterSkinList", menuName = "Scriptable Object/characterSkinList", order = int.MaxValue)]
public class CharacterPrefabList : ScriptableObject
{
    public eCharacterClass characterClass;
    public List<CharacterPrefab> prefabList;

    public CharacterPrefab GetPrefab(string rcode)
    {
        foreach (CharacterPrefab prefab in prefabList)
        {
            if (prefab.rcode == rcode)
            {
                return prefab;
            }
        }
        Debug.Log($"{rcode}에 해당하는 프리팹 세트가 없습니다.");
        return null;
    }
}