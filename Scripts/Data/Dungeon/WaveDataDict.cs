public class WaveDataDict : DataDictionary<string, WaveData>
{
    public WaveDataDict(DataSheetController controller) : base(controller, data => data.Rcode) { }

    public void AssignWaveDataToDungeon(DungeonInfoDict dungeonList)
    {
        foreach(var data in dataDict.Values) 
        {
            DungeonInfo dungeonInfo = dungeonList.GetData(data.DungeonRcode);
            if(dungeonInfo == null)
            {
                UnityEngine.Debug.LogError($"No WaveData Where Rcode : {data.DungeonRcode}!!");
                continue;
            }
            dungeonInfo.AddWaveData(data);
        }
    }
}