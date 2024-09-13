using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MedalUpgrade : MonoBehaviour
{
    public MedalUpgradeInfo medalUpgradeInfo;
    public MedalUpgradeType medalUpgradeType;

    public TextMeshProUGUI statName;
    public TextMeshProUGUI statLv;

    public Toggle toggle;
    public GameObject UpgradeInfo;

    [Title("��ȭ ����")]
    public TextMeshProUGUI statCurrentLevel;
    public TextMeshProUGUI statCurrentImpact;
    public TextMeshProUGUI statNextLevel;
    public TextMeshProUGUI statNextImpact;
    public TextMeshProUGUI needMedal;

    private void Awake()
    {
        StatInfoSet(medalUpgradeType);
    }

    public void StatInfoSet(MedalUpgradeType type)
    {
        this.medalUpgradeType = type;
        UpdateStatText();
    }

    private void UpdateStatText()
    {
        switch (medalUpgradeType)
        {
            case MedalUpgradeType.entry:
                statName.text = medalUpgradeInfo.medalUpgradeName;
                statLv.text = $"Lv. {DataManager.Instance.UpgradeLevel.entryLevel}";
                break;
            case MedalUpgradeType.employPrice:
                statName.text = medalUpgradeInfo.medalUpgradeName;
                statLv.text = $"Lv. {DataManager.Instance.UpgradeLevel.employPriceLevel}";
                break;
            case MedalUpgradeType.characterLvUpPrice:
                statName.text = medalUpgradeInfo.medalUpgradeName;
                statLv.text = $"Lv. {DataManager.Instance.UpgradeLevel.characterLvUpPriceLevel}";
                break;
            case MedalUpgradeType.rarePercent:
                statName.text = medalUpgradeInfo.medalUpgradeName;
                statLv.text = $"Lv. {DataManager.Instance.UpgradeLevel.rarePercentLevel}";
                break;
            case MedalUpgradeType.heroPercent:
                statName.text = medalUpgradeInfo.medalUpgradeName;
                statLv.text = $"Lv. {DataManager.Instance.UpgradeLevel.heroPercentLevel}";
                break;
            case MedalUpgradeType.maxCharacterListCount:
                statName.text = medalUpgradeInfo.medalUpgradeName;
                statLv.text = $"Lv. {DataManager.Instance.UpgradeLevel.maxCharacterListCountLevel}";
                break;
            default: break;
        }
    }

    public void ShowUpgradeInfo(bool isOn)
    {
        SoundManager.Instance.PlaySound("SFX_UI_Click");
        if (!DataManager.Instance.userInfo.isUserTutorials && ToturialsManager.Instance.phase == 28)
        {
            ToturialsManager.Instance.isClear[10] = true;
            ToturialsManager.Instance.OnNextPhase();
        }
        UpgradeInfo.SetActive(true);
        UpdateUI(medalUpgradeType);

        var medalUpgradeBtn = FindObjectOfType<MedalUpgradeBtn>();
        if (medalUpgradeBtn != null)
        {
            medalUpgradeBtn.SetSelectedStatInfo(this);
        }
    }

    public void UpdateUI(MedalUpgradeType medalUpgradeType)
    {
        int currentLevel = GetCurrentLevel();
        int nextLevel = currentLevel + 1;

        statCurrentLevel.text = $"Lv. {currentLevel}";
        statCurrentImpact.text = GetCurrentImpact(currentLevel);

        if (currentLevel < medalUpgradeInfo.maxLevel)
        {
            statNextLevel.text = $"Lv. {nextLevel}";
            statNextImpact.text = GetNextImpact(nextLevel);
            needMedal.text = GetRequiredMedal(nextLevel).ToString();
        }
        else
        {
            statNextLevel.text = "";
            statNextImpact.text = "";
            needMedal.text = "�ִ� ����";
        }
    }

    public int GetCurrentLevel()
    {
        return medalUpgradeType switch
        {
            MedalUpgradeType.entry => DataManager.Instance.UpgradeLevel.entryLevel,
            MedalUpgradeType.employPrice => DataManager.Instance.UpgradeLevel.employPriceLevel,
            MedalUpgradeType.characterLvUpPrice => DataManager.Instance.UpgradeLevel.characterLvUpPriceLevel,
            MedalUpgradeType.rarePercent => DataManager.Instance.UpgradeLevel.rarePercentLevel,
            MedalUpgradeType.heroPercent => DataManager.Instance.UpgradeLevel.heroPercentLevel,
            MedalUpgradeType.maxCharacterListCount => DataManager.Instance.UpgradeLevel.maxCharacterListCountLevel,
            _ => 0
        };
    }

    private string GetCurrentImpact(int level)
    {
        float percentage = 0.05f * level;
        float gachaPercent = 0.01f * level;
        return medalUpgradeType switch
        {
            MedalUpgradeType.entry => $"���� ȿ��: ��Ƽ �� �ִ�ġ({UserInfo.userInfo.MaxPartyNumber}).",
            MedalUpgradeType.employPrice => $"���� ȿ��: ��� ����� {percentage * 100}% �����մϴ�.",
            MedalUpgradeType.characterLvUpPrice => $"���� ȿ��: ������ ����� {percentage * 100}% �����մϴ�.",
            MedalUpgradeType.rarePercent => $"���� ȿ��: Rare ����� ���� Ȯ���� {gachaPercent * 100}% �����մϴ�.",
            MedalUpgradeType.heroPercent => $"���� ȿ��: Hero ����� ���� Ȯ���� {gachaPercent * 100}% �����մϴ�.",
            MedalUpgradeType.maxCharacterListCount => $"���� ȿ��: ĳ���� ��� �ִ�ġ({10 + 5 * DataManager.Instance.UpgradeLevel.maxCharacterListCountLevel}��)",
            _ => ""
        };
    }

    private string GetNextImpact(int nextLevel)
    {
        float percentage = 0.05f * nextLevel;
        float gachaPercent = 0.01f * nextLevel;
        return medalUpgradeType switch
        {
            MedalUpgradeType.entry => $"���� ȿ��: ��Ƽ �� �ִ�ġ({UserInfo.userInfo.MaxPartyNumber + 1}).",
            MedalUpgradeType.employPrice => $"���� ȿ��: ��� ����� {percentage * 100}% �����մϴ�.",
            MedalUpgradeType.characterLvUpPrice => $"���� ȿ��: ������ ����� {percentage * 100}% �����մϴ�.",
            MedalUpgradeType.rarePercent => $"���� ȿ��: Rare ����� ���� Ȯ���� {gachaPercent * 100}% �����մϴ�.",
            MedalUpgradeType.heroPercent => $"���� ȿ��: Hero ����� ���� Ȯ���� {gachaPercent * 100}% �����մϴ�.",
            MedalUpgradeType.maxCharacterListCount => $"���� ȿ��: ĳ���� ��� �ִ�ġ({10 + 5 * (DataManager.Instance.UpgradeLevel.maxCharacterListCountLevel + 1)}��)",
            _ => ""
        };
    }

    public int GetRequiredMedal(int nextLevel)
    {
        if (nextLevel > medalUpgradeInfo.needMedal.Count)
        {
            return 0;
        }
        return medalUpgradeInfo.needMedal[nextLevel - 1];
    }
}
