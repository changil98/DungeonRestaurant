using UnityEngine;
using TMPro;

public class UIGoldPopup : UIBase
{
    [SerializeField] private Currency currencyGold;

    private void Awake()
    {
        UIManager.Instance.AddUIDictionary(UIType.CurrencyUI_Gold, this);
    }

    void Start()
    {
        UserInfo.userInfo.SetGoldEvent(currencyGold.SetText);
    }

    private void OnDestroy()
    {
        UserInfo.userInfo.RemoveGoldEvent(currencyGold.SetText);
    }
}
