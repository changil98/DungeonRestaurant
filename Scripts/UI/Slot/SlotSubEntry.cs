using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


public class SlotSubEntry : EntrySlotBase, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject entryImage;
    public TMP_Text characterNameText;
    public TMP_Text levelText;
    private DragSlotSubEntry dragSlot;
    private bool isRegistered = false;
    public bool IsRegistered { get { return isRegistered; } }

    protected override void Awake()
    {
        base.Awake();
        OnDataChange += SetText;
        dragSlot = UIStrategy.Instance.dragSlotSub;
    }

    public void OnEntryRegister()
    {
        toggle.interactable = false;
        isRegistered = true;
        entryImage?.SetActive(true);
    }

    public void OnEntryUnRegister()
    {
        toggle.interactable = true;
        isRegistered = false;
        entryImage?.SetActive(false);
    }

    public void SetText()
    {
        levelText.text = CharacterData.Level.ToString();
        characterNameText.text = CharacterData.characterName;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isRegistered) return;

        if (UIStrategy.Instance.subEntry.selectedSlot != null)
            UIStrategy.Instance.subEntry.selectedSlot.toggle.isOn = false;
        dragSlot.SetActive(true);
        dragSlot.Slot = this;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isRegistered) return;
        dragSlot.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragSlot.Slot = null;
        dragSlot.SetActive(false);
    }

}
