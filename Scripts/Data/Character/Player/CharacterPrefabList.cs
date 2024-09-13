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
        Debug.Log($"{rcode}�� �ش��ϴ� ������ ��Ʈ�� �����ϴ�.");
        return null;
    }
}