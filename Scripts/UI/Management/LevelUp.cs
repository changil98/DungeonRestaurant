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
        else buyLevel.text = "최대 레벨입니다.";
        UpdateLevelUpCost();
    }

    public void UpdateLevelUpCost()
    {
        if (levelUpCosts.TryGetValue(infoManagement.characterData.Level, out requireGold))
        {
            float discount = UpgradeManager.Instance.GetCharacterLevelUpPriceDiscount();
            int discountedPrice = Mathf.RoundToInt(requireGold * (1 - discount));
            //Debug.Log($"원래 가격: {requireGold}, 감소량: {discount * 100}%, 감소된 가격: {discountedPrice}");
            price.text = $"비용 : {discountedPrice}";
        }
        else
        {
            price.text = "Max Level";
        }
    }

    public void YesLVUp() // 10렙이 되면 버튼 클릭 x
    {
        SoundManager.Instance.PlaySound("SFX_UI_Click");
        if (infoManagement.characterData.Level <= 9 && infoManagement.characterData.Level < UserInfo.userInfo.UserLevel)
        {
            if (levelUpCosts.TryGetValue(infoManagement.characterData.Level, out requireGold))
            {
                float discount = UpgradeManager.Instance.GetCharacterLevelUpPriceDiscount();
                int discountedPrice = Mathf.RoundToInt(requireGold * (1 - discount));
                //Debug.Log($"원래 가격: {requireGold}, 감소량: {discount * 100}%, 감소된 가격: {discountedPrice}");

                if (UserInfo.userInfo.Gold >= discountedPrice)
                {
                    infoManagement.characterData.LevelUp();
                    infoManagement.UpdateUI();
                    Destroy(gameObject);
                    UserInfo.userInfo.Gold -= discountedPrice;
                }
                else
                {
                    Debug.Log("돈이 부족합니다..");
                }
            }
            else
            {
                Debug.Log("Max레벨 입니다.");
            }
        }
    }

    public void NoLVUp()
    {
        SoundManager.Instance.PlaySound("SFX_UI_Click");
        Destroy(gameObject);
    }
}
