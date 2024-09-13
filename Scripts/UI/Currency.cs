using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public enum CurrencyType
{
    Gold,
    Medal,
    RecipePoint
}

public class Currency : MonoBehaviour
{
    [SerializeField, EnumPaging] private CurrencyType currencyType;
    [SerializeField] private List<Sprite> iconSprites;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text text;
    [SerializeField] private Button button;

    private void Start()
    {
        SetUI();
    }

    public void SetUI()
    {
        if (icon != null)
            icon.sprite = iconSprites[(int)currencyType];
    }

    public void SetText(string str)
    {
        text.text = str;
    }

    public void SetText(int value)
    {
        text.text = string.Format("{0:#,##0}", value);
    }

    public void SetText(int value, int max)
    {
        text.text = string.Format("{0} / {1}", value, max);
    }

    public void OnButtonClick()
    {
        SoundManager.Instance.PlaySound("SFX_UI_Click");
        UIManager.Instance.CurrencyPopupUI(currencyType);
    }
}
