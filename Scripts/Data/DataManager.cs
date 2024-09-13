using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
//using System.Diagnostics;
using System.IO;
using UnityEngine;
using System.Runtime.InteropServices; // Don't erase this namespace!!

[System.Serializable]
public class SaveData
{
    public UserInfo userInfo;
    public int dungeonClearCount;
    public RestaurantUpgradeLevel UpgradeLevel;
    public List<CharacterDataSerializable> characterList;
    public EntryPreset entryPreset;
}

public class DataManager : Singleton<DataManager>
{
    //private Stopwatch stopwatch = new Stopwatch();

    #region User Data Fields
    [TabGroup("UserInfo")]
    public UserInfo userInfo = new UserInfo();

    [TabGroup("CharacterList")]
    [SerializeField] public List<CharacterData> characterList = new List<CharacterData>();

    [TabGroup("DungeonClearCount")]
    public int dungeonClearCount;

    [TabGroup("UpgradeLevel")]
    [ReadOnly]
    public RestaurantUpgradeLevel UpgradeLevel = new RestaurantUpgradeLevel();

    [TabGroup("RecipeData")]
    [ReadOnly]
    [ShowInInspector] public List<RecipeData> recipeDataList = new List<RecipeData>();

    [TabGroup("EntryPreset")]
    [SerializeField] public EntryPreset entryPreset = new EntryPreset();

    [Title("Save&Load")]
    string saveFilePath;
    #endregion

    #region Resource Data Paths
    [TabGroup("DataPath", "Data Path")]
    [SerializeField]
    [FolderPath(ParentFolder = "Assets/Resources", RequireExistingPath = true)]
    private string characterPrefabPath;

    [TabGroup("DataPath", "Data Path")]
    [SerializeField]
    [FolderPath(ParentFolder = "Assets/Resources", RequireExistingPath = true)]
    private string enemyPrefabPath;

    [TabGroup("DataPath", "Data Path")]
    [SerializeField]
    [FolderPath(ParentFolder = "Assets/Resources", RequireExistingPath = true)]
    private string dungeonThemeImagePath;
    #endregion

    #region Data Container Fields
    private static bool isInit = false;
    public DataSheetController DataSheetController { get; private set; }

    private static bool isDataLoaded = false;

    // Player Character Datas
    [TabGroup("PlayerCharacter", "CharacterInfo"), ShowInInspector]
    public CharacterInfoDict InfoDict { get; set; } = null;

    [TabGroup("PlayerCharacter", "InitialStatList"), ShowInInspector]
    public InitialStatDict InitStatDict { get; set; } = null;

    [TabGroup("PlayerCharacter", "CharacterPrefab"), ShowInInspector]
    public CharacterPrefabList[] PrefabList { get; set; }

    [TabGroup("PlayerCharacter", "NameList"), ShowInInspector]
    public CharacterNameList NameList { get; set; } = new CharacterNameList();


    [TabGroup("Enemy", "EnemyInfo"), ShowInInspector]
    public EnemyInfoDict EnemyInfoDict { get; set; } = null;


    [TabGroup("Dungeon", "DungeonList")]
    public DungeonDataList dungeonList;
    [TabGroup("Dungeon", "DungeonList")]
    public DungeonThemeImageDict DungeonThemeList { get; set; }
    #endregion

    #region Fields For Data Management


#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void CallSaveData();

    [DllImport("__Internal")]
    private static extern void SyncFS();
#endif

    #endregion

    #region Unity Lifecycle Functions
    protected override void Awake()
    {
        base.Awake();
        
        //stopwatch.Start();
        InitialzeDataContainer();
    }

    private void Start()
    {
        LoadData();
        //stopwatch.Stop();
        //UnityEngine.Debug.Log("Total Loading Time : " + stopwatch.ElapsedMilliseconds + " ms");

#if UNITY_WEBGL && !UNITY_EDITOR
    CallSaveData();
#endif
    }

    #endregion

    #region Data Container Initialize Function

    private void InitialzeDataContainer()
    {
        if (isDataLoaded == false)
        {
            saveFilePath = Application.persistentDataPath + "/save.json";
            DataSheetController = new DataSheetController();
            //Stopwatch stopwatch2 = Stopwatch.StartNew();
            InitializeCharacterData();
            //stopwatch2.Stop();
            //UnityEngine.Debug.Log("Time - CharacterData : " + stopwatch2.ElapsedMilliseconds + " ms");

            //stopwatch2.Restart();
            InitializeEnemyData();
            //stopwatch2.Stop();
            //UnityEngine.Debug.Log("Time - EnemyData : " + stopwatch2.ElapsedMilliseconds + " ms");

            //stopwatch2.Restart();
            InitializeDungeonData();
            //stopwatch2.Stop();
            //UnityEngine.Debug.Log("Time - DungeonData : " + stopwatch2.ElapsedMilliseconds + " ms");

            isDataLoaded = true;
        }
    }

