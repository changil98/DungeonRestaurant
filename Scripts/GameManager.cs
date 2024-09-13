using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum GameState
{
    Intro,
    Restaurant,
    DungeonEnter,
    DungeonCombat
}

public class GameManager : MonoBehaviour
{
    private static GameManager _Instance;
    public static GameManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = new GameObject("GameManager").AddComponent<GameManager>();
            }
            return _Instance;
        }
    }
    public CombatController combatController;

    public GameState currentGameState { get; private set; }

    public Dictionary<GameState, int> gameScene { get; private set; }

    public const int MAX_POSITION = 9;
    public const int MAX_PARTY_NUMBER = 6;
    public const int ENEMY_MAX_POSITION = 3;

    public bool isCombatStart { get; private set; } = false;

    private void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        gameScene = new Dictionary<GameState, int>()
        {
            {GameState.Intro, 0 },
            {GameState.Restaurant, 1 },
            {GameState.DungeonEnter, 2 },
            {GameState.DungeonCombat, 3 }
        };
    }

    public void IntroState()
    {
        LoadScene(GameState.Intro);
    }

    public void RestaurantState()
    {
        SoundManager.Instance.LoadSoundBank("RestaurantSoundBank");
        if (!SoundManager.Instance.IsBGMPlaying("BGM_Restaurant1"))
        {
            SoundManager.Instance.PlaySound("BGM_Restaurant1", 1f,true);
        }
        LoadScene(GameState.Restaurant);
    }

    public void DungeonEnterState()
    {
        SoundManager.Instance.LoadSoundBank("CombatSoundBank");
        if (!SoundManager.Instance.IsBGMPlaying("BGM_Restaurant1"))
        {
            SoundManager.Instance.PlaySound("BGM_Restaurant1", 1f,true);
        }      
        LoadScene(GameState.DungeonEnter);
    }

    public void CombatState()
    { 
        LoadScene(GameState.DungeonCombat);
    }

    public void LoadScene(GameState state)
    {
        currentGameState = state;
        SceneManager.LoadScene(gameScene[state]);
    }

    public void SetCombatFlag(bool flag)
    {
        isCombatStart = flag;
    }

    public void CombatStart()
    {   
        UIManager.Instance.SetActiveUI(UIType.StrategyUI, false);
        UIManager.Instance.SetActiveUI(UIType.CombatUI, true);
        CombatSceneBGM();
        combatController.SpawnPlayerCharacter();
        combatController.SpawnEnemiesInWave(0);
    }

    private void CombatSceneBGM()
    {
        switch (DataManager.Instance.dungeonList.CurrentDungeon.Theme)
        {
            case DungeonTheme.DungeonEntrance:
                SoundManager.Instance.PlaySound("BGM_Combat1", 0.5f, true);
                break;
            case DungeonTheme.CollapsedCastle:
                SoundManager.Instance.PlaySound("BGM_Combat2", 0.5f, true);
                break;
            case DungeonTheme.ChilledPlain:
                SoundManager.Instance.PlaySound("BGM_Combat3", 0.5f, true);
                break;

        }
    }

    public void CombatOver()
    {
        isCombatStart = false;
        if (combatController.isPlayerWin)
        {
            DataManager.Instance.dungeonList.OnClearDungeon();
        }
        StartCoroutine(SetCombatOverUIAndBGM());
    }

    private IEnumerator SetCombatOverUIAndBGM()
    {
        yield return new WaitForSeconds(1f);
        UIManager.Instance.SetActiveUI(UIType.CombatResultUI, true);
        if (combatController.isPlayerWin)
        {
            SoundManager.Instance.PlaySound("BGM_Victory");
        }
        else
        {
            SoundManager.Instance.PlaySound("BGM_Defeat");
        }
    }

}
