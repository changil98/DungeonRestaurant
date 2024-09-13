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

    [Title("강화 정보")]
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
            needMedal.text = "최대 레벨";
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
            MedalUpgradeType.entry => $"현재 효과: 파티 편성 최대치({UserInfo.userInfo.MaxPartyNumber}).",
            MedalUpgradeType.employPrice => $"현재 효과: 고용 비용이 {percentage * 100}% 감소합니다.",
            MedalUpgradeType.characterLvUpPrice => $"현재 효과: 레벨업 비용이 {percentage * 100}% 감소합니다.",
            MedalUpgradeType.rarePercent => $"현재 효과: Rare 등급의 등장 확률이 {gachaPercent * 100}% 증가합니다.",
            MedalUpgradeType.heroPercent => $"현재 효과: Hero 등급의 등장 확률이 {gachaPercent * 100}% 증가합니다.",
            MedalUpgradeType.maxCharacterListCount => $"현재 효과: 캐릭터 목록 최대치({10 + 5 * DataManager.Instance.UpgradeLevel.maxCharacterListCountLevel}명)",
            _ => ""
        };
    }

    private string GetNextImpact(int nextLevel)
    {
        float percentage = 0.05f * nextLevel;
        float gachaPercent = 0.01f * nextLevel;
        return medalUpgradeType switch
        {
            MedalUpgradeType.entry => $"다음 효과: 파티 편성 최대치({UserInfo.userInfo.MaxPartyNumber + 1}).",
            MedalUpgradeType.employPrice => $"다음 효과: 고용 비용이 {percentage * 100}% 감소합니다.",
            MedalUpgradeType.characterLvUpPrice => $"다음 효과: 레벨업 비용이 {percentage * 100}% 감소합니다.",
            MedalUpgradeType.rarePercent => $"다음 효과: Rare 등급의 등장 확률이 {gachaPercent * 100}% 증가합니다.",
            MedalUpgradeType.heroPercent => $"다음 효과: Hero 등급의 등장 확률이 {gachaPercent * 100}% 증가합니다.",
            MedalUpgradeType.maxCharacterListCount => $"다음 효과: 캐릭터 목록 최대치({10 + 5 * (DataManager.Instance.UpgradeLevel.maxCharacterListCountLevel + 1)}명)",
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
