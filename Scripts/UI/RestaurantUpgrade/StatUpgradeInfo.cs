using Sirenix.OdinInspector;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatUpgradeInfo : MonoBehaviour
{
    public StatInfo statInfo;
    public UpgradeType upgradeType;

    public TextMeshProUGUI recipeName;
    public Image recipeImage;
    public TextMeshProUGUI statName;
    public TextMeshProUGUI statLv;

    public Toggle toggle;
    public GameObject UpgradeInfo;

    [Title("��ȭ ����")]
    public TextMeshProUGUI statInfoName;
    public TextMeshProUGUI statCurrentLevel;
    public TextMeshProUGUI statCurrentImpact;
    public TextMeshProUGUI statNextLevel;
    public TextMeshProUGUI statNextImpact;
    public TextMeshProUGUI needRecipePoint;

    private void Awake()
    {
        StatInfoSet(upgradeType);
    }

    public void StatInfoSet(UpgradeType upgradeType)
    {
        this.upgradeType = upgradeType;
        UpdateStatText();
    }

    private void UpdateStatText()
    {
        var upgradeLevel = new Dictionary<UpgradeType, Func<int>>()
        {
            { UpgradeType.Atk, () => DataManager.Instance.UpgradeLevel.atkLevel },
            { UpgradeType.Def, () => DataManager.Instance.UpgradeLevel.defLevel },
            { UpgradeType.Resist, () => DataManager.Instance.UpgradeLevel.resistLevel },
            { UpgradeType.Hp, () => DataManager.Instance.UpgradeLevel.hpLevel },
            { UpgradeType.AttackSpeed, () => DataManager.Instance.UpgradeLevel.AttackSpeedLevel },
            { UpgradeType.CriticalPercent, () => DataManager.Instance.UpgradeLevel.criticalPercentLevel },
            { UpgradeType.CriticalDmg, () => DataManager.Instance.UpgradeLevel.criticalDmgLevel },
        };

        if (upgradeLevel.TryGetValue(upgradeType, out var getLevel))
        {
            recipeName.text = statInfo.recipeName;
            recipeImage.sprite = statInfo.recipeImage;
            statName.text = statInfo.statName;
            statLv.text = $"Lv. {getLevel()}";
        }
    }

    public void ShowUpgradeInfo(bool isOn)
    {
        SoundManager.Instance.PlaySound("SFX_UI_Click");
        UpgradeInfo.SetActive(true);
        UpdateUI(upgradeType);

        var statUpgradeBtn = FindObjectOfType<StatUpgradeBtn>();
        if (statUpgradeBtn != null)
        {
            statUpgradeBtn.SetSelectedStatInfo(this);
        }
    }

    public void UpdateUI(UpgradeType upgradeType)
    {
        int currentLevel = GetCurrentLevel();
        int nextLevel = currentLevel + 1;

        statInfoName.text = recipeName.text;
        statCurrentLevel.text = $"Lv. {currentLevel}";
        statCurrentImpact.text = GetCurrentImpact(currentLevel);

        if (currentLevel < statInfo.maxLevel)
        {
            statNextLevel.text = $"Lv. {nextLevel}";
            statNextImpact.text = GetNextImpact(nextLevel);
            needRecipePoint.text = GetRequiredRecipePoint(nextLevel).ToString();
        }
        else
        {
            statNextLevel.text = "";
            statNextImpact.text = "";
            needRecipePoint.text = "�ִ� ����";
        }
    }

    public int GetCurrentLevel()
    {
        return upgradeType switch
        {
            UpgradeType.Atk => DataManager.Instance.UpgradeLevel.atkLevel,
            UpgradeType.Def => DataManager.Instance.UpgradeLevel.defLevel,
            UpgradeType.Resist => DataManager.Instance.UpgradeLevel.resistLevel,
            UpgradeType.Hp => DataManager.Instance.UpgradeLevel.hpLevel,
            UpgradeType.AttackSpeed => DataManager.Instance.UpgradeLevel.AttackSpeedLevel,
            UpgradeType.CriticalPercent => DataManager.Instance.UpgradeLevel.criticalPercentLevel,
            UpgradeType.CriticalDmg => DataManager.Instance.UpgradeLevel.criticalDmgLevel,
            _ => 0
        };
    }

    private string GetCurrentImpact(int level)
    {
        float percentage = 0.05f * level * 100;
        
        if (level == 9)
        {
            percentage = 0.05f * (level + 1) * 100;
        }

        return upgradeType switch
        {
            UpgradeType.Atk => $"���� ȿ��: ���ݷ��� {percentage}%�� �����Ѵ�.",
            UpgradeType.Def => $"���� ȿ��: ������ {percentage}%�� �����Ѵ�.",
            UpgradeType.Hp => $"���� ȿ��: ü���� {percentage}%�� �����Ѵ�.",
            UpgradeType.Resist => $"���� ȿ��: ������ {10 * level} �����Ѵ�.",
            UpgradeType.CriticalDmg => $"���� ȿ��: ġ��Ÿ �������� {percentage}% �����Ѵ�.",
            UpgradeType.AttackSpeed => $"���� ȿ��: ���ݼӵ��� {percentage}%�� �����Ѵ�.",
            UpgradeType.CriticalPercent => $"���� ȿ��: ġ��Ÿ Ȯ���� {percentage}% �����Ѵ�.",
            _ => ""
        };
    }

    private string GetNextImpact(int nextLevel)
    {
        float percentage = 0.05f * nextLevel * 100;

        if (nextLevel == 9)
        {
            percentage = 0.05f * (nextLevel + 1) * 100;
        }

        return upgradeType switch
        {
            UpgradeType.Atk => $"���� ȿ��: ���ݷ��� {percentage}%�� �����Ѵ�.",
            UpgradeType.Def => $"���� ȿ��: ������ {percentage}%�� �����Ѵ�.",
            UpgradeType.Hp => $"���� ȿ��: ü���� {percentage}%�� �����Ѵ�.",
            UpgradeType.Resist => $"���� ȿ��: ������ {10 * nextLevel} �����Ѵ�.",
            UpgradeType.CriticalDmg => $"���� ȿ��: ġ��Ÿ �������� {percentage}% �����Ѵ�.",
            UpgradeType.AttackSpeed => $"���� ȿ��: ���ݼӵ��� {percentage}%�� �����Ѵ�.",
            UpgradeType.CriticalPercent => $"���� ȿ��: ġ��Ÿ Ȯ���� {percentage}% �����Ѵ�.",
            _ => ""
        };
    }

    public int GetRequiredRecipePoint(int nextLevel)
    {
        if (nextLevel > statInfo.needRecipePoint.Count)
        {
            return 0;
        }
        return statInfo.needRecipePoint[nextLevel - 1];
    }
}
