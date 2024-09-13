using TMPro;
using UnityEngine;

public class UIVictory : UIBase
{
    [SerializeField] private Currency GetGold;
    [SerializeField] private Currency GetMedal;
    [SerializeField] private Currency GetRecipePoint;

    [SerializeField] private TMP_Text clearGoldText;
    [SerializeField] private TMP_Text clearMedalText;
    [SerializeField] private TMP_Text clearRecipePointText;

    // TODO : Ŭ���� �� ������ ȹ�� ���
    [SerializeField] private Transform itemPanel;
    [SerializeField] private GameObject itemSlotPrefab;

    private void OnEnable()
    {
        opened?.Invoke(null);
    }

    public override void OnOpened(object[] param)
    {
        SetResultUI();
    }


    public void SetResultUI()
    {
        GetGold.SetText(GameManager.Instance.combatController.GainGold);
        GetMedal.SetText(GameManager.Instance.combatController.CurrentDungeon.ClearMedal);
        GetRecipePoint.SetText(GameManager.Instance.combatController.GainRecipePoint);

        // TODO : Ŭ����� ȹ�� ������ ���� ����
        //foreach(Item item in ItemList)
        //{
        //    GameObject obj = Instantiate(itemSlotPrefab, itemPanel);
        //    ItemSlot itemSlot = obj.GetComponent<ItemSlot>;
        //    itemSlot.item = item;
        //}
    }
}
