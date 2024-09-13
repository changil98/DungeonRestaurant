using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEmployment : MonoBehaviour
{
    [SerializeField] private Currency currencyGold;
    void Start()
    {
        UserInfo.userInfo.SetGoldEvent(currencyGold.SetText);
    }

    private void OnDestroy()
    {
        UserInfo.userInfo.RemoveGoldEvent(currencyGold.SetText);
    }
}
