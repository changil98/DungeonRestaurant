using Sirenix.OdinInspector;
using System.Collections;


public class EnemyInfo : StatBase
{
    [PropertyOrder(-1), ShowInInspector] public string Rcode { get; set; }
    [PropertyOrder(-1), ShowInInspector] public string EnemyName { get; set; }
    [PropertyOrder(-1), ShowInInspector] public string PrefabRcode { get; set; }
    [ShowInInspector] public string Description { get; set; }

    [ShowInInspector] public EnemyPrefab Prefab { get; set; }

    public override bool Equals(object obj)
    {
        var other = obj as EnemyInfo;
        if (other == null)
            return false;

        return (this.Rcode == other.Rcode);
    }

    public override int GetHashCode()
    {
        return Rcode.GetHashCode();
    }
}
