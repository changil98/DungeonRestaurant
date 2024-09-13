using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ManageSlotInstance : MonoBehaviour
{
    public List<Management> managements;
    public Transform background;
    public GameObject ManagementPrefab;
    [SerializeField] private Currency currencyGold;

    public Sprite common;
    public Sprite rare;
    public Sprite hero;

    public TextMeshProUGUI characterCount;

    private void Awake()
    {
        SetCharacter();
        SetCharacterCount();
    }

    private void Start()
    {
        UserInfo.userInfo.SetGoldEvent(currencyGold.SetText);
    }


    private void OnDestroy()
    {
        UserInfo.userInfo.RemoveGoldEvent(currencyGold.SetText);
    }

    public void SetCharacter()
    {
        int managemetsIndex = 0;
        foreach (CharacterData info in DataManager.Instance.characterList)
        {
            GameObject slot;
            slot = Instantiate(ManagementPrefab, background);

            Management management = slot.GetComponent<Management>();

            management.CharacterData = info;
            management.index = managemetsIndex;
            managemetsIndex++;
            management.manageSlotInstance = this;

            managements.Add(management);
            CharacterBackgroundColor(management);
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

    private void CharacterBackgroundColor(Management management)
    {
        switch (management.CharacterData.Info.Rank)
        {
            case eCharacterRank.Common:
                management.backgroundImage.sprite = common;
                break;
            case eCharacterRank.Rare:
                management.backgroundImage.sprite = rare;
                break;
            case eCharacterRank.Hero:
                management.backgroundImage.sprite = hero;
                break;
            default: break;
        }
    }

    public void Refresh()
    {
        ClearSlots();
        SetCharacter();
        SetCharacterCount();
    }

    private void ClearSlots()
    {
        foreach (var management in managements)
        {
            Destroy(management.gameObject);
        }
        managements.Clear();
    }

    public void BackBtn()
    {
        SoundManager.Instance.PlaySound("SFX_UI_Click");
        gameObject.SetActive(false);
    }
}
