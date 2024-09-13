using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class CharacterPrefab
{
    public string rcode;
    [AssetSelector(Paths = "Assets/SPUM/ScreenShots", Filter = "idle_0")]
    public Sprite characterThumnail;
    [AssetSelector(Filter = "idle, t:prefab")]
    public GameObject characterIdle;
    [AssetSelector(Paths = "Assets/Resources/SPUM/SPUM_Units", Filter = "t:prefab")]
    public GameObject prefab;
}
