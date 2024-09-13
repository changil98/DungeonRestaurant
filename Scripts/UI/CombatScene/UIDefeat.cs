using UnityEngine;
using TMPro;
using static UserInfo;

public class UIDefeat : UIBase
{
    [SerializeField] private Currency GetGold;
    [SerializeField] private Currency GetRecipePoint;

    [SerializeField] private TMP_Text clearGoldText;
    [SerializeField] private TMP_Text clearRecipePointText;

    private void OnEnable()
    {
        opened?.Invoke(null);
    }

    public override void OnOpened(object[] param) 
    {
        SetResultUI();
    }

    public virtual void SetResultUI()
    {
        GetGold.SetText(GameManager.Instance.combatController.GainGold);
        GetRecipePoint.SetText(GameManager.Instance.combatController.GainRecipePoint);
    }
}
