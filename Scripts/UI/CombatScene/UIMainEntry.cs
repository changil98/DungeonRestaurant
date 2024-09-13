using System.Collections.Generic;
using UnityEngine;

public class UIMainEntry : UIEntryBase
{
    [SerializeField] public List<SlotMainEntry> slots;
    [SerializeField] private SlotMainEntry _selectedSlot;

    public SlotMainEntry selectedSlot
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

        if (!IsInit)
            Initialize();
    }

    private void Initialize()
    {
        EntryPreset preset = DataManager.Instance.entryPreset;
        for (int i = 0; i < GameManager.MAX_POSITION; i++)
        {
            GameObject obj;
            obj = Instantiate(slotPrefab, background);
            SlotMainEntry slot = obj.GetComponent<SlotMainEntry>();
            slot.CharacterData = null;
            slot.Index = i;
            slot.OnSlotUpdate += preset.EntryList[i].SetEntry;
            slot.toggle.onValueChanged.AddListener(OnToggleValueChanged);
            slot.toggle.group = toggleGroup;
            slots.Add(slot);
        }
        IsInit = true;
    }

    private void OnDestroy()
    {
        RemoveEntryEvent(DataManager.Instance.entryPreset);
    }

    public void RemoveEntryEvent(EntryPreset preset)
    {
        int i = 0;
        foreach (var entry in preset.EntryList)
        {
            slots[i].OnSlotUpdate -= entry.SetEntry;
            i++;
        }
    }

    public override void OnToggleValueChanged(bool isOn)
    {
        base.OnToggleValueChanged(isOn);
        selectedSlot = currentActiveToggle?.gameObject.GetComponent<SlotMainEntry>();
    }

}
