using Sirenix.OdinInspector;
using System;
using UnityEngine;

[Serializable]
public class UserInfo
{
    public static UserInfo userInfo { get => DataManager.Instance.userInfo; }

    public UserInfo()
    {
        InitialzeUserInfo();
    }

    #region Fields
    [Title("InitData")]
    [SerializeField] private int initGold = 1000;
    [SerializeField] private int initMaxCharaListCount = 10;

    [Title("User Data")]
    [SerializeField] private int userLevel;
    public int UserLevel
    {
        get { return userLevel; }
        set
        {
            userLevel = value;
            setUserLevel?.Invoke(userLevel);
        }
    }

    [SerializeField] private int exp;
    public int Exp
    {
        get { return exp; }
        set
        {
            exp = value;
            setExp?.Invoke(exp);
        }
    }

    public int MaxExp = 100;

    [SerializeField] private int gold;
    public int Gold
    {
        get { return gold; }
        set
        {
            gold = value;
            setGold?.Invoke(gold);
        }
    }

    [SerializeField] private int medal;
    public int Medal
    {
        get => medal;
        set
        {
            medal = value;
            setMedal?.Invoke(medal);
        }
    }

    [SerializeField] private int recipePoint;
    public int RecipePoint
    {
        get => recipePoint;
        set
        {
            recipePoint = value;
            setRecipePoint?.Invoke(recipePoint);
        }
    }

    [SerializeField] private int maxPartyNumber = 1;
    public int MaxPartyNumber
    {
        get => maxPartyNumber;
        set
        {
            if (value > GameManager.MAX_PARTY_NUMBER) return;
            maxPartyNumber = value;
            setMaxPartyNumber?.Invoke(value);
        }
    }

    public int maxCharacterListCount = 10;

    public bool isUserTutorials;

    private int[] medalsRequiredForLevelUp = { 0, 1, 3, 3, 3, 3, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 7, 7, 7, 7, 7, 10, 10, 10, 10, 10, 20, 20, 20, 20, 20 };

    #endregion

    #region Delegate & Event
    public delegate void SetGold(int value);
    private event SetGold setGold;

    public delegate void SetUserLevel(int value);
    private event SetUserLevel setUserLevel;

    public delegate void SetExp(int value);
    private event SetExp setExp;

    public delegate void SetMedal(int value);
    private event SetMedal setMedal;

    public delegate void SetRecipePoint(int value);
    private event SetRecipePoint setRecipePoint;

    public delegate void SetMaxPartyNumber(int value);
    private event SetMaxPartyNumber setMaxPartyNumber;
    #endregion

    #region SetEventFunctions
    public void SetGoldEvent(SetGold action)
    {
        setGold += action;
        setGold.Invoke(gold);
    }

    public void RemoveGoldEvent(SetGold action)
    {
        setGold -= action;
    }

    public void SetUserLevelEvent(SetUserLevel action)
    {
        setUserLevel += action;
        setUserLevel.Invoke(userLevel);
    }

    public void RemoveUserLevelEvent(SetUserLevel action)
    {
        setUserLevel -= action;
    }

    public void SetExpEvent(SetExp action)
    {
        setExp += action;
        setExp.Invoke(exp);
    }

    public void RemoveExpEvent(SetExp action)
    {
        setExp -= action;
    }

    public void SetMedalEvent(SetMedal action)
    {
        setMedal += action;
        setMedal.Invoke(medal);
    }

    public void RemoveMedalEvent(SetMedal action)
    {
        setMedal -= action;
    }

    public void SetRecipePointEvent(SetRecipePoint action)
    {
        Debug.Log("SetRecipePointEvent()");
        setRecipePoint += action;
        setRecipePoint.Invoke(recipePoint);
    }

    public void RemoveRecipePointEvent(SetRecipePoint action)
    {
        setRecipePoint -= action;
    }

    public void SetMaxPartyNumberEvent(SetMaxPartyNumber action)
    {
        setMaxPartyNumber += action;
        setMaxPartyNumber.Invoke(maxPartyNumber);
    }

    public void RemoveMaxPartyNumberEvent(SetMaxPartyNumber action)
    {
        setMaxPartyNumber -= action;
    }
    #endregion

    #region Functions
    public void InitialzeUserInfo()
    {
        InitializeData();
        setUserLevel?.Invoke(userLevel);
        setExp += CheckLevelUp;
        setExp?.Invoke(exp);
    }

    private void InitializeData()
    {
        userLevel = 1;
        exp = 0;
        gold = initGold;
        medal = 0;
        RecipePoint = 0;
        maxPartyNumber = 1;
        maxCharacterListCount = initMaxCharaListCount;
        isUserTutorials = false;
    }



    private void CheckLevelUp(int value)
    {
        while (exp >= MaxExp)
        {
            exp -= MaxExp;
            UserLevel++;
        }
    }

    public bool UseMedal(int value)
    {
        if (Medal - value < 0)
        {
            return false;
        }
        Medal -= value;
        return true;
    }

    public void ExpUpUsingMedal(int useMedal)
    {
        int currentLevel = userLevel;

        int maxLevel = medalsRequiredForLevelUp.Length - 1;
        if (userLevel >= maxLevel)
        {
            return;
        }

        int medalsRequired = medalsRequiredForLevelUp.Length > currentLevel ? medalsRequiredForLevelUp[currentLevel] : 0;

        MaxExp = medalsRequired;

        exp += useMedal;

        while (exp >= MaxExp)
        {
            exp -= MaxExp;
            userLevel++;
            if (userLevel < medalsRequiredForLevelUp.Length)
            {
                MaxExp = medalsRequiredForLevelUp[userLevel];
            }
            else
            {
                MaxExp = int.MaxValue;
            }

            if (userLevel >= maxLevel)
            {
                UserLevel = maxLevel;
            }
        }

        setUserLevel?.Invoke(userLevel);
        setExp?.Invoke(exp);
    }

    public void Clear()
    {
        InitializeData();
    }

    #endregion
}
