public class EnemyPrefabDict : DataDictionary<string, EnemyPrefab>
{
    public EnemyPrefabDict(EnemyPrefab[] list) : base(list, data => data.rcode) { }
}