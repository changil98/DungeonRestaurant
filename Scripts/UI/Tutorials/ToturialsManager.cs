using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class ToturialsManager : Singleton<ToturialsManager>
{
    [Title("TutorialsCheck")]
    public GameObject toturialsCheck;

    [Title("Tutorials Object")]
    public GameObject objectPosition;
    public GameObject ToturialsUI;
    public GameObject frame;
    public TextMeshProUGUI tutorialsDescriptionTxt;
    public GameObject arrow;
    public string[] desTxt = new string[37];
    public GameObject[] setPosition;
    public List<Button> allButtons;

    [Title("Tutorials Bool")]
    public bool[] isClear;
    public bool isTutorials = false;

    [Title("Tutorials Phase")]
    public int phase = 0;
    private float lastClickTime;
    private const float clickDelay = 0.7f;
    private readonly Dictionary<int, int> phasePositions = new Dictionary<int, int>
    {
        {3, 0}, {5, 1}, {6, 2}, {7, 3}, {9, 4}, {10, 5},
        {13, 6},  {18, 10}, {19, 11},
        {20, 12}, {21, 13}, {23, 14}, {24, 15}, {25, 16},
        {28, 19}, {30, 20}, {31, 21}, {33, 22}, {34, 22}
    };

    private readonly Dictionary<int, List<string>> phaseButtonNames = new Dictionary<int, List<string>>
    {
        { 3, new List<string> { "EmployBtn" } },
        { 5, new List<string> { "Employment Button_1" } },
        { 6, new List<string> { "BackBtn"} },
        { 7, new List<string> { "EnterDungeonButton" } },
        { 10, new List<string>{ "EnterDungeonButton" } },
        { 13, new List<string>{ "StartCombatButton" } },
        { 21, new List<string>{ "Button_Home" } },
        { 23, new List<string>{ "RecipeBtn" , "BackBtn" } },
        { 25, new List<string>{ "RestarantUpgradeBtn", "BackBtn" } },
        { 26, new List<string>{ "BackBtn" } },
        { 27, new List<string>{ "BackBtn" } },
        { 29, new List<string>{ "UpgradeBtn", "BackBtn" } },
        { 30, new List<string>{ "ManagementBtn" , "BackBtn"}},
        { 35, new List<string>{ "LevelUpBtn" } }
    };

    protected override void Awake()
    {
        base.Awake();
        InitializeDescriptions();
        InitializeisClear();
        arrow.SetActive(false);
        SceneManager.sceneLoaded += OnSceneLoaded;
        if (DataManager.Instance?.userInfo?.isUserTutorials ?? true)
        {
            Destroy(gameObject);
        }
        
        if (isTutorials)
        {
            Destroy(toturialsCheck);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ���� �ε�� �� ��ư�� �ҷ����� �Լ� ȣ��
        if (!DataManager.Instance.userInfo.isUserTutorials && isTutorials)
        {
            FindAllButtonsInScene();
            ActivateButtonsForCurrentPhase();
        }
    }

    private void FindAllButtonsInScene()
    {
        // ���� ����Ʈ �ʱ�ȭ
        allButtons.Clear();

        // ���� ���� �ִ� ��� Button ������Ʈ�� ã���ϴ�.
        Button[] buttonsInScene = GameObject.FindObjectsOfType<Button>();

        // ����Ʈ�� �߰�
        foreach (var button in buttonsInScene)
        {
            allButtons.Add(button);
        }

        Debug.Log($"�� '{SceneManager.GetActiveScene().name}'���� {allButtons.Count}���� ��ư�� ã�ҽ��ϴ�.");
    }

    IEnumerator SetPosition()
    {
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < setPosition.Length; i++)
        {
            if(i == 2 || i == 10)
            {
                setPosition[i].transform.position = new Vector3(setPosition[i].transform.position.x, setPosition[i].transform.position.y + 31f, setPosition[i].transform.position.z);
            }
            else if (i == 1 || i == 11 || i==12 || i ==13)
            {

            }
            else
            {
                setPosition[i].transform.position = new Vector3(setPosition[i].transform.position.x, setPosition[i].transform.position.y - 31f, setPosition[i].transform.position.z);
            }
           
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= lastClickTime + clickDelay && isTutorials)
        {
            OnNextPhase();
            lastClickTime = Time.time;
        }
    }

    private void UpdateDescription()
    {
        if (phase < desTxt.Length)
        {
            tutorialsDescriptionTxt.text = desTxt[phase];
            if (phasePositions.TryGetValue(phase, out int posIndex))
            {
                arrow.SetActive(true);
                SetPosition(posIndex);
            }
            else
            {
                arrow.SetActive(false);
            }
            FindAllButtonsInScene();
            ActivateButtonsForCurrentPhase();
        }
    }

    private void ActivateButtonsForCurrentPhase()
    {
        // ��� ��ư ��Ȱ��ȭ
        foreach (var button in allButtons)
        {
            button.interactable = false;
        }

        // ���� Phase�� �ش��ϴ� ��ư�鸸 Ȱ��ȭ
        if (phaseButtonNames.TryGetValue(phase, out List<string> buttonNames))
        {
            foreach (string name in buttonNames)
            {
                var buttonToActivate = allButtons.FirstOrDefault(b => b.name == name);
                if (buttonToActivate != null)
                {
                    buttonToActivate.interactable = true;
                }
            }
        }
    }

    public void OnNextPhase()
    {
        if (phase >= desTxt.Length)
        {
            DataManager.Instance.userInfo.isUserTutorials = true;
            Destroy(gameObject);
            foreach (var button in allButtons)
            {
                button.interactable = true;
            }
            return;
        }

        if (!IsPhaseConditionMet(phase))
        {
            return;
        }
        if (phase == 18)
        {
            this.gameObject.SetActive(false);
        }

        phase++;
        UpdateDescription();
    }

    private bool IsPhaseConditionMet(int currentPhase)
    {

        switch (currentPhase)
        {
            case 3:
                return CheckConditionForPhase3();
            case 5:
                return CheckConditionForPhase5();
            case 6: 
                return CheckConditionForPhase6();
            case 7:
                return CheckConditionForPhase7();
            case 10:
                return CheckConditionForPhase10();
            case 13:
                return CheckConditionForPhase13();
            case 21:
                return CheckConditionForPhase21();
            case 23:
                return CheckConditionForPhase23();
            case 24:
                return CheckConditionForPhase24();
            case 25:
                return CheckConditionForPhase25();
            case 28:
                return CheckConditionForPhase28();
            case 29:
                return CheckConditionForPhase29();
            case 30:
                return CheckConditionForPhase30();
            case 31:
                return CheckConditionForPhase31();
            case 35:
                return CheckConditionForPhase35();
            default:
                return true;
        }
    }

    private void SetPosition(int i)
    {
        arrow.transform.position = setPosition[i].transform.position;
    }


    private bool CheckConditionForPhase3()
    {
        if (isClear[0])
        {
            return true;
        }
        return false;
    }

    private bool CheckConditionForPhase5()
    {
        if (DataManager.Instance.characterList.Count > 0)
        {
            return true;
        }
        return false;
    }

    private bool CheckConditionForPhase6()
    {
        if (isClear[1])
        {
            return true;
        }
        return false;
    }
    private bool CheckConditionForPhase7()
    {
        if (isClear[2])
        {
            return true;
        }
        return false;
    }

    private bool CheckConditionForPhase10()
    {
        if (isClear[3])
        {
            return true;
        }
        return false;
    }
    private bool CheckConditionForPhase13()
    {
        if (isClear[5])
        {
            return true;
        }
        return false;
    }

    private bool CheckConditionForPhase21()
    {
        if (isClear[6])
        {
            return true;
        }
        return false;
    }

    private bool CheckConditionForPhase23()
    {
        if (isClear[7])
        {
            return true;
        }
        return false;
    }

    private bool CheckConditionForPhase24()
    {
        if (isClear[8])
        {
            return true;
        }
        return false;
    }

    private bool CheckConditionForPhase25()
    {
        if (isClear[9])
        {
            return true;
        }
        return false;
    }
    private bool CheckConditionForPhase28()
    {
        if (isClear[10])
        {
            return true;
        }
        return false;
    }

    private bool CheckConditionForPhase29()
    {
        if (DataManager.Instance.UpgradeLevel.entryLevel >= 1)
        {
            return true;
        }
        return false;
    }

    private bool CheckConditionForPhase30()
    {
        if (isClear[13])
        {
            return true;
        }
        return false;
    }

    private bool CheckConditionForPhase31()
    {
        if (isClear[14])
        {
            return true;
        }
        return false;
    }

    private bool CheckConditionForPhase35()
    {
        if (isClear[15])
        {
            return true;
        }
        return false;
    }

    public void ToturialsStart()
    {
        if (DataManager.Instance?.userInfo?.isUserTutorials ?? true)
        {
            Destroy(gameObject);
        }
        else
        {
            objectPosition.SetActive(true);
            ToturialsUI.SetActive(true);
            isTutorials = true;
            ActivateButtonsForCurrentPhase();
        }
    }

    private void InitializeDescriptions()
    {
        desTxt[0] = "�ȳ��ϼ���. �̰��� �����Ĵ��Դϴ�. \n����� �����Ĵ��� �ֹ����� �Ǿ� \n������ Ž���ϴ� ���谡�鿡�� �Ļ縦 �����ϴ� ���� �ϰ� �ֽ��ϴ�.";
        desTxt[1] = "�Ĵ��� �湮�� ���谡���� ����� ��Ƽ�� �����\n �׵��� ���� ������ ��� Ŭ�����ϴ� ���� ����� ��ǥ�Դϴ�.";
        desTxt[2] = "�׷� ���谡�� ����� ���ڽ��ϴ�.";
        desTxt[3] = "���谡 ��� ��ư�� ���� ���谡 ��� �޴��� �� �� �ֽ��ϴ�."; //
        desTxt[4] = "�̰����� ���谡���� ����� �� �ֽ��ϴ�. ���谡 �̹����� ������ ���谡�� �� ������ �� ���� �ֽ��ϴ�.";
        desTxt[5] = "���谡�� �� �� ����� ������.";//
        desTxt[6] = "���谡 ����� �Ϸ��߽��ϴ�. �ڷΰ��� ��ư�� ���� �Ĵ����� ��� �� �ֽ��ϴ�.";//
        desTxt[7] = "����� ���谡�� �̿��� ������ ������ ���ô�. �� ��ư�� ���� �������� �� �� �ֽ��ϴ�.";//
        desTxt[8] = "������ �����ϱ� �� ���� ������ Ȯ���� �� �ִ� ȭ���Դϴ�.";
        desTxt[9] = "�̰��� ���콺�� ������ �� �����ϴ� ���� ������ Ȯ���� �� �ֽ��ϴ�.";//
        desTxt[10] = "���� ��ư�� ���� ��� �����մϴ�.";//
        desTxt[11] = "��Ƽ ���� ȭ���Դϴ�. �� ȭ�鿡�� ����� ���谡��� ������ ������ ��Ƽ�� ������ �� �ֽ��ϴ�.";
        desTxt[12] = "������ ��Ͽ��� ���谡�� �巡���ϰų� Ŭ���� �� ������ ��Ƽ �� ������ ���谡�� ��ġ�غ�����.";
        desTxt[13] = "��ġ�� �Ϸ�Ǿ����ϴ�. ��Ƽ�� �����Ͽ����� ��¥ ������ ������ �����Դϴ�.";//
        desTxt[14] = "���� ���� ���͸� ������ �丮 �������� ���� �� �ֽ��ϴ�.";//
        desTxt[15] = "�丮 �������� ������ �Ĵ翡�� �丮���� �����Ǹ� ��ȭ�Ҽ��ֽ��ϴ�.";//
        desTxt[16] = "������ ĳ���͵��� ��Ÿ�� ���������� ������ �������� ��ų�� ����Ҽ��ֽ��ϴ�.";//
        desTxt[17] = "������� óġ�ϸ� ���̺긦 �����ϰԵ˴ϴ�.";
        desTxt[18] = "��� ���̺긦 �����ϸ� �ش� ���������� Ŭ�����ϰ� �˴ϴ�.";//
        desTxt[19] = "���������� Ŭ�����ϸ� ���� ���, �޴��� ȹ���� �� �ֽ��ϴ�. ������������ �й��ϴ��� ���� ��Ḧ ȹ���� �� ������, �޴��� �¸� �ÿ��� ȹ���� �� �ֽ��ϴ�.";//
        desTxt[20] = "��ư�� ���� ���� ���������� �̵��ϰų� �Ĵ����� ������ �� �ֽ��ϴ�. ";//
        desTxt[21] = "�ϴ� �Ĵ����� ������ ���ô�.";//
        desTxt[22] = "�Ĵ翡 �湮�� ���谡�� �մԵ��� �Ӹ� �� ��ǳ���� ������ �ֹ��� ���� �� �ֽ��ϴ�.";
        desTxt[23] = "�������� ȹ���� �丮 ��Ḧ ����� �����Ǹ� �ر��ϰ� ��ȭ�� �� �ֽ��ϴ�.";//
        desTxt[24] = "������ ��Ͽ��� ������� �ر��� �丮 ����� �� �� �ֽ��ϴ�. ���� �����Ǹ� ���� ������ ������ �����Ǹ� ��ȭ�� �� �ֽ��ϴ�. �����Ǹ� ��ȭ�ϸ� ���� �� �����ϴ� �丮 ī���� ���ɰ�, �մԵ��� �ֹ����� ȹ���ϴ� ��尡 �����մϴ�.";//
        desTxt[25] = "�Ĵ� ��ȭ ��ư�� ���� �Ĵ� ��ȭ �޴��� �� �� �ֽ��ϴ�.";//
        desTxt[26] = "���������� Ŭ�����ϰ� ���� �޴޷� �ش����������� ��ȭ �� �����մϴ�.";//
        desTxt[27] = "�޴��� ����� ���� ���࿡ ������ �ִ� Ư���� ȿ���� �ر��ϰų� ��ȭ�� �� �ֽ��ϴ�.";//
        desTxt[28] = "��Ƽ �� �ִ� �ο� ���� �÷����ô�.\r\n";//
        desTxt[29] = "�޴��� ������� ����ϸ� �Ĵ� ������ �Բ� �����մϴ�. �޴��� �� ���� ȹ���ϰ� �Һ��� �Ĵ��� ���׷��̵��غ�����.";
        desTxt[30] = "���谡 ��ư�� ���� ����� ���谡 ����� �� �� �ֽ��ϴ�.";//
        desTxt[31] = "���谡 ���� �޴��Դϴ�. ���谡�� �����ϸ� �ش� ���谡�� �� ������ �� �� �ֽ��ϴ�.";//
        desTxt[32] = "�̰����� ���谡�� �ɷ�ġ�� ��ų ������ Ȯ���� �� �ֽ��ϴ�. ";
        desTxt[33] = "�� ������ �ذ� ��ư�� ���� ���谡�� ��Ͽ��� ���� �� �ֽ��ϴ�. ���谡�� �ذ��ϸ� �ش� ���谡�� �� �̻� ����� �� �����ϴ�.";//
        desTxt[34] = "���� �� ��ư�� ���� ���谡�� ������ �ø� �� �ֽ��ϴ�. ���� ������ �ø� �� �ִ� �ִ� ������ �Ĵ� ������ �����մϴ�";//
        desTxt[35] = "���� �Ĵ� ������ 2��, ���� ������ �ø� �� �ִ� ���谡�� �ִ� ������ 2�Դϴ�. ���� �� ��ư�� ���� ���谡 ������ �÷�������.";
        desTxt[36] = "�̻����� Ʃ�丮���� �����ϰڽ��ϴ� �����մϴ�";
    }

    private void InitializeisClear()
    {
        isClear = new bool[16];

        isClear[0] = false; //EmploymentOpen
        isClear[1] = false; //EmploymentBack
        isClear[2] = false; //DungeonIn
        isClear[3] = false; //CombatIn
        isClear[4] = false; //EntryCharacter
        isClear[5] = false; //EnterCombat
        isClear[6] = false; //ReturnRes
        isClear[7] = false; //EnterRecipe
        isClear[8] = false; //ClickRecipe
        isClear[9] = false; //EnterRes
        isClear[10] = false; //ClickGold
        isClear[11] = false; //ClickMedal
        isClear[12] = false; //PartyCount
        isClear[13] = false; //CharacterManagement
        isClear[14] = false; //EnterCharacterInfo
        isClear[15] = false; //CharacterLevelUp
    }
}
