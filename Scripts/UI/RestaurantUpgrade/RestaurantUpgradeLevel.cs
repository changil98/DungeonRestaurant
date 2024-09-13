using Sirenix.OdinInspector;
using System;

[Serializable]
public class RestaurantUpgradeLevel
{
    [Title("StatLevel")]
    public int atkLevel = 0;
    public int defLevel = 0;
    public int resistLevel = 0;
    public int hpLevel = 0;
    public int AttackSpeedLevel = 0;
    public int criticalPercentLevel = 0;
    public int criticalDmgLevel = 0;

    [Title("MedalLevel")]
    public int entryLevel = 0;
    public int employPriceLevel = 0;
    public int characterLvUpPriceLevel = 0;
    public int rarePercentLevel = 0;
    public int heroPercentLevel = 0;
    public int maxCharacterListCountLevel = 0;

    [Title("Employ")]
    public float heroPercent = 0.01f;
    public float rarePercent = 0.09f;
    public float commonPercent = 0.9f;
    public int rerollPrice = 1000;

    public RestaurantUpgradeLevel()
    {
        InitializeData();
    }

    private void InitializeData()
    {
        atkLevel = 0;
        defLevel = 0;
        resistLevel = 0;
        hpLevel = 0;
        AttackSpeedLevel = 0;
        criticalPercentLevel = 0;
        criticalDmgLevel = 0;

        entryLevel = 0;
        employPriceLevel = 0;
        characterLvUpPriceLevel = 0;
        rarePercentLevel = 0;
        heroPercentLevel = 0;
        maxCharacterListCountLevel = 0;

        heroPercent = 0.01f;
        rarePercent = 0.09f;
        commonPercent = 0.9f;
        rerollPrice = 1000;
    }

    public void UpdateRarePercent() // ·¹¾î µî±Þ µîÀå È®·ü »ó½Â
    {
        float rarePercentIncrease = 0.01f;
        rarePercent = rarePercent + rarePercentIncrease;
        commonPercent = 1f - rarePercent - heroPercent;
    }

    public void UpdateHeroPercent() // ¿µ¿õ µî±Þ µîÀå È®·ü »ó½Â
    {
        float heroPercentIncrease = 0.01f;
        heroPercent = heroPercent + heroPercentIncrease;
        commonPercent = 1f - rarePercent - heroPercent;
    }

    public void Clear()
    {
        InitializeData();
    }
}
