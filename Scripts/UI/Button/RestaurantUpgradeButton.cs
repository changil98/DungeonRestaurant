using UnityEngine;

public class RestaurantUpgradeButton : MonoBehaviour
{
    public GameObject restaurantUpgrade;
    public GameObject recipe;

    public void OnClickRestaurantUpgrade()
    {
        SoundManager.Instance.PlaySound("SFX_UI_Click");
        if (!DataManager.Instance.userInfo.isUserTutorials && ToturialsManager.Instance.phase == 25)
        {
            ToturialsManager.Instance.isClear[9] = true;
            ToturialsManager.Instance.OnNextPhase();
        }
        if (restaurantUpgrade.activeSelf == true)
        {
            restaurantUpgrade.SetActive(false);
        }
        else
        {
            restaurantUpgrade.SetActive(true);
            if (recipe.activeSelf == true)
            {
                recipe.SetActive(false);
            }
        }
    }
}
