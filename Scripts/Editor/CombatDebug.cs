using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class CombatDebug : OdinEditorWindow
{
    [MenuItem("Debug Tools/Combat")]
    private static void OpenWindow()
    {
        GetWindow<CombatDebug>().Show();
    }

    #region Combat

    [BoxGroup("Combat")]
    [DisableInEditorMode]
    bool isCombatControllerNotExist = true;

    [OnInspectorGUI]
    [DisableInEditorMode]
    private void checkCombatControllerExist()
    {
        if (CombatController.instance != null)
        {
            isCombatControllerNotExist = false;
        }
        else
        {
            isCombatControllerNotExist = true;
        }
    }

    [BoxGroup("Combat")]
    [DisableInEditorMode]
    [DisableIf("isCombatControllerNotExist")]
    [Button("전투 승리")]
    public void Victory()
    {
        CombatController.instance.Debug_BattleWin();
    }

    [BoxGroup("Combat")]
    [DisableInEditorMode]
    [DisableIf("isCombatControllerNotExist")]
    [Button("전투 패배")]
    public void Defeat()
    {
        CombatController.instance.Debug_BattleDefeat();
    }

    [BoxGroup("Combat")]
    [DisableInEditorMode]
    [DisableIf("isCombatControllerNotExist")]
    [Button("다음 웨이브")]
    public void NextWave()
    {
        CombatController.instance.Debug_NextWave();
    }

    [BoxGroup("Combat")]
    [DisableInEditorMode]
    [DisableIf("isCombatControllerNotExist")]
    [Button("적 죽이기")]
    public void KillEnemies()
    {
        CombatController.instance.Debug_KillAllEnemies();
    }

    [BoxGroup("Combat")]
    [DisableInEditorMode]
    [DisableIf("isCombatControllerNotExist")]
    [Button("플레이어 부활")]
    public void RessurectPlayerCharacters()
    {
        CombatController.instance.Debug_ResurrectPlayerCharacters();
    }

    [BoxGroup("Combat")]
    [HorizontalGroup("Combat/Row1")]
    [PropertyOrder(1)]
    [DisableIf("isCombatControllerNotExist")]
    [DisableInEditorMode]
    public int wave;


    [BoxGroup("Combat")]
    [HorizontalGroup("Combat/Row1")]
    [PropertyOrder(2)]
    [DisableIf("isCombatControllerNotExist")]
    [DisableInEditorMode]
    [Button("적 생성(wave)")]
    public void SpawnWaveEnemies()
    {
        try
        {
            CombatController.instance.Debug_SpawnEnemiesInWave(wave);
        }
        catch
        {
            int maxwave = DataManager.Instance.dungeonList.CurrentDungeon.WaveList.Count;
            if (wave < 0)
                wave = 0;
            else if (wave >= maxwave)
                wave = maxwave - 1;
            CombatController.instance.Debug_SpawnEnemiesInWave(wave);
        }

    }
    #endregion

    #region Enemy Spawn
    private EnemyInfo info;
    private List<EnemyInfo> enemyInfolist;
    private int index = 0;
    public int Index
    {
        get => index;
        set
        {
            index = value;
            OnIndexChange();
        }
    }
    [DisableInEditorMode]
    [DisableIf("isCombatControllerNotExist")]
    [OnInspectorGUI]
    public void InitEnemyList()
    {
        if (Application.isPlaying && enemyInfolist == null)
        {
            enemyInfolist = DataManager.Instance.EnemyInfoDict.DataDictToList();
            info = enemyInfolist[index];
            isListInit = true;
        }
    }
    private bool isListInit = false;


    [BoxGroup("Enemy Spawn")]
    [HorizontalGroup("Enemy Spawn/Split", 55), PropertyOrder(-1)]
    [DisableInEditorMode]
    [ReadOnly]
    [ShowInInspector]
    [PreviewField(50, Sirenix.OdinInspector.ObjectFieldAlignment.Left), HideLabel]
    [DisableIf("isCombatControllerNotExist")]
    private Sprite enemySprite;

    [BoxGroup("Enemy Spawn")]
    [FoldoutGroup("Enemy Spawn/Split/Info"), PropertyOrder(-1)]
    [DisableInEditorMode]
    [ReadOnly]
    [ShowInInspector]
    [DisableIf("isCombatControllerNotExist")]
    private string enemyRcode, enemyName;

    private void OnIndexChange()
    {
        info = enemyInfolist[index];
        enemySprite = info.Prefab.thumnailSprite;
        enemyRcode = info.Rcode;
        enemyName = info.EnemyName;
    }


    [BoxGroup("Enemy Spawn")]
    [HorizontalGroup("Enemy Spawn/Row1")]
    [PropertyOrder(1)]
    [DisableIf("isCombatControllerNotExist")]
    [DisableInEditorMode]
    [Button("Before")]
    public void BeforeEnemy()
    {
        if (index > 0)
            Index--;
    }


    [BoxGroup("Enemy Spawn")]
    [HorizontalGroup("Enemy Spawn/Row1")]
    [PropertyOrder(1)]
    [DisableIf("isCombatControllerNotExist")]
    [DisableInEditorMode]
    [Button("Next")]
    public void NextEnemy()
    {
        if (index < enemyInfolist.Count - 1)
            Index++;
    }

    [BoxGroup("Enemy Spawn")]
    [PropertyOrder(2)]
    [DisableIf("isCombatControllerNotExist")]
    [DisableInEditorMode]
    [EnumPaging]
    public SpawnArea spawnArea;

    [BoxGroup("Enemy Spawn")]
    [PropertyOrder(3)]
    [DisableIf("isCombatControllerNotExist")]
    [DisableInEditorMode]
    [Button("몬스터 생성")]
    public void SpawnEnemy()
    {
        CombatController.instance.Debug_SpawnEnemy(info, spawnArea);
    }

    #endregion

    //[BoxGroup("Card")]
    //[ShowInInspector, ReadOnly] private CardDeck cardDeck = CardDeck.Instance;

    //[DisableInEditorMode]
    //bool isCardDeckNotExist = true;

    //[OnInspectorGUI]
    //[DisableInEditorMode]
    //private void check()
    //{
    //    if (CardDeck.Instance != null)
    //    {
    //        isCardDeckNotExist = false;
    //    }
    //    else
    //    {
    //        isCardDeckNotExist = true;
    //    }
    //}

    //[BoxGroup("Card")]
    //[DisableInEditorMode]
    //[ReadOnly]
    //[DisableIf("isCardDeckNotExist")]
    //public RecipeData currentRecipe;

    //[BoxGroup("Card")]
    //[HorizontalGroup("Card/Row1")]
    //[PropertyOrder(1)]
    //[DisableInEditorMode]
    //[Button("-", ButtonSizes.Small)]
    //public void BeforeRecipe()
    //{
    //    currentRecipe = RecipeDataManager.Instance.recipes[--index];
    //}

    //[BoxGroup("Card")]
    //[HorizontalGroup("Card/Row1")]
    //[PropertyOrder(2)]
    //[DisableInEditorMode]
    //[HideLabel]
    //public int index;

    //[BoxGroup("Card")]
    //[HorizontalGroup("Card/Row1")]
    //[PropertyOrder(3)]
    //[DisableInEditorMode]
    //[Button("+", ButtonSizes.Small)]
    //public void NextRecipe()
    //{
    //    currentRecipe = RecipeDataManager.Instance.recipes[++index];
    //}

    //[BoxGroup("Card")]
    //[DisableInEditorMode]
    //[DisableIf("isCardDeckNotExist")]
    //[Button("Draw")]
    //public void Draw()
    //{
    //    CardDeck.Instance.Darw(currentRecipe);
    //}

}
