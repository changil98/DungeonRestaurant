using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RecipeDataManager : MonoBehaviour
{
    public static RecipeDataManager Instance;

    public RecipeInfo[] recipesInfo;
    [ShowInInspector]public List<RecipeData> recipes = new List<RecipeData>();
    [ShowInInspector]public List<RecipeData> recipesCommon = new List<RecipeData>();
    [ShowInInspector]public List<RecipeData> recipesMagic = new List<RecipeData>();
    [ShowInInspector]public List<RecipeData> recipesLegend = new List<RecipeData>();

    private readonly float[] maxCollectionIncreases = { 1, 2, 4, 8, 14, 30, 30, 30, 80 };

    private void Start()
    {
        LoadRecipes();
        CreateList();
        Test();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void IncreaseCollection(RecipeData recipeInfo, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            recipeInfo.recipeCurExp++;
            if (recipeInfo.recipeCurExp >= recipeInfo.recipeMaxExp)
            {
                LevelUp(recipeInfo);
            }
        }
    }

    public void LevelUp(RecipeData recipe)
    {
        if (recipe.recipeCurLevel < recipe.recipeMaxLevel)
        {
            recipe.recipeCurLevel++;

            if (recipe.recipeCurLevel - 1 < maxCollectionIncreases.Length)
            {
                recipe.recipeMaxLevel += maxCollectionIncreases[(int)(recipe.recipeCurLevel - 1)];
            }
        }
        else
        {
            Debug.Log("이미 만렙입니다.");
        }
    }
    private void LoadRecipes()
    {
        recipesInfo = Resources.LoadAll<RecipeInfo>("ScriptableObjects/Recipe");
        if (recipesInfo == null || recipesInfo.Length == 0)
        {
            Debug.LogError("RecipeInfo assets could not be loaded. Please check the path and ensure there are assets in the specified directory.");
        }
        //else
        //{
        //    Debug.Log($"{recipesInfo.Length} RecipeInfo assets loaded successfully.");
        //}
    }


    private void Test()
    {
        for (int i = 0; i < recipes.Count; i++)
        {
            if (
                recipes[i].recipeInfo.recipeType == Recipes.HpPotion ||
                recipes[i].recipeInfo.recipeType == Recipes.MpPotion ||
                recipes[i].recipeInfo.recipeType == Recipes.Egg
               )
            {
                recipes[i].recipeCurLevel = 1;
                recipes[i].recipeMaxLevel = 1;
                recipes[i].recipeCurExp = 1;
                recipes[i].recipeMaxExp = 1;
            }
        }
        
    }

    private void CreateList()
    {
        for(int i = 0; i< recipesInfo.Length; i++)
        {
            RecipeData data = new RecipeData();
            data.recipeInfo = recipesInfo[i];
            data.recipeCurLevel = 1;
            data.recipeMaxLevel = 5;
            data.recipeCurExp = 1;
            data.recipeMaxExp = 1;
            if (recipesInfo[i].recipeRare == RecipeRare.Common)
            {
                recipesCommon.Add(data);
            }
            else if (recipesInfo[i].recipeRare == RecipeRare.Magic)
            {
                recipesMagic.Add(data);
            }
            else
            {
                recipesLegend.Add(data);
            }
            recipes.Add(data);
        }
        recipes = recipes.OrderBy(p => p.recipeInfo.idxNum).ToList();
    }
}
