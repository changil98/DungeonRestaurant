using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class DragSlotSubEntry : DragSlotBase
{
    private SlotSubEntry slot;
    public SlotSubEntry Slot
    {
        get { return slot; }
        set
        {
            slot = value;
            if (slot)
            {
                OnDataUpdate?.Invoke(slot.CharacterData);
            }
        }
    }

    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text characterNameText;

    protected override void Awake()
    {
        base.Awake();
        OnDataUpdate += SetText;
    }

    protected void SetText(CharacterData characterData)
    {
        levelText.text = characterData.Level.ToString();
        characterNameText.text = characterData.characterName.ToString();
    }
}
