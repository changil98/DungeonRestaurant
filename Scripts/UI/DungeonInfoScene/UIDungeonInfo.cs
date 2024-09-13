using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDungeonInfo : UIBase, IDataChangeHandler
{
    [Title("UI")]
    [SerializeField] private TMP_Text dungeonNameText;
    [SerializeField] private TMP_Text stageNumberText;
    [SerializeField] private Image dungeonImage;
    [SerializeField] private UIEnemyDescriptionPanel descipritonPanel;
    [SerializeField] GameObject enemyInfoSlotPrefab;
    [SerializeField] Transform slotParent;
    [SerializeField, ReadOnly] List<EnemyInfoSlot> slots;
    [SerializeField, ReadOnly] private int slotCount = 0;
    

    [Title("Dungeon Data")]
    private DungeonInfo currentDungeon;
    public DungeonInfo CurrentDungeon
    {
        get => currentDungeon;
        set
        {
            currentDungeon = value;
            OnDataChange?.Invoke();
        }
    }

    public event DataChangeEventHandler OnDataChange;
    private HashSet<EnemyInfo> enemyHashSet = new HashSet<EnemyInfo>();

    private void Awake()
    {
        UIManager.Instance.AddUIDictionary(UIType.DungeonInfoUI, this);
    }

    private void Start()
    {
        DataManager.Instance.dungeonList.SetCurrentDungeonEvent(LoadCurrentDungeon);
        OnDataChange += SetText;
        OnDataChange += SetImage;
        OnDataChange += ClearEnemyList;
        OnDataChange += LoadEnemyList;
        OnDataChange += SetSlot;
        CurrentDungeon = DataManager.Instance.dungeonList.CurrentDungeon;
    }

    private void OnDestroy()
    {
        DataManager.Instance.dungeonList.RemoveCurrentDungeonEvent(LoadCurrentDungeon);
    }


    private void LoadCurrentDungeon()
    {
        CurrentDungeon = DataManager.Instance.dungeonList.CurrentDungeon;
    }

    private void SetText()
    {
        dungeonNameText.text = currentDungeon.DungeonName;
        stageNumberText.text = currentDungeon.Stage.ToString();
    }

    private void SetImage()
    {
        dungeonImage.sprite = DataManager.Instance.DungeonThemeList.GetImage(currentDungeon.Theme);
    }

    private void LoadEnemyList()
    {
        if (enemyHashSet.Count > 0)
        {
            enemyHashSet.Clear();
            Debug.Log("enemyList.Clear() in LoadEnemyList();");
        }

        enemyHashSet = currentDungeon.GetEnemyHashSet();
    }

    private void SetSlot()
    {
        while(slots.Count < enemyHashSet.Count)
        {
            MakeSlot();
        }
        int i = 0;
        foreach (EnemyInfo enemyInfo in enemyHashSet)
        {
            slots[i].Data = enemyInfo;
            i++;
        }
    }

    private void MakeSlot()
    {
        GameObject obj = Instantiate(enemyInfoSlotPrefab, slotParent);
        EnemyInfoSlot slot = obj.GetComponent<EnemyInfoSlot>();
        slot.descriptionPanel = descipritonPanel;
        slots.Add(slot);
    }

    private void ClearEnemyList()
    {
        if (slots == null) return;
        for(int i = 0; i < enemyHashSet.Count; i++)
        {
            slots[i].Clear();
        }
        enemyHashSet.Clear();
    }
}

