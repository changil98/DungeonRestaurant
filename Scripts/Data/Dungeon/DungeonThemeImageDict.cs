using UnityEngine;

public class DungeonThemeImageDict : DataDictionary<DungeonTheme, DungeonThemeImage>
{
    public DungeonThemeImageDict(DungeonThemeImage[] list) : base(list, data => data.theme) { }

    public Sprite GetImage(DungeonTheme theme)
    {
        if (dataDict.ContainsKey(theme))
        {
            return dataDict[theme].dungeonImage;
        }
        return null;
    }

    public GameObject GetIntroTileMap(DungeonTheme theme)
    {
        if (dataDict.ContainsKey(theme))
        {
            GameObject timeMap = dataDict[theme].introTileMap;
            if (timeMap != null)
                return timeMap;
            else
            {
                Debug.LogError($"{theme}'s IntroTimeMap == NULL!!");
                return null;
            }
        }

        Debug.LogError($"No ContainsKey({theme}) in DungeonThemeImageDict");
        return null;
    }

    public GameObject GetCombatTileMap(DungeonTheme theme)
    {
        if (dataDict.ContainsKey(theme))
        {
            GameObject timeMap = dataDict[theme].combatTileMap;
            if (timeMap != null)
                return timeMap;
            else
            {
                Debug.LogError($"{theme}'s CombatTimeMap == NULL!!");
                return null;
            }
        }

        Debug.LogError($"No ContainsKey({theme}) in DungeonThemeImageDict");
        return null;
    }
}