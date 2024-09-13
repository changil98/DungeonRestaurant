using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class RestaurantUpgrade : MonoBehaviour
{
    public GameObject MedalUpgradeInfo;

    public List<GameObject> Lock;

    private void Awake()
    {
        if (DataManager.Instance.UpgradeLevel.entryLevel >= 3)
        {
            foreach (var obj in Lock)
            {
                obj.SetActive(false);
            }
        }
    }

    public void BackBtn()
    {
        SoundManager.Instance.PlaySound("SFX_UI_Click");
        if (MedalUpgradeInfo.activeSelf == true)
        {
            MedalUpgradeInfo.SetActive(false);
        }
        else gameObject.SetActive(false);
    }

    public void CheckEntryLevel()
    {
        if (DataManager.Instance.UpgradeLevel.entryLevel >= 3)
        {
            foreach (var obj in Lock)
            {
                obj.SetActive(false);
            }
        }
    }
}
