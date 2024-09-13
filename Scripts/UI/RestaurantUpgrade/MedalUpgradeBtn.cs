using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedalUpgradeBtn : MonoBehaviour
{
    public RestaurantUpgrade restaurantUpgrade;
    public MedalUpgradeType medalUpgradeType;
    public MedalUpgrade medalUpgradeInfo;

    public void SetSelectedStatInfo(MedalUpgrade medalUpgrade)
    {
        medalUpgradeInfo = medalUpgrade;
        SetUpgradeType(medalUpgrade.medalUpgradeType);
    }

    public void SetUpgradeType(MedalUpgradeType medalUpgradeType)
    {
        this.medalUpgradeType = medalUpgradeType;
    }

    public void MedalLevelUp()
    {
        SoundManager.Instance.PlaySound("SFX_UI_Click");
        int currentLevel = medalUpgradeInfo.GetCurrentLevel();
        int maxLevel = medalUpgradeInfo.medalUpgradeInfo.maxLevel;
        int requiredMedal = medalUpgradeInfo.GetRequiredMedal(currentLevel + 1);

        if (currentLevel >= maxLevel)
        {
            Debug.Log("최대 레벨");
            return;
        }

        if (UserInfo.userInfo.Medal < requiredMedal)
        {
            Debug.Log("메달 부족.");
            return;
        }

        UpgradeMedal(requiredMedal);
        UpgradeManager.Instance.IncreaseMedalLevel(medalUpgradeType);
        UserInfo.userInfo.ExpUpUsingMedal(requiredMedal);
        medalUpgradeInfo?.UpdateUI(medalUpgradeType);
        medalUpgradeInfo?.StatInfoSet(medalUpgradeType);

        var restaurantUpgrade = FindObjectOfType<RestaurantUpgrade>();
        restaurantUpgrade?.CheckEntryLevel();
    }

    public void UpgradeMedal(int amount)
    {
        if (UserInfo.userInfo.Medal >= amount)
        {
            UserInfo.userInfo.Medal -= amount;
        }
    }
}
