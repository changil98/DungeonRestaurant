using UnityEngine;
using Sirenix.OdinInspector;
using System;

[CreateAssetMenu(fileName = "EnemyPrefab", menuName = "Scriptable Object/EnemyPrefab", order = int.MaxValue)]
public class EnemyPrefab : ScriptableObject
{
    public string rcode;
    public Sprite thumnailSprite;
    [AssetSelector(Paths = "Assets/Resources/SPUM/SPUM_Units", Filter = "t:prefab")]
    public GameObject prefab;
}