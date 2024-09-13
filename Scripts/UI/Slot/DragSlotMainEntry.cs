using System;
using UnityEngine;

public class DragSlotMainEntry : DragSlotBase
{
    private SlotMainEntry slot;
    public SlotMainEntry Slot
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

    [SerializeField] private GameObject emptyImage;

    protected override void Awake()
    {
        base.Awake();
        OnDataUpdate += SetEmptyImage;
    }

    protected void SetEmptyImage(CharacterData characterData)
    {
        if (characterData != null)
            emptyImage.SetActive(false);
        else if (characterData == null)
            emptyImage.SetActive(true);
    }
}
