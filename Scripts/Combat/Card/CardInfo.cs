using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardInfo : MonoBehaviour
{
    [Title("Card Data SO")]
    public RecipeData recipeInfo;

    [Title("CardInfo")]
    public TextMeshProUGUI cardName;
    public TextMeshProUGUI cardDescription;
    public TextMeshProUGUI cardCost;
    public Image cardImage;

    [Title("CardFrame")]
    public Image[] cardFrame;

    private void Start()
    {
        CardInfoUpdate();
        CardFrameUpdate();
    }

    private void CardInfoUpdate()
    {
        cardName.text = recipeInfo.recipeInfo.recipeName;
        cardDescription.text = recipeInfo.recipeInfo.recipeDescription;
        cardCost.text = recipeInfo.recipeInfo.StomachCharging.ToString();
        cardImage.sprite = recipeInfo.recipeInfo.sprite;
    }
    private void CardFrameUpdate()
    {
        foreach (var frame in cardFrame)
        {
            frame.gameObject.SetActive(false);
        }

        if (recipeInfo.recipeInfo.recipeRare == RecipeRare.Common)
        {
            cardFrame[0].gameObject.SetActive(true);
        }
        else if (recipeInfo.recipeInfo.recipeRare == RecipeRare.Magic)
        {
            cardFrame[1].gameObject.SetActive(true);
        }
        else
        {
            cardFrame[2].gameObject.SetActive(true);
        }
    }

}
