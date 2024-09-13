using Sirenix.OdinInspector;
using System.Collections.Generic;

[System.Serializable]
public class DungeonInfo
{
    [ShowInInspector] public string Rcode { get; set; }
    [ShowInInspector] public DungeonTheme Theme { get; set; }
    [ShowInInspector] public string DungeonName { get; set; }
    [ShowInInspector] public int Stage { get; set; }
    [ShowInInspector] public float WaveTime { get; set; }
    [ShowInInspector] public List<WaveData> WaveList { get; set; } = new List<WaveData>();

    [ShowInInspector] public int ClearGold { get; set; }
    [ShowInInspector] public int ClearMedal { get; set; }
    [ShowInInspector] public float DefeatMultiplier { get; set; }

    public bool IsClear { get; set; } = false;

    public void AddWaveData(WaveData wave)
    {
        WaveList.Add(wave);
    }

    public void DungeonClear()
    {
        IsClear = true;
    }

    public void DungeonClearCancel()
    {
        IsClear = false;
    }

    public WaveData GetWaveData(int wave)
    {
        return WaveList[wave];
    }

    public HashSet<EnemyInfo> GetEnemyHashSet()
    {
        List<EnemyInfo> dataList = new List<EnemyInfo>();
        foreach(var list in WaveList)
        {
            dataList.AddRange(list.GetEnemyList());
        }

        HashSet<EnemyInfo> result = new HashSet<EnemyInfo>(dataList);

        return result;
    }

}