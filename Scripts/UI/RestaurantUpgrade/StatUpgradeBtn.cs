using TMPro;
using UnityEngine;

public class StatUpgradeBtn : MonoBehaviour
{
    public RestaurantUpgrade restaurantUpgrade;
    public UpgradeType upgradeType;
    public StatUpgradeInfo selectedStatInfo;

    public void SetSelectedStatInfo(StatUpgradeInfo statInfo)
    {
        selectedStatInfo = statInfo;
        SetUpgradeType(statInfo.upgradeType);
    }

    public void SetUpgradeType(UpgradeType upgradeType)
    {
        this.upgradeType = upgradeType;
    }

    public void StatLevelUp()
    {
        SoundManager.Instance.PlaySound("SFX_UI_Click");
        int currentLevel = selectedStatInfo.GetCurrentLevel();
        int maxLevel = selectedStatInfo.statInfo.maxLevel;
        int requiredGold = selectedStatInfo.GetRequiredRecipePoint(currentLevel + 1);

        if (currentLevel >= maxLevel)
        {
            Debug.Log("Max Level");
            return;
        }

        if (UserInfo.userInfo.RecipePoint < requiredGold)
        {
            Debug.Log("Not Enough RecipePoint.");
            return;
        }

        UpgradeGold(requiredGold);
        UpgradeManager.Instance.IncreaseStatLevel(upgradeType);

        foreach (var character in DataManager.Instance.characterList)
        {
            switch (upgradeType)
            {
                case UpgradeType.Atk:
                case UpgradeType.Def:
                case UpgradeType.Hp:
                case UpgradeType.Resist:
                case UpgradeType.AttackSpeed:
                case UpgradeType.CriticalDmg:
                case UpgradeType.CriticalPercent:
                    UpgradeManager.Instance.StatUpgradeAllCharacters(upgradeType);
                    UpgradeManager.Instance.StatUpgradeEmployCharacters(upgradeType);
                    break;
                default: break;
            }
        }

        selectedStatInfo?.UpdateUI(upgradeType);
        selectedStatInfo?.StatInfoSet(upgradeType);
    }

    public void UpgradeGold(int amount)
    {
        if (UserInfo.userInfo.RecipePoint >= amount)
        {
            UserInfo.userInfo.RecipePoint -= amount;
        }
    }
}
