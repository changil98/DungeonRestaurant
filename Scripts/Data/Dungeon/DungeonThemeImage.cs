using UnityEngine;

[CreateAssetMenu(fileName = "DungeonThemeImage", menuName = "Scriptable Object/DungeonTheme", order = int.MaxValue)]
public class DungeonThemeImage : ScriptableObject
{
    public DungeonTheme theme;
    public Sprite dungeonImage;
    public GameObject combatTileMap;
    public GameObject introTileMap;
}