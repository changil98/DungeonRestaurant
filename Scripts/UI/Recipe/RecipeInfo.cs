using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "Scriptable Object/Recipe", order = int.MaxValue)]
public class RecipeInfo : ScriptableObject
{
    [Title("RecipeInfo")]
    public string rcode;
    public string idxNum;
    public string recipeName;
    public Sprite sprite;
    public float StomachCharging;
    public float sellPrice;
    public string recipeDescription;
    public Recipes recipeType;
    public RecipeRare recipeRare;
}
