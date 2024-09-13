using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmployInfo : MonoBehaviour, IDataChangeHandler
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

    public TextMeshProUGUI stat;
    public TextMeshProUGUI level;
    public TextMeshProUGUI job;
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI characterExplanation;
    public GameObject infoThumnail;
    public Currency currencyGold;

    [Header("스킬")]
    public Image skillImg;
    public TextMeshProUGUI skillName;
    public TextMeshProUGUI skillDescription;

    private Camera _camera;
    private Canvas canvas;

    private CharacterEmployment characterEmployment;

    public event DataChangeEventHandler OnDataChange;

    private void Awake()
    {
        _camera = Camera.main;
        canvas = GetComponent<Canvas>();
        canvas.worldCamera = _camera;

        OnDataChange += UpdateUI;
    }

    private void Start()
    {
        UserInfo.userInfo.SetGoldEvent(currencyGold.SetText);
    }

    private void OnDestroy()
    {
        UserInfo.userInfo.RemoveGoldEvent(currencyGold.SetText);
    }

    public void SetInfo(CharacterData characterData, CharacterEmployment characterEmployment) // 캐릭터 관리에서 캐릭트 확인할 때 데이터 값 업데이트
    {
        CharacterData = characterData;
        this.characterEmployment = characterEmployment;
    }

    // 모집할 때 캐릭터의 레벨을 랜덤으로 지정해주면 그 때 어빌리티 스킬 업데이트 넣기

    private void UpdateUI() // UI 업데이트
    {
        UpdateCharacterStats();
        UpdateCharacterInfo();
        UpdateSkill();
    }

    private void UpdateCharacterStats() // 스탯 관련 UI 업데이트
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
    private void UpdateCharacterInfo() // 캐릭터 정보 업데이트
    {
        level.text = $"LV. {characterData.Level}";
        job.text = characterData.Info.ClassString;
        characterName.text = characterData.characterName;
        characterExplanation.text = characterData.Info.Description;

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

    public void BackBtn()
    {
        SoundManager.Instance.PlaySound("SFX_UI_Click");
        Destroy(gameObject);
    }

    public void EmployBtn()
    {
        SoundManager.Instance.PlaySound("SFX_UI_Click");
        int cost = characterData.Level * 1000;
        if (DataManager.Instance.userInfo.Gold >= cost)
        {
            DataManager.Instance.userInfo.Gold -= cost;
            DataManager.Instance.characterList.Add(characterData);
            characterEmployment.characterEmploymentList.CreateScreen(characterEmployment.index);
            EmploymentManager.Instance.isEmploymentAvailability[Array.IndexOf(characterEmployment.characterEmploymentList.characterEmployments,characterEmployment)] = false;
            Debug.Log("고용 성공");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("돈이 부족합니다.");
        }
    }
}
