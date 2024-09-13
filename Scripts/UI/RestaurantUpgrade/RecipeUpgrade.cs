using UnityEngine;

public class RecipeUpgrade : MonoBehaviour
{
    public GameObject StatUpgradeInfo;
    [SerializeField] private Currency currencyRecipePoint;

    void Start()
    {
        UserInfo.userInfo.SetRecipePointEvent(currencyRecipePoint.SetText);
    }

    private void OnDestroy()
    {
        UserInfo.userInfo.RemoveRecipePointEvent(currencyRecipePoint.SetText);
    }

    public void BackBtn()
    {
        SoundManager.Instance.PlaySound("SFX_UI_Click");
        if (!DataManager.Instance.userInfo.isUserTutorials && ToturialsManager.Instance.phase == 24)
        {
            ToturialsManager.Instance.isClear[8] = true;
            ToturialsManager.Instance.OnNextPhase();
        }

        if (StatUpgradeInfo.activeSelf == true)
        {
            StatUpgradeInfo.SetActive(false);
        }
        else gameObject.SetActive(false);
    }
}
