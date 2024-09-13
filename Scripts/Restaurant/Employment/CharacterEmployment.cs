using Sirenix.OdinInspector;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterEmployment : MonoBehaviour, IDataChangeHandler
{
    private CharacterData characterData;
    public CharacterData CharacterData
    {
        get => characterData;
        set
        {
            characterData = value;
            OnDataChange?.Invoke();
        }
    }

    public event DataChangeEventHandler OnDataChange;

    [Title("CharacterInfo")]
    public int index;

    public Toggle toggle;
    public Image characterImage;
    public GameObject employInfo;

    [Title("Character Txt")]
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI characterJob;
    public TextMeshProUGUI characterLevel;
    public TextMeshProUGUI characterSkill;
    public TextMeshProUGUI employPrice;

    [Title("Character Image")]
    public Image characterSkillImage;
    public Image characterBackground;

    [Title("CharacterEmploymentList")]
    public CharacterEmploymentList characterEmploymentList;

    private void Awake()
    {
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    public void Start()
    {
        OnDataChange += UpdateCharacterInfo;
        UpdateCharacterInfo();
    }

    public void UpdateCharacterInfo()
    {
        characterImage.sprite = CharacterData.skin.characterThumnail;
        characterImage.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
        characterName.text = characterData.characterName;
        characterJob.text = characterData.Info.ClassString;
        characterLevel.text = CharacterData.Level.ToString();
        characterSkill.text = CharacterData.skill.skillName;
        characterSkillImage.sprite = CharacterData.skill.skillImg;
        employPrice.text = Price().ToString();
    }

    public int Price()
    {
        int cost = CharacterData.Level * 1000;
        float discount = UpgradeManager.Instance.GetEmployPriceDiscount();
        int discountedPrice = Mathf.RoundToInt(cost * (1 - discount));
        return discountedPrice;
    }

    public void UpdateList()
    {
        SoundManager.Instance.PlaySound("SFX_UI_Click");
        if (DataManager.Instance.characterList.Count >= UserInfo.userInfo.maxCharacterListCount)
        {
            Debug.Log("관리등급을 업그레이드 하세요.");
            return;
        }

        int cost = CharacterData.Level * 1000;
        float discount = UpgradeManager.Instance.GetEmployPriceDiscount();
        int discountedPrice = Mathf.RoundToInt(cost * (1 - discount));

        //Debug.Log($"원래 가격: {cost}, 감소량: {discount * 100}%, 감소된 가격: {discountedPrice}");
        if (DataManager.Instance.userInfo.Gold >= discountedPrice)
        {
            CharacterData.characterName = characterName.text;
            DataManager.Instance.characterList.Add(CharacterData);
            //int index = Array.IndexOf(characterEmploymentList.characterEmployments, this);
            EmploymentManager.Instance.isEmploymentAvailability[index] = false;
            characterEmploymentList.CreateScreen(index);
            DataManager.Instance.userInfo.Gold -= discountedPrice;
            characterEmploymentList.SetCharacterCount();
        }
        else
        {
            Debug.Log("비용 부족!");
        }
    }

    public void OnToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            SoundManager.Instance.PlaySound("SFX_UI_Click");
            UpgradeManager.Instance.RemoveLastIncreaseAll(CharacterData.Stat);
            UpgradeManager.Instance.StatUpgradeAll(CharacterData.Stat);
            StartCoroutine(SetCharacterInfo(isOn, 1));
        }
    }

    private IEnumerator SetCharacterInfo(bool isOn, float time)
    {
        GameObject infoInstance = Instantiate(employInfo);
        EmployInfo infoManagementScript = infoInstance.GetComponent<EmployInfo>();
        infoManagementScript.SetInfo(CharacterData, this);
        yield return new WaitForSeconds(time);
        toggle.isOn = false;
    }

}
