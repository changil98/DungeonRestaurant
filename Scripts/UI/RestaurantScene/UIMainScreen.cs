using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMainScreen : MonoBehaviour
{
    [SerializeField] private Currency currencyGold;
    [SerializeField] private Currency currencyMedal;
    [SerializeField] private UserLevelDisplay userLevelDisplay;

    void Start()
    {
        UserInfo.userInfo.SetGoldEvent(currencyGold.SetText);
        UserInfo.userInfo.SetMedalEvent(currencyMedal.SetText);
        UserInfo.userInfo.SetUserLevelEvent(userLevelDisplay.SetUserLevel);
    }

    private void OnDestroy()
    {
        UserInfo.userInfo.RemoveGoldEvent(currencyGold.SetText);
        UserInfo.userInfo.RemoveMedalEvent(currencyMedal.SetText);
        UserInfo.userInfo.RemoveUserLevelEvent(userLevelDisplay.SetUserLevel);
    }
}
