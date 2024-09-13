using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;
using UnityEditor;

public class DebugEditor : OdinEditorWindow
{
    [MenuItem("Debug Tools/User Data")]
    private static void OpenWindow()
    {
        GetWindow<DebugEditor>().Show();
    }

    #region Gold
    [BoxGroup("Gold")]
    [ReadOnly]
    [SerializeField]
    private int currentGold;

    [OnInspectorGUI]
    [DisableInEditorMode]
    private void LoadCurrentGold()
    {
        currentGold = UserInfo.userInfo.Gold;
    }


    [BoxGroup("Gold")]
    [HorizontalGroup("Gold/Row1")]
    [PropertyOrder(1)]
    [DisableInEditorMode]
    [Button("-", ButtonSizes.Small)]
    public void DecreaseGold()
    {
        UserInfo.userInfo.Gold -= gold;
    }


    [BoxGroup("Gold")]
    [HorizontalGroup("Gold/Row1")]
    [PropertyOrder(2)]
    [DisableInEditorMode]
    [HideLabel]
    public int gold = 10000;


    [BoxGroup("Gold")]
    [HorizontalGroup("Gold/Row1")]
    [PropertyOrder(3)]
    [DisableInEditorMode]
    [Button("+", ButtonSizes.Small)]
    public void IncreaseGold()
    {
        UserInfo.userInfo.Gold += gold;
    }

    #endregion

    #region Exp

    [BoxGroup("Exp")]
    [ReadOnly]
    [SerializeField]
    private int currentExp;

    [OnInspectorGUI]
    [DisableInEditorMode]
    private void LoadCurrentExp()
    {
        currentExp = UserInfo.userInfo.Exp;
    }


    [BoxGroup("Exp")]
    [HorizontalGroup("Exp/Row1")]
    [PropertyOrder(1)]
    [DisableInEditorMode]
    [Button("-", ButtonSizes.Small)]
    public void DecreaseExp()
    {
        UserInfo.userInfo.Exp -= exp;
    }


    [BoxGroup("Exp")]
    [HorizontalGroup("Exp/Row1")]
    [PropertyOrder(2)]
    [DisableInEditorMode]
    [HideLabel]
    public int exp = 10;


    [BoxGroup("Exp")]
    [HorizontalGroup("Exp/Row1")]
    [PropertyOrder(3)]
    [DisableInEditorMode]
    [Button("+", ButtonSizes.Small)]
    public void IncreaseExp()
    {
        UserInfo.userInfo.Exp += exp;
    }

    #endregion

    #region User Level

    [BoxGroup("User Level")]
    [ReadOnly]
    [SerializeField]
    private int currentUserLevel;

    [OnInspectorGUI]
    private void LoadCurrentUserLevel()
    {
        currentUserLevel = UserInfo.userInfo.UserLevel;
    }


    [BoxGroup("User Level")]
    [HorizontalGroup("User Level/Row1")]
    [PropertyOrder(1)]
    [DisableInEditorMode]
    [Button("-", ButtonSizes.Small)]
    public void DecreaseUserLevel()
    {
        UserInfo.userInfo.UserLevel -= level;
    }


    [BoxGroup("User Level")]
    [HorizontalGroup("User Level/Row1")]
    [PropertyOrder(2)]
    [DisableInEditorMode]
    [HideLabel]
    public int level = 1;


    [BoxGroup("User Level")]
    [HorizontalGroup("User Level/Row1")]
    [PropertyOrder(3)]
    [DisableInEditorMode]
    [Button("+", ButtonSizes.Small)]
    public void IncreaseUserLevel()
    {
        UserInfo.userInfo.UserLevel += level;
    }

    #endregion

    #region Medal

    [BoxGroup("Medal")]
    [ReadOnly]
    [SerializeField]
    private int currentMedal;

    [OnInspectorGUI]
    [DisableInEditorMode]
    private void LoadcurrentMedal()
    {
        currentMedal = UserInfo.userInfo.Medal;
    }


    [BoxGroup("Medal")]
    [HorizontalGroup("Medal/Row1")]
    [PropertyOrder(1)]
    [DisableInEditorMode]
    [Button("-", ButtonSizes.Small)]
    public void DecreaseMedal()
    {
        UserInfo.userInfo.Medal -= medal;
    }


    [BoxGroup("Medal")]
    [HorizontalGroup("Medal/Row1")]
    [PropertyOrder(2)]
    [DisableInEditorMode]
    [HideLabel]
    public int medal = 1;


    [BoxGroup("Medal")]
    [HorizontalGroup("Medal/Row1")]
    [PropertyOrder(3)]
    [DisableInEditorMode]
    [Button("+", ButtonSizes.Small)]
    public void IncreaseMedal()
    {
        UserInfo.userInfo.Medal += medal;
    }
    #endregion

    #region RecipePoint

    [BoxGroup("RecipePoint")]
    [ReadOnly]
    [SerializeField]
    private int currentRecipePoint;

    [OnInspectorGUI]
    [DisableInEditorMode]
    private void LoadcurrentRecipePoint()
    {
        currentRecipePoint = UserInfo.userInfo.RecipePoint;
    }


    [BoxGroup("RecipePoint")]
    [HorizontalGroup("RecipePoint/Row1")]
    [PropertyOrder(1)]
    [DisableInEditorMode]
    [Button("-", ButtonSizes.Small)]
    public void DecreaseRecipePoint()
    {
        UserInfo.userInfo.RecipePoint -= recipePoint;
    }


    [BoxGroup("RecipePoint")]
    [HorizontalGroup("RecipePoint/Row1")]
    [PropertyOrder(2)]
    [DisableInEditorMode]
    [HideLabel]
    public int recipePoint = 1;


    [BoxGroup("RecipePoint")]
    [HorizontalGroup("RecipePoint/Row1")]
    [PropertyOrder(3)]
    [DisableInEditorMode]
    [Button("+", ButtonSizes.Small)]
    public void IncreaseRecipePoint()
    {
        UserInfo.userInfo.RecipePoint += recipePoint;
    }
    #endregion

    #region Dungeon
    [BoxGroup("Dungeon")]
    [DisableInEditorMode]
    [Button("Next Dungeon")]
    public void NextDungeon()
    {
        DataManager.Instance.dungeonList.OnClearDungeon();
    }

    [BoxGroup("Dungeon")]
    [DisableInEditorMode]
    [Button("Before Dungeon")]
    public void BeforeDungeon()
    {
        DataManager.Instance.dungeonList.BeforeDungeon();
    }

    //[BoxGroup("Dungeon")]
    //[ReadOnly]
    //[ShowInInspector]
    //private DungeonInfo currentDungeon;

    //[OnInspectorGUI]
    //[DisableInEditorMode]
    //private void LoadCurrentDungeon()
    //{
    //    currentDungeon = DataManager.Instance.dungeonList.CurrentDungeon;
    //}
    #endregion

}
