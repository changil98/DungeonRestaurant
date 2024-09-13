public class EnemySpawnInfo
{
    public string Rcode { get; set; }
    public string WaveRcode { get; set; }
    public SpawnArea SpawnArea { get; set; }
    public string EnemyRcode { get; set; }
    public EnemyInfo EnemyInfo { get; set; }
    public float SpawnRate { get; set; }
    public bool IsClear { get; set; } = false;
}