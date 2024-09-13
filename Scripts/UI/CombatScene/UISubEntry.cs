using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UISubEntry : UIEntryBase
{
    public List<SlotSubEntry> slots;

    [SerializeField] private SlotSubEntry _selectedSlot;
    public SlotSubEntry selectedSlot
    {
        get => _selectedSlot;
        set
        {
            _selectedSlot = value;
            UIStrategy.Instance.Check();
        }
    }


    protected override void Awake()
    {
        base.Awake();
        if(!IsInit)
            Initialize();
    }

    private void Initialize()
    {
        int slotIndex = 0;
        foreach (CharacterData data in DataManager.Instance.characterList)
        {
            GameObject obj;
            obj = Instantiate(slotPrefab, background);
            SlotSubEntry slot = obj.GetComponent<SlotSubEntry>();
            slot.CharacterData = data;
            slot.Index = slotIndex;
            slotIndex++;
            slot.toggle.onValueChanged.AddListener(OnToggleValueChanged);
            slot.toggle.group = toggleGroup;
            slots.Add(slot);
        }
        IsInit = true;
    }

    public override void OnToggleValueChanged(bool isOn)
    {
        base.OnToggleValueChanged(isOn);
        selectedSlot = currentActiveToggle?.gameObject.GetComponent<SlotSubEntry>();
    }

}
