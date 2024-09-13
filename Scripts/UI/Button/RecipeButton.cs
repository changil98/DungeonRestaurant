using TMPro;
using UnityEngine;

public class RecipeButton : MonoBehaviour
{
    public GameObject recipe;
    public GameObject restaurantUpgrade;

    public void OnClickRecipeButton()
    {
        SoundManager.Instance.PlaySound("SFX_UI_Click");
        if (!DataManager.Instance.userInfo.isUserTutorials && ToturialsManager.Instance.phase == 23)
        {
            ToturialsManager.Instance.isClear[7] = true;
            ToturialsManager.Instance.OnNextPhase();
        }
        if (recipe.activeSelf == true)
        {
            recipe.SetActive(false);
        }
        else
        {
            recipe.SetActive(true);
            if (restaurantUpgrade.activeSelf == true)
            {
                restaurantUpgrade.SetActive(false);
            }
        }
    }
}