using UnityEngine;

public class UICombatScene : MonoBehaviour
{
    [SerializeField] UIBase StrategyUI;
    [SerializeField] UIBase CombatUI;
    [SerializeField] UIBase CombatResultUI;

    private void Awake()
    {
        UIManager.Instance.AddUIDictionary(UIType.StrategyUI, StrategyUI);
        UIManager.Instance.AddUIDictionary(UIType.CombatUI, CombatUI);
        UIManager.Instance.AddUIDictionary(UIType.CombatResultUI, CombatResultUI);
    }

}
