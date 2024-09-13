using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CrateRecipeSlot : MonoBehaviour
{
    [Title("SlotInfo")]
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private GameObject slotPanel;
    [SerializeField] private TextMeshProUGUI textDescription;
    [SerializeField] private Button backBtn;

    private void Start()
    {
        for (int i = 0; i < RecipeDataManager.Instance.recipes.Count; i++)
        {
            GameObject slot = Instantiate(slotPrefab ,transform);
            Recipe recipe = slot.GetComponent<Recipe>();
            recipe.info = RecipeDataManager.Instance.recipes[i];
            recipe.recipeDescription = slotPanel;
            recipe.description = textDescription;
            recipe.button = backBtn;
        }
    }
}
