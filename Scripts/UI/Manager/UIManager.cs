using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum UIType
{
    IntroUI,
    RestaurantUI,
    StrategyUI,
    CombatUI,
    CombatResultUI,
    DungeonInfoUI,
    CurrencyUI_Gold,
    CurrencyUI_Medal,
    Popup_SettingMenu
}


public class UIManager : SerializedMonoBehaviour
{
    public static UIManager Instance;

    public Camera MainCamera { get; private set; }

    [Title("Scene UI Prefab")]
    [SerializeField] private Dictionary<GameState, GameObject> uiPrefabDict = new Dictionary<GameState, GameObject>();

    [Title("Popup UI Prefab")]
    [SerializeField] private GameObject goldPopupUIPrefab;
    [SerializeField] private GameObject medalPopupUIPrefab;
    [SerializeField] private GameObject settingMenuUIPrefab;

    [Title("UI Dictionary")]
    [SerializeField, ReadOnly] private Dictionary<UIType, UIBase> uiDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            uiDictionary = new Dictionary<UIType, UIBase>();
            Instantiate(uiPrefabDict[GameState.Intro]);
        }
        else
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
        }
        MainCamera = Camera.main;
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        GameState state = GameManager.Instance.currentGameState;
        MainCamera = Camera.main;
        uiDictionary.Clear();
        Instantiate(uiPrefabDict[state]);
    }


    public bool SetActiveUI(UIType uiType, bool isActive)
    {
        if (uiDictionary.ContainsKey(uiType) == false)
        {
            Debug.Log("Dictionary null");
            return false;
        }
        uiDictionary[uiType].SetActive(isActive);
        return true;
    }


    public void AddUIDictionary(UIType type, UIBase ui)
    {
        uiDictionary.Add(type, ui);
    }

    public void ActivePopupUI(UIType uiType)
    {
        switch (uiType)
        {
            case UIType.Popup_SettingMenu:
                if (uiDictionary.ContainsKey(uiType) == false)
                    Instantiate(settingMenuUIPrefab);
                break;
            default:
                return;
        }
        SetActiveUI(uiType, true);
    }

    public void CurrencyPopupUI(CurrencyType currencyType)
    {
        UIType uiType;
        switch (currencyType)
        {
            case CurrencyType.Gold:
                uiType = UIType.CurrencyUI_Gold;
                if (uiDictionary.ContainsKey(uiType) == false)
                    Instantiate(goldPopupUIPrefab);
                break;
            case CurrencyType.Medal:
                uiType = UIType.CurrencyUI_Medal;
                if (uiDictionary.ContainsKey(uiType) == false)
                    Instantiate(medalPopupUIPrefab);
                break;
            default:
                return;
        }
        SetActiveUI(uiType, true);
    }
}

