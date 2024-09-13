using UnityEngine;

public class UICombatResult : UIBase
{
    [SerializeField] private UIBase VictoryUI;
    [SerializeField] private UIBase DefeatUI;

    private void Start()
    {
        SetActive(false);
    }

    private void OnEnable()
    {
        opened?.Invoke(null);
    }

    public override void OnOpened(object[] param)
    {
        if (GameManager.Instance.combatController.isPlayerWin)
        {
            DefeatUI.SetActive(false);
            if (!DataManager.Instance.userInfo.isUserTutorials) 
            {
                ToturialsManager.Instance.gameObject.SetActive(true);
            }
            VictoryUI.SetActive(true);
        }
        else
        {
            VictoryUI.SetActive(false);
            DefeatUI.SetActive(true);
        }
    }
}
