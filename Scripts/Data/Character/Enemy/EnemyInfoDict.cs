public class EnemyInfoDict : DataDictionary<string, EnemyInfo>
{
    public EnemyInfoDict(DataSheetController controller) : base(controller, data => data.Rcode) { }

    public void AssignPrefabData(EnemyPrefabDict prefabList)
    {
        foreach (var data in dataDict.Values)
        {
            data.Prefab = prefabList.GetData(data.PrefabRcode);
            if (data.Prefab == null)
            {
                UnityEngine.Debug.LogError($"No Prefab Data Where Rcode : {data.PrefabRcode}!!");
                continue;
            }
        }
    }
}
