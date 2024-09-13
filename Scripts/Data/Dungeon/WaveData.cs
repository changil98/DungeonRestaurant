using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum SpawnArea
{
    Front,
    Top,
    Bottom
}

[System.Serializable]
public class WaveData
{
    [ShowInInspector] public string Rcode { get; set; }
    [ShowInInspector] public string DungeonRcode { get; set; }
    [ShowInInspector] public int WaveNumber { get; set; }

    [ShowInInspector]
    public Dictionary<SpawnArea, List<EnemySpawnInfo>> SpawnInfoDict { get; set; }
        = new Dictionary<SpawnArea, List<EnemySpawnInfo>>();

    [ShowInInspector] public int TopSpawnLimit { get; set; }
    [ShowInInspector] public int BottomSpawnLimit { get; set; }
    [ShowInInspector] public int FrontSpawnLimit { get; set; }
    [ShowInInspector] public int MaxMonsterCount { get; set; }

    public void AddEnemySpawnInfo(EnemySpawnInfo enemySpawnInfo)
    {
        if (!SpawnInfoDict.ContainsKey(enemySpawnInfo.SpawnArea))
        {
            SpawnInfoDict[enemySpawnInfo.SpawnArea] = new List<EnemySpawnInfo>();
        }

        SpawnInfoDict[enemySpawnInfo.SpawnArea].Add(enemySpawnInfo);
    }

    public List<EnemyInfo> GetEnemyList()
    {
        List<EnemyInfo> result = new List<EnemyInfo>();

        foreach(var list in SpawnInfoDict.Values)
        {
            foreach (var item in list)
            {
                result.Add(item.EnemyInfo);
            }
        }
        return result;
    }

    public List<EnemyInfo> GetEnemieListBySpawnArea(SpawnArea area)
    {
        List<EnemyInfo> result = new List<EnemyInfo>();

        if (SpawnInfoDict.ContainsKey(area))
        {
            List<EnemySpawnInfo> spawnInfos = SpawnInfoDict[area];
            int spawnLimit = GetSpawnLimit(area);

            while (result.Count < spawnLimit)
            {
                float randomValue = UnityEngine.Random.value;
                float cumulativeProbability = 0f;

                foreach (var spawnInfo in spawnInfos)
                {
                    cumulativeProbability += spawnInfo.SpawnRate;
                    if (randomValue <= cumulativeProbability)
                    {
                        result.Add(spawnInfo.EnemyInfo);
                        break;
                    }
                }
            }
            return result;
        }
        else
        {
            return null;
        }
    }

    private int GetSpawnLimit(SpawnArea area)
    {
        switch (area)
        {
            case SpawnArea.Front:
                return FrontSpawnLimit;
            case SpawnArea.Top:
                return TopSpawnLimit;
            case SpawnArea.Bottom:
                return BottomSpawnLimit;
            default:
                return 0;
        }
    }
}