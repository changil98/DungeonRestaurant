using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class CharacterEmploymentList : MonoBehaviour
{
    [Title("CharacterEmployment")]
    public CharacterEmployment[] characterEmployments;

    public Sprite common;
    public Sprite rare;
    public Sprite hero;

    public int cost;
    public TextMeshProUGUI rerollCost;
    public TextMeshProUGUI characterCount;

    public List<GameObject> employmentScreens = new List<GameObject>();


    public void Start()
    {
        SetCharacterCount();
        RerollCost();

        for (int i = 0; i < characterEmployments.Length; i++)
        {
            characterEmployments[i].CharacterData = EmploymentManager.Instance.employmentList[i];
            characterEmployments[i].characterEmploymentList = this;
            UpgradeManager.Instance.StatUpgradeAll(characterEmployments[i].CharacterData.Stat);
            CharacterBackgroundColor(characterEmployments[i]);
            if (!EmploymentManager.Instance.isEmploymentAvailability[i])
            {
                employmentScreens[i].SetActive(true);
            }
        }
    }

    public void Reroll()
    {
        SoundManager.Instance.PlaySound("SFX_UI_Click");
        RerollCost();
        if (DataManager.Instance.userInfo.Gold >= cost) 
        {
            foreach (GameObject screen in employmentScreens)
            {
                screen.SetActive(false);
            }

            EmploymentManager.Instance.ReRoll();
            DataManager.Instance.userInfo.Gold = (DataManager.Instance.userInfo.Gold - (cost));
            for (int i = 0; i < characterEmployments.Length; i++)
            {
                characterEmployments[i].CharacterData = EmploymentManager.Instance.employmentList[i];
                UpgradeManager.Instance.StatUpgradeAll(characterEmployments[i].CharacterData.Stat);
                CharacterBackgroundColor(characterEmployments[i]);
            }
        }
        else
        {
            Debug.Log("비용이 부족합니다!");
        }
    }

    public void RerollCost()
    {
        cost = DataManager.Instance.UpgradeLevel.rerollPrice * UserInfo.userInfo.UserLevel;
        rerollCost.text = cost.ToString();
    }


    public void CreateScreen(int idx)
    {
        employmentScreens[idx].SetActive(true);
    }

    private void CharacterBackgroundColor(CharacterEmployment character)
    {
        switch (character.CharacterData.Info.Rank)
        {
            case eCharacterRank.Common:
                character.characterBackground.sprite = common;
                break;
            case eCharacterRank.Rare:
                character.characterBackground.sprite = rare;
                break;
            case eCharacterRank.Hero:
                character.characterBackground.sprite = hero;      
                break;
            default:
                break;
        }
    }

    public void SetCharacterCount()
    {
        characterCount.text = $"{DataManager.Instance.characterList.Count} / {UserInfo.userInfo.maxCharacterListCount}";
        if (DataManager.Instance.characterList.Count == UserInfo.userInfo.maxCharacterListCount)
        {
            characterCount.color = Color.red;
        }
        else characterCount.color = Color.white;
    }
}