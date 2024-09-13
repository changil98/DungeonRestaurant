using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class DungeonDataList
{
    [Title("DungeonList")]
    public List<DungeonInfo> DungeonList { get; set; } = new List<DungeonInfo>();


    [Title("CurrentDungeon")]
    [SerializeField] private int currentDungeonIndex = 0;
    [SerializeField] private DungeonInfo currentDungeon;
    public DungeonInfo CurrentDungeon
    {
        get
        {
            return currentDungeon;
        }
        set
        {
            currentDungeon = value;
            onCurrentDungeonChanged?.Invoke();
        }
    }

    public delegate void CurrentDungeonHandler();
    event CurrentDungeonHandler onCurrentDungeonChanged;

    public DungeonDataList(List<DungeonInfo> list)
    {
        InitializeDungeonData(list);
    }


    public void SetCurrentDungeonEvent(CurrentDungeonHandler action)
    {
        onCurrentDungeonChanged += action;
    }
    public void RemoveCurrentDungeonEvent(CurrentDungeonHandler action)
    {
        onCurrentDungeonChanged -= action;
    }

    public void InitializeDungeonData(List<DungeonInfo> list)
    {
        DungeonList = list.
            OrderBy(dungeon => dungeon.Theme)
            .ThenBy(dungeon => dungeon.Stage)
            .ToList();

        foreach (DungeonInfo dungeon in DungeonList)
        {
            if (dungeon.IsClear == false)
            {
                CurrentDungeon = dungeon;
                break;
            }
            currentDungeonIndex++;
        }
    }

    public void OnClearDungeon()
    {
        currentDungeon.DungeonClear();
        
        if (currentDungeonIndex < DungeonList.Count - 1)
        {
            currentDungeonIndex++;
        }
        else
        {
            Debug.Log("모든 던전을 클리어 했습니다.\n");
            currentDungeonIndex = DungeonList.Count - 1;
        }
        CurrentDungeon = DungeonList[currentDungeonIndex];
    }

    public void BeforeDungeon()
    {
        currentDungeon.DungeonClearCancel();
        if (currentDungeonIndex > 0)
            currentDungeonIndex--;
        CurrentDungeon = DungeonList[currentDungeonIndex];
    }

    public int SaveClearData()
    {
        int dungeonClearCount = 0;

        foreach (DungeonInfo data in DungeonList)
        {
            if (data.IsClear == true)
            {
                dungeonClearCount++;
            }
        }
        return dungeonClearCount;
    }

    public void LoadClearData(int dungonClearCount)
    {
        for (int i = 0; i < dungonClearCount; i++)
        {
            OnClearDungeon();
        }
    }

    public void ResetDungeonClear()
    {
        while(currentDungeonIndex > 0)
        {
            BeforeDungeon();
        }
        currentDungeon.DungeonClearCancel();
    }


    public void CheckListSorting() // for Debug
    {
        foreach (DungeonInfo dungeon in DungeonList)
        {
            Debug.Log(dungeon.DungeonName + " - " + dungeon.Stage);
        }
    }
}
