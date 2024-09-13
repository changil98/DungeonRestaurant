using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelUp : MonoBehaviour
{
    public InfoManagement infoManagement;

    public TextMeshProUGUI buyLevel;
    public TextMeshProUGUI price;

    private int requireGold;

    private readonly Dictionary<int, int> levelUpCosts = new Dictionary<int, int>
    {
        {1, 1250}, {2, 3750}, {3, 5000}, {4, 6250}, {5, 7500},
        {6, 8750}, {7, 10000}, {8, 11250}, {9, 12500}
    };

    private void Awake()
    {
        infoManagement = FindObjectOfType<InfoManagement>();

        if (infoManagement.characterData.Level < 10)
        {
            buyLevel.text = $"{infoManagement.characterData.Level} -> {infoManagement.characterData.Level + 1}";
        }
        else buyLevel.text = "�ִ� �����Դϴ�.";
        UpdateLevelUpCost();
    }

    public void UpdateLevelUpCost()
    {
        if (levelUpCosts.TryGetValue(infoManagement.characterData.Level, out requireGold))
        {
            float discount = UpgradeManager.Instance.GetCharacterLevelUpPriceDiscount();
            int discountedPrice = Mathf.RoundToInt(requireGold * (1 - discount));
            //Debug.Log($"���� ����: {requireGold}, ���ҷ�: {discount * 100}%, ���ҵ� ����: {discountedPrice}");
            price.text = $"��� : {discountedPrice}";
        }
        else
        {
            price.text = "Max Level";
        }
    }

    public void YesLVUp() // 10���� �Ǹ� ��ư Ŭ�� x
    {
        SoundManager.Instance.PlaySound("SFX_UI_Click");
        if (infoManagement.characterData.Level <= 9 && infoManagement.characterData.Level < UserInfo.userInfo.UserLevel)
        {
            if (levelUpCosts.TryGetValue(infoManagement.characterData.Level, out requireGold))
            {
                float discount = UpgradeManager.Instance.GetCharacterLevelUpPriceDiscount();
                int discountedPrice = Mathf.RoundToInt(requireGold * (1 - discount));
                //Debug.Log($"���� ����: {requireGold}, ���ҷ�: {discount * 100}%, ���ҵ� ����: {discountedPrice}");

                if (UserInfo.userInfo.Gold >= discountedPrice)
                {
                    infoManagement.characterData.LevelUp();
                    infoManagement.UpdateUI();
                    Destroy(gameObject);
                    UserInfo.userInfo.Gold -= discountedPrice;
                }
                else
                {
                    Debug.Log("���� �����մϴ�..");
                }
            }
            else
            {
                Debug.Log("Max���� �Դϴ�.");
            }
        }
    }

    public void NoLVUp()
    {
        SoundManager.Instance.PlaySound("SFX_UI_Click");
        Destroy(gameObject);
    }
}
