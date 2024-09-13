using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoManagement : MonoBehaviour
{
    public CharacterData characterData;

    public TextMeshProUGUI stat;
    public TextMeshProUGUI level;
    public TextMeshProUGUI job;
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI needGold;
    public TextMeshProUGUI characterExplanation;
    public GameObject infoThumnail;

    public int index;

    [SerializeField] private Currency currencyGold;

    [Header("��ų")]
    public Image skillImg;
    public TextMeshProUGUI skillName;
    public TextMeshProUGUI skillDescription;

    [Header("������Ʈ")]
    public GameObject LVUpPanel;
    public GameObject firePanel;
    public GameObject warningPanel;

    private Camera _camera;
    private Canvas canvas;

    private int requireGold;

    private readonly Dictionary<int, int> levelUpCosts = new Dictionary<int, int>();

    private void Awake()
    {
        _camera = Camera.main;
        canvas = GetComponent<Canvas>();
        canvas.worldCamera = _camera;

        for (int level = 1; level <= 30; level++)
        {
            int cost = (int)((level + 1) * 1000 * 1.25);
            levelUpCosts.Add(level, cost);
        }
    }

    private void Start()
    {
        UserInfo.userInfo.SetGoldEvent(currencyGold.SetText);
    }

    private void OnDestroy()
    {
        UserInfo.userInfo.RemoveGoldEvent(currencyGold.SetText);
        characterData.RemoveLevelUpEvent(UpdateUI);
    }

    public void SetInfo(CharacterData characterData, int index) // ĳ���� �������� ĳ��Ʈ Ȯ���� �� ������ �� ������Ʈ
    {
        this.index = index;
        this.characterData = characterData;
        InitializeUI();
        characterData.SetLevelUpEvent(UpdateUI);
    }

    private void InitializeUI()
    {
        UpdateCharacterInfo();
        UpdateCharacterStats();
        UpdateSkill();
    }

    public void UpdateUI()
    {
        UpdateCharacterStats();
        LevelUpUpdateUI();
    }


    private void UpdateCharacterStats() // ���� ���� UI ������Ʈ
    {
        var statType = characterData.Stat;

        stat.text = $"{statType.HP} <color=green>(+{statType.increaseHp})</color>\n" +
                    $"{statType.ATK} <color=green>(+{statType.increaseAtk})</color>\n" +
                    $"{statType.DEF} <color=green>(+{statType.increaseDef})</color>\n" +
                    $"{statType.Resistance} <color=green>(+{statType.increaseResist})</color>\n" +
                    $"{statType.AttackSpeed:F2} <color=green>(+{statType.increaseAttackSpeed:F2})</color>\n" +
                    $"{statType.CriticalPercent * 100}% <color=green>(+{statType.increaseCriticalPercent * 100}%)</color>\n" +
                    $"{statType.CriticalDamage}% <color=green>(+{statType.increaseCriticalDmg}%)</color>";
    }

    private void UpdateCharacterInfo() // ĳ���� ���� ������Ʈ
    {
        level.text = $"LV. {characterData.Level}";
        job.text = characterData.Info.ClassString;
        characterName.text = characterData.characterName;
        characterExplanation.text = characterData.Info.Description;

        levelUpCosts.TryGetValue(characterData.Level, out requireGold);
        float discount = UpgradeManager.Instance.GetCharacterLevelUpPriceDiscount();
        int discountedPrice = Mathf.RoundToInt(requireGold * (1 - discount));
        needGold.text = discountedPrice.ToString();

        GameObject instantiatedThumbnail = Instantiate(characterData.skin.characterIdle, infoThumnail.transform);
        instantiatedThumbnail.transform.localScale = new Vector3(250f, 250f, 1f);
        instantiatedThumbnail.transform.localPosition = new Vector2(0f, -30f);
    }

    private void UpdateSkill()
    {
        skillImg.sprite = characterData.skill.skillImg;
        skillName.text = characterData.skill.skillName;
        skillDescription.text = characterData.skill.skillDiscription;
    }

    public void YesLVUp()
    {
        SoundManager.Instance.PlaySound("SFX_UI_Click");
        if (characterData.Level <= levelUpCosts.Count && characterData.Level < UserInfo.userInfo.UserLevel)
        {
            if (levelUpCosts.TryGetValue(characterData.Level, out requireGold))
            {
                float discount = UpgradeManager.Instance.GetCharacterLevelUpPriceDiscount();
                int discountedPrice = Mathf.RoundToInt(requireGold * (1 - discount));
                //Debug.Log($"���� ����: {requireGold}, ���ҷ�: {discount * 100}%, ���ҵ� ����: {discountedPrice}");

                if (UserInfo.userInfo.Gold >= discountedPrice)
                {
                    characterData.LevelUp();
                    UpdateUI();
                    UserInfo.userInfo.Gold -= discountedPrice;
                }
                else
                {
                    Debug.Log("Not Enough Money.");
                }
            }
            else
            {
                Debug.Log("Max Level.");
            }
        }
        else
        {
            Instantiate(warningPanel);
        }
    }

    //public void OnClickLVUpBtn()
    //{
    //    SoundManager.Instance.PlaySound("SFX_UI_Click");
    //    Instantiate(LVUpPanel);
    //}

    private void LevelUpUpdateUI()
    {
        if (characterData.Level >= levelUpCosts.Count)
        {
            level.text = $"LV. {characterData.Level}";
            needGold.text = $"�ִ� ����";
        }
        else
        {
            level.text = $"LV. {characterData.Level}";

            levelUpCosts.TryGetValue(characterData.Level, out requireGold);
            float discount = UpgradeManager.Instance.GetCharacterLevelUpPriceDiscount();
            int discountedPrice = Mathf.RoundToInt(requireGold * (1 - discount));
            needGold.text = discountedPrice.ToString();
        }
    }

    public void FireBtn()
    {
        SoundManager.Instance.PlaySound("SFX_UI_Click");
        Instantiate(firePanel);
    }

    public void Fire()
    {
        Destroy(gameObject);
    }

    public void BackBtn()
    {
        SoundManager.Instance.PlaySound("SFX_UI_Click");
        FindObjectOfType<ManageSlotInstance>().Refresh();
        Destroy(gameObject);
    }

}
