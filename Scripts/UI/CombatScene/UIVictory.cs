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

    // TODO : Å¬¸®¾î ½Ã ¾ÆÀÌÅÛ È¹µæ ¸ñ·Ï
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

        // TODO : Å¬¸®¾î½Ã È¹µæ ¾ÆÀÌÅÛ ½½·Ô »ý¼º
        //foreach(Item item in ItemList)
        //{
        //    GameObject obj = Instantiate(itemSlotPrefab, itemPanel);
        //    ItemSlot itemSlot = obj.GetComponent<ItemSlot>;
        //    itemSlot.item = item;
        //}
    }
}
