using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatController : SerializedMonoBehaviour
{
    #region Fields
    public static CombatController instance;

    [Title("Player")]
    [SerializeField] private CharacterData[] entry;
    [SerializeField] public List<PlayerCombatAI> playerCharacters;
    private int[] playerSpawnIndex;
    private int entryCount = 0;
    private int playerDataLoadCount = 0;
    private int PlayerDataLoadCount
    {
        get { return playerDataLoadCount; }
        set
        {
            playerDataLoadCount = value;
            CheckLoading();
        }
    }
    [SerializeField] private int playerAliveCount = 0;
    public int PlayerAliveCount
    {
        get { return playerAliveCount; }
        private set
        {
            playerAliveCount = value;
            CheckCombatOver();
        }
    }

    [Title("Enemy")]
    [SerializeField] public List<EnemyCombatAI> enemyCharacters;
    [SerializeField] private int enemyAliveCount = 0;
    public int EnemyAliveCount
    {
        get { return enemyAliveCount; }
        set
        {
            enemyAliveCount = value;
            CheckCombatOver();
        }
    }

    [Title("Dungeon")]
    [ShowInInspector, ReadOnly] public DungeonInfo CurrentDungeon { get; private set; }
    private int currentWave = 0;

    private int maxWave;
    private float time;
    private float waveTime;
    public int GainGold { get; set; }
    public int GainRecipePoint {  get; set; }
    [SerializeField] private int killCount = 0;
    [SerializeField] private int killGold = 100;
    [SerializeField] private int killRecipePoint = 1;

    [Title("SpawnPosition")]
    [SerializeField] private Transform[] playerSpawnPosition = new Transform[GameManager.MAX_POSITION];
    [SerializeField] public Dictionary<SpawnArea, EnemySpawnArea> enemySpawnArea = new Dictionary<SpawnArea, EnemySpawnArea>();

    [Title("UI")]
    public TMP_Text timeText;
    public TMP_Text waveText;
    private string waveString;
    public Slider progressBar;
    public TMP_Text dungeonNameText;
    public TMP_Text stageText;
    private string stageString = "Stage ";

    [Title("Camera Settings")]
    [SerializeField] private float cameraMoveDistance = 10f;
    [Title("TileMap")]
    private List<GameObject> instantiatedTileMaps = new List<GameObject>();

    private readonly Vector3[] tileMapPositions = new Vector3[]
    {
        new Vector3(4.8f, 3.63479f, 0f),
        new Vector3(-102.2f, 3.63479f, 0f),
        new Vector3(-31.9f, 3.63479f, 0f),
        new Vector3(41f, 3.63479f, 0f),
        new Vector3(77.6f, 3.63479f, 0f),
        new Vector3(113.7f, 3.63479f, 0f),
        new Vector3(-68.21f, 3.63479f, 0f)
    };

    public bool isPlayerWin { get; private set; }
    #endregion


    #region Unity Lifecycle Functions

    private void Awake()
    {
        instance = this;
        GameManager.Instance.combatController = this;
        entry = new CharacterData[GameManager.MAX_PARTY_NUMBER];
        playerCharacters = new List<PlayerCombatAI>();
        playerSpawnIndex = new int[GameManager.MAX_PARTY_NUMBER];

        SetDungeonData();

        InstantiateTileMap();

        StartCoroutine(SetCombatUI());
    }

    private void Update()
    {
        CheckTime();
    }
    private void OnDestroy()
    {
        instance = null;
        GameManager.Instance.combatController = null;
    }

    #endregion


    #region Set Data & UI Functions

    IEnumerator SetCombatUI()
    {
        yield return new WaitUntil(() => waveText != null);

        StringBuilder sb = new StringBuilder();
        sb.Append(" / ").Append(maxWave);
        waveString = sb.ToString();
        progressBar.maxValue = maxWave;
        SetWaveProgressUI();

        sb.Clear();
        sb.Append(stageString).Append(CurrentDungeon.Stage);
        stageText.text = sb.ToString();
        dungeonNameText.text = CurrentDungeon.DungeonName;
    }

    private void SetDungeonData()
    {
        CurrentDungeon = DataManager.Instance.dungeonList.CurrentDungeon;
        waveTime = CurrentDungeon.WaveTime;
        time = waveTime;
        maxWave = CurrentDungeon.WaveList.Count;
        currentWave = 0;
    }

    private void InstantiateTileMap()
    {
        GameObject tilemap = DataManager.Instance.DungeonThemeList.GetCombatTileMap(CurrentDungeon.Theme);

        foreach(Vector3 position in tileMapPositions)
        {
            GameObject tileMapInstance = Instantiate(tilemap, position, Quaternion.identity);
            tileMapInstance.transform.SetParent(transform);
            instantiatedTileMaps.Add(tilemap);
        }
    }

    public void SetEntry(List<SlotMainEntry> mainEntry)
    {
        int idx = 0;

        for (int i = 0; i < GameManager.MAX_POSITION; i++)
        {
            if (mainEntry[i].IsSlotEmpty() == false)
            {
                entry[idx] = mainEntry[i].CharacterData;
                playerSpawnIndex[idx] = i;
                idx++;
                entryCount++;
            }
        }
    }

    void SetWaveProgressUI()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(currentWave + 1);
        sb.Append(waveString);
        waveText.text = sb.ToString();
        progressBar.value = currentWave + 1;
    }

    #endregion


    #region Data Await Functions
    void CheckLoading()
    {
        if (PlayerDataLoadCount >= entryCount)
        {
            GameManager.Instance.SetCombatFlag(true);
        }
        return;
    }
    public void PlayerDataLoadComplete()
    {
        PlayerDataLoadCount++;
    }

    #endregion


    #region Combat Logic Functions

    public void CheckTime()
    {
        if (GameManager.Instance.isCombatStart)
        {
            time -= Time.deltaTime;
            if (time < 0)
            {
                NextWave();
            }
            timeText.text = time.ToString("00");
        }
    }

    public void SpawnPlayerCharacter()
    {
        for (int i = 0; i < entry.Length; i++)
        {
            if (entry[i] != null)
            {
                GameObject obj;
                PlayerCombatAI player;
                int spawnIndex = playerSpawnIndex[i];
                obj = Instantiate(entry[i].skin.prefab, playerSpawnPosition[spawnIndex].position, Quaternion.identity);
                player = obj.GetComponent<PlayerCombatAI>();
                player.SetPlayerData(entry[i]);
                playerCharacters.Add(player);
                playerAliveCount++;
            }
        }
    }

    public void SpawnEnemiesInWave(int wave)
    {
        WaveData waveData = CurrentDungeon.GetWaveData(wave);

        foreach(SpawnArea eSpawnArea in Enum.GetValues(typeof(SpawnArea)))
        {
            List<EnemyInfo> enemyList = waveData.GetEnemieListBySpawnArea(eSpawnArea);

            if (enemyList != null)
            {
                EnemySpawnArea spawnArea = enemySpawnArea[eSpawnArea];
                spawnArea.ClearOccupiedPositions();

                foreach (EnemyInfo enemyInfo in enemyList)
                {
                    Vector2 spawnPosition = spawnArea.GetRandomPosition();
                    SpawnEnemy(enemyInfo, spawnPosition);
                }
            }
        }
    }

    private void SpawnEnemy(EnemyInfo enemyInfo, Vector2 spawnPosition)
    {
        EnemyCombatAI enemy;
        GameObject obj;

        obj = Instantiate(enemyInfo.Prefab.prefab, spawnPosition, Quaternion.identity);
        enemy = obj.GetComponent<EnemyCombatAI>();
        enemy.SetEnemyData(enemyInfo);
        enemyCharacters.Add(enemy);
        enemyAliveCount++;
    }


    void NextWave()
    {
        if (currentWave < (maxWave - 1))
        {
            time = waveTime;
            currentWave++;
            SpawnEnemiesInWave(currentWave);
            SetWaveProgressUI();
        }
        else
        {
            Defeat();
        }
    }
    public void CharacterDie(Team team)
    {
        if (team == Team.Ally)
        {
            PlayerAliveCount--;
        }
        else if (team == Team.Enemy)
        {
            EnemyAliveCount--;
            killCount++;
        }
    }

    public void CharacterResurrect(Team team)
    {
        if (team == Team.Ally)
        {
            PlayerAliveCount++;
        }
        else if (team == Team.Enemy)
        {
            EnemyAliveCount++;
        }
    }

    public void CheckCombatOver()
    {
        if (GameManager.Instance.isCombatStart)
        {
            if (playerAliveCount == 0)
            {
                Defeat();
            }
            else if (enemyAliveCount <= 0)
            {
                if (currentWave < (maxWave - 1))
                    NextWave();
                else
                    Victory();

            }
            else return;
        }
    }

    public void Defeat()
    {
        isPlayerWin = false;
        CombatOver();
    }

    public void Victory()
    {
        isPlayerWin = true;
        EmploymentManager.Instance.ReRoll();
        CombatOver();
    }

    private void CombatOver()
    {
        GetClearReward();
        GameManager.Instance.CombatOver();
        CameraController.Instance.ResetCamera();
    }

    public void GetClearReward()
    {
        GetGold();
        GetMedal();
        GetRecipePoint();
    }

    private void GetGold()
    {
        GainGold = CurrentDungeon.ClearGold;
        if (!isPlayerWin)
        {
            GainGold = (int)(GainGold * CurrentDungeon.DefeatMultiplier);
        }
        GainGold += killCount * killGold;
        UserInfo.userInfo.Gold += GainGold;
    }

    private void GetMedal()
    {
        if (isPlayerWin)
        {
            UserInfo.userInfo.Medal += CurrentDungeon.ClearMedal;
        }
    }

    private void GetRecipePoint()
    {
        GainRecipePoint += killCount * killRecipePoint;
        UserInfo.userInfo.RecipePoint += GainRecipePoint;
    }

    #endregion


    #region Debug

    public void Debug_BattleWin()
    {
        Victory();
    }
    public void Debug_BattleDefeat()
    {
        Defeat();
    }


    public void Debug_NextWave()
    {
        time = 0;
    }

    public void Debug_KillAllEnemies()
    {
        foreach (EnemyCombatAI enemy in enemyCharacters)
        {
            if (enemy.CheckAlive() == true)
            {
                enemy.Die();
            }
        }
    }


    public void Debug_SpawnEnemiesInWave(int wave)
    {
        SpawnEnemiesInWave(wave);
    }

    public void Debug_SpawnEnemy(EnemyInfo info, SpawnArea eSpawnArea)
    {
        EnemySpawnArea spawnArea = enemySpawnArea[eSpawnArea];
        spawnArea.ClearOccupiedPositions();
        Vector2 spawnPosition = spawnArea.GetRandomPosition();
        SpawnEnemy(info, spawnPosition);
    }

    public void Debug_ResurrectPlayerCharacters()
    {
        foreach (PlayerCombatAI character in playerCharacters)
        {
            if (character == null) return;
            if (character.CheckAlive() == false)
                character.Resurrection(100);
        }
    }

    #endregion
}