    private void InitializeCharacterData()
    {
        PrefabList = Resources.LoadAll<CharacterPrefabList>(characterPrefabPath);
        InitStatDict = new InitialStatDict(DataSheetController);
        InfoDict = new CharacterInfoDict(DataSheetController);
    }

    private void InitializeEnemyData()
    {
        EnemyInfoDict = new EnemyInfoDict(DataSheetController);
        EnemyPrefabDict prefabList = new EnemyPrefabDict(Resources.LoadAll<EnemyPrefab>(enemyPrefabPath));
        EnemyInfoDict.AssignPrefabData(prefabList);
    }

    private void InitializeDungeonData()
    {
        // Data Loading
        DungeonInfoDict dungeonInfoDict = new DungeonInfoDict(DataSheetController);
        WaveDataDict waveDataList = new WaveDataDict(DataSheetController);
        List<EnemySpawnInfo> enemySpawnInfos = DataSheetController.Deserialize<EnemySpawnInfo>();
        DungeonThemeList = new DungeonThemeImageDict(Resources.LoadAll<DungeonThemeImage>(dungeonThemeImagePath));

        // Data Mapping
        MappingDatasFromRcode(enemySpawnInfos, waveDataList);
        waveDataList.AssignWaveDataToDungeon(dungeonInfoDict);
        dungeonList = new DungeonDataList(dungeonInfoDict.DataDictToList());
    }

    public void MappingDatasFromRcode(List<EnemySpawnInfo> enemySpawnInfos, WaveDataDict waveDataList)
    {
        foreach (var data in enemySpawnInfos)
        {
            data.EnemyInfo = EnemyInfoDict.GetData(data.EnemyRcode);
            if (data.EnemyInfo == null)
            {
                UnityEngine.Debug.LogError($"No EnemyInfo Where Rcode : {data.EnemyRcode}!!");
                continue;
            }

            WaveData wave = waveDataList.GetData(data.WaveRcode);
            if (wave == null)
            {
                UnityEngine.Debug.LogError($"No WaveData Where Rcode : {data.WaveRcode}!!");
                continue;
            }
            wave.AddEnemySpawnInfo(data);
        }
    }

    public CharacterPrefab GetPrefab(eCharacterClass characterClass, string rcode)
    {
        foreach (CharacterPrefabList prefabList in PrefabList)
        {
            if (prefabList.characterClass == characterClass)
            {
                return prefabList.GetPrefab(rcode);
            }
        }

        UnityEngine.Debug.Log($"Can't load characterskin : {rcode}");
        return null;
    }

    #endregion

    #region UserData Save & Load

    public void SaveData()
    {
        dungeonClearCount = dungeonList.SaveClearData();

        List<CharacterDataSerializable> serializableCharacterList = new List<CharacterDataSerializable>();
        foreach (CharacterData character in characterList)
        {
            serializableCharacterList.Add(character.Serialization());
        }

        SaveData saveData = new SaveData()
        {
            userInfo = this.userInfo,
            dungeonClearCount = dungeonClearCount,
            UpgradeLevel = this.UpgradeLevel,
            characterList = serializableCharacterList,
            entryPreset = this.entryPreset
        };

        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(saveFilePath, json);

#if UNITY_WEBGL && !UNITY_EDITOR
            SyncFS();
#endif
    }

    public void LoadData()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);

            this.userInfo = saveData.userInfo;
            dungeonClearCount = saveData.dungeonClearCount;
            dungeonList.LoadClearData(dungeonClearCount);
            this.UpgradeLevel = saveData.UpgradeLevel;
            this.entryPreset = saveData.entryPreset;

            characterList.Clear();
            foreach (CharacterDataSerializable data in saveData.characterList)
            {
                CharacterData character = new CharacterData(data);
                characterList.Add(character);
            }
        }
    }

    private void ResetData()
    {
        userInfo.Clear();
        dungeonClearCount = 0;
        UpgradeLevel.Clear();
        characterList.Clear();
        //recipeDataList.Clear();
        dungeonList.ResetDungeonClear();

        Destroy(ToturialsManager.Instance.gameObject);

        EmploymentManager.Instance.ReRoll();
    }

    public void DeleteSaveData()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
#if UNITY_WEBGL && !UNITY_EDITOR
            SyncFS();
#endif
        }

        ResetData();
        GameManager.Instance.IntroState();
    }

    #endregion

    #region On Application Quit

    private void OnApplicationQuit()
    {
        SaveData();
#if UNITY_WEBGL && !UNITY_EDITOR
            CallSaveData();
#endif
    }
    private void OnDisable()
    {
#if UNITY_EDITOR
        if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode && !UnityEditor.EditorApplication.isPlaying)
        {
            SaveData();
        }
#endif
    }

    #endregion
}
