using UnityEngine;
using TMPro;

public class UIMedalPopup : UIBase
{
    [SerializeField] private Currency currencyMedal;
    [SerializeField] private TMP_Text maxPartyNumberText;

    private void Awake()
    {
        UIManager.Instance.AddUIDictionary(UIType.CurrencyUI_Medal, this);
    }

    void Start()
    {
        UserInfo.userInfo.SetMedalEvent(currencyMedal.SetText);
        UserInfo.userInfo.SetMaxPartyNumberEvent(SetMaxPartyNumberText);
    }

    private void OnDestroy()
    {
        UserInfo.userInfo.RemoveMedalEvent(currencyMedal.SetText);
        UserInfo.userInfo.RemoveMaxPartyNumberEvent(SetMaxPartyNumberText);
    }

    public void SetMaxPartyNumberText(int value)
    {
        maxPartyNumberText.text = UserInfo.userInfo.MaxPartyNumber.ToString();
    }
}
