using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatUpgradeInfo", menuName = "Scriptable Object/StatUpgradeInfo", order = int.MaxValue)]
public class StatInfo : ScriptableObject
{
    public string statName;
    public string recipeName;
    public Sprite recipeImage;
    public int maxLevel;
    public List<int> needRecipePoint;
}